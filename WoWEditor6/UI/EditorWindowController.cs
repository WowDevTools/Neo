using WoWEditor6.UI.Components;

namespace WoWEditor6.UI
{
    class EditorWindowController
    {
        public static EditorWindowController Instance { get; private set; }

        private readonly EditorWindow mWindow;

        public LoadingScreenControl LoadingScreen { get { return mWindow.LoadingScreenView; } }

        public EditorWindowController(EditorWindow window)
        {
            Instance = this;
            mWindow = window;
        }

        public void OnEnterWorld()
        {
            mWindow.WelcomeDocument.Close();
        }
    }
}
