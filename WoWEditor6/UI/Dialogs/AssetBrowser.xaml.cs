using System.Windows.Controls;
using WoWEditor6.IO;

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
            InitializeComponent();

            FileManager.Instance.LoadComplete += () => Dispatcher.Invoke(() => AssetTreeView.ItemsSource = new[] {RootDirectory});
        }
    }
}
