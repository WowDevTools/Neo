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
            mRoot = root;
        }

        public bool Intersect(IntersectionParams parameters, ref Ray globalRay, out float distance, out WmoInstance instance)
        {
            distance = float.MaxValue;
            instance = null;

            if (mInstances == null)
            {
	            return false;
            }

	        lock (mInstances)
            {
                foreach (var inst in mInstances.Values)
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
            if (mActiveInstances != null)
            {
                mActiveInstances.Clear();
                mActiveInstances = null;
            }

            if (mInstances != null)
            {
                foreach (var inst in mInstances.Values)
                {
	                inst.Dispose();
                }

	            mInstances.Clear();
                mInstances = null;
            }

            if (mRoot != null)
            {
                mRoot.Dispose();
                mRoot = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool RemoveInstance(int uuid)
        {
            if (mInstances == null)
            {
	            return false;
            }

	        lock (mInstances)
            {
                WmoInstance instance;
                if (mInstances.TryGetValue(uuid, out instance) == false || instance == null)
                {
	                return false;
                }

	            --instance.ReferenceCount;
                if (instance.ReferenceCount > 0)
                {
                    ++instance.ReferenceCount;
                    return false;
                }

                mInstances.Remove(uuid);
                instance.Dispose();
                mInstancesChanged = true;
                return instance.ReferenceCount == 0;
            }
        }

        public bool DeleteInstance(int uuid)
        {
            if (mInstances == null)
            {
	            return false;
            }

	        lock (mInstances)
            {
                WmoInstance instance;
                if (mInstances.TryGetValue(uuid, out instance) == false || mInstances == null)
                {
	                return false;
                }

	            mInstances.Remove(uuid);
                instance.Dispose();
                mInstancesChanged = true;

                return mInstances.Count == 0;
            }
        }

        public void OnFrame()
        {
            if (mInstancesChanged)
            {
	            UpdateVisibility();
            }

	        if (mActiveInstances.Count == 0)
	        {
		        return;
	        }

	        mRoot.OnFrame(mActiveInstances);
        }

        public void AddInstance(int uuid, Vector3 position, Vector3 rotation)
        {
            lock (mInstances)
            {
                WmoInstance instance;
                if (mInstances.TryGetValue(uuid, out instance))
                {
                    ++instance.ReferenceCount;
                    return;
                }
            }

            var inst = new WmoInstance(uuid, position, rotation, mRoot);

            lock (mInstances)
            {
                mInstances.Add(uuid,inst);
                mInstancesChanged = true;
            }
        }

        private void UpdateVisibility()
        {
            mInstancesChanged = false;

            lock (mInstances)
            {
                if (mInstances.Count == 0)
                {
	                return;
                }

	            mActiveInstances.Clear();
                mActiveInstances.AddRange(mInstances.Values);
            }
        }
    }
}
