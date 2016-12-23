using System.Windows;
using Neo.UI.Models;

namespace Neo.UI.Widgets
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
            Editing.ModelSpawnManager.Instance.SelectModel(this.ModelNameBox.Text);
        }

        private void CopyModelButtonClick(object sender, RoutedEventArgs e)
        {
            Editing.ModelSpawnManager.Instance.CopyClickedModel();
        }
    }
}
