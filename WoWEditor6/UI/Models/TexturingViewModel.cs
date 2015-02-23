using WoWEditor6.UI.Dialogs;

namespace WoWEditor6.UI.Models
{
    class TexturingViewModel
    {
        private readonly TexturingWidget mWidget;

        public TexturingViewModel(TexturingWidget widget)
        {
            EditorWindowController.Instance.TexturingModel = this;
            mWidget = widget;
        }

        public void HandleSelectFromAssets()
        {
            EditorWindowController.Instance.ShowAssetBrowser();
        }

        public void HandleInnerRadiusChanged(float newRadius)
        {
            mWidget.InnerRadiusSlider.Value = newRadius;
        }

        public void HandleOuterRadiusChanged(float newRadius)
        {
            mWidget.OuterRadiusSlider.Value = newRadius;
        }
    }
}
