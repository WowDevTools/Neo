using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WoWEditor6.IO;

namespace WoWEditor6.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for AssetBrowser.xaml
    /// </summary>
    public partial class AssetBrowser : UserControl
    {
        public DirectoryEntry RootDirectory { get { return IO.FileManager.Instance.FileListing.RootEntry; } }

        public AssetBrowser()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            AssetTreeView.ItemsSource = new[] {RootDirectory};
        }
    }
}
