using System.Runtime.InteropServices;
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
        [StructLayout(LayoutKind.Sequential)]
        struct GlobalParamsBuffer
        {
            public Vector4 mapAmbient;
            public Vector4 mapDiffuse;
            public Vector4 fogColor;
            public Vector4 fogParams;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct GlobalBuffer
        {
            public Matrix matView;
            public Matrix matProj;
            public Vector4 eyePosition;
        }

        public static WorldFrame Instance { get; } = new WorldFrame();

        public MapManager MapManager { get; } = new MapManager();

        private AppState mState;
        private readonly PerspectiveCamera mMainCamera = new PerspectiveCamera();
        public Camera ActiveCamera { get; private set; }
        private ConstantBuffer mGlobalBuffer;
        private ConstantBuffer mGlobalParamsBuffer;
        private GlobalParamsBuffer mGlobalParamsBufferStore;
        private GlobalBuffer mGlobalBufferStore;
        private bool mGlobalParamsChanged;
        private bool mGlobalChanged;

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

        public void UpdatePosition(Vector3 position)
        {
            lock (mGlobalBuffer)
            {
                mGlobalBufferStore.eyePosition = new Vector4(position, 1.0f);
                mGlobalChanged = true;
            }
        }

        public void Initialize(MainWindow window, GxContext context)
        {
            mGlobalBuffer = new ConstantBuffer(context);
            mGlobalParamsBuffer = new ConstantBuffer(context);
            mGlobalParamsBufferStore = new GlobalParamsBuffer
            {
                mapAmbient = new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
                mapDiffuse = new Vector4(0.25f, 0.5f, 1.0f, 1.0f),
                fogColor = new Vector4(0.25f, 0.5f, 1.0f, 1.0f),
                fogParams = new Vector4(500.0f, 900.0f, 0.0f, 0.0f)
            };

            mGlobalParamsBuffer.UpdateData(mGlobalParamsBufferStore);

            mGlobalBufferStore = new GlobalBuffer
            {
                eyePosition = Vector4.Zero,
                matProj = Matrix.Identity,
                matView = Matrix.Identity
            };

            mGlobalBuffer.UpdateData(mGlobalBufferStore);

            Dispatcher = Dispatcher.CurrentDispatcher;
            MapChunkRender.Initialize(context);

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
            State = AppState.World;
            Utils.TimeManager.Instance.Reset();
            mMainCamera.SetParameters(position, position + Vector3.UnitX, Vector3.UnitZ, Vector3.UnitY);
            UpdatePosition(position);
        }

        public void Shutdown()
        {
            TextureManager.Instance.Shutdown();
            MapManager.Shutdown();
        }

        public void OnFrame()
        {
            if(mGlobalParamsChanged)
            {
                lock (mGlobalParamsBuffer)
                {
                    mGlobalParamsBuffer.UpdateData(mGlobalParamsBufferStore);
                    mGlobalParamsChanged = false;
                }
            }

            if(mGlobalChanged)
            {
                lock(mGlobalBuffer)
                {
                    mGlobalBuffer.UpdateData(mGlobalBufferStore);
                    mGlobalChanged = false;
                }
            }

            GraphicsContext.Context.VertexShader.SetConstantBuffer(0, mGlobalBuffer.Native);
            GraphicsContext.Context.PixelShader.SetConstantBuffer(0, mGlobalParamsBuffer.Native);
            MapManager.OnFrame();
        }

        public void UpdateMapAmbient(Vector3 ambient)
        {
            lock (mGlobalParamsBuffer)
            {
                mGlobalParamsBufferStore.mapAmbient = new Vector4(ambient, 1.0f);
                mGlobalParamsChanged = true;
            }
        }

        public void UpdateMapDiffuse(Vector3 diffuse)
        {
            lock (mGlobalParamsBuffer)
            {
                mGlobalParamsBufferStore.mapDiffuse = new Vector4(diffuse, 1.0f);
                mGlobalParamsChanged = true;
            }
        }

        public void UpdateFogParams(Vector3 fogColor, float fogStart)
        {
            lock(mGlobalParamsBuffer)
            {
                mGlobalParamsBufferStore.fogColor = new Vector4(fogColor, 1.0f);
                mGlobalParamsBufferStore.fogParams = new Vector4(fogStart, 900.0f, 0.0f, 0.0f);
                mGlobalParamsChanged = true;
            }
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

            mGlobalBufferStore.matView = matView;
            mGlobalChanged = true;
        }

        private void ProjectionChanged(Camera camera, Matrix matProj)
        {
            if (camera != ActiveCamera)
                return;

            mGlobalBufferStore.matProj = matProj;
            mGlobalChanged = true;
        }
    }
}
