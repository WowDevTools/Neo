using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.UI.Themes
{
    class DarkTheme : Xceed.Wpf.AvalonDock.Themes.Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri("pack://application:,,,/UI/Themes/DarkTheme.xaml");
        }
    }
}
