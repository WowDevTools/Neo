using System.Windows;
using System.Windows.Controls;
using WoWEditor6.UI.Models;

namespace WoWEditor6.UI.Widgets
{

    public partial class IEditingWidget
    {
        public IEditingWidget()
        {
            DataContext = new IEditingViewModel(this);
            InitializeComponent();
        }
    }
}
