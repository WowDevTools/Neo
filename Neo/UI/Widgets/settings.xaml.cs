using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Neo.Scene;

namespace Neo.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings
    {
        public Settings()
        {
            InitializeComponent();
	        this.CheckBoxHighlightModel.IsChecked = Properties.Settings.Default.HighlightModelsInBrush;
	        this.CheckBoxDrawBrushModels.IsChecked = Properties.Settings.Default.UpdateDrawBrushOnModels;
	        this.DayNightScalingSlider.Value = Properties.Settings.Default.DayNightScaling * 10.0f;
	        this.UseDayNightCycle.IsChecked = Properties.Settings.Default.UseDayNightCycle;
            ChangeUseDayNight_Click(this.UseDayNightCycle, new RoutedEventArgs());
	        this.DefaultDayTimeTextBox.Text = Properties.Settings.Default.DefaultDayTime.ToString();

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
            {
	            return;
            }

	        Properties.Settings.Default.UpdateDrawBrushOnModels = cb.IsChecked ?? false;
            Properties.Settings.Default.Save();

            //if (WorldFrame.Instance != null)
            //WorldFrame.Instance.UpdateDrawBrushOnModels = cb.IsChecked ?? false;
        }

        private void CheckBox_HighlightModel_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null)
            {
	            return;
            }

	        Properties.Settings.Default.HighlightModelsInBrush = cb.IsChecked ?? false;
            Properties.Settings.Default.Save();

            if (WorldFrame.Instance != null)
            {
	            WorldFrame.Instance.HighlightModelsInBrush = cb.IsChecked ?? false;
            }
        }


        private void DayNightDefaultTime_Changed(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null || this.DefaultDayNightTimeIndicator == null)
            {
	            return;
            }

	        int time;
            if (!int.TryParse(tb.Text, out time))
            {
	            return;
            }

	        var ts = TimeSpan.FromMinutes(time / 2.0);
	        this.DefaultDayNightTimeIndicator.Text = "= " + (ts.Hours.ToString("D2") + ":" + ts.Minutes.ToString("D2"));
            lock (Properties.Settings.Default)
            {
	            Properties.Settings.Default.DefaultDayTime = time;
            }

	        Properties.Settings.Default.Save();
        }

        private void ChangeUseDayNight_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null)
            {
	            return;
            }

	        this.DefaultTimePanel.Visibility = (cb.IsChecked ?? false) ? Visibility.Collapsed : Visibility.Visible;
	        this.DayNightCycleScalingPanel.Visibility = (cb.IsChecked ?? false) ? Visibility.Visible : Visibility.Collapsed;

            lock (Properties.Settings.Default)
            {
	            Properties.Settings.Default.UseDayNightCycle = cb.IsChecked ?? false;
            }

	        Properties.Settings.Default.Save();
        }

        private void DayNightScaling_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            if (slider == null)
            {
	            return;
            }

	        lock (Properties.Settings.Default)
	        {
		        Properties.Settings.Default.DayNightScaling = (float) slider.Value / 10.0f;
	        }

	        Properties.Settings.Default.Save();
        }

        private void KeyBindingWidget_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void AssetRenderColorCombo_Loaded(object sender, RoutedEventArgs e)
        {
	        this.AssetRenderColorCombo.Items.Clear();
	        this.AssetRenderColorCombo.Items.Add("(Default)");

            foreach (PropertyInfo property in typeof(System.Drawing.Color).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
	            if (property.PropertyType == typeof(System.Drawing.Color))
	            {
		            this.AssetRenderColorCombo.Items.Add(property.Name);
	            }
            }

	        string selected = "(Default)";
            foreach (System.Drawing.KnownColor kc in Enum.GetValues(typeof(System.Drawing.KnownColor)))
            {
                System.Drawing.Color known = System.Drawing.Color.FromKnownColor(kc);
                if (Properties.Settings.Default.AssetRenderBackgroundColor.ToArgb() == known.ToArgb())
                {
                    selected = known.Name;
                    break;
                }
            }

	        this.AssetRenderColorCombo.SelectedItem = selected;
        }

        private void AssetRenderColorCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.AssetRenderColorCombo.SelectedItem == null)
            {
	            return;
            }

	        if(this.AssetRenderColorCombo.SelectedItem.ToString() == "(Default)")
	        {
		        Properties.Settings.Default.AssetRenderBackgroundColor = System.Drawing.Color.FromArgb(1, 71, 95, 121);
	        }
	        else
	        {
		        Properties.Settings.Default.AssetRenderBackgroundColor = System.Drawing.Color.FromName(this.AssetRenderColorCombo.SelectedItem.ToString());
	        }

	        Properties.Settings.Default.Save();
            SetAssetRenderColorBox();
        }

        private void SetAssetRenderColorBox()
        {
            var c = Properties.Settings.Default.AssetRenderBackgroundColor;
	        this.AssetRenderColorShow.Color = System.Windows.Media.Color.FromRgb(c.R, c.G, c.B);
        }
    }
}
