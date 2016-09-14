using System;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Neo.Resources;
using Neo.UI.Models;

namespace Neo.UI.Components
{
    /// <summary>
    /// Interaction logic for AssetBrowserFilePreview.xaml
    /// </summary>
    public partial class AssetBrowserFilePreview
    {
        private static readonly BitmapSource PageImageSource =
            WpfImageSource.FromGdiImage(Images.Page_Icon_48);

        public AssetBrowserFile FileEntry { get; private set; }

        public AssetBrowserFilePreview(AssetBrowserFile file)
        {
            FileEntry = file;
            DataContext = file;
            InitializeComponent();

            if (file.Extension == ".blp")
                LoadImage(file);
            else
                PreviewImage.Source = PageImageSource;

        }

        private void LoadImage(AssetBrowserFile file)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => PreviewImage.Source = WpfImageSource.FromTexture(file.FullPath)));
        }
    }
}
