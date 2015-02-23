using WoWEditor6.UI.Dialogs;

namespace WoWEditor6.UI.Models
{
    class TexturingViewModel
    {
        private readonly TexturingWidget mWidget;
        private bool mIsValueChangedSurpressed;

        public TexturingViewModel(TexturingWidget widget)
        {
            EditorWindowController.Instance.TexturingModel = this;
            mWidget = widget;
        }

        public void HandleSelectFromAssets()
        {
            EditorWindowController.Instance.ShowAssetBrowser();
        }

        public void HandleAmountSlider(float value)
        {
            mIsValueChangedSurpressed = true;
            Editing.TextureChangeManager.Instance.Amount = value;
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

        public void HandleInnerRadiusChanged(float newRadius)
        {
            if (mIsValueChangedSurpressed)
                return;

            mWidget.InnerRadiusSlider.Value = newRadius;
        }

        public void HandleOuterRadiusChanged(float newRadius)
        {
            if (mIsValueChangedSurpressed)
                return;

            mWidget.OuterRadiusSlider.Value = newRadius;
        }

        public void HandleAmoutChanged(float newAmount)
        {
            if (mIsValueChangedSurpressed)
                return;

            mWidget.AmountSlider.Value = newAmount;
        }
    }
}
