using System.Collections;
using System.Windows;
using WoWEditor6.UI.Dialogs;
using System.Windows.Forms;
using SharpDX;
using WoWEditor6.UI.Widget;

namespace WoWEditor6.UI.Models
{
    class IEditingViewModel
    {
        private readonly IEditingWidget mWidget;
        private bool mIsValueChangedSurpressed;

        public IEditingWidget Widget { get { return mWidget; } }

        public IEditingViewModel(IEditingWidget widget)
        {
            if (EditorWindowController.Instance != null)
                EditorWindowController.Instance.IEditingModel = this;

            mWidget = widget;
        }

        public void SwitchWidgets(int widget)
        {
            switch (widget)
            {
                case 0:
                {
                    mWidget.TexturingWidget.Visibility = Visibility.Hidden;
                    mWidget.TerrainSettingsWidget.Visibility = Visibility.Hidden;
                    break;
                }

                case 1:
                {
                    mWidget.TexturingWidget.Visibility = Visibility.Hidden;
                    mWidget.TerrainSettingsWidget.Visibility = Visibility.Visible;
                    break;
                }

                case 3:
                {
                    mWidget.TexturingWidget.Visibility = Visibility.Visible;
                    mWidget.TerrainSettingsWidget.Visibility = Visibility.Hidden;
                    break;
                }
            }

        }


    }
}
