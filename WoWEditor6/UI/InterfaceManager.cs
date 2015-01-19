using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Threading;
using WoWEditor6.Graphics;
using WoWEditor6.Resources;
using WoWEditor6.Scene;
using WoWEditor6.UI.Components;
using WoWEditor6.UI.Views;

namespace WoWEditor6.UI
{
    class InterfaceManager
    {
        public static InterfaceManager Instance { get; } = new InterfaceManager();

        private Mesh mMesh;
        private GxContext mContext;
        private Sampler mQuadSampler;
        private IView mActiveView;
        private bool mIsResizing;

        private readonly Dictionary<AppState, IView> mViews = new Dictionary<AppState, IView>(); 

        public ComponentRoot Root { get; } = new ComponentRoot();
        public DrawSurface Surface { get; private set; }
        public MainWindow Window { get; private set; }

        public Dispatcher Dispatcher { get; private set; }

        public void Initialize(MainWindow window, GxContext context)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;

            mContext = context;
            Window = window;
            Surface = new DrawSurface(context);
            Surface.GraphicsInit();
            Surface.OnResize(window.ClientSize.Width, window.ClientSize.Height);
            mQuadSampler = new Sampler(context)
            {
                AddressMode = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                Filter = SharpDX.Direct3D11.Filter.MinMagMipPoint
            };

            Window.Text = Strings.MainWindowTitle;

            mViews.Add(AppState.Splash, new SplashView());
            mViews.Add(AppState.FileSystemInit, new FileSystemInitView());
            mViews.Add(AppState.MapSelect, new MapSelectView());
            mViews.Add(AppState.EntrySelect, new EntrySelectView());
            mViews.Add(AppState.LoadingScreen, new LoadingScreenView());
            mViews.Add(AppState.World, new WorldView());
            mActiveView = mViews[AppState.Splash];

            InitMesh();
            InitMessages();

            foreach(var pair in mViews)
                pair.Value.OnResize(new SharpDX.Vector2(Window.ClientSize.Width, Window.ClientSize.Height));
        }

        public void UpdateState(AppState state)
        {
            lock(mViews)
            {
                mViews.TryGetValue(state, out mActiveView);
                mActiveView?.OnShow();
            }
        }

        public T GetViewForState<T>(AppState state) where T : class, IView
        {
            lock(mViews)
            {
                IView activeView;
                if (mViews.TryGetValue(state, out activeView) == false)
                    return default(T);

                return activeView as T;
            }
        }

        public void OnFrame()
        {
            if (mActiveView is WorldView)
                return;

            Surface.RenderFrame(rt =>
            {
                Root.OnRender(rt);
                lock (mViews)
                    mActiveView?.OnRender(rt);
            });
            mMesh.Program.SetPixelTexture(0, Surface.NativeView);
            mMesh.Program.SetPixelSampler(0, mQuadSampler);

            mMesh.BeginDraw();
            mMesh.Draw();

            Surface.EndFrame();
        }

        private void InitMessages()
        {
            Window.MouseMove += (sender, args) =>
            {
                var msg = new MouseMessage(MessageType.MouseMove, new SharpDX.Vector2(args.X, args.Y), GetButton(args.Button));
                Root.OnMessage(msg);
                mActiveView?.OnMessage(msg);
            };

            Window.MouseDown += (sender, args) =>
            {
                var msg = new MouseMessage(MessageType.MouseDown, new SharpDX.Vector2(args.X, args.Y), GetButton(args.Button));
                Root.OnMessage(msg);
                mActiveView?.OnMessage(msg);
            };

            Window.MouseUp += (sender, args) =>
            {
                var msg = new MouseMessage(MessageType.MouseUp, new SharpDX.Vector2(args.X, args.Y), GetButton(args.Button));
                Root.OnMessage(msg);
                mActiveView?.OnMessage(msg);
            };

            Window.MouseWheel += (sender, args) =>
            {
                var msg = new MouseMessage(MessageType.MouseWheel, new SharpDX.Vector2(args.X, args.Y),
                    GetButton(args.Button)) { Delta = -args.Delta / 120 };
                Root.OnMessage(msg);
                mActiveView?.OnMessage(msg);
            };

            Window.KeyDown += (sender, args) =>
            {
                var c = KeyboardMessage.GetCharacter(args);
                var msg = new KeyboardMessage(MessageType.KeyDown, c, args.KeyCode);
                Root.OnMessage(msg);
                mActiveView?.OnMessage(msg);
            };

            Window.KeyUp += (sender, args) =>
            {
                var c = KeyboardMessage.GetCharacter(args);
                var msg = new KeyboardMessage(MessageType.KeyUp, c, args.KeyCode);
                Root.OnMessage(msg);
                mActiveView?.OnMessage(msg);
            };

            Window.ResizeBegin += (sender, args) => mIsResizing = true;
            Window.Resize += OnResize;
            Window.ResizeEnd += (sender, args) =>
            {
                mIsResizing = false;
                OnResize(sender, args);
            };
        }

        private void OnResize(object sender, EventArgs args)
        {
            if (mIsResizing)
                return;

            if (Window.WindowState == FormWindowState.Minimized)
                return;

            Surface.OnResize(Window.ClientSize.Width, Window.ClientSize.Height);
            lock(mViews)
                foreach (var pair in mViews)
                    pair.Value.OnResize(new SharpDX.Vector2(Window.ClientSize.Width, Window.ClientSize.Height));

            WorldFrame.Instance.OnResize(Window.ClientSize.Width, Window.ClientSize.Height);
        }

        private static MouseButton GetButton(MouseButtons button)
        {
            switch(button)
            {
                case MouseButtons.Left:
                    return MouseButton.Left;

                case MouseButtons.Right:
                    return MouseButton.Right;

                case MouseButtons.Middle:
                    return MouseButton.Middle;

                default:
                    return MouseButton.Left;
            }
        }

        private void InitMesh()
        {
            var program = new ShaderProgram(mContext);
            program.SetVertexShader(Shaders.TexturedQuadVertex, "main");
            program.SetPixelShader(Shaders.TextureQuadPixel, "main");

            mMesh = new Mesh(mContext) {IndexCount = 6, Stride = 16};

            mMesh.VertexBuffer.UpdateData(new[]
            {
                -1.0f, 1.0f, 0.0f, 0.0f,
                1.0f, 1.0f, 1.0f, 0.0f,
                1.0f, -1.0f, 1.0f, 1.0f,
                -1.0f, -1.0f, 0.0f, 1.0f
            });
            mMesh.IndexBuffer.UpdateData(new uint[] {0, 2, 1, 0, 3, 2});
            mMesh.IndexBuffer.IndexFormat = SharpDX.DXGI.Format.R32_UInt;

            mMesh.AddElement("POSITION", 0, 2);
            mMesh.AddElement("TEXCOORD", 0, 2);
            mMesh.Program = program;
            mMesh.DepthState.DepthEnabled = false;
            mMesh.BlendState.BlendEnabled = true;
        }
    }
}
