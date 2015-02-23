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
using WoWEditor6.Scene;

namespace WoWEditor6.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
            this.CheckBoxHighlightModel.IsChecked = Properties.Settings.Default.HighlightModelsInBrush;
            this.CheckBoxDrawBrushModels.IsChecked = Properties.Settings.Default.UpdateDrawBrushOnModels;
            DayNightScalingSlider.Value = Properties.Settings.Default.DayNightScaling * 10.0f;
            UseDayNightCycle.IsChecked = Properties.Settings.Default.UseDayNightCycle;
            ChangeUseDayNight_Click(UseDayNightCycle, new RoutedEventArgs());
            DefaultDayTimeTextBox.Text = Properties.Settings.Default.DefaultDayTime.ToString();
        }

        /// <summary>
        /// todo: You need to also add teh load of the settings in the WorldFrame!!! If you add options here.
        /// The settings only get set if the worldFrame realy exist. So you can change settings also if editor not still loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void CheckBox_DrawBrushModels_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null)
                return;

            Properties.Settings.Default.UpdateDrawBrushOnModels = cb.IsChecked ?? false;
            Properties.Settings.Default.Save();

            //if (WorldFrame.Instance != null)
            //WorldFrame.Instance.UpdateDrawBrushOnModels = cb.IsChecked ?? false;
        }

        private void CheckBox_HighlightModel_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null)
                return;

            Properties.Settings.Default.HighlightModelsInBrush = cb.IsChecked ?? false;
            Properties.Settings.Default.Save();

            if (WorldFrame.Instance != null)
                WorldFrame.Instance.HighlightModelsInBrush = cb.IsChecked ?? false;
        }


        private void DayNightDefaultTime_Changed(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null || DefaultDayNightTimeIndicator == null)
                return;

            int time;
            if (!int.TryParse(tb.Text, out time))
                return;

            var ts = TimeSpan.FromMinutes(time / 2.0);
            DefaultDayNightTimeIndicator.Text = "= " + (ts.Hours.ToString("D2") + ":" + ts.Minutes.ToString("D2"));
            lock (Properties.Settings.Default)
                Properties.Settings.Default.DefaultDayTime = time;

            Properties.Settings.Default.Save();
        }

        private void ChangeUseDayNight_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null)
                return;

            DefaultTimePanel.Visibility = (cb.IsChecked ?? false) ? Visibility.Collapsed : Visibility.Visible;
            DayNightCycleScalingPanel.Visibility = (cb.IsChecked ?? false) ? Visibility.Visible : Visibility.Collapsed;

            lock (Properties.Settings.Default)
                Properties.Settings.Default.UseDayNightCycle = cb.IsChecked ?? false;

            Properties.Settings.Default.Save();
        }

        private void DayNightScaling_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            if (slider == null)
                return;

            lock (Properties.Settings.Default)
                Properties.Settings.Default.DayNightScaling = (float) slider.Value / 10.0f;

            Properties.Settings.Default.Save();
        }
    }
}
