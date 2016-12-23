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

        public ShadingWidget Widget { get { return mWidget; } }

        public ShadingViewModel(ShadingWidget widget)
        {
            if (EditorWindowController.Instance != null)
            {
	            EditorWindowController.Instance.ShadingModel = this;
            }

	        mWidget = widget;
        }

        #region HandleSliders
        public void HandleIntensitySlider(float value)
        {
            mIsValueChangedSurpressed = true;
            Editing.TerrainChangeManager.Instance.Amount = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleInnerRadiusSlider(float value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.InnerRadius = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleOuterRadiusSlider(float value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.OuterRadius = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleShadingMultiplier(Vector3 value)
        {
            mIsValueChangedSurpressed = true;
            Editing.TerrainChangeManager.Instance.ShadingMultiplier = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleType(Editing.TerrainChangeType value)
        {
            mIsValueChangedSurpressed = true;
            Editing.TerrainChangeManager.Instance.ChangeType = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleAlgorithm(Editing.TerrainAlgorithm value)
        {
            mIsValueChangedSurpressed = true;
            Editing.TerrainChangeManager.Instance.ChangeAlgorithm = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleAlignToGround(bool value)
        {
            mIsValueChangedSurpressed = true;
            Editing.TerrainChangeManager.Instance.AlignModelsToGround = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandlePenSensivity(float value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.PenSensivity = value;
            mIsValueChangedSurpressed = false;
        }

        public void HandleTabletControl(bool value)
        {
            mIsValueChangedSurpressed = true;
            Editing.EditManager.Instance.IsTabletOn = value;
            mIsValueChangedSurpressed = false;
        }
        #endregion

        #region HandleChange
        public void HandleInnerRadiusChanged(float newRadius)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.InnerRadiusSlider.Value = newRadius;
        }

        public void HandleOuterRadiusChanged(float newRadius)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.OuterRadiusSlider.Value = newRadius;
        }

        public void HandleIntensityChanged(float newAmount)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.IntensitySlider.Value = newAmount;
        }

        public void HandlePenSensivityChanged(float newSensivity)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.Tablet_SensivitySlider.Value = newSensivity;
        }

        public void HandleTabletControlChanged(bool newIsTabletOn)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.TabletControlBox.IsChecked = newIsTabletOn;
        }

        public void HandleShadingMultiplierChanged(Vector3 value)
        {
            if (mIsValueChangedSurpressed)
            {
	            return;
            }

	        mWidget.RedBox.Text = value.X.ToString();
            mWidget.GreenBox.Text = value.Y.ToString();
            mWidget.BlueBox.Text = value.Z.ToString();
        }
        #endregion

    }
}
