using Neo.UI.Dialogs;
using System.Windows.Forms;
using SharpDX;

namespace Neo.UI.Models
{
	internal class SculptingViewModel
    {
        private readonly TerrainSettingsWidget mWidget;
        private bool mIsValueChangedSurpressed;

        public TerrainSettingsWidget Widget { get { return this.mWidget; } }

        public SculptingViewModel(TerrainSettingsWidget widget)
        {
            if (EditorWindowController.Instance != null)
            {
	            EditorWindowController.Instance.TerrainManager = this;
            }

	        this.mWidget = widget;
        }

        #region HandleSliders
        public void HandleIntensitySlider(float value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.TerrainChangeManager.Instance.Amount = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleInnerRadiusSlider(float value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.InnerRadius = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleOuterRadiusSlider(float value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.OuterRadius = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleShadingMultiplier(Vector3 value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.TerrainChangeManager.Instance.ShadingMultiplier = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleType(Editing.TerrainChangeType value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.TerrainChangeManager.Instance.ChangeType = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleAlgorithm(Editing.TerrainAlgorithm value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.TerrainChangeManager.Instance.ChangeAlgorithm = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleAlignToGround(bool value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.TerrainChangeManager.Instance.AlignModelsToGround = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandlePenSensivity(float value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.PenSensivity = value;
	        this.mIsValueChangedSurpressed = false;
        }

        public void HandleTabletControl(bool value)
        {
	        this.mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.IsTabletOn = value;
	        this.mIsValueChangedSurpressed = false;
        }
        #endregion

        #region HandleChange
        public void HandleInnerRadiusChanged(float newRadius)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.InnerRadiusSlider.Value = newRadius;
        }

        public void HandleOuterRadiusChanged(float newRadius)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.OuterRadiusSlider.Value = newRadius;
        }

        public void HandleIntensityChanged(float newAmount)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.IntensitySlider.Value = newAmount;
        }

        public void HandlePenSensivityChanged(float newSensivity)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.Tablet_SensivitySlider.Value = newSensivity;
        }

        public void HandleTabletControlChanged(bool newIsTabletOn)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.TabletControlBox.IsChecked = newIsTabletOn;
        }

        public void HandleTypeChanged(Editing.TerrainChangeType value)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        switch(value)
            {
                case Editing.TerrainChangeType.Blur:
	                this.mWidget.BlurRadio.IsChecked = true;
                    break;
                case Editing.TerrainChangeType.Elevate:
	                this.mWidget.RaiseRadio.IsChecked = true;
                    break;
                case Editing.TerrainChangeType.Flatten:
	                this.mWidget.FlattenRadio.IsChecked = true;
                    break;
                case Editing.TerrainChangeType.Shading:
	                this.mWidget.ShadingRadio.IsChecked = true;
                    break;
            }
        }

        public void HandleAlgorithmChanged(Editing.TerrainAlgorithm value)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        switch (value)
            {
                case Editing.TerrainAlgorithm.Flat:
	                this.mWidget.FlatRadio.IsChecked = true;
                    break;
                case Editing.TerrainAlgorithm.Linear:
	                this.mWidget.LinearRadio.IsChecked = true;
                    break;
                case Editing.TerrainAlgorithm.Quadratic:
	                this.mWidget.QuadraticRadio.IsChecked = true;
                    break;
                case Editing.TerrainAlgorithm.Trigonometric:
	                this.mWidget.TrigonometricRadio.IsChecked = true;
                    break;
            }
        }

        public void HandleAlignToGroundChanged(bool value)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.AlignModelsBox.IsChecked = value;
        }
        #endregion

        public void SwitchToSculpting()
        {
            Editing.EditManager.Instance.EnableSculpting();
        }
    }
}
