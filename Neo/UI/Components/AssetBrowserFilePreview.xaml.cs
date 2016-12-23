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

            ReloadImage();
        }

        public void ReloadImage()
        {
            if (FileEntry.Extension == ".blp")
            {
	            LoadImage(this.FileEntry);
            }
            else if (FileEntry.Extension == ".m2")
            {
	            this.PreviewImage.Source = WpfImageSource.FromGdiImage(ThumbnailCache.TryGetThumbnail(this.FileEntry.FullPath, Images.Page_Icon_48));
            }
            else
            {
	            this.PreviewImage.Source = PageImageSource;
            }
        }

        private void LoadImage(AssetBrowserFile file)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => PreviewImage.Source = WpfImageSource.FromTexture(file.FullPath)));
        }
    }
}
