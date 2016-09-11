using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using WoWEditor6.Scene;
using System.Drawing;

namespace WoWEditor6.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings
    {
        private readonly Color defaultColour = Color.FromArgb(1, 71, 95, 121);

        public Settings()
        {
            InitializeComponent();
            CheckBoxHighlightModel.IsChecked = Properties.Settings.Default.HighlightModelsInBrush;
            CheckBoxDrawBrushModels.IsChecked = Properties.Settings.Default.UpdateDrawBrushOnModels;
            DayNightScalingSlider.Value = Properties.Settings.Default.DayNightScaling * 10.0f;
            UseDayNightCycle.IsChecked = Properties.Settings.Default.UseDayNightCycle;
            ChangeUseDayNight_Click(UseDayNightCycle, new RoutedEventArgs());
            DefaultDayTimeTextBox.Text = Properties.Settings.Default.DefaultDayTime.ToString();
            
            SetAssetRenderColorBox();
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

        private void KeyBindingWidget_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void AssetRenderColorCombo_Loaded(object sender, RoutedEventArgs e)
        {
            AssetRenderColorCombo.Items.Clear();
            AssetRenderColorCombo.Items.Add("(Default)");           

            string selected = "(Default)";
            foreach (KnownColor kc in Enum.GetValues(typeof(KnownColor))) //Use system colours
            {
                Color known = Color.FromKnownColor(kc);
                AssetRenderColorCombo.Items.Add(known.Name);
                if (Properties.Settings.Default.AssetRenderBackgroundColor.ToArgb() == known.ToArgb())
                {
                    selected = known.Name;
                    break;
                }
            }

            AssetRenderColorCombo.SelectedItem = selected;
        }

        private void AssetRenderColorCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssetRenderColorCombo.SelectedItem == null)
                return;

            if(AssetRenderColorCombo.SelectedItem.ToString() == "(Default)")
                Properties.Settings.Default.AssetRenderBackgroundColor = defaultColour;
            else
                Properties.Settings.Default.AssetRenderBackgroundColor = System.Drawing.Color.FromName(AssetRenderColorCombo.SelectedItem.ToString());

            Properties.Settings.Default.Save();
            SetAssetRenderColorBox();
        }

        private void SetAssetRenderColorBox()
        {
            var c = Properties.Settings.Default.AssetRenderBackgroundColor;
            AssetRenderColorShow.Color = System.Windows.Media.Color.FromRgb(c.R, c.G, c.B);
        }
    }
}
