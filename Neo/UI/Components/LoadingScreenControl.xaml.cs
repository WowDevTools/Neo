using System;
using System.Windows.Media;
using SharpDX;
using Neo.Scene;
using Point = System.Windows.Point;

namespace Neo.UI.Components
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
	        this.LoadingScreenImage.Source = WpfImageSource.FromTexture(loadScreenPath);
	        this.LoadingScreenImage.RenderTransform = new ScaleTransform(wideScreen ? (16.0f / 9.0f) : (4.0f / 3.0f), 1);
	        this.LoadingScreenImage.RenderTransformOrigin = new Point(0.5, 0.5);

            var bmp = WpfImageSource.FromTexture(@"Interface\Glues\LoadingBar\Loading-BarBorder.blp");
	        this.LoadingScreenBarImage.Width = bmp.PixelWidth * (wideScreen ? (16.0f / 9.0f) : (4.0f / 3.0f));
	        this.LoadingScreenBarImage.Height = bmp.PixelHeight;
	        this.LoadingScreenBarImage.Source = bmp;
	        this.LoadingScreenBarImage.RenderTransform = new ScaleTransform(wideScreen ? (16.0f / 9.0f) : (4.0f / 3.0f), 1);
	        this.LoadingScreenBarImage.RenderTransformOrigin = new Point(0.5, 0.5);

            bmp = WpfImageSource.FromTexture(@"Interface\Glues\LoadingBar\Loading-BarFill.blp");
	        this.LoadingScreenBarFillImage.Source = bmp;
	        this.LoadingScreenBarFillImage.Width = 0;
	        this.LoadingScreenBarFillImage.Height = this.LoadingScreenBarImage.Height - 30;
	        this.LoadingScreenBarFillImage.Stretch = Stretch.Fill;

	        this.LoadingFillBorder.Width = (this.LoadingScreenBarImage.Width - 70);
	        this.LoadingFillBorder.Height = this.LoadingScreenBarImage.Height - 30;

            entryPoint.Y = 64.0f * Metrics.TileSize - entryPoint.Y;
            WorldFrame.Instance.MapManager.EnterWorld(entryPoint, mapId);
        }

        public void UpdateProgress(float pct)
        {
            Dispatcher.BeginInvoke(new Action(() => this.LoadingScreenBarFillImage.Width = (this.LoadingScreenBarImage.Width - 70) * pct));
        }
    }
}
