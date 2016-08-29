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
            mWidget = widget;
        }

        public void SwitchWidgets(int widget)
        {
            if(widget == 0)
            mWidget.TexturingWidget.Visibility = Visibility.Hidden;
            mWidget.TerrainSettingsWidget.Visibility = Visibility.Hidden;
        }


    }
}
