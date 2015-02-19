using System;
using System.Windows;
using System.Windows.Threading;
using WoWEditor6.UI.Models;

namespace WoWEditor6.UI.Components
{
    /// <summary>
    /// Interaction logic for AssetBrowserFilePreview.xaml
    /// </summary>
    public partial class AssetBrowserFilePreview
    {
        public AssetBrowserFile FileEntry { get; private set; }

        public AssetBrowserFilePreview(AssetBrowserFile file, AssetBrowserViewModel viewModel)
        {
            FileEntry = file;
            DataContext = file;
            InitializeComponent();

            if (file.Extension == ".blp")
            {
                if (viewModel.HideKnownFileNames)
                    FileNameBlock.Visibility = Visibility.Hidden;
                LoadImage(file);
            }

        }

        public void UpdateState(AssetBrowserViewModel model)
        {
            FileNameBlock.Visibility = model.HideKnownFileNames ? Visibility.Hidden : Visibility.Visible;
        }

        private void LoadImage(AssetBrowserFile file)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => PreviewImage.Source = WpfImageSource.FromTexture(file.FullPath)));
        }
    }
}
