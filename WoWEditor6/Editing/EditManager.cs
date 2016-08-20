using System;
using SharpDX;
using WoWEditor6.Scene;
using WoWEditor6.UI;
using WoWEditor6.Utils;
using System.Windows.Forms;
using Point = System.Drawing.Point;

namespace WoWEditor6.Editing
{
    class EditManager
    {
        public static EditManager Instance { get; private set; }

        private DateTime mLastChange = DateTime.Now;

        private Point mLastCursorPosition = Cursor.Position;

        private float mInnerRadius = 45.0f;
        private float mOuterRadius = 55.0f;

        public float InnerRadius
        {
            get { return mInnerRadius; }
            set { HandleInnerRadiusChanged(value); }
        }

        public float OuterRadius
        {
            get { return mOuterRadius; }
            set { HandleOuterRadiusChanged(value); }
        }

        public bool IsTexturing { get { return (CurrentMode & EditMode.Texturing) != 0; } }

        public Vector3 MousePosition { get; set; }
        public bool IsTerrainHovered { get; set; }

        public EditMode CurrentMode { get; private set; }

        static EditManager()
        {
            Instance = new EditManager();
        }

        public void UpdateChanges()
        {
            ModelSpawnManager.Instance.OnUpdate();
            ModelEditManager.Instance.Update();

            var diff = DateTime.Now - mLastChange;
            if (diff.TotalMilliseconds < (IsTexturing ? 40 : 20))
                return;

            mLastChange = DateTime.Now;
            if ((CurrentMode & EditMode.Sculpting) != 0)
                TerrainChangeManager.Instance.OnChange(diff);
            else if ((CurrentMode & EditMode.Texturing) != 0)
                TextureChangeManager.Instance.OnChange(diff);

            var keyState = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(keyState);
            var altDown = KeyHelper.IsKeyDown(keyState, Keys.Menu);
            var LMBDown = KeyHelper.IsKeyDown(keyState, Keys.LButton);
            var RMBDown = KeyHelper.IsKeyDown(keyState, Keys.RButton);

            var curPos = Cursor.Position;

            if (curPos != mLastCursorPosition)
            { 
                if (altDown && RMBDown)
                {
                    var amount = -(mLastCursorPosition.X - curPos.X) / 32.0f;

                    mInnerRadius += amount;


                    if (mInnerRadius < 0)
                    {
                        mInnerRadius = 0.0f;
                    }

                    if (mInnerRadius > 1000)
                    {
                        mInnerRadius = 1000.0f;
                    }

                    if (mInnerRadius > mOuterRadius)
                    {
                        mInnerRadius = mOuterRadius;
                    }

                    HandleInnerRadiusChanged(mInnerRadius);

                }
                

                if (altDown && LMBDown)
                {
                    var amount = -(mLastCursorPosition.X - curPos.X) / 32.0f;

                    mInnerRadius += amount;
                    mOuterRadius += amount;

                    if (mInnerRadius < 0)
                    {
                        mInnerRadius = 0.0f;
                    }

                    if (mInnerRadius > 1000)
                    {
                        mInnerRadius = 1000.0f;
                    }

                    if (mOuterRadius < 0)
                    {
                        mInnerRadius = 0.0f;
                    }

                    if (mOuterRadius > 1000)
                    {
                        mInnerRadius = 1000.0f;
                    }

                    if (mInnerRadius > mOuterRadius)
                    {
                        mInnerRadius = mOuterRadius;
                    }

                    HandleInnerRadiusChanged(mInnerRadius);
                    HandleOuterRadiusChanged(mOuterRadius);
                    

                }

                mLastCursorPosition = Cursor.Position;

            }


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

        private void HandleInnerRadiusChanged(float value)
        {
            mInnerRadius = value;
            WorldFrame.Instance.UpdateBrush(mInnerRadius, mOuterRadius);
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleInnerRadiusChanged(value);
        }

        private void HandleOuterRadiusChanged(float value)
        {
            mOuterRadius = value;
            WorldFrame.Instance.UpdateBrush(mInnerRadius, mOuterRadius);
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleOuterRadiusChanged(value);
        }
    }
}
