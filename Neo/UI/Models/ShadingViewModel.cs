using System.Collections;
using System.Windows;
using Neo.UI.Dialogs;
using System.Windows.Forms;
using SharpDX;
using Neo.UI.Widgets;

namespace Neo.UI.Models
{
	internal class ShadingViewModel
    {
        private readonly ShadingWidget mWidget;
        private bool mIsValueChangedSurpressed;

        public ShadingWidget Widget { get { return this.mWidget; } }

        public ShadingViewModel(ShadingWidget widget)
        {
            if (EditorWindowController.Instance != null)
            {
	            EditorWindowController.Instance.ShadingModel = this;
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

        public void HandleShadingMultiplierChanged(Vector3 value)
        {
            if (this.mIsValueChangedSurpressed)
            {
	            return;
            }

	        this.mWidget.RedBox.Text = value.X.ToString();
	        this.mWidget.GreenBox.Text = value.Y.ToString();
	        this.mWidget.BlueBox.Text = value.Z.ToString();
        }
        #endregion

    }
}
