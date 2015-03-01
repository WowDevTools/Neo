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

namespace WoWEditor6.UI.Widgets
{
    /// <summary>
    /// Interaction logic for ModelSpawnWidget.xaml
    /// </summary>
    public partial class ModelSpawnWidget : UserControl
    {
        public ModelSpawnWidget()
        {
            InitializeComponent();
        }

        private void PlaceModel_Click(object sender, RoutedEventArgs e)
        {
            Editing.ModelSpawnManager.Instance.SelectModel(ModelNameBox.Text);
        }
    }
}
