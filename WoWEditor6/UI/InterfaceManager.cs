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
        public static InterfaceManager Instance { get; private set; }

        private Mesh mMesh;
        private GxContext mContext;
        private Sampler mQuadSampler;
        private IView mActiveView;

        private readonly Dictionary<AppState, IView> mViews = new Dictionary<AppState, IView>(); 

        public ComponentRoot Root { get; private set; }
        public DrawSurface Surface { get; private set; }
        public RenderControl RenderWindow { get; private set; }

        public Dispatcher Dispatcher { get; private set; }

        static InterfaceManager()
        {
            Instance = new InterfaceManager();
        }

        private InterfaceManager()
        {
            Root = new ComponentRoot();
        }

        public void Initialize(RenderControl window, GxContext context)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;

            mContext = context;
            RenderWindow = window;
            Surface = new DrawSurface(context);
            Surface.GraphicsInit();
            Surface.OnResize(window.ClientSize.Width, window.ClientSize.Height);
            mQuadSampler = new Sampler(context)
            {
                AddressMode = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                Filter = SharpDX.Direct3D11.Filter.MinMagMipPoint
            };

            RenderWindow.Text = Strings.MainWindowTitle;

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
                pair.Value.OnResize(new SharpDX.Vector2(RenderWindow.ClientSize.Width, RenderWindow.ClientSize.Height));
        }

        public void UpdateState(AppState state)
        {
            lock(mViews)
            {
                mViews.TryGetValue(state, out mActiveView);
                if (mActiveView != null)
                    mActiveView.OnShow();
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
            Surface.RenderFrame(rt =>
            {
                Root.OnRender(rt);
                lock (mViews)
                {
                    if (mActiveView != null)
                        mActiveView.OnRender(rt);
                }
            });
            mMesh.Program.SetPixelTexture(0, Surface.NativeView);
            mMesh.Program.SetPixelSampler(0, mQuadSampler);

            mMesh.BeginDraw();
            mMesh.Draw();

            Surface.EndFrame();
        }

        private void InitMessages()
        {
            RenderWindow.MouseMove += (sender, args) =>
            {
                var msg = new MouseMessage(MessageType.MouseMove, new SharpDX.Vector2(args.X, args.Y), GetButton(args.Button));
                Root.OnMessage(msg);
                if (mActiveView != null)
                    mActiveView.OnMessage(msg);
            };

            RenderWindow.MouseDown += (sender, args) =>
            {
                var msg = new MouseMessage(MessageType.MouseDown, new SharpDX.Vector2(args.X, args.Y), GetButton(args.Button));
                Root.OnMessage(msg);
                if (mActiveView != null)
                    mActiveView.OnMessage(msg);
            };

            RenderWindow.MouseUp += (sender, args) =>
            {
                var msg = new MouseMessage(MessageType.MouseUp, new SharpDX.Vector2(args.X, args.Y), GetButton(args.Button));
                Root.OnMessage(msg);
                if (mActiveView != null)
                    mActiveView.OnMessage(msg);
            };

            RenderWindow.MouseWheel += (sender, args) =>
            {
                var msg = new MouseMessage(MessageType.MouseWheel, new SharpDX.Vector2(args.X, args.Y),
                    GetButton(args.Button)) { Delta = -args.Delta / 120 };
                Root.OnMessage(msg);
                if (mActiveView != null)
                    mActiveView.OnMessage(msg);
                WorldFrame.Instance.OnMouseWheel(args.Delta);
            };

            RenderWindow.KeyDown += (sender, args) =>
            {
                var c = KeyboardMessage.GetCharacter(args);
                var msg = new KeyboardMessage(MessageType.KeyDown, c, args.KeyCode);
                Root.OnMessage(msg);
                if (mActiveView != null)
                    mActiveView.OnMessage(msg);
            };

            RenderWindow.KeyUp += (sender, args) =>
            {
                var c = KeyboardMessage.GetCharacter(args);
                var msg = new KeyboardMessage(MessageType.KeyUp, c, args.KeyCode);
                Root.OnMessage(msg);
                if (mActiveView != null)
                    mActiveView.OnMessage(msg);
            };

            RenderWindow.Resize += OnResize;
        }

        private void OnResize(object sender, EventArgs args)
        {
            Surface.OnResize(RenderWindow.ClientSize.Width, RenderWindow.ClientSize.Height);
            lock(mViews)
                foreach (var pair in mViews)
                    pair.Value.OnResize(new SharpDX.Vector2(RenderWindow.ClientSize.Width, RenderWindow.ClientSize.Height));

            WorldFrame.Instance.OnResize(RenderWindow.ClientSize.Width, RenderWindow.ClientSize.Height);
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
