using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Neo.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for KeyBindingControl.xaml
    /// </summary>
    public partial class KeyBindingControl : UserControl
    {
        public KeyBindingControl()
        {
            InitializeComponent();
        }

        public Button ToggleButton { get { return Button; } }
        public TextBlock BindingLabel { get { return Label; } }
    }
}
