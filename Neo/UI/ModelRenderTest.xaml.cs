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

namespace WoWEditor6.UI
{
    /// <summary>
    /// Interaction logic for ModelRenderTest.xaml
    /// </summary>
    public partial class ModelRenderTest
    {
        public ModelRenderTest()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var text = DisplayIdBox.Text;
            int displayId;
            if (int.TryParse(text, out displayId) == false)
                return;

            ModelControl.SetCreatureDisplayEntry(displayId);
        }
    }
}
