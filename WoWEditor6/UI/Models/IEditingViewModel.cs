using WoWEditor6.UI.Dialogs;
using System.Windows.Forms;
using SharpDX;
using WoWEditor6.UI.Widget;

namespace WoWEditor6.UI.Models
{
    class IEditingViewModel
    {
        private readonly IEditingWidget mWidget;

        public IEditingWidget Widget { get { return mWidget; } }

        public IEditingViewModel(IEditingWidget widget)
        {
            mWidget = widget;
        }

    }
}
