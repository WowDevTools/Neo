using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WoWEditor6.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for TerrainSettingsImpl.xaml
    /// </summary>
    public partial class TerrainSettingsImpl
    {
        private bool mPreventUpdate;

        public TerrainSettingsImpl()
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
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Views.WorldView.Instance.KeySettingsDialog.Visible = true;
        }
    }
}
