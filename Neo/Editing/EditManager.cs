using System;
using Neo.Scene;
using Neo.UI;
using Neo.Utils;
using OpenTK;
using OpenTK.Input;
using Point = System.Drawing.Point;

namespace Neo.Editing
{
	internal class EditManager
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
            get { return this.mChangeType; }
        }

        public float InnerRadius
        {
            get { return this.mInnerRadius; }
            set { HandleInnerRadiusChanged(value); }
        }

        public float OuterRadius
        {
            get { return this.mOuterRadius; }
            set { HandleOuterRadiusChanged(value); }
        }

        public float Intensity
        {
            get { return this.mIntensity; }
            set { HandleIntensityChanged(value); }
        }

        public float Amount
        {
            get { return this.mAmount; }
            set { HandleAmountChanged(value); }
        }

        public float Opacity
        {
            get { return this.mOpacity; }
            set { HandleOpacityChanged(value); }

        }

        public float PenSensivity
        {
            get { return this.mPenSensivity; }
            set { HandlePenSensivityChanged(value);  }
        }

        public bool IsTabletOn
        {
            get { return this.mIsTabletOn; }
            set { HandleTabletControlChanged(value);  }
        }

        public bool IsSprayOn
        {
            get { return this.mIsSprayOn;  }
            set { HandleSprayModeChanged(value);  }
        }

        public bool IsTablet_RChange
        {
            get { return this.mIsTablet_RChange; }
            set { HandleTabletRadiusChanged(value); }
        }

        public bool IsTablet_IRChange
        {
            get { return this.mIsTablet_IRChange; }
            set { HandleTabletInnerRadiusChanged(value); }
        }

        public bool IsTablet_PChange
        {
            get { return this.mIsTablet_PChange; }
            set { HandleTabletControlPressureChanged(value); }
        }

        public float Amplitude
        {
            get { return this.mAmplitude;  }
            set { HandleAllowedAmplitudeChanged(value); }
        }

        public float InnerAmplitude
        {
            get { return this.mInnerAmplitude; }
            set { HandleAllowedInnerAmplitudeChanged(value); }
        }

        public float SprayParticleSize
        {
            get { return this.mSprayParticleSize;  }
            set { HandleParticleSizeChanged(value);  }
        }

        public float SprayParticleAmount
        {
            get { return this.mSprayParticleAmount; }
            set { HandleParticleAmountChanged(value); }
        }

        public float SprayParticleHarndess
        {
            get { return this.mSprayParticleHardness;  }
            set { HandleParticleHardnessChanged(value);  }
        }

        public bool IsSpraySolidInnerRadius
        {
            get { return this.mIsSpraySolidInnerRadius; }
            set { HandleSpraySolidInnerRadiusChanged(value);  }
        }

        public bool IsTexturing { get { return (this.CurrentMode & EditMode.Texturing) != 0; } }

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

            var diff = DateTime.Now - this.mLastChange;
            if (diff.TotalMilliseconds < (this.IsTexturing ? 40 : 20))
            {
	            return;
            }

	        this.mLastChange = DateTime.Now;
	        if ((this.CurrentMode & EditMode.Sculpting) != 0)
	        {
		        TerrainChangeManager.Instance.OnChange(diff);
	        }
            else if ((this.CurrentMode & EditMode.Texturing) != 0)
	        {
		        TextureChangeManager.Instance.OnChange(diff);
	        }
            else if ((this.CurrentMode & EditMode.Chunk) != 0)
	        {
		        ChunkEditManager.Instance.OnChange(diff);
	        }


	        KeyboardState keyboardState = Keyboard.GetState();
	        var altDown = keyboardState.IsKeyDown(Key.AltLeft);
	        var spaceDown =  keyboardState.IsKeyDown(Key.Space);
	        var tDown = keyboardState.IsKeyDown(Key.T);

	        MouseState mouseState = Mouse.GetState();
	        var leftDown = mouseState.IsButtonDown(MouseButton.Left);
	        var rightDown = mouseState.IsButtonDown(MouseButton.Right);
	        var middleDown = mouseState.IsButtonDown(MouseButton.Middle);

	        Point curPos = InterfaceHelper.GetCursorPosition();
	        var amount = -(this.mLastCursorPosition.X - curPos.X) / 32.0f;

            if (this.mIsTabletOn) // All tablet control editing is here.
            {
                if (this.mIsTablet_PChange)
                {
                    if (EditorWindowController.Instance.TexturingModel != null)
                    {
	                    this.mAmount = TabletManager.Instance.TabletPressure * this.mPenSensivity / 10.0f;
                        HandleAmountChanged(this.mAmount);
                    }

                    if (EditorWindowController.Instance.TerrainManager != null)
                    {
	                    this.mIntensity = TabletManager.Instance.TabletPressure * this.mPenSensivity / 10.0f;
                        HandleIntensityChanged(this.mIntensity);
                    }
                }

                if (this.mIsTablet_RChange) // If outer radius change is enabled.
                {
                    if (EditorWindowController.Instance.TexturingModel != null)
                    {
	                    this.mOuterRadius = Math.Max( TabletManager.Instance.TabletPressure * (this.mAmplitude / 10.0f), 0.1f );

	                    this.mInnerRadius = Math.Min(this.mInnerRadius, this.mAmplitude);
	                    this.mInnerRadius = Math.Min(this.mInnerRadius, this.mOuterRadius);


                        HandleOuterRadiusChanged(this.mOuterRadius);
                        HandleInnerRadiusChanged(this.mInnerRadius);
                    }

                }

                if (this.mIsTablet_IRChange) // If inner radius change is enabled.
                {
                    if (EditorWindowController.Instance.TexturingModel != null)
                    {
	                    this.mInnerRadius = Math.Max( TabletManager.Instance.TabletPressure * (this.mInnerAmplitude / 10.0f), 0.1f);
	                    this.mInnerRadius = Math.Min(this.mInnerRadius, this.mOuterRadius);


                        HandleInnerRadiusChanged(this.mInnerRadius);

                    }
                }
            }


            /*if (!LMBDown && IsTabletOn) // When tablet mode is on we always set those to minimal value (NEEDS TO BE MOVED OUT OF HERE).
            {
                mAmount = 1.0f;
                mIntensity = 1.0f;
            }*/


            if (curPos != this.mLastCursorPosition)
            {
                if (altDown && rightDown)
                {
	                this.mInnerRadius += amount;

	                this.mInnerRadius = Math.Max(this.mInnerRadius, 0.1f);
	                this.mInnerRadius = Math.Min(this.mInnerRadius, 200.0f);
	                this.mInnerRadius = Math.Min(this.mInnerRadius, this.mOuterRadius);

                    HandleInnerRadiusChanged(this.mInnerRadius);

                }


                if (altDown && leftDown)
                {
	                this.mInnerRadius += amount;
	                this.mOuterRadius += amount;

	                this.mInnerRadius = Math.Max(this.mInnerRadius, 0.1f);
	                this.mInnerRadius = Math.Min(this.mInnerRadius, 200.0f);

	                this.mOuterRadius = Math.Max(this.mOuterRadius, 0.1f);
	                this.mOuterRadius = Math.Min(this.mOuterRadius, 200.0f);

	                this.mInnerRadius = Math.Min(this.mInnerRadius, this.mOuterRadius);

                    HandleInnerRadiusChanged(this.mInnerRadius);
                    HandleOuterRadiusChanged(this.mOuterRadius);


                }

                if(spaceDown && leftDown)
                {
	                this.mIntensity += amount;
	                this.mAmount += amount;

                    if (EditorWindowController.Instance.TerrainManager != null)
                    {
	                    this.mIntensity = Math.Max(this.mIntensity, 1.0f);
	                    this.mIntensity = Math.Min(this.mIntensity, 40.0f);

                        HandleIntensityChanged(this.mIntensity);
                    }

                    if (EditorWindowController.Instance.TexturingModel != null)
                    {
	                    this.mAmount = Math.Max(this.mAmount, 1.0f);
	                    this.mAmount = Math.Min(this.mAmount, 40.0f);

                        HandleAmountChanged(this.mAmount);
                    }

                }

                if(altDown && middleDown)
                {
	                this.mOpacity += amount;

                    if (EditorWindowController.Instance.TexturingModel != null)
                    {
	                    this.mOpacity = Math.Max(this.mOpacity, 0.0f);
	                    this.mOpacity = Math.Min(this.mOpacity, 255.0f);

                        HandleOpacityChanged(this.mOpacity);
                    }
                }

                if(spaceDown && middleDown)
                {
	                this.mPenSensivity += amount / 32.0f;

                    if(EditorWindowController.Instance.TexturingModel != null)
                    {
	                    this.mPenSensivity = Math.Max(this.mPenSensivity, 0.1f);
	                    this.mPenSensivity = Math.Min(this.mPenSensivity, 1.0f);

                        HandlePenSensivityChanged(this.mPenSensivity);
                    }
                }

                if (spaceDown && tDown) // DOES NOT WORK PROPERLY. NEEDS TO BE MOVED OUT OF THIS METHOD.
                {
                    if (this.mIsTabletOn)
                    {
	                    this.mIsTabletOn = false;
                    }
                    else
                    {
	                    this.mIsTabletOn = true;
                    }
                    HandleTabletControlChanged(this.mIsTabletOn);
                }

	            this.mLastCursorPosition = curPos;
            }
        }

        public void EnableShading()
        {
	        this.CurrentMode |= EditMode.Sculpting;
	        this.CurrentMode &= ~EditMode.Texturing;
	        this.CurrentMode &= ~EditMode.Chunk;
	        this.mChangeType = TerrainChangeType.Shading;

        }

        public void EnableSculpting()
        {
	        this.CurrentMode |= EditMode.Sculpting;
	        this.CurrentMode &= ~EditMode.Texturing;
	        this.CurrentMode &= ~EditMode.Chunk;
	        this.mChangeType = TerrainChangeType.Elevate;
        }

        public void DisableSculpting()
        {
	        this.CurrentMode &= ~EditMode.Sculpting;
        }

        public void EnableTexturing()
        {
	        this.CurrentMode |= EditMode.Texturing;
	        this.CurrentMode &= ~EditMode.Sculpting;
	        this.CurrentMode &= ~EditMode.Chunk;
        }

        public void DisableTexturing()
        {
	        this.CurrentMode &= ~EditMode.Texturing;
        }

        public void EnableChunkEditing()
        {
	        this.CurrentMode = EditMode.Chunk;
        }

        public void DisableChunkEditing()
        {
	        this.CurrentMode &= ~EditMode.Chunk;
        }

        private void HandleInnerRadiusChanged(float value)
        {
	        this.mInnerRadius = value;
            WorldFrame.Instance.UpdateBrush(this.mInnerRadius, this.mOuterRadius);
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleInnerRadiusChanged(value);
            }

	        if (EditorWindowController.Instance.TerrainManager != null)
	        {
		        EditorWindowController.Instance.TerrainManager.HandleInnerRadiusChanged(value);
	        }

	        if (EditorWindowController.Instance.ShadingModel != null)
	        {
		        EditorWindowController.Instance.ShadingModel.HandleInnerRadiusChanged(value);
	        }
        }

        private void HandleOuterRadiusChanged(float value)
        {
	        this.mOuterRadius = value;
            WorldFrame.Instance.UpdateBrush(this.mInnerRadius, this.mOuterRadius);
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleOuterRadiusChanged(value);
            }

	        if (EditorWindowController.Instance.TerrainManager != null)
	        {
		        EditorWindowController.Instance.TerrainManager.HandleOuterRadiusChanged(value);
	        }

	        if (EditorWindowController.Instance.ShadingModel != null)
	        {
		        EditorWindowController.Instance.ShadingModel.HandleOuterRadiusChanged(value);
	        }
        }

        private void HandleIntensityChanged(float value)
        {
	        this.mIntensity = value;
            if (EditorWindowController.Instance.TerrainManager != null)
            {
	            EditorWindowController.Instance.TerrainManager.HandleIntensityChanged(value);
            }

	        if (EditorWindowController.Instance.ShadingModel != null)
	        {
		        EditorWindowController.Instance.ShadingModel.HandleIntensityChanged(value);
	        }
        }

        private void HandleAmountChanged(float value)
        {
	        this.mAmount = value;
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleAmoutChanged(value);
            }
        }

        private void HandleOpacityChanged(float value)
        {
	        this.mOpacity = value;
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleOpacityChanged(value);
            }
        }

        private void HandlePenSensivityChanged(float value)
        {
	        this.mPenSensivity = value;
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandlePenSensivityChanged(value);
            }
	        if (EditorWindowController.Instance.TerrainManager != null)
	        {
		        EditorWindowController.Instance.TerrainManager.HandlePenSensivityChanged(value);
	        }
        }

        private void HandleTabletControlChanged(bool value)
        {
	        this.mIsTabletOn = value;
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleTabletControlChanged(value);
            }
	        if (EditorWindowController.Instance.TerrainManager != null)
	        {
		        EditorWindowController.Instance.TerrainManager.HandleTabletControlChanged(value);
	        }
	        if (EditorWindowController.Instance.ShadingModel != null)
	        {
		        EditorWindowController.Instance.ShadingModel.HandleTabletControlChanged(value);
	        }
        }

        private void HandleTabletRadiusChanged(bool value)
        {
	        this.mIsTablet_RChange = value;
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleTabletChangeRadiusChanged(value);
            }
        }

        private void HandleTabletInnerRadiusChanged(bool value)
        {
	        this.mIsTablet_IRChange = value;
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleTabletChangeInnerRadiusChanged(value);
            }
        }

        private void HandleAllowedAmplitudeChanged(float value)
        {
	        this.mAmplitude = value;
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleAllowedAmplitudeChanged(value);
            }
        }

        private void HandleAllowedInnerAmplitudeChanged(float value)
        {
	        this.mInnerAmplitude = value;
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleAllowedInnerAmplitudeChanged(value);
            }
        }

        private void HandleTabletControlPressureChanged(bool value)
        {
	        this.mIsTablet_PChange = value;
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleTabletControlPressureChanged(value);
            }
        }

        private void HandleSprayModeChanged(bool value)
        {
	        this.mIsSprayOn = value;
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleSprayModeChanged(value);
            }
        }

        private void HandleParticleSizeChanged(float value)
        {
	        this.mSprayParticleSize = value;
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleParticleSizeChanged(value);
            }
        }

        private void HandleParticleAmountChanged(float value)
        {
	        this.mSprayParticleAmount = value;
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleParticleAmountChanged(value);
            }
        }

        private void HandleParticleHardnessChanged(float value)
        {
	        this.mSprayParticleHardness = value;
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleParticleHardnessChanged(value);
            }
        }

        private void HandleSpraySolidInnerRadiusChanged(bool value)
        {
	        this.mIsSpraySolidInnerRadius = value;
            if (EditorWindowController.Instance.TexturingModel != null)
            {
	            EditorWindowController.Instance.TexturingModel.HandleSpraySolidInnerRadiusChanged(value);
            }
        }
    }
}
