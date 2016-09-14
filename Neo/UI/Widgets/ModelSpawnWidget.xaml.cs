using System.Windows;
using WoWEditor6.UI.Models;

namespace WoWEditor6.UI.Widgets
{
    /// <summary>
    /// Interaction logic for ModelSpawnWidget.xaml
    /// </summary>
    public partial class ModelSpawnWidget
    {
        public ModelSpawnWidget()
        {
            DataContext = new ObjectSpawnModel();
            InitializeComponent();
        }

        private void PlaceModel_Click(object sender, RoutedEventArgs e)
        {
            Editing.ModelSpawnManager.Instance.SelectModel(ModelNameBox.Text);
        }

        private void CopyModelButtonClick(object sender, RoutedEventArgs e)
        {
            Editing.ModelSpawnManager.Instance.CopyClickedModel();
        }
    }
}
