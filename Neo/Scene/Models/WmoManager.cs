using System.Collections.Generic;
using System.Threading;
using Neo.Scene.Models.WMO;
using System;
using OpenTK;

namespace Neo.Scene.Models
{
	internal class WmoManager
    {
        private readonly Dictionary<int, WmoBatchRender> mRenderer = new Dictionary<int, WmoBatchRender>();
        private readonly object mAddLock = new object();
        private Thread mUnloadThread;
        private readonly List<WmoBatchRender> mUnloadItems = new List<WmoBatchRender>();
        private bool mIsRunning = true;

        public void Initialize()
        {
	        this.mUnloadThread = new Thread(UnloadThread);
	        this.mUnloadThread.Start();
        }

        public void Shutdown()
        {
	        this.mIsRunning = false;
	        this.mUnloadThread.Join();
        }

        public void Intersect(IntersectionParams parameters)
        {
	        if (this.mRenderer == null)
	        {
		        return;
	        }

            var globalRay = Picking.Build(ref parameters.ScreenPosition, ref parameters.InverseView,
                ref parameters.InverseProjection);

            var minDistance = float.MaxValue;
            WmoInstance wmoHit = null;

            lock (this.mRenderer)
            {
                foreach (var renderer in this.mRenderer)
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
		        parameters.WorldModel = wmoHit.ModelRoot;
		        parameters.WmoPosition = globalRay.Position + minDistance * globalRay.Direction;
		        parameters.WmoDistance = minDistance;
	        }
	        else
	        {
		        parameters.WmoHit = false;
	        }
        }

        private void PreloadModel(string model)
        {
            var hash = model.ToUpperInvariant().GetHashCode();
            lock(this.mRenderer)
            {
	            if (this.mRenderer.ContainsKey(hash))
	            {
		            return;
	            }

                var root = IO.Files.Models.ModelFactory.Instance.CreateWmo();

	            if (root.Load(model) == false)
	            {
		            Log.Warning("Unable to load WMO '" + model + "'. Further instances wont be loaded again");
	            }

                var renderer = new WmoRootRender();
                renderer.OnAsyncLoad(root);

                var batch = new WmoBatchRender(renderer);

	            lock (this.mAddLock)
	            {
		            this.mRenderer.Add(hash, batch);
	            }
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
	        if (this.mRenderer == null)
	        {
		        return;
	        }

            lock (this.mRenderer)
            {
                WmoBatchRender batch;
	            if (!this.mRenderer.TryGetValue(hash, out batch))
	            {
		            return;
	            }

                if (delete && batch.DeleteInstance(uuid))
                {
	                lock (this.mAddLock)
	                {
		                this.mRenderer.Remove(hash);
	                }

	                lock (this.mUnloadItems)
	                {
		                this.mUnloadItems.Add(batch);
	                }
                }
                else if (batch.RemoveInstance(uuid))
                {
	                lock (this.mAddLock)
	                {
		                this.mRenderer.Remove(hash);
	                }

	                lock (this.mUnloadItems)
	                {
		                this.mUnloadItems.Add(batch);
	                }
                }
            }
        }

        public void AddInstance(string model, int uuid, Vector3 position, Vector3 rotation)
        {
            var hash = model.ToUpperInvariant().GetHashCode();

            WmoBatchRender batch;
            lock(this.mRenderer)
            {
                if(this.mRenderer.TryGetValue(hash, out batch) == false)
                {
                    PreloadModel(model);
	                this.mRenderer.TryGetValue(hash, out batch);
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
            lock(this.mAddLock)
            {
	            foreach (var pair in this.mRenderer)
	            {
		            pair.Value.OnFrame();
	            }
            }
        }

        private void UnloadThread()
        {
            while(this.mIsRunning)
            {
                WmoBatchRender element = null;
                lock (this.mUnloadItems)
                {
                    if (this.mUnloadItems.Count > 0)
                    {
                        element = this.mUnloadItems[0];
	                    this.mUnloadItems.RemoveAt(0);
                    }
                }

	            if (element != null)
	            {
		            element.Dispose();
	            }

	            if (element == null)
	            {
		            Thread.Sleep(200);
	            }
            }
        }
    }
}
