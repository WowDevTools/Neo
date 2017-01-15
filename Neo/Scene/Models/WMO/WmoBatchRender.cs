using System;
using System.Collections.Generic;
using OpenTK;
using SlimTK;

namespace Neo.Scene.Models.WMO
{
	internal class WmoBatchRender : IDisposable
    {
        private WmoRootRender mRoot;
        private Dictionary<int, WmoInstance> mInstances = new Dictionary<int, WmoInstance>();
        private List<WmoInstance> mActiveInstances = new List<WmoInstance>();

        private bool mInstancesChanged;

        public WmoBatchRender(WmoRootRender root)
        {
	        this.mRoot = root;
        }

        public bool Intersect(IntersectionParams parameters, ref Ray globalRay, out float distance, out WmoInstance instance)
        {
            distance = float.MaxValue;
            instance = null;

            if (this.mInstances == null)
            {
	            return false;
            }

	        lock (this.mInstances)
            {
                foreach (var inst in this.mInstances.Values)
                {
                    float dist;
                    if (inst.Intersects(parameters, ref globalRay, out dist) && dist < distance)
                    {
                        distance = dist;
                        instance = inst;
                    }
                }
            }

            return instance != null;
        }

        ~WmoBatchRender()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (this.mActiveInstances != null)
            {
	            this.mActiveInstances.Clear();
	            this.mActiveInstances = null;
            }

            if (this.mInstances != null)
            {
                foreach (var inst in this.mInstances.Values)
                {
	                inst.Dispose();
                }

	            this.mInstances.Clear();
	            this.mInstances = null;
            }

            if (this.mRoot != null)
            {
	            this.mRoot.Dispose();
	            this.mRoot = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool RemoveInstance(int uuid)
        {
            if (this.mInstances == null)
            {
	            return false;
            }

	        lock (this.mInstances)
            {
                WmoInstance instance;
                if (this.mInstances.TryGetValue(uuid, out instance) == false || instance == null)
                {
	                return false;
                }

	            --instance.ReferenceCount;
                if (instance.ReferenceCount > 0)
                {
                    ++instance.ReferenceCount;
                    return false;
                }

	            this.mInstances.Remove(uuid);
                instance.Dispose();
	            this.mInstancesChanged = true;
                return instance.ReferenceCount == 0;
            }
        }

        public bool DeleteInstance(int uuid)
        {
            if (this.mInstances == null)
            {
	            return false;
            }

	        lock (this.mInstances)
            {
                WmoInstance instance;
                if (this.mInstances.TryGetValue(uuid, out instance) == false || this.mInstances == null)
                {
	                return false;
                }

	            this.mInstances.Remove(uuid);
                instance.Dispose();
	            this.mInstancesChanged = true;

                return this.mInstances.Count == 0;
            }
        }

        public void OnFrame()
        {
            if (this.mInstancesChanged)
            {
	            UpdateVisibility();
            }

	        if (this.mActiveInstances.Count == 0)
	        {
		        return;
	        }

	        this.mRoot.OnFrame(this.mActiveInstances);
        }

        public void AddInstance(int uuid, Vector3 position, Vector3 rotation)
        {
            lock (this.mInstances)
            {
                WmoInstance instance;
                if (this.mInstances.TryGetValue(uuid, out instance))
                {
                    ++instance.ReferenceCount;
                    return;
                }
            }

            var inst = new WmoInstance(uuid, position, rotation, this.mRoot);

            lock (this.mInstances)
            {
	            this.mInstances.Add(uuid,inst);
	            this.mInstancesChanged = true;
            }
        }

        private void UpdateVisibility()
        {
	        this.mInstancesChanged = false;

            lock (this.mInstances)
            {
                if (this.mInstances.Count == 0)
                {
	                return;
                }

	            this.mActiveInstances.Clear();
	            this.mActiveInstances.AddRange(this.mInstances.Values);
            }
        }
    }
}
