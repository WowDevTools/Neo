using System;
using System.Windows.Threading;
using WoWEditor6.UI.Models;

namespace WoWEditor6.UI.Components
{
    /// <summary>
    /// Interaction logic for AssetBrowserFilePreview.xaml
    /// </summary>
    public partial class AssetBrowserFilePreview
    {
        public AssetBrowserFilePreview(AssetBrowserFile file)
        {
            DataContext = file;
            InitializeComponent();

            if (file.Extension == ".blp")
                LoadImage(file);
            
        }

        private void LoadImage(AssetBrowserFile file)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => PreviewImage.Source = WpfImageSource.FromTexture(file.FullPath)));
        }
    }
}
