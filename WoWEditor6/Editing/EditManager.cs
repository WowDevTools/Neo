using System;
using SharpDX;
using WoWEditor6.Scene;

namespace WoWEditor6.Editing
{
    class EditManager
    {
        public static EditManager Instance { get; private set; }

        private DateTime mLastChange = DateTime.Now;

        private float mInnerRadius = 45.0f;
        private float mOuterRadius = 55.0f;

        public float InnerRadius
        {
            get { return mInnerRadius; }
            set
            {
                mInnerRadius = value;
                WorldFrame.Instance.UpdateBrush(mInnerRadius, mOuterRadius);
            }
        }

        public float OuterRadius
        {
            get { return mOuterRadius; }
            set
            {
                mOuterRadius = value;
                WorldFrame.Instance.UpdateBrush(mInnerRadius, mOuterRadius);
            }
        }

        public Vector3 MousePosition { get; set; }
        public bool IsTerrainHovered { get; set; }

        public EditMode CurrentMode { get; private set; }

        static EditManager()
        {
            Instance = new EditManager();
        }

        public void UpdateChanges()
        {
            var diff = DateTime.Now - mLastChange;
            if (diff.TotalMilliseconds < 20)
                return;

            mLastChange = DateTime.Now;
            if ((CurrentMode & EditMode.Sculpting) != 0)
                TerrainChangeManager.Instance.OnChange(diff);
            else if ((CurrentMode & EditMode.Texturing) != 0)
                TextureChangeManager.Instance.OnChange(diff);
        }

        public void EnableSculpting()
        {
            CurrentMode |= EditMode.Sculpting;
            CurrentMode &= ~EditMode.Texturing;
        }

        public void DisableSculpting()
        {
            CurrentMode &= ~EditMode.Sculpting;
        }

        public void EnableTexturing()
        {
            CurrentMode |= EditMode.Texturing;
            CurrentMode &= ~EditMode.Sculpting;
        }

        public void DisableTexturing()
        {
            CurrentMode &= ~EditMode.Texturing;
        }
    }
}
