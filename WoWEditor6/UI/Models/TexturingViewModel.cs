using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WoWEditor6.UI.Dialogs;

namespace WoWEditor6.UI.Models
{
    class TexturingViewModel
    {
        private readonly TexturingWidget mWidget;
        private bool mIsValueChangedSurpressed;
        private bool mIsTileSelected;

        public bool IsTileSelected { get { return mIsTileSelected; } }

        public TexturingViewModel(TexturingWidget widget)
        {
            EditorWindowController.Instance.TexturingModel = this;
            mWidget = widget;
        }

        public void SetSelectedTileTextures(IEnumerable<string> textures)
        {
            mWidget.CurrentTileWrapPanel.Items.Clear();
             
            foreach (var tex in textures)
            {
                var img = new Image
                {
                    Width = 96,
                    Height = 96,
                    Margin = new Thickness(5, 5, 0, 0)
                };

                mWidget.CurrentTileWrapPanel.Items.Add(img);

                var texName = tex;
                Task.Factory.StartNew(() =>
                {
                    var wpfImage = WpfImageSource.FromTexture(texName);
                    Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => img.Source = wpfImage));
                });
            }

            mIsTileSelected = true;
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
