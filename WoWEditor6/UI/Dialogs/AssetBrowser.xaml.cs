using System.Windows;
using System.Windows.Controls;
using WoWEditor6.IO;
using WoWEditor6.UI.Models;

namespace WoWEditor6.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for AssetBrowser.xaml
    /// </summary>
    public partial class AssetBrowser
    {
        public DirectoryEntry RootDirectory { get { return FileManager.Instance.FileListing.RootEntry; } }

        public AssetBrowser()
        {
            DataContext = new Models.AssetBrowserViewModel(this);
            InitializeComponent();
        }

        private void AssetBrowser_ItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var viewModel = DataContext as Models.AssetBrowserViewModel;
            if (viewModel == null)
                return;

            viewModel.Handle_BrowserSelectionChanged(AssetTreeView.SelectedItem as AssetBrowserDirectory);
        }
    }
}
