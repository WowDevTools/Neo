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
        private float mIntensity = 32.0f;
        private float mAmount = 32.0f;
        private float mOpacity = 255.0f;

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

        public float Intensity
        {
            get { return mIntensity; }
            set { HandleIntensityChanged(value); }
        }

        public float Amount
        {
            get { return mAmount; }
            set { HandleAmountChanged(value); }
        }

        public float Opacity
        {
            get { return mOpacity; }
            set { HandleOpacityChanged(value); }

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
            var spaceDown = KeyHelper.IsKeyDown(keyState, Keys.Space);
            var MMBDown = KeyHelper.IsKeyDown(keyState, Keys.MButton);

            var curPos = Cursor.Position;
            var amount = -(mLastCursorPosition.X - curPos.X) / 32.0f;

            if (curPos != mLastCursorPosition)
            { 
                if (altDown && RMBDown)
                {
                    mInnerRadius += amount;


                    if (mInnerRadius < 0)
                    {
                        mInnerRadius = 0.0f;
                    }

                    if (mInnerRadius > 200)
                    {
                        mInnerRadius = 200.0f;
                    }

                    if (mInnerRadius > mOuterRadius)
                    {
                        mInnerRadius = mOuterRadius;
                    }

                    HandleInnerRadiusChanged(mInnerRadius);

                }
                

                if (altDown && LMBDown)
                {
                    mInnerRadius += amount;
                    mOuterRadius += amount;

                    if (mInnerRadius < 0)
                    {
                        mInnerRadius = 0.0f;
                    }

                    if (mInnerRadius > 200)
                    {
                        mInnerRadius = 200.0f;
                    }

                    if (mOuterRadius < 0)
                    {
                        mInnerRadius = 0.0f;
                    }

                    if (mOuterRadius > 200)
                    {
                        mInnerRadius = 200.0f;
                    }

                    if (mInnerRadius > mOuterRadius)
                    {
                        mInnerRadius = mOuterRadius;
                    }

                    HandleInnerRadiusChanged(mInnerRadius);
                    HandleOuterRadiusChanged(mOuterRadius);
                    

                }

                if(spaceDown && LMBDown)
                {
                    mIntensity += amount;
                    mAmount += amount;

                    if (EditorWindowController.Instance.TerrainManager != null)
                    {
                        if (mIntensity < 1)
                        {
                            mInnerRadius = 1.0f;
                        }

                        if (mIntensity > 40)
                        {
                            mInnerRadius = 40.0f;
                        }

                        HandleIntensityChanged(mIntensity);
                    }

                    if (EditorWindowController.Instance.TexturingModel != null)
                    {
                        if(mAmount < 1)
                        {
                            mInnerRadius = 1.0f;
                        }

                        if (mAmount > 40)
                        {
                            mInnerRadius = 40.0f;
                        }

                        HandleAmountChanged(mAmount);
                    }
                                     
                }

                if(altDown && MMBDown)
                {
                    mOpacity += amount;

                    if (EditorWindowController.Instance.TexturingModel != null)
                    {
                        if (mOpacity < 0)
                        {
                            mInnerRadius = 0.0f;
                        }

                        if (mOpacity > 255)
                        {
                            mInnerRadius = 255.0f;
                        }

                        HandleOpacityChanged(mOpacity);
                    }
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

            if (EditorWindowController.Instance.TerrainManager != null)
                EditorWindowController.Instance.TerrainManager.HandleInnerRadiusChanged(value);

        }

        private void HandleOuterRadiusChanged(float value)
        {
            mOuterRadius = value;
            WorldFrame.Instance.UpdateBrush(mInnerRadius, mOuterRadius);
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleOuterRadiusChanged(value);

            if (EditorWindowController.Instance.TerrainManager != null)
                EditorWindowController.Instance.TerrainManager.HandleOuterRadiusChanged(value);
        }

        private void HandleIntensityChanged(float value)
        {
            mIntensity = value;
            if (EditorWindowController.Instance.TerrainManager != null)
                EditorWindowController.Instance.TerrainManager.HandleIntensityChanged(value);

        }

        private void HandleAmountChanged(float value)
        {
            mAmount = value;
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleAmoutChanged(value);
        }

        private void HandleOpacityChanged(float value)
        {
            mOpacity = value;
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleOpacityChanged(value);
        }
    }
}
