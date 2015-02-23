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

namespace WoWEditor6.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for TexturingWidget.xaml
    /// </summary>
    public partial class TexturingWidget : UserControl
    {
        public TexturingWidget()
        {
            DataContext = new TexturingViewModel(this);
            InitializeComponent();
        }

        private void Handle_AssetBrowserClick(object sender, RoutedEventArgs e)
        {
            var model = DataContext as TexturingViewModel;
            if (model == null)
                return;

            model.HandleSelectFromAssets();
        }

        private void Handle_AmountSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newValue = e.NewValue;
            var model = DataContext as TexturingViewModel;
            if (model == null)
                return;

            model.HandleAmountSlider((float) newValue);
        }

        private void Handle_InnerRadiusSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newValue = e.NewValue;
            var model = DataContext as TexturingViewModel;
            if (model == null)
                return;

            model.HandleInnerRadiusSlider((float) newValue);
        }

        private void Handle_OuterRadiusSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newValue = e.NewValue;
            var model = DataContext as TexturingViewModel;
            if (model == null)
                return;

            model.HandleOuterRadiusSlider((float)newValue);
        }
    }
}
