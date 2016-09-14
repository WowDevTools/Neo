using Neo.UI.ViewModels;

namespace Neo.UI.Widgets
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
