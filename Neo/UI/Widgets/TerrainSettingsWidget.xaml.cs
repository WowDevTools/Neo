using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Neo.UI.Models;
using Neo.Editing;

namespace Neo.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for TerrainSettingsImpl.xaml
    /// </summary>
    public partial class TerrainSettingsWidget
    {
        private bool mPreventUpdate;

        public TerrainSettingsWidget()
        {
            DataContext = new SculptingViewModel(this);
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IntensitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var model = DataContext as SculptingViewModel;
            if (model == null)
                return;

            model.HandleIntensityChanged((float)IntensitySlider.Value);
        }

        private void InnerRadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newValue = e.NewValue;
            var model = DataContext as SculptingViewModel;
            if (model == null)
                return;

            model.HandleInnerRadiusChanged((float)newValue);
        }

        private void OuterRadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newValue = e.NewValue;
            var model = DataContext as SculptingViewModel;
            if (model == null)
                return;

            model.HandleOuterRadiusChanged((float)newValue);
        }

        private void ChangeMode_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button == null)
                return;

            if(button.IsChecked ?? false)
            {
                TerrainChangeType mode;
                if (!System.Enum.TryParse(button.Tag as string ?? "", out mode))
                    return;

                var model = DataContext as SculptingViewModel;
                if (model == null)
                    return;

                model.HandleType(mode);
            }
        }

        private void ChangeAlgo_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button == null)
                return;

            if (button.IsChecked ?? false)
            {
                TerrainAlgorithm algo;
                if (!System.Enum.TryParse(button.Tag as string ?? "", out algo))
                    return;

                var model = DataContext as SculptingViewModel;
                if (model == null)
                    return;

                model.HandleAlgorithm(algo);
            }
        }

        private void AlignModels_Changed(object sender, RoutedEventArgs args)
        {
            var model = DataContext as SculptingViewModel;
            if (model == null)
                return;

            model.HandleAlignToGround(AlignModelsBox.IsChecked ?? false);
        }

        private void TabletControl_Changed(object sender, RoutedEventArgs e)
        {
            var model = DataContext as SculptingViewModel;
            if (model == null)
                return;

            model.HandleTabletControl(TabletControlBox.IsChecked ?? false);
        }

        private void Handle_PenSensivityChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newValue = e.NewValue;
            var model = DataContext as SculptingViewModel;
            if (model == null)
                return;

            model.HandlePenSensivity((float)newValue);
        }
    }
}
