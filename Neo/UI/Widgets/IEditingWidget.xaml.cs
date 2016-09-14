using System.Windows;
using System.Windows.Controls;
using WoWEditor6.UI.Models;

namespace WoWEditor6.UI.Widget
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
