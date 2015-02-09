using System.Windows;

namespace WoWEditor6.UI
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow
    {
        public EditorWindow()
        {
            InitializeComponent();
        }

        public RenderControl DrawTarget { get { return RenderTarget; } }

        private void MenuSaveItem_Click(object sender, RoutedEventArgs e)
        {
            Scene.WorldFrame.Instance.MapManager.OnSaveAllFiles();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SculptingPane.ToggleAutoHide();
            KeyBindingPane.ToggleAutoHide();
        }
    }
}
