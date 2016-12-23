using Neo.UI.Components;
using Neo.UI.Models;
using Neo.Editing;
using OpenTK;

namespace Neo.UI
{
	internal class EditorWindowController
    {
        public static EditorWindowController Instance { get; private set; }

        private readonly EditorWindow mWindow;

        public LoadingScreenControl LoadingScreen { get { return this.mWindow.LoadingScreenView; } }
        public TexturingViewModel TexturingModel { get; set; }
        public SculptingViewModel TerrainManager { get; set; }
        public IEditingViewModel IEditingModel { get; set; }
        public ShadingViewModel ShadingModel { get; set; }
        public AssetBrowserViewModel AssetBrowserModel { get; set; }
        public ObjectSpawnModel SpawnModel { get; set; }

        public Dispatcher WindowDispatcher { get { return this.mWindow.Dispatcher; } }

        public EditorWindowController(EditorWindow window)
        {
            Instance = this;
	        this.mWindow = window;
        }

        public void ShowMapOverview()
        {
	        this.mWindow.SplashDocument.Visibility = Visibility.Collapsed;
	        this.mWindow.LoadingDocument.Visibility = Visibility.Collapsed;
	        this.mWindow.EntrySelectView.Visibility = Visibility.Collapsed;
	        this.mWindow.LoadingScreenView.Visibility = Visibility.Collapsed;
	        this.mWindow.MapOverviewGrid.Visibility = Visibility.Visible;
        }

        public void ShowAssetBrowser()
        {
	        this.mWindow.AssetBrowserDocument.IsSelected = true;
        }

        public void OnEnterWorld()
        {
	        this.mWindow.WelcomeDocument.Close();
        }

        public void OnUpdatePosition(Vector3 position)
        {
	        this.mWindow.OnUpdatePosition(position);
        }

        public void OnUpdate(Vector3 modelPosition, Vector3 namePlatePosition)
        {
	        this.mWindow.OnUpdate(modelPosition, namePlatePosition);
        }

        public void OnUpdateTileIndex(int x, int y)
        {
	        this.mWindow.OnUpdateCurrentAdt(x, y);
        }

        public void OnUpdateChunkIndex(int x, int y)
        {
	        this.mWindow.OnUpdateCurrentChunk(x, y);
        }
    }
}
