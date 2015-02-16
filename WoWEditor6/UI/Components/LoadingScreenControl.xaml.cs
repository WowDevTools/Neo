using System.Threading.Tasks;
using SharpDX;
using WoWEditor6.Scene;

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

        public async void OnLoadStarted(int mapId, string loadScreenPath, bool wideScreen, Vector2 entryPoint)
        {
            LoadingScreenImage.Source = WpfImageSource.FromTexture(loadScreenPath);
            LoadingScreenImage.Width = (wideScreen ? (16.0f / 9.0f) : (4.0f / 3.0f)) * LoadingScreenImage.Height;

            entryPoint.Y = 64.0f * Metrics.TileSize - entryPoint.Y;
            await Task.Factory.StartNew(() => WorldFrame.Instance.MapManager.EnterWorld(entryPoint, mapId));
        }

        public void UpdateProgress(float pct)
        {
            
        }
    }
}
