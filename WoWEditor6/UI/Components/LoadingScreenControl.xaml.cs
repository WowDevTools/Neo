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

namespace WoWEditor6.UI.Components
{
    /// <summary>
    /// Interaction logic for LoadingScreenControl.xaml
    /// </summary>
    public partial class LoadingScreenControl : UserControl
    {
        public LoadingScreenControl()
        {
            InitializeComponent();
        }

        public void OnLoadStarted(string loadScreenPath, bool wideScreen)
        {
            LoadingScreenImage.Source = WpfImageSource.FromTexture(loadScreenPath);
            LoadingScreenImage.Width = (wideScreen ? (16.0f / 9.0f) : (4.0f / 3.0f)) * LoadingScreenImage.Height;
        }
    }
}
