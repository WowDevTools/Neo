using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpDX;
using WoWEditor6.Graphics;
using WoWEditor6.Scene.Models;
using WoWEditor6.Scene.Models.M2;
using WoWEditor6.Scene.Models.WMO;
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
            public Vector4 mousePosition;
            public Vector4 brushParameters;
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
        public WmoManager WmoManager { get; } = new WmoManager();
        public M2Manager M2Manager { get; } = new M2Manager();

        private AppState mState;
        private readonly PerspectiveCamera mMainCamera = new PerspectiveCamera();
        public Camera ActiveCamera { get; private set; }
        private ConstantBuffer mGlobalBuffer;
        private ConstantBuffer mGlobalParamsBuffer;
        private GlobalParamsBuffer mGlobalParamsBufferStore;
        private GlobalBuffer mGlobalBufferStore;
        private bool mGlobalParamsChanged;
        private bool mGlobalChanged;
        public CameraControl CamControl { get; private set; }
        private IntersectionParams mIntersection;
        private Point mLastCursorPosition;

        public AppState State { get { return mState; } set { UpdateAppState(value); } }
        public GxContext GraphicsContext { get; private set; }
        public GraphicsDispatcher Dispatcher { get; private set; }

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
                fogParams = new Vector4(500.0f, 900.0f, mMainCamera.FarClip, 0.0f),
                brushParameters = new Vector4(45.0f, 55.0f, 0.0f, 0.0f),
                mousePosition = new Vector4(float.MaxValue)
            };

            mGlobalParamsBuffer.UpdateData(mGlobalParamsBufferStore);

            mGlobalBufferStore = new GlobalBuffer
            {
                eyePosition = Vector4.Zero,
                matProj = Matrix.Identity,
                matView = Matrix.Identity
            };

            mGlobalBuffer.UpdateData(mGlobalBufferStore);

            Dispatcher = new GraphicsDispatcher();
            MapChunkRender.Initialize(context);
            MapAreaLowRender.Initialize(context);
            WmoGroupRender.Initialize(context);
            M2BatchRenderer.Initialize(context);

            StaticAnimationThread.Instance.Initialize();

            WmoManager.Initialize();
            M2Manager.Initialize();

            GraphicsContext = context;

            SetActiveCamera(mMainCamera);
            TextureManager.Instance.Initialize(context);

            MapManager.Initialize();

            mMainCamera.ViewChanged += ViewChanged;
            mMainCamera.ProjectionChanged += ProjectionChanged;

            ViewChanged(mMainCamera, mMainCamera.View);
            ProjectionChanged(mMainCamera, mMainCamera.Projection);

            CamControl = new CameraControl(window);
        }

        public void OnEnterWorld(Vector3 position)
        {
            State = AppState.World;
            Utils.TimeManager.Instance.Reset();
            mMainCamera.SetParameters(position, position + Vector3.UnitX, Vector3.UnitZ, -Vector3.UnitY);
            UpdatePosition(position);
        }

        public void Shutdown()
        {
            TextureManager.Instance.Shutdown();
            MapManager.Shutdown();
            WmoManager.Shutdown();
            StaticAnimationThread.Instance.Shutdown();
            M2Manager.Shutdown();
        }

        public void OnFrame()
        {
            Dispatcher.ProcessFrame();

            CamControl.Update();

            // do not move before mCamControl.Update to have the latest view/projection
            UpdateCursorPosition();

            UpdateBuffers();

            GraphicsContext.Context.VertexShader.SetConstantBuffer(0, mGlobalBuffer.Native);
            GraphicsContext.Context.PixelShader.SetConstantBuffer(0, mGlobalParamsBuffer.Native);
            MapManager.OnFrame();
            WmoManager.OnFrame();
            M2Manager.OnFrame();
        }

        public void OnMouseWheel(int delta)
        {
            CamControl.HandleMouseWheel(delta);
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
                mGlobalParamsBufferStore.fogParams = new Vector4(fogStart, 900.0f, mMainCamera.FarClip, 0.0f);
                mGlobalParamsChanged = true;
            }
        }

        private void UpdateCursorPosition()
        {
            var pos = InterfaceManager.Instance.Window.PointToClient(Cursor.Position);
            if (mIntersection == null || pos.X != mLastCursorPosition.X || pos.Y != mLastCursorPosition.Y)
            {
                mLastCursorPosition = new Point(pos.X, pos.Y);
                mIntersection = new IntersectionParams(ActiveCamera.ViewInverse, ActiveCamera.ProjectionInverse,
                    new Vector2(mLastCursorPosition.X, mLastCursorPosition.Y));

                MapManager.Intersect(mIntersection);
                mGlobalParamsBufferStore.mousePosition = new Vector4(mIntersection.TerrainPosition, 0.0f);
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

            M2Manager.ViewChanged();
        }

        private void ProjectionChanged(Camera camera, Matrix matProj)
        {
            if (camera != ActiveCamera)
                return;

            mGlobalBufferStore.matProj = matProj;
            mGlobalChanged = true;

            var perspectiveCamera = camera as PerspectiveCamera;
            if (perspectiveCamera == null) return;

            mGlobalParamsBufferStore.fogParams.Z = perspectiveCamera.FarClip;
            mGlobalParamsChanged = true;

            M2Manager.ViewChanged();
        }

        private void UpdateBuffers()
        {
            if (mGlobalParamsChanged)
            {
                lock (mGlobalParamsBuffer)
                {
                    mGlobalParamsBuffer.UpdateData(mGlobalParamsBufferStore);
                    mGlobalParamsChanged = false;
                }
            }

            if (!mGlobalChanged) return;

            lock (mGlobalBuffer)
            {
                mGlobalBuffer.UpdateData(mGlobalBufferStore);
                mGlobalChanged = false;
            }
        }
    }
}