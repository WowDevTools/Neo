using System.Windows;
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
    }
}
