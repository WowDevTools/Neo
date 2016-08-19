using System.Windows;
using System.Windows.Threading;
using SharpDX;
using WoWEditor6.UI.Components;
using WoWEditor6.UI.Models;

namespace WoWEditor6.UI
{
    class EditorWindowController
    {
        public static EditorWindowController Instance { get; private set; }

        private readonly EditorWindow mWindow;

        public LoadingScreenControl LoadingScreen { get { return mWindow.LoadingScreenView; } }
        public TexturingViewModel TexturingModel { get; set; }
        public AssetBrowserViewModel AssetBrowserModel { get; set; }
        public ObjectSpawnModel SpawnModel { get; set; }

        public Dispatcher WindowDispatcher { get { return mWindow.Dispatcher; } }

        public EditorWindowController(EditorWindow window)
        {
            Instance = this;
            mWindow = window;
        }

        public void ShowMapOverview()
        {
            mWindow.SplashDocument.Visibility = Visibility.Collapsed;
            mWindow.LoadingDocument.Visibility = Visibility.Collapsed;
            mWindow.EntrySelectView.Visibility = Visibility.Collapsed;
            mWindow.LoadingScreenView.Visibility = Visibility.Collapsed;
            mWindow.MapOverviewGrid.Visibility = Visibility.Visible;
        }

        public void ShowAssetBrowser()
        {
            mWindow.AssetBrowserDocument.IsSelected = true;
        }

        public void OnEnterWorld()
        {
            mWindow.WelcomeDocument.Close();
        }

        public void OnUpdatePosition(Vector3 position)
        {
            mWindow.OnUpdatePosition(position);
        }

        public void OnUpdate(Vector3 modelPosition, Vector3 namePlatePosition)
        {
            mWindow.OnUpdate(modelPosition, namePlatePosition);
        }

        public void OnUpdateTileIndex(int x, int y)
        {
            mWindow.OnUpdateCurrentAdt(x, y);
        }
    }
}
