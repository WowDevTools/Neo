using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WoWEditor6.UI.Models;
using WoWEditor6.Editing;

namespace WoWEditor6.UI.Dialogs
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

        private void ColorPickerControl_ColorChanged(Color obj)
        {
            var model = DataContext as SculptingViewModel;
            if (model == null)
                return;

            ColorPreviewRect.Fill = new SolidColorBrush(obj);
            mPreventUpdate = true;
            RedBox.Text = obj.R.ToString();
            BlueBox.Text = obj.B.ToString();
            GreenBox.Text = obj.G.ToString();
            mPreventUpdate = false;

            model.HandleShadingMultiplier(new SharpDX.Vector3((obj.R / 255.0f) * 2.0f,
                (obj.G / 255.0f) * 2.0f, (obj.B / 255.0f) * 2.0f));
        }

        private void SculptingEnableButton_Clicked(object sender, RoutedEventArgs args)
        {
            var model = DataContext as SculptingViewModel;
            if (model == null)
                return;

            model.SwitchToSculpting();
        }

        private void RedBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mPreventUpdate)
                return;

            var model = DataContext as SculptingViewModel;
            if (model == null)
                return;

            int r, g, b;
            if (!int.TryParse(RedBox.Text, out r) || !int.TryParse(GreenBox.Text, out g) ||
                !int.TryParse(BlueBox.Text, out b))
                return;

            if (r > 255 || g > 255 || b > 255 || r < 0 || g < 0 || b < 0)
                return;

            ColorPreviewRect.Fill = new SolidColorBrush(Color.FromRgb((byte) r, (byte) g, (byte) b));
            model.HandleShadingMultiplier(new SharpDX.Vector3((r / 255.0f) * 2.0f,
                (g / 255.0f) * 2.0f, (b / 255.0f) * 2.0f));
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
