using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Neo.Editing;
using Neo.Graphics;
using Neo.Scene.Models;
using Neo.Scene.Models.M2;
using Neo.Scene.Models.WMO;
using Neo.Scene.Terrain;
using Neo.Scene.Texture;
using Neo.UI;
using Neo.Utils;
using OpenTK;
using OpenTK.Input;

namespace Neo.Scene
{
	internal class WorldFrame
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct GlobalParamsBuffer
        {
            public Matrix4 matView;
            public Matrix4 matProj;
            public Vector4 viewport;

            public Vector4 ambientLight;
            public Vector4 diffuseLight;

            public Vector4 fogColor;
            // x -> fogStart
            // y -> fotEnd
            // z -> farClip
            public Vector4 fogParams;

            public Vector4 mousePosition;
            public Vector4 eyePosition;

            // x -> innerRadius
            // y -> outerRadius
            // z -> brushTime
            public Vector4 brushParams;
        }

        private IModelInstance mSelectedInstance;
        private BoundingBoxInstance mSelectedBoundingBox;

        public static WorldFrame Instance { get; private set; }

        public MapManager MapManager { get; private set; }
        public WmoManager WmoManager { get; private set; }
        public M2Manager M2Manager { get; private set; }
        public WorldTextManager WorldTextManager { get; private set; }
        public BoundingBoxDrawManager BoundingBoxDrawManager { get; private set; }

        public bool LeftHandedCamera
        {
            get { return this.mMainCamera.LeftHanded; }
            set
            {
                if (this.mMainCamera.LeftHanded != value)
                {
	                this.CamControl.InvertX = !this.CamControl.InvertX;
                }

	            this.mMainCamera.LeftHanded = value;
	            this.mMainCamera.Update();
            }
        }

        private AppState mState;
        private readonly PerspectiveCamera mMainCamera = new PerspectiveCamera();
        public Camera ActiveCamera { get; private set; }

        private UniformBuffer mGlobalBuffer;
        private GlobalParamsBuffer mGlobalBufferStore;
        private bool mGlobalBufferChanged;

        public CameraControl CamControl { get; private set; }
        private Point mLastCursorPosition;
        private RenderControl mWindow;

        public AppState State { get { return this.mState; } set { UpdateAppState(value); } }
        public GraphicsDispatcher Dispatcher { get; private set; }

        public IntersectionParams LastMouseIntersection { get; private set; }

        public event Action<IntersectionParams> OnWorldClicked;

        public bool HighlightModelsInBrush { get; set; }
        public bool HideWMO { get; set; } = false;
        public bool HideM2 { get; set; } = false;

        static WorldFrame()
        {
            Instance = new WorldFrame();
        }

        private WorldFrame()
        {
	        this.MapManager = new MapManager();
	        this.WmoManager = new WmoManager();
	        this.M2Manager = new M2Manager();
	        this.WorldTextManager = new WorldTextManager();
	        this.BoundingBoxDrawManager = new BoundingBoxDrawManager();
	        this.mState = AppState.Idle;

            // set the settings on creation
	        this.HighlightModelsInBrush = Properties.Settings.Default.HighlightModelsInBrush;
            //this.UpdateDrawBrushOnModels = Properties.Settings.Default.UpdateDrawBrushOnModels; // todo: notimplemented!
        }

        public void ClearSelection()
        {
            if (this.mSelectedBoundingBox != null)
            {
	            this.BoundingBoxDrawManager.RemoveDrawableBox(this.mSelectedBoundingBox);
            }

	        ModelEditManager.Instance.SelectedModel = null;
        }

        public void UpdateBrush(float innerRadius, float outerRadius)
        {
	        this.mGlobalBufferStore.brushParams.X = innerRadius;
	        this.mGlobalBufferStore.brushParams.Y = outerRadius;
	        this.mGlobalBufferChanged = true;
        }

        public void OnResize(int width, int height)
        {
            if (width == 0 || height == 0)
            {
	            return;
            }

	        this.mMainCamera.SetAspect((float) width / height);
        }

        public void UpdatePosition(Vector3 position)
        {
            lock (this.mGlobalBuffer)
            {
	            this.mGlobalBufferStore.eyePosition = new Vector4(position, 1.0f);
	            this.mGlobalBufferChanged = true;
            }
        }

        public void Initialize(RenderControl window)
        {
	        this.mWindow = window;
	        this.mGlobalBuffer = new UniformBuffer();
	        this.mGlobalBuffer = new UniformBuffer();
	        this.mGlobalBufferStore = new GlobalParamsBuffer
            {
                matView = Matrix4.Identity,
                matProj = Matrix4.Identity,
                viewport = Vector4.Zero,
                ambientLight = new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
                diffuseLight = new Vector4(0.25f, 0.5f, 1.0f, 1.0f),
                fogColor = new Vector4(0.25f, 0.5f, 1.0f, 1.0f),
                fogParams = new Vector4(500.0f, 900.0f, this.mMainCamera.FarClip, 0.0f),
                mousePosition = new Vector4(float.MaxValue),
                eyePosition = Vector4.Zero,
                brushParams = new Vector4(45.0f, 55.0f, 0.0f, 0.0f)
            };

	        this.mGlobalBuffer.BufferData(this.mGlobalBufferStore);

	        this.Dispatcher = new GraphicsDispatcher();
	        this.Dispatcher.AssignToThread();
            MapChunkRender.Initialize();
            MapAreaLowRender.Initialize();
            WmoGroupRender.Initialize();
            M2BatchRenderer.Initialize();
            M2SingleRenderer.Initialize();
            M2PortraitRenderer.Initialize();
            WorldText.Initialize();
            BoundingBoxDrawManager.Initialize();
            ChunkEditManager.Instance.Initialize();

            StaticAnimationThread.Instance.Initialize();

	        this.WmoManager.Initialize();
	        this.M2Manager.Initialize();
	        this.WorldTextManager.Initialize();

            SetActiveCamera(this.mMainCamera);
            TextureManager.Instance.Initialize();

	        this.MapManager.Initialize();

	        this.mMainCamera.ViewChanged += ViewChanged;
	        this.mMainCamera.ProjectionChanged += ProjectionChanged;

            OnResize(this.mWindow.Width, this.mWindow.Height);

            ViewChanged(this.mMainCamera, this.mMainCamera.View);
            ProjectionChanged(this.mMainCamera, this.mMainCamera.Projection);

	        this.CamControl = new CameraControl(window);
	        this.CamControl.PositionChanged += this.MapManager.UpdatePosition;

            if (!this.LeftHandedCamera)
            {
	            this.CamControl.InvertY = false;
            }

	        window.MouseDown += OnRenderWindowMouseDown;
        }

        public void OnEnterWorld(Vector3 position)
        {
	        this.State = AppState.World;
            TimeManager.Instance.Reset();
	        this.mMainCamera.SetParameters(position, position + Vector3.UnitX, Vector3.UnitZ, -Vector3.UnitY);
            UpdatePosition(position);
        }

        public void Shutdown()
        {
            TextureManager.Instance.Shutdown();
	        this.MapManager.Shutdown();
	        this.WmoManager.Shutdown();
            StaticAnimationThread.Instance.Shutdown();
	        this.M2Manager.Shutdown();
	        this.WorldTextManager.Shutdown();
        }

        public void OnFrame()
        {
	        this.Dispatcher.ProcessFrame();

	        this.CamControl.Update(this.ActiveCamera, this.State != AppState.World);

            EditManager.Instance.UpdateChanges();

            // do not move before mCamControl.Update to have the latest view/projection
            if (this.State == AppState.World)
            {
                UpdateCursorPosition();
                UpdateBrushTime(TimeManager.Instance.GetTime());
                CheckUpdateGlobalBuffer();
            }

	        // TODO: Figure out the purpose of this
            //GraphicsContext.Context.VertexShader.SetConstantBuffer(0, mGlobalBuffer.BufferID);
            //GraphicsContext.Context.PixelShader.SetConstantBuffer(0, mGlobalBuffer.BufferID);

	        this.MapManager.OnFrame(this.ActiveCamera);

            if (!this.HideWMO)
            {
	            this.WmoManager.OnFrame(this.ActiveCamera);
            }

	        if (!this.HideM2)
	        {
		        this.M2Manager.OnFrame(this.ActiveCamera);
	        }

	        this.WorldTextManager.OnFrame(this.ActiveCamera);
	        this.BoundingBoxDrawManager.OnFrame();
        }

        public void OnMouseWheel(int delta)
        {
	        this.CamControl.HandleMouseWheel(delta);
        }

        public void UpdateMapAmbient(Vector3 ambient)
        {
            lock (this.mGlobalBuffer)
            {
	            this.mGlobalBufferStore.ambientLight = new Vector4(ambient, 1.0f);
	            this.mGlobalBufferChanged = true;
            }
        }

        public void UpdateMapDiffuse(Vector3 diffuse)
        {
            lock (this.mGlobalBuffer)
            {
	            this.mGlobalBufferStore.diffuseLight = new Vector4(diffuse, 1.0f);
	            this.mGlobalBufferChanged = true;
            }
        }

        public void UpdateFogParams(Vector3 fogColor, float fogStart)
        {
            fogStart = Math.Min(fogStart, 899.0f);
            lock (this.mGlobalBuffer)
            {
	            this.mGlobalBufferStore.fogColor = new Vector4(fogColor, 1.0f);
	            this.mGlobalBufferStore.fogParams = new Vector4(fogStart, 900.0f, this.mMainCamera.FarClip, 0.0f);
	            this.mGlobalBufferChanged = true;
            }
        }

        public void UpdateSelectedBoundingBox()
        {
            if(this.mSelectedBoundingBox != null && this.mSelectedInstance != null)
            {
	            this.mSelectedBoundingBox.UpdateBoundingBox(this.mSelectedInstance.InstanceCorners);
            }
        }

        private void UpdateBrushTime(TimeSpan frameTime)
        {
            var timeSecs = frameTime.TotalMilliseconds / 1000.0;
	        this.mGlobalBufferStore.brushParams.Z = (float)timeSecs;
	        this.mGlobalBufferChanged = true;
        }

        private void UpdateCursorPosition(bool forced = false)
        {
            var pos = this.mWindow.PointToClient(InterfaceHelper.GetCursorPosition());
            if (this.LastMouseIntersection == null || pos.X != this.mLastCursorPosition.X || pos.Y != this.mLastCursorPosition.Y || forced)
            {
	            this.mLastCursorPosition = new Point(pos.X, pos.Y);
	            this.LastMouseIntersection = new IntersectionParams(this.ActiveCamera.ViewInverse, this.ActiveCamera.ProjectionInverse,
                    new Vector2(this.mLastCursorPosition.X, this.mLastCursorPosition.Y));

	            this.MapManager.Intersect(this.LastMouseIntersection);

                EditManager.Instance.MousePosition = this.LastMouseIntersection.TerrainPosition;
	            this.mGlobalBufferStore.mousePosition = new Vector4(this.LastMouseIntersection.TerrainPosition, 0.0f);
	            this.mGlobalBufferChanged = true;

                EditManager.Instance.IsTerrainHovered = this.LastMouseIntersection.TerrainHit;
                EditManager.Instance.MousePosition = this.LastMouseIntersection.TerrainPosition;

                ChunkEditManager.Instance.OnFrame();
            }
        }

        private void UpdateAppState(AppState newState)
        {
	        this.mState = newState;
        }

        private void SetActiveCamera(Camera camera)
        {
	        this.ActiveCamera = camera;
        }

        private void ViewChanged(Camera camera, Matrix4 matView)
        {
            if (camera != this.ActiveCamera)
            {
	            return;
            }

	        this.mGlobalBufferStore.matView = matView;
	        this.mGlobalBufferChanged = true;

	        this.M2Manager.ViewChanged();

            UpdateCursorPosition(true);
        }

        private void ProjectionChanged(Camera camera, Matrix4 matProj)
        {
            if (camera != this.ActiveCamera)
            {
	            return;
            }

	        // TODO: Get these values someplace else
            // var vp = GraphicsContext.Viewport;
	        this.mGlobalBufferStore.matProj = matProj;
	        this.mGlobalBufferStore.viewport = new Vector4(vp.Width, vp.Height, vp.MinDepth, vp.MaxDepth);

            var perspectiveCamera = camera as PerspectiveCamera;
            if (perspectiveCamera != null)
            {
	            this.mGlobalBufferStore.fogParams.Z = perspectiveCamera.FarClip;
            }

	        this.mGlobalBufferChanged = true;
	        this.M2Manager.ViewChanged();
        }

        private void CheckUpdateGlobalBuffer()
        {
            if (this.mGlobalBufferChanged)
            {
                lock (this.mGlobalBuffer)
                {
	                this.mGlobalBuffer.BufferData(this.mGlobalBufferStore);
	                this.mGlobalBufferChanged = false;
                }
            }
        }

        public bool RenderWindowContainsMouse()
        {
            return this.mWindow.ClientRectangle.Contains(this.mWindow.PointToClient(Cursor.Position));
        }

        private void OnRenderWindowMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            var pos = this.mWindow.PointToClient(InterfaceHelper.GetCursorPosition());
            var intersection = new IntersectionParams(this.ActiveCamera.ViewInverse, this.ActiveCamera.ProjectionInverse,
                new Vector2(pos.X, pos.Y));

	        this.MapManager.Intersect(intersection);
	        this.M2Manager.Intersect(intersection);
	        this.WmoManager.Intersect(intersection);

            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                IModelInstance selected = null;
                if (intersection.M2Hit)
                {
	                selected = intersection.M2Instance;
                }
                else if (intersection.WmoHit)
                {
	                selected = intersection.WmoInstance;
                }

	            if (selected != this.mSelectedInstance)
                {
                    if(this.mSelectedBoundingBox != null)
                    {
	                    this.BoundingBoxDrawManager.RemoveDrawableBox(this.mSelectedBoundingBox);
                    }

	                this.mSelectedBoundingBox = null;

                    if (this.mSelectedInstance != null)
                    {
	                    this.mSelectedInstance.DestroyModelNameplate();
                    }

	                if (ModelEditManager.Instance.IsCopying && selected != ModelEditManager.Instance.SelectedModel)
                    {
                        selected = ModelEditManager.Instance.SelectedModel;
	                    this.mSelectedBoundingBox = this.BoundingBoxDrawManager.AddDrawableBox(selected.InstanceCorners);
                    }
                    else if (selected != null && selected.IsSpecial == false)
                    {
                        selected.CreateModelNameplate();
	                    this.mSelectedBoundingBox = this.BoundingBoxDrawManager.AddDrawableBox(selected.InstanceCorners);
                        ModelEditManager.Instance.SelectedModel = selected;
                    }
                    else if (selected == null)
                    {
                        ModelEditManager.Instance.SelectedModel = null;
                    }

                    if(EditManager.Instance.CurrentMode != EditMode.Chunk)
                    {
	                    this.mSelectedInstance = selected;
                        ModelSpawnManager.Instance.ClickedInstance = selected as M2RenderInstance;
                    }
                }
            }

	        if (OnWorldClicked != null)
	        {
		        OnWorldClicked(intersection, Mouse.GetState());
	        }
        }
    }
}
