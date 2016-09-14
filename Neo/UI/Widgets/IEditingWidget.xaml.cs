using System.Windows;
using System.Windows.Controls;
using Neo.UI.Models;

namespace Neo.UI.Widget
{

    public partial class IEditingWidget
    {
        public IEditingWidget()
        {
            DataContext = new IEditingViewModel(this);
            InitializeComponent();
        }

        public void SwitchWidgets(int widget)
        {
            var model = DataContext as IEditingViewModel;
            if (model == null)
                return;

            model.SwitchWidgets(widget);
        }
    }
}
