using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.Scene.Models.WMO
{
    class WmoBatchRender
    {
        private readonly WmoRootRender mRoot;
        private readonly List<WmoInstance> mInstances = new List<WmoInstance>();
        private List<WmoInstance> mActiveInstances = new List<WmoInstance>();

        private bool mInstancesChanged;

        public WmoBatchRender(WmoRootRender root)
        {
            mRoot = root;
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
            mInstances.Add(inst);
            mInstancesChanged = true;
        }

        public void UpdateVisibility()
        {
            mInstancesChanged = false;

            if (mInstances.Count == 0)
                return;

            mActiveInstances = mInstances.Where(w => WorldFrame.Instance.ActiveCamera.Contains(ref w.BoundingBox)).ToList();
        }
    }
}
