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
using WoWEditor6.UI.Models;

namespace WoWEditor6.UI.Components
{
    /// <summary>
    /// Interaction logic for AssetBrowserFilePreview.xaml
    /// </summary>
    public partial class AssetBrowserFilePreview : UserControl
    {
        public AssetBrowserFilePreview(AssetBrowserFile file)
        {
            DataContext = file;
            InitializeComponent();

            if (file.Extension == ".blp")
                PreviewImage.Source = WpfImageSource.FromTexture(file.FullPath);
        }
    }
}
