using System.Collections.Generic;
using System.Threading;
using Neo.Scene.Models.WMO;
using System;
using OpenTK;

namespace Neo.Scene.Models
{
    class WmoManager
    {
        private readonly Dictionary<int, WmoBatchRender> mRenderer = new Dictionary<int, WmoBatchRender>();
        private readonly object mAddLock = new object();
        private Thread mUnloadThread;
        private readonly List<WmoBatchRender> mUnloadItems = new List<WmoBatchRender>();
        private bool mIsRunning = true;

        public void Initialize()
        {
            mUnloadThread = new Thread(UnloadThread);
            mUnloadThread.Start();
        }

        public void Shutdown()
        {
            mIsRunning = false;
            mUnloadThread.Join();
        }

        public void Intersect(IntersectionParams parameters)
        {
            if (mRenderer == null)
                return;

            var globalRay = Picking.Build(ref parameters.ScreenPosition, ref parameters.InverseView,
                ref parameters.InverseProjection);

            var minDistance = float.MaxValue;
            WmoInstance wmoHit = null;

            lock (mRenderer)
            {
                foreach (var renderer in mRenderer)
                {
                    WmoInstance hit;
                    float distance;
                    if (renderer.Value.Intersect(parameters, ref globalRay, out distance, out hit) &&
                        distance < minDistance)
                    {
                        minDistance = distance;
                        wmoHit = hit;
                    }
                }
            }

            if (wmoHit != null)
            {
                parameters.WmoHit = true;
                parameters.WmoInstance = wmoHit;
                parameters.WmoModel = wmoHit.ModelRoot;
                parameters.WmoPosition = globalRay.Position + minDistance * globalRay.Direction;
                parameters.WmoDistance = minDistance;
            }
            else
                parameters.WmoHit = false;
        }

        private void PreloadModel(string model)
        {
            var hash = model.ToUpperInvariant().GetHashCode();
            lock(mRenderer)
            {
                if (mRenderer.ContainsKey(hash))
                    return;

                var root = IO.Files.Models.ModelFactory.Instance.CreateWmo();

                if (root.Load(model) == false)
                    Log.Warning("Unable to load WMO '" + model + "'. Further instances wont be loaded again");

                var renderer = new WmoRootRender();
                renderer.OnAsyncLoad(root);

                var batch = new WmoBatchRender(renderer);

                lock (mAddLock)
                    mRenderer.Add(hash, batch);
            }
        }

        public void RemoveInstance(string model, int uuid, bool delete)
        {
            try
            {
                var hash = model.ToUpperInvariant().GetHashCode();
                RemoveInstance(hash, uuid, delete);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void RemoveInstance(int hash, int uuid,bool delete)
        {
            if (mRenderer == null)
                return;

            lock (mRenderer)
            {
                WmoBatchRender batch;
                if (!mRenderer.TryGetValue(hash, out batch))
                    return;
                if (delete && batch.DeleteInstance(uuid))
                {
                    lock (mAddLock)
                        mRenderer.Remove(hash);

                    lock (mUnloadItems)
                        mUnloadItems.Add(batch);
                }
                else if (batch.RemoveInstance(uuid))
                {
                    lock (mAddLock)
                        mRenderer.Remove(hash);

                    lock (mUnloadItems)
                        mUnloadItems.Add(batch);
                }
            }
        }

        public void AddInstance(string model, int uuid, Vector3 position, Vector3 rotation)
        {
            var hash = model.ToUpperInvariant().GetHashCode();

            WmoBatchRender batch;
            lock(mRenderer)
            {
                if(mRenderer.TryGetValue(hash, out batch) == false)
                {
                    PreloadModel(model);
                    mRenderer.TryGetValue(hash, out batch);
                }
            }

            if(batch == null)
            {
                Log.Error("Internal error adding an instance of a WMO");
                return;
            }

            batch.AddInstance(uuid, position, rotation);
        }

        public void OnFrame(Camera camera)
        {
            WmoGroupRender.Mesh.BeginDraw();
            WmoGroupRender.Mesh.Program.SetPixelSampler(0, WmoGroupRender.Sampler);
            lock(mAddLock)
            {
                foreach (var pair in mRenderer)
                    pair.Value.OnFrame();
            }
        }

        private void UnloadThread()
        {
            while(mIsRunning)
            {
                WmoBatchRender element = null;
                lock (mUnloadItems)
                {
                    if (mUnloadItems.Count > 0)
                    {
                        element = mUnloadItems[0];
                        mUnloadItems.RemoveAt(0);
                    }
                }

                if (element != null)
                    element.Dispose();

                if (element == null)
                    Thread.Sleep(200);
            }
        }
    }
}
