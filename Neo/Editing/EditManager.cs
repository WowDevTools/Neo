using System;
using Gdk;
using Neo.Scene;
using Neo.UI;
using Neo.Utils;
using OpenTK;
using Point = System.Drawing.Point;

namespace Neo.Editing
{
    class EditManager
    {
        public static EditManager Instance { get; private set; }

        private DateTime mLastChange = DateTime.Now;

	    private Point mLastCursorPosition = InterfaceHelper.GetCursorPosition();

        private float mInnerRadius = 18.0f;
        private float mOuterRadius = 20.0f;
        private float mIntensity = 32.0f;
        private float mAmount = 32.0f;
        private float mOpacity = 255.0f;
        private float mPenSensivity;
        private float mAmplitude;
        private float mInnerAmplitude;
        private float mSprayParticleSize;
        private float mSprayParticleAmount;
        private float mSprayParticleHardness;

        private bool mIsTabletOn = false;
        private bool mIsTablet_RChange = false;
        private bool mIsTablet_IRChange = false;
        private bool mIsTablet_PChange = false;
        private bool mIsSprayOn = false;
        private bool mIsSpraySolidInnerRadius = false;

        private TerrainChangeType mChangeType;

        public TerrainChangeType ChangeType
        {
            get { return mChangeType; }
        }

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

        public float PenSensivity
        {
            get { return mPenSensivity; }
            set { HandlePenSensivityChanged(value);  }
        }

        public bool IsTabletOn
        {
            get { return mIsTabletOn; }
            set { HandleTabletControlChanged(value);  }
        }

        public bool IsSprayOn
        {
            get { return mIsSprayOn;  }
            set { HandleSprayModeChanged(value);  }
        }

        public bool IsTablet_RChange
        {
            get { return mIsTablet_RChange; }
            set { HandleTabletRadiusChanged(value); }
        }

        public bool IsTablet_IRChange
        {
            get { return mIsTablet_IRChange; }
            set { HandleTabletInnerRadiusChanged(value); }
        }

        public bool IsTablet_PChange
        {
            get { return mIsTablet_PChange; }
            set { HandleTabletControlPressureChanged(value); }
        }

        public float Amplitude
        {
            get { return mAmplitude;  }
            set { HandleAllowedAmplitudeChanged(value); }
        }

        public float InnerAmplitude
        {
            get { return mInnerAmplitude; }
            set { HandleAllowedInnerAmplitudeChanged(value); }
        }

        public float SprayParticleSize
        {
            get { return mSprayParticleSize;  }
            set { HandleParticleSizeChanged(value);  }
        }

        public float SprayParticleAmount
        {
            get { return mSprayParticleAmount; }
            set { HandleParticleAmountChanged(value); }
        }

        public float SprayParticleHarndess
        {
            get { return mSprayParticleHardness;  }
            set { HandleParticleHardnessChanged(value);  }
        }

        public bool IsSpraySolidInnerRadius
        {
            get { return mIsSpraySolidInnerRadius; }
            set { HandleSpraySolidInnerRadiusChanged(value);  }
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

	        // TODO: GDK hotkey implementation
            var keyState = new byte[256];
            UnsafeNativeMethods.GetKeyboardState(keyState);
            var altDown = KeyHelper.IsKeyDown(keyState, Key.Menu);
            var LMBDown = KeyHelper.IsKeyDown(keyState, Key.LButton);
            var RMBDown = KeyHelper.IsKeyDown(keyState, Key.RButton);
            var spaceDown = KeyHelper.IsKeyDown(keyState, Key.Space);
            var MMBDown = KeyHelper.IsKeyDown(keyState, Key.MButton);
            var tDown = KeyHelper.IsKeyDown(keyState, Key.T);

	        Point curPos = InterfaceHelper.GetCursorPosition();
	        var amount = -(mLastCursorPosition.X - curPos.X) / 32.0f;

            if (mIsTabletOn) // All tablet control editing is here.
            {
                if (mIsTablet_PChange)
                {
                    if (EditorWindowController.Instance.TexturingModel != null)
                    {
                        mAmount = TabletManager.Instance.TabletPressure * mPenSensivity / 10.0f;
                        HandleAmountChanged(mAmount);
                    }

                    if (EditorWindowController.Instance.TerrainManager != null)
                    {
                        mIntensity = TabletManager.Instance.TabletPressure * mPenSensivity / 10.0f;
                        HandleIntensityChanged(mIntensity);
                    }
                }

                if (mIsTablet_RChange) // If outer radius change is enabled.
                {
                    if (EditorWindowController.Instance.TexturingModel != null)
                    {
                        //float proportion = mInnerRadius / mOuterRadius;

                        mOuterRadius = TabletManager.Instance.TabletPressure * (mAmplitude / 10.0f);

                        if(mOuterRadius < 0.1f)
                        {
                            mOuterRadius = 0.1f;
                        }

                        if(mInnerRadius > mAmplitude)
                        {
                            mInnerRadius = mAmplitude;
                        }

                        if(mInnerRadius > mOuterRadius)
                        {
                            mInnerRadius = mOuterRadius;
                        }

                        HandleOuterRadiusChanged(mOuterRadius);
                        HandleInnerRadiusChanged(mInnerRadius);
                    }

                }

                if (mIsTablet_IRChange) // If inner radius change is enabled.
                {
                    if (EditorWindowController.Instance.TexturingModel != null)
                    {
                        mInnerRadius = TabletManager.Instance.TabletPressure * mInnerAmplitude / 10.0f;
                        if(mInnerRadius < 0.1f)
                        {
                            mInnerRadius = 0.1f;
                        }

                        if(mInnerRadius > mOuterRadius)
                        {
                            mInnerRadius = mOuterRadius;
                        }

                        HandleInnerRadiusChanged(mInnerRadius);

                    }
                }
            }


            /*if (!LMBDown && IsTabletOn) // When tablet mode is on we always set those to minimal value (NEEDS TO BE MOVED OUT OF HERE).
            {
                mAmount = 1.0f;
                mIntensity = 1.0f;
            }*/


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
                        mOuterRadius = 0.0f;
                    }

                    if (mOuterRadius > 200)
                    {
                        mOuterRadius = 200.0f;
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
                            mIntensity = 1.0f;
                        }

                        if (mIntensity > 40)
                        {
                            mIntensity = 40.0f;
                        }

                        HandleIntensityChanged(mIntensity);
                    }

                    if (EditorWindowController.Instance.TexturingModel != null)
                    {
                        if(mAmount < 1)
                        {
                            mAmount = 1.0f;
                        }

                        if (mAmount > 40)
                        {
                            mAmount = 40.0f;
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
                            mOpacity = 0.0f;
                        }

                        if (mOpacity > 255)
                        {
                            mOpacity = 255.0f;
                        }

                        HandleOpacityChanged(mOpacity);
                    }
                }

                if(spaceDown && MMBDown)
                {
                    mPenSensivity += amount / 32.0f;

                    if(EditorWindowController.Instance.TexturingModel != null)
                    {
                        if (mPenSensivity < 0.1f)
                        {
                            mPenSensivity = 0.1f;
                        }

                        if (mPenSensivity > 1.0f)
                        {
                            mPenSensivity = 1.0f;
                        }

                        HandlePenSensivityChanged(mPenSensivity);
                    }
                }

                if (spaceDown && tDown) // DOES NOT WORK PROPERLY. NEEDS TO BE MOVED OUT OF THIS METHOD.
                {
                    if (mIsTabletOn)
                    {
                        mIsTabletOn = false;
                    }
                    else
                    {
                        mIsTabletOn = true;
                    }
                    HandleTabletControlChanged(mIsTabletOn);
                }

	            mLastCursorPosition = curPos;
            }
        }

        public void EnableShading()
        {
            CurrentMode |= EditMode.Sculpting;
            CurrentMode &= ~EditMode.Texturing;
            mChangeType = TerrainChangeType.Shading;

        }

        public void EnableSculpting()
        {
            CurrentMode |= EditMode.Sculpting;
            CurrentMode &= ~EditMode.Texturing;
            mChangeType = TerrainChangeType.Elevate;
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

            if (EditorWindowController.Instance.ShadingModel != null)
                EditorWindowController.Instance.ShadingModel.HandleInnerRadiusChanged(value);


        }

        private void HandleOuterRadiusChanged(float value)
        {
            mOuterRadius = value;
            WorldFrame.Instance.UpdateBrush(mInnerRadius, mOuterRadius);
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleOuterRadiusChanged(value);

            if (EditorWindowController.Instance.TerrainManager != null)
                EditorWindowController.Instance.TerrainManager.HandleOuterRadiusChanged(value);

            if (EditorWindowController.Instance.ShadingModel != null)
                EditorWindowController.Instance.ShadingModel.HandleOuterRadiusChanged(value);
        }

        private void HandleIntensityChanged(float value)
        {
            mIntensity = value;
            if (EditorWindowController.Instance.TerrainManager != null)
                EditorWindowController.Instance.TerrainManager.HandleIntensityChanged(value);

            if (EditorWindowController.Instance.ShadingModel != null)
                EditorWindowController.Instance.ShadingModel.HandleIntensityChanged(value);

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

        private void HandlePenSensivityChanged(float value)
        {
            mPenSensivity = value;
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandlePenSensivityChanged(value);
            if (EditorWindowController.Instance.TerrainManager != null)
                EditorWindowController.Instance.TerrainManager.HandlePenSensivityChanged(value);
        }

        private void HandleTabletControlChanged(bool value)
        {
            mIsTabletOn = value;
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleTabletControlChanged(value);
            if (EditorWindowController.Instance.TerrainManager != null)
                EditorWindowController.Instance.TerrainManager.HandleTabletControlChanged(value);
            if (EditorWindowController.Instance.ShadingModel != null)
                EditorWindowController.Instance.ShadingModel.HandleTabletControlChanged(value);
        }

        private void HandleTabletRadiusChanged(bool value)
        {
            mIsTablet_RChange = value;
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleTabletChangeRadiusChanged(value);
        }

        private void HandleTabletInnerRadiusChanged(bool value)
        {
            mIsTablet_IRChange = value;
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleTabletChangeInnerRadiusChanged(value);
        }

        private void HandleAllowedAmplitudeChanged(float value)
        {
            mAmplitude = value;
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleAllowedAmplitudeChanged(value);
        }

        private void HandleAllowedInnerAmplitudeChanged(float value)
        {
            mInnerAmplitude = value;
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleAllowedInnerAmplitudeChanged(value);
        }

        private void HandleTabletControlPressureChanged(bool value)
        {
            mIsTablet_PChange = value;
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleTabletControlPressureChanged(value);
        }

        private void HandleSprayModeChanged(bool value)
        {
            mIsSprayOn = value;
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleSprayModeChanged(value);
        }

        private void HandleParticleSizeChanged(float value)
        {
            mSprayParticleSize = value;
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleParticleSizeChanged(value);
        }

        private void HandleParticleAmountChanged(float value)
        {
            mSprayParticleAmount = value;
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleParticleAmountChanged(value);
        }

        private void HandleParticleHardnessChanged(float value)
        {
            mSprayParticleHardness = value;
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleParticleHardnessChanged(value);
        }

        private void HandleSpraySolidInnerRadiusChanged(bool value)
        {
            mIsSpraySolidInnerRadius = value;
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleSpraySolidInnerRadiusChanged(value);
        }
    }
}
