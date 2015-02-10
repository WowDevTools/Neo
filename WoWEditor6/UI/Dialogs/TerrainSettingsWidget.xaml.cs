using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            InitializeComponent();
        }

        private void ColorPickerControl_ColorChanged(Color obj)
        {
            ColorPreviewRect.Fill = new SolidColorBrush(obj);
            mPreventUpdate = true;
            RedBox.Text = obj.R.ToString();
            BlueBox.Text = obj.B.ToString();
            GreenBox.Text = obj.G.ToString();
            mPreventUpdate = false;

            TerrainChangeManager.Instance.ShadingMultiplier = new SharpDX.Vector3((obj.R / 255.0f) * 2.0f,
                (obj.G / 255.0f) * 2.0f, (obj.B / 255.0f) * 2.0f);
        }

        private void SculptingEnableButton_Clicked(object sender, RoutedEventArgs args)
        {
            EditManager.Instance.EnableSculpting();
        }

        private void RedBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mPreventUpdate)
                return;

            int r, g, b;
            if (!int.TryParse(RedBox.Text, out r) || !int.TryParse(GreenBox.Text, out g) ||
                !int.TryParse(BlueBox.Text, out b))
                return;

            if (r > 255 || g > 255 || b > 255 || r < 0 || g < 0 || b < 0)
                return;

            ColorPreviewRect.Fill = new SolidColorBrush(Color.FromRgb((byte) r, (byte) g, (byte) b));
            TerrainChangeManager.Instance.ShadingMultiplier = new SharpDX.Vector3((r / 255.0f) * 2.0f,
                (g / 255.0f) * 2.0f, (b / 255.0f) * 2.0f);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void IntensitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TerrainChangeManager.Instance.Amount = (float) IntensitySlider.Value;
        }

        private void InnerRadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TerrainChangeManager.Instance.InnerRadius = (float) InnerRadiusSlider.Value;
        }

        private void OuterRadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TerrainChangeManager.Instance.OuterRadius = (float) OuterRadiusSlider.Value;
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

                TerrainChangeManager.Instance.ChangeType = mode;
            }
        }

        private void ChangeAlgo_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button == null)
                return;

            if(button.IsChecked ?? false)
            {
                TerrainAlgorithm algo;
                if (!System.Enum.TryParse(button.Tag as string ?? "", out algo))
                    return;

                TerrainChangeManager.Instance.ChangeAlgorithm = algo; 
            }
        }

        private void AlignModels_Changed(object sender, RoutedEventArgs args)
        {
            TerrainChangeManager.Instance.AlignModelsToGround = AlignModelsBox.IsChecked ?? false;
        }
    }
}
