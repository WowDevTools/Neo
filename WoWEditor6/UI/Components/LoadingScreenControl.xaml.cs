using System;
using System.Windows.Media;
using SharpDX;
using WoWEditor6.Scene;
using Point = System.Windows.Point;

namespace WoWEditor6.UI.Components
{
    /// <summary>
    /// Interaction logic for LoadingScreenControl.xaml
    /// </summary>
    public partial class LoadingScreenControl
    {
        public LoadingScreenControl()
        {
            InitializeComponent();
        }

        public void OnLoadStarted(int mapId, string loadScreenPath, bool wideScreen, Vector2 entryPoint)
        {
            LoadingScreenImage.Source = WpfImageSource.FromTexture(loadScreenPath);
            LoadingScreenImage.RenderTransform = new ScaleTransform(wideScreen ? (16.0f / 9.0f) : (4.0f / 3.0f), 1);
            LoadingScreenImage.RenderTransformOrigin = new Point(0.5, 0.5);

            var bmp = WpfImageSource.FromTexture(@"Interface\Glues\LoadingBar\Loading-BarBorder.blp");
            LoadingScreenBarImage.Width = bmp.PixelWidth * (wideScreen ? (16.0f / 9.0f) : (4.0f / 3.0f));
            LoadingScreenBarImage.Height = bmp.PixelHeight;
            LoadingScreenBarImage.Source = bmp;
            LoadingScreenBarImage.RenderTransform = new ScaleTransform(wideScreen ? (16.0f / 9.0f) : (4.0f / 3.0f), 1);
            LoadingScreenBarImage.RenderTransformOrigin = new Point(0.5, 0.5);

            bmp = WpfImageSource.FromTexture(@"Interface\Glues\LoadingBar\Loading-BarFill.blp");
            LoadingScreenBarFillImage.Source = bmp;
            LoadingScreenBarFillImage.Width = 0;
            LoadingScreenBarFillImage.Height = LoadingScreenBarImage.Height - 30;
            LoadingScreenBarFillImage.Stretch = Stretch.Fill;

            LoadingFillBorder.Width = (LoadingScreenBarImage.Width - 70);
            LoadingFillBorder.Height = LoadingScreenBarImage.Height - 30;

            entryPoint.Y = 64.0f * Metrics.TileSize - entryPoint.Y;
            WorldFrame.Instance.MapManager.EnterWorld(entryPoint, mapId);
        }

        public void UpdateProgress(float pct)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                LoadingScreenBarFillImage.Width = (LoadingScreenBarImage.Width - 70) * pct));
        }
    }
}
