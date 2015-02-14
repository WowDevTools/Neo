using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using WoWEditor6.Scene;

namespace WoWEditor6.UI
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : ILogSink
    {
        public EditorWindow()
        {
            InitializeComponent();
        }

        public RenderControl DrawTarget { get { return RenderTarget; } }

        private void MenuSaveItem_Click(object sender, RoutedEventArgs e)
        {
            WorldFrame.Instance.MapManager.OnSaveAllFiles();
        }

        private void ModelRenderTest_Click(object sender, RoutedEventArgs e)
        {
            if (WorldFrame.Instance.State == AppState.FileSystemInit ||
                WorldFrame.Instance.State == AppState.Splash)
                return;

            var mrt = new ModelRenderTest();
            mrt.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SculptingPane.ToggleAutoHide();
            KeyBindingPane.ToggleAutoHide();
            BrushSettingsPane.ToggleAutoHide();
            Log.AddSink(this);
        }

        public void AddMessage(LogLevel logLevel, string title, string message)
        {
            if (Dispatcher.HasShutdownFinished || Dispatcher.HasShutdownStarted)
                return;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var pr = new Paragraph {Margin = new Thickness(0, 0, 0, 0)};
                var titleRun = new Run(title);
                switch (logLevel)
                {
                    case LogLevel.Debug:
                        titleRun.Foreground = new SolidColorBrush(Color.FromRgb(70, 70, 70));
                        break;

                    case LogLevel.Error:
                        titleRun.Foreground = new SolidColorBrush(Color.FromRgb(180, 30, 20));
                        break;

                    case LogLevel.Fatal:
                        titleRun.Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 40));
                        break;

                    case LogLevel.Warning:
                        titleRun.Foreground = new SolidColorBrush(Color.FromRgb(250, 250, 18));
                        break;
                }

                pr.Inlines.Add(titleRun);
                pr.Inlines.Add(new Run(message));
                LogDocument.Blocks.Add(pr);
            }));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Log.RemoveSink(this);
        }

    }
}
