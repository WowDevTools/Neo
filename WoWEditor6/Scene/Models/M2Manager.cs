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
        private readonly Dictionary<int, M2Renderer> mRenderer = new Dictionary<int, M2Renderer>();
        private readonly Dictionary<int, M2RenderInstance> mVisibleInstances = new Dictionary<int, M2RenderInstance>();
        private readonly object mAddLock = new object();
        private Thread mUnloadThread;
        private bool mIsRunning;
        private readonly List<M2Renderer> mUnloadList = new List<M2Renderer>();

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
            if (WorldFrame.Instance.HighlightModelsInBrush)
            {
                var brushPosition = Editing.EditManager.Instance.MousePosition;
                var highlightRadius = Editing.EditManager.Instance.OuterRadius;
                UpdateBrushHighlighting(brushPosition, highlightRadius);
            }

            M2BatchRenderer.Mesh.BeginDraw();
            M2BatchRenderer.Mesh.Program.SetPixelSampler(0, M2BatchRenderer.Sampler);

            lock (mAddLock)
            {
                foreach (var pair in mRenderer)
                    pair.Value.RenderBatch();

                // TODO: Sort this by depth (instance.Renderer.Depth)
                foreach (var instance in mVisibleInstances.Values)
                    instance.Renderer.RenderAlphaInstance(instance);
            }

            IsViewDirty = false;
        }

        public void PushMapReferences(M2Instance[] instances)
        {
            lock (mAddLock)
            {
                foreach (var instance in instances)
                {
                    if (instance == null || instance.RenderInstance == null || instance.RenderInstance.IsUpdated)
                        continue;

                    M2Renderer renderer;
                    if (mRenderer.TryGetValue(instance.Hash, out renderer))
                        renderer.PushMapReference(instance);

                    mVisibleInstances.Add(instance.Uuid, instance.RenderInstance);
                }
            }
        }

        private void UpdateBrushHighlighting(Vector3 brushPosition, float radius)
        {
            lock (mAddLock)
            {
                foreach (var t in mVisibleInstances)
                    t.Value.UpdateBrushHighlighting(brushPosition, radius);
            }
        }

        public void ViewChanged()
        {
            IsViewDirty = true;
            lock(mAddLock)
            {
                mVisibleInstances.Clear();
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
                lock (mAddLock)
                    mVisibleInstances.Remove(uuid);

                M2Renderer renderer;
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

                var render = new M2Renderer(file);
                lock (mAddLock)
                    mRenderer.Add(hash, render);

                return render.AddInstance(uuid, position, rotation, scaling);
            }
        }

        private void UnloadProc()
        {
            while(mIsRunning)
            {
                M2Renderer element = null;
                lock(mUnloadList)
                {
                    if(mUnloadList.Count > 0)
                    {
                        element = mUnloadList[0];
                        mUnloadList.RemoveAt(0);
                    }
                }

                if (element != null)
                    element.Dispose();

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
