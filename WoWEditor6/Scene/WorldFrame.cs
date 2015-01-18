using WoWEditor6.Graphics;
using WoWEditor6.Scene.Terrain;
using WoWEditor6.UI;

namespace WoWEditor6.Scene
{
    class WorldFrame
    {
        public static WorldFrame Instance { get; } = new WorldFrame();

        public MapManager MapManager { get; } = new MapManager();

        private AppState mState;
        private GxContext mContext;
        private MainWindow mWindow;
        private readonly PerspectiveCamera mMainCamera = new PerspectiveCamera();
        private Camera mActiveCamera;

        public AppState State { get { return mState; } set { UpdateAppState(value); } }

        private WorldFrame()
        {
            mState = AppState.FileSystemInit;
        }

        public void Initialize(MainWindow window, GxContext context)
        {
            mWindow = window;
            mContext = context;

            SetActiveCamera(mMainCamera);
        }

        public void OnFrame()
        {

        }

        private void UpdateAppState(AppState newState)
        {
            mState = newState;
            InterfaceManager.Instance.UpdateState(newState);
        }

        private void SetActiveCamera(Camera camera)
        {
            mActiveCamera = camera;
        }
    }
}
