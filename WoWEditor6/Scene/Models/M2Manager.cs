using System;
using System.Collections.Generic;
using System.Threading;
using SharpDX;
using WoWEditor6.IO.Files.Models;
using WoWEditor6.Scene.Models.M2;

namespace WoWEditor6.Scene.Models
{
    class M2Manager
    {
        private readonly Dictionary<int, M2BatchRenderer> mRenderer = new Dictionary<int, M2BatchRenderer>();
        private readonly object mAddLock = new object();
        private Thread mUnloadThread;
        private bool mIsRunning;
        private readonly List<M2BatchRenderer> mUnloadList = new List<M2BatchRenderer>();

        public static bool IsViewDirty { get; private set; }

        public void Initialize()
        {
            mUnloadThread = new Thread(UnloadProc);
            mUnloadThread.Start();
        }

        public void Shutdown()
        {
            mIsRunning = false;
            mUnloadThread.Join();
        }

        public void OnFrame()
        {
            M2BatchRenderer.Mesh.BeginDraw();
            M2BatchRenderer.Mesh.Program.SetPixelSampler(0, M2BatchRenderer.Sampler);

            lock(mAddLock)
            {
                foreach (var pair in mRenderer)
                    pair.Value.OnFrame();
            }

            IsViewDirty = false;
        }

        public void PushMapReferences(M2Instance[] instances)
        {
            lock (mAddLock)
            {
                foreach (var instance in instances)
                {
                    if (instance?.RenderInstance?.IsUpdated ?? true)
                        continue;

                    if (instance == null)
                        continue;

                    M2BatchRenderer renderer;
                    if (mRenderer.TryGetValue(instance.Hash, out renderer))
                        renderer.PushMapReference(instance);
                }
            }
        }

        public void ViewChanged()
        {
            IsViewDirty = true;
            lock(mAddLock)
            {
                foreach (var pair in mRenderer)
                    pair.Value.ViewChanged();
            }
        }

        public void RemoveInstance(string model, int uuid)
        {
            var hash = model.ToUpperInvariant().GetHashCode();
            RemoveInstance(hash, uuid);
        }

        public void RemoveInstance(int hash, int uuid)
        {
            lock (mRenderer)
            {
                M2BatchRenderer renderer;
                if (mRenderer.TryGetValue(hash, out renderer) == false)
                    return;

                if (renderer.RemoveInstance(uuid))
                {
                    lock (mAddLock)
                        mRenderer.Remove(hash);

                    lock (mUnloadList)
                        mUnloadList.Add(renderer);
                }
            }
        }

        public M2RenderInstance AddInstance(string model, int uuid, Vector3 position, Vector3 rotation, Vector3 scaling)
        {
            var hash = model.ToUpperInvariant().GetHashCode();
            lock(mRenderer)
            {
                if (mRenderer.ContainsKey(hash))
                {
                    var renderer = mRenderer[hash];
                    return renderer.AddInstance(uuid, position, rotation, scaling);
                }

                var file = LoadModel(model);
                if (file == null)
                    return null;

                var batch = new M2BatchRenderer(file);
                lock (mAddLock)
                    mRenderer.Add(hash, batch);

                return batch.AddInstance(uuid, position, rotation, scaling);
            }
        }

        private void UnloadProc()
        {
            while(mIsRunning)
            {
                M2BatchRenderer element = null;
                lock(mUnloadList)
                {
                    if(mUnloadList.Count > 0)
                    {
                        element = mUnloadList[0];
                        mUnloadList.RemoveAt(0);
                    }
                }

                element?.Dispose();

                if (element == null)
                    Thread.Sleep(200);
            }
        }

        private static M2File LoadModel(string fileName)
        {
            var file = ModelFactory.Instance.CreateM2(fileName);
            try
            {
                return file.Load() == false ? null : file;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
