using System;
using System.Collections.Generic;
using System.Threading;
using Neo.IO.Files.Models;
using Neo.Scene.Models.M2;
using OpenTK;

namespace Neo.Scene.Models
{
	internal class M2Manager
    {
        private class InstanceSortComparer : IComparer<int>
        {
            private readonly IDictionary<int, M2RenderInstance> mInstances;

            public InstanceSortComparer(IDictionary<int, M2RenderInstance> dict)
            {
	            this.mInstances = dict;
            }

            public int Compare(int first, int second)
            {
                M2RenderInstance renderA, renderB;
                if (this.mInstances.TryGetValue(first, out renderA) && this.mInstances.TryGetValue(second, out renderB))
                {
                    int compare = renderB.Depth.CompareTo(renderA.Depth);
                    if (compare != 0)
                    {
	                    return compare;
                    }
                }
                return first.CompareTo(second);
            }
        }

        private readonly Dictionary<int, M2Renderer> mRenderer = new Dictionary<int, M2Renderer>();
        private readonly Dictionary<int, M2RenderInstance> mVisibleInstances = new Dictionary<int, M2RenderInstance>();
        private readonly Dictionary<int, M2RenderInstance> mNonBatchedInstances = new Dictionary<int, M2RenderInstance>();
        private readonly SortedDictionary<int, M2RenderInstance> mSortedInstances;

        private readonly object mAddLock = new object();
        private Thread mUnloadThread;
        private bool mIsRunning;
        private readonly List<M2Renderer> mUnloadList = new List<M2Renderer>();

        public static bool IsViewDirty { get; private set; }

        public M2Manager()
        {
	        this.mSortedInstances = new SortedDictionary<int, M2RenderInstance>(
                new InstanceSortComparer(this.mVisibleInstances));
        }

        public void Initialize()
        {
	        this.mIsRunning = true;
	        this.mUnloadThread = new Thread(UnloadProc);
	        this.mUnloadThread.Start();
        }

        public void Shutdown()
        {
	        this.mIsRunning = false;
	        this.mUnloadThread.Join();
        }

        public void Intersect(IntersectionParams parameters)
        {
            if (this.mVisibleInstances == null || this.mNonBatchedInstances == null || this.mSortedInstances == null)
            {
	            return;
            }

	        var globalRay = Picking.Build(ref parameters.ScreenPosition, ref parameters.InverseView,
                ref parameters.InverseProjection);

            var minDistance = float.MaxValue;
            M2RenderInstance selectedInstance = null;

            lock (this.mVisibleInstances)
            {
                foreach (var pair in this.mVisibleInstances)
                {
                    if (pair.Value.Uuid == Editing.ModelSpawnManager.M2InstanceUuid)
                    {
	                    continue;
                    }

	                float dist;
                    if (pair.Value.Intersects(parameters, ref globalRay, out dist) && dist < minDistance)
                    {
                        minDistance = dist;
                        selectedInstance = pair.Value;
                    }
                }
            }

            lock (this.mNonBatchedInstances)
            {
                foreach (var pair in this.mNonBatchedInstances)
                {
                    float dist;
                    if (pair.Value.Intersects(parameters, ref globalRay, out dist) && dist < minDistance)
                    {
                        minDistance = dist;
                        selectedInstance = pair.Value;
                    }
                }
            }

            lock (this.mSortedInstances)
            {
                foreach (var pair in this.mSortedInstances)
                {
                    float dist;
                    if (pair.Value.Intersects(parameters, ref globalRay, out dist) && dist < minDistance)
                    {
                        minDistance = dist;
                        selectedInstance = pair.Value;
                    }
                }
            }

            if (selectedInstance != null)
            {
                parameters.M2Instance = selectedInstance;
                parameters.M2Model = selectedInstance.Model;
                parameters.M2Position = globalRay.Position + minDistance * globalRay.Direction;
                parameters.M2Distance = minDistance;
            }

            parameters.M2Hit = selectedInstance != null;
        }

        public void OnFrame(Camera camera)
        {
            if (WorldFrame.Instance.HighlightModelsInBrush)
            {
                var brushPosition = Editing.EditManager.Instance.MousePosition;
                var highlightRadius = Editing.EditManager.Instance.OuterRadius;
                UpdateBrushHighlighting(brushPosition, highlightRadius);
            }

            lock (this.mAddLock)
            {
                M2BatchRenderer.BeginDraw();
                // First draw all the instance batches
                foreach (var renderer in this.mRenderer.Values)
                {
	                renderer.RenderBatch();
                }

	            M2SingleRenderer.BeginDraw();
                // Now draw those objects that need per instance animation
                foreach (var instance in this.mNonBatchedInstances.Values)
                {
	                instance.Renderer.RenderSingleInstance(instance);
                }

	            // Then draw those that have alpha blending and need ordering
                foreach (var instance in this.mSortedInstances.Values)
                {
	                instance.Renderer.RenderSingleInstance(instance);
                }
            }

            IsViewDirty = false;
        }

        public void PushMapReferences(M2Instance[] instances)
        {
            foreach (var instance in instances)
            {
                if (instance == null || instance.RenderInstance == null || instance.RenderInstance.IsUpdated)
                {
	                continue;
                }

	            lock (this.mAddLock)
                {
                    M2Renderer renderer;
                    if (!this.mRenderer.TryGetValue(instance.Hash, out renderer))
                    {
	                    continue;
                    }

	                renderer.PushMapReference(instance);
	                this.mVisibleInstances.Add(instance.Uuid, instance.RenderInstance);

                    var model = renderer.Model;
                    if (model.HasBlendPass)
                    {
                        // The model has an alpha pass and therefore needs to be ordered by depth
	                    this.mSortedInstances.Add(instance.Uuid, instance.RenderInstance);
                    }
                    else if (model.NeedsPerInstanceAnimation)
                    {
                        // The model needs per instance animation and therefore cannot be batched
	                    this.mNonBatchedInstances.Add(instance.Uuid, instance.RenderInstance);
                    }
                }
            }
        }

        private void UpdateBrushHighlighting(Vector3 brushPosition, float radius)
        {
            lock (this.mAddLock)
            {
                foreach (var instance in this.mVisibleInstances.Values)
                {
	                instance.UpdateBrushHighlighting(brushPosition, radius);
                }
            }
        }

        public void ViewChanged()
        {
            IsViewDirty = true;
            lock(this.mAddLock)
            {
	            this.mSortedInstances.Clear();
	            this.mNonBatchedInstances.Clear();
	            this.mVisibleInstances.Clear();

                foreach (var renderer in this.mRenderer.Values)
                {
	                renderer.ViewChanged();
                }
            }
        }

        public void RemoveInstance(string model, int uuid)
        {
            try
            {
                var hash = model.ToUpperInvariant().GetHashCode();
                RemoveInstance(hash, uuid);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void RemoveInstance(int hash, int uuid)
        {
            if (this.mRenderer == null || this.mAddLock == null || this.mSortedInstances == null || this.mNonBatchedInstances == null || this.mVisibleInstances == null)
            {
	            return;
            }

	        lock (this.mRenderer)
            {
                lock (this.mAddLock)
                {
	                this.mSortedInstances.Remove(uuid);
	                this.mNonBatchedInstances.Remove(uuid);
	                this.mVisibleInstances.Remove(uuid);
                }

                M2Renderer renderer;
                if (!this.mRenderer.TryGetValue(hash, out renderer))
                {
	                return;
                }

	            if (renderer.RemoveInstance(uuid))
                {
                    lock (this.mAddLock)
                    {
	                    this.mRenderer.Remove(hash);
                    }

	                lock (this.mUnloadList)
	                {
		                this.mUnloadList.Add(renderer);
	                }
                }
            }
        }

        public M2RenderInstance AddInstance(string model, int uuid, Vector3 position, Vector3 rotation, Vector3 scaling)
        {
            var hash = model.ToUpperInvariant().GetHashCode();
            lock(this.mRenderer)
            {
                M2Renderer renderer;
                if (this.mRenderer.TryGetValue(hash, out renderer))
                {
	                return renderer.AddInstance(uuid, position, rotation, scaling);
                }

	            var file = LoadModel(model);
                if (file == null)
                {
	                return null;
                }

	            var render = new M2Renderer(file);
                lock (this.mAddLock)
                {
	                this.mRenderer.Add(hash, render);
                }

	            return render.AddInstance(uuid, position, rotation, scaling);
            }
        }

        private void UnloadProc()
        {
            while(this.mIsRunning)
            {
                M2Renderer element = null;
                lock(this.mUnloadList)
                {
                    if(this.mUnloadList.Count > 0)
                    {
                        element = this.mUnloadList[0];
	                    this.mUnloadList.RemoveAt(0);
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
