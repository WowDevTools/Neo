using System.Windows.Threading;
using SharpDX;
using WoWEditor6.Graphics;
using WoWEditor6.Scene.Terrain;
using WoWEditor6.Scene.Texture;
using WoWEditor6.UI;

namespace WoWEditor6.Scene
{
    class WorldFrame
    {
        public static WorldFrame Instance { get; } = new WorldFrame();

        public MapManager MapManager { get; } = new MapManager();

        private AppState mState;
        private MainWindow mWindow;
        private readonly PerspectiveCamera mMainCamera = new PerspectiveCamera();
        public Camera ActiveCamera { get; private set; }
        private ConstantBuffer mGlobalBuffer;

        public AppState State { get { return mState; } set { UpdateAppState(value); } }
        public GxContext GraphicsContext { get; private set; }
        public Dispatcher Dispatcher { get; private set; }

        private WorldFrame()
        {
            mState = AppState.FileSystemInit;
        }

        public void OnResize(int width, int height)
        {
            mMainCamera.SetAspect((float) width / height);
        }

        public void Initialize(MainWindow window, GxContext context)
        {
            mGlobalBuffer = new ConstantBuffer(context);

            Dispatcher = Dispatcher.CurrentDispatcher;
            MapChunkRender.Initialize(context);

            mWindow = window;
            GraphicsContext = context;

            SetActiveCamera(mMainCamera);
            TextureManager.Instance.Initialize(context);

            MapManager.Initialize();

            mMainCamera.ViewChanged += ViewChanged;
            mMainCamera.ProjectionChanged += ProjectionChanged;

            ViewChanged(mMainCamera, mMainCamera.View);
            ProjectionChanged(mMainCamera, mMainCamera.Projection);
        }

        public void OnEnterWorld(Vector3 position)
        {
            mMainCamera.SetParameters(position, position + Vector3.UnitX, Vector3.UnitZ, Vector3.UnitY);
        }

        public void Shutdown()
        {
            TextureManager.Instance.Shutdown();
            MapManager.Shutdown();
        }

        public void OnFrame()
        {
            GraphicsContext.Context.VertexShader.SetConstantBuffer(0, mGlobalBuffer.Native);
            MapManager.OnFrame();
        }

        private void UpdateAppState(AppState newState)
        {
            mState = newState;
            InterfaceManager.Instance.UpdateState(newState);
        }

        private void SetActiveCamera(Camera camera)
        {
            ActiveCamera = camera;
        }

        private void ViewChanged(Camera camera, Matrix matView)
        {
            if (camera != ActiveCamera)
                return;

            mGlobalBuffer.UpdateData(new[] {ActiveCamera.View, ActiveCamera.Projection});
        }

        private void ProjectionChanged(Camera camera, Matrix matProj)
        {
            if (camera != ActiveCamera)
                return;

            mGlobalBuffer.UpdateData(new[] { ActiveCamera.View, ActiveCamera.Projection });
        }
    }
}
