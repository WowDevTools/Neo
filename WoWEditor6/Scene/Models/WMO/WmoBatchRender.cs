using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace WoWEditor6.Scene.Models.WMO
{
    class WmoBatchRender : IDisposable
    {
        private WmoRootRender mRoot;
        private List<WmoInstance> mInstances = new List<WmoInstance>();
        private List<WmoInstance> mActiveInstances = new List<WmoInstance>();

        private bool mInstancesChanged;

        public WmoBatchRender(WmoRootRender root)
        {
            mRoot = root;
        }

        ~WmoBatchRender()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (mInstances != null)
            {
                mInstances.Clear();
                mInstances = null;
            }

            if (mActiveInstances != null)
            {
                mActiveInstances.Clear();
                mActiveInstances = null;
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
            lock(mInstances)
            {
                var instance = mInstances.FirstOrDefault(w => w.Uuid == uuid);
                if (instance != null)
                    mInstances.Remove(instance);

                return mInstances.Count == 0;
            }
        }

        public void OnFrame()
        {
            if (mInstancesChanged)
                UpdateVisibility();

            if (mActiveInstances.Count == 0)
                return;

            mRoot.OnFrame(mActiveInstances);
        }

        public void AddInstance(int uuid, Vector3 position, Vector3 rotation)
        {
            if (mInstances.Any(w => w.Uuid == uuid))
                return;

            var inst = new WmoInstance(uuid, position, rotation, mRoot);

            lock (mInstances)
            {
                mInstances.Add(inst);
                mInstancesChanged = true;
            }
        }

        private void UpdateVisibility()
        {
            mInstancesChanged = false;

            lock(mInstances)
            {
                if (mInstances.Count == 0)
                    return;

                mActiveInstances.Clear();
                mActiveInstances.AddRange(mInstances);
            }
        }
    }
}
