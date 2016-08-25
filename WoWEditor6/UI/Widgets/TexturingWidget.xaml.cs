using System.Windows;
using System.Windows.Controls;
using WoWEditor6.UI.Models;

namespace WoWEditor6.UI.Widgets
{
    /// <summary>
    /// Interaction logic for TexturingWidget.xaml
    /// </summary>
    public partial class TexturingWidget
    {
        public TexturingWidget()
        {
            DataContext = new TexturingViewModel(this);
            InitializeComponent();

            foreach(CheckBox cb in FilterWrapPanel.Children)
                cb.Click += FilterCheckBoxClicked;
        }

        private void FilterCheckBoxClicked(object sender, RoutedEventArgs routedEventArgs)
        {
            var model = DataContext as TexturingViewModel;
            if (model == null)
                return;

            model.UpdateFilters();
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

        private void Handle_PenSensivityChanged(object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            var newValue = e.NewValue;
            var model = DataContext as TexturingViewModel;
            if (model == null)
                return;

            model.HandlePenSensivity((float)newValue);
        }

        private void EnableTexturing_Click(object sender, RoutedEventArgs e)
        {
            var model = DataContext as TexturingViewModel;
            if (model == null)
                return;

            model.SwitchToTexturing();
        }

        private void Handle_GradientSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Editing.TextureChangeManager.Instance.TargetValue = (float) e.NewValue;
        }

        private void FavoriteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var model = DataContext as TexturingViewModel;
            if (model == null)
                return;

            model.OnFavoriteButtonClicked();
        }

        private void TextureQueryText_Changed(object sender, TextChangedEventArgs e)
        {
            var model = DataContext as TexturingViewModel;
            if (model == null)
                return;

            model.SearchForTexture(((TextBox) e.Source).Text);
        }

        private void TabletControl_Changed(object sender, RoutedEventArgs e)
        {
            var model = DataContext as TexturingViewModel;
            if (model == null)
                return;

            model.HandleTabletControl(TabletControlBox.IsChecked ?? false);
        }
    }
}
