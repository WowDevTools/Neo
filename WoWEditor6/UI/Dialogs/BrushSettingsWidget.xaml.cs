using System.Windows;
using System.Windows.Controls;
using WoWEditor6.Scene;

namespace WoWEditor6.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for BrushSettingsWidget.xaml
    /// </summary>
    public partial class BrushSettingsWidget
    {
        public BrushSettingsWidget()
        {
            InitializeComponent();
        }

        void DrawBrushModels_Click(object sender, RoutedEventArgs args)
        {
            var cb = sender as CheckBox;
            if (cb == null)
                return;

            // WorldFrame.Instance.UpdateDrawBrushOnModels(cb.IsChecked ?? false);
        }

        void HighlightModel_Click(object sender, RoutedEventArgs args)
        {
            var cb = sender as CheckBox;
            if (cb == null)
                return;

            WorldFrame.Instance.HighlightModelsInBrush = cb.IsChecked ?? false;
        }
    }
}
