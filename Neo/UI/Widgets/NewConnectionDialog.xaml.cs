using WoWEditor6.UI.ViewModels;

namespace WoWEditor6.UI.Widgets
{
    public partial class NewConnectionDialog
    {
        public NewConnectionDialog()
        {
            InitializeComponent();
            DataContext = new NewConnectionViewModel();
        }
    }
}
