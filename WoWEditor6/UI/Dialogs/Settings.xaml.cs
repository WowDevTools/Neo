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
            this.CheckBox_HighlightModel.IsChecked = Properties.Settings.Default.HighlightModelsInBrush;
            this.CheckBox_DrawBrushModels.IsChecked = Properties.Settings.Default.UpdateDrawBrushOnModels;
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


 

    }
}
