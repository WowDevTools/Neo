using System.Windows;

namespace WoWEditor6.UI.Widgets
{
    /// <summary>
    /// Interaction logic for ModelSpawnWidget.xaml
    /// </summary>
    public partial class ModelSpawnWidget
    {
        public ModelSpawnWidget()
        {
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
