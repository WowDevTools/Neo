using System.Windows;
using System.Windows.Controls;
using Neo.IO;
using Neo.UI.Models;

namespace Neo.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for AssetBrowser.xaml
    /// </summary>
    public partial class AssetBrowser
    {
        public DirectoryEntry RootDirectory { get { return FileManager.Instance.FileListing.RootEntry; } }

        public AssetBrowser()
        {
            DataContext = new AssetBrowserViewModel(this);
            InitializeComponent();
        }

        private void AssetBrowser_ItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var viewModel = DataContext as AssetBrowserViewModel;
            if (viewModel == null)
            {
	            return;
            }

	        viewModel.Handle_BrowserSelectionChanged(AssetTreeView.SelectedItem as AssetBrowserDirectory);
        }

        private void HideUnknownFiles_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as AssetBrowserViewModel;
            if (viewModel == null)
            {
	            return;
            }

	        var cb = sender as CheckBox;
            if (cb == null)
            {
	            return;
            }

	        viewModel.HideUnknownFiles = cb.IsChecked ?? false;
        }

        private void ShowTextures_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as AssetBrowserViewModel;
            if (viewModel == null)
            {
	            return;
            }

	        var cb = sender as CheckBox;
            if (cb == null)
            {
	            return;
            }

	        viewModel.ShowTextures = cb.IsChecked ?? false;
        }

        private void ShowModels_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as AssetBrowserViewModel;
            if (viewModel == null)
            {
	            return;
            }

	        var cb = sender as CheckBox;
            if (cb == null)
            {
	            return;
            }

	        viewModel.ShowModels = cb.IsChecked ?? false;
        }

        private void SpecularTextures_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as AssetBrowserViewModel;
            if (viewModel == null)
            {
	            return;
            }

	        var cb = sender as CheckBox;
            if (cb == null)
            {
	            return;
            }

	        viewModel.ShowSpecularTextures = cb.IsChecked ?? false;
        }

        private void FileBox_ItemSelected(object sender, RoutedEventArgs e)
        {
            if (SelectedFilesListView.SelectedItem == null)
            {
	            return;
            }

	        var viewModel = DataContext as AssetBrowserViewModel;
            if (viewModel == null)
            {
	            return;
            }

	        viewModel.Handle_FileClicked(SelectedFilesListView.SelectedItem as AssetBrowserFilePreviewElement);
        }

        private void ExportSelected_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as AssetBrowserViewModel;
            if (viewModel == null)
            {
	            return;
            }

	        viewModel.HandleExportSelectedFile();
        }

        private void ExportFolder_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as AssetBrowserViewModel;
            if (viewModel == null)
            {
	            return;
            }

	        viewModel.HandleExportSelectedFolder();
        }

        private void ImportFile_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as AssetBrowserViewModel;
            if (viewModel == null)
            {
	            return;
            }

	        viewModel.HandleImportFile();
        }
    }
}
