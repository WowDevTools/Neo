using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using WoWEditor6.Graphics;
using WoWEditor6.Resources;
using WoWEditor6.UI.Components;
using WoWEditor6.UI.Views;

namespace WoWEditor6.UI
{
    class InterfaceManager
    {
        public static InterfaceManager Instance { get; } = new InterfaceManager();

        private MainWindow mWindow;
        private Mesh mMesh;
        private GxContext mContext;
        private Sampler mQuadSampler;
        private IView mActiveView;

        private readonly Dictionary<Scene.AppState, IView> mViews = new Dictionary<Scene.AppState, IView>(); 

        public ComponentRoot Root { get; } = new ComponentRoot();
        public DrawSurface Surface { get; private set; }

        public void Initialize(MainWindow window, GxContext context)
        {
            mContext = context;
            mWindow = window;
            Surface = new DrawSurface(context);
            Surface.GraphicsInit();
            Surface.OnResize(window.ClientSize.Width, window.ClientSize.Height);
            mQuadSampler = new Sampler(context)
            {
                AddressMode = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                Filter = SharpDX.Direct3D11.Filter.MinMagMipPoint
            };

            mWindow.Text = Strings.MainWindowTitle;

            mViews.Add(Scene.AppState.Splash, new SplashView());
            mActiveView = mViews[Scene.AppState.Splash];

            InitMesh();
            InitMessages();

            foreach(var pair in mViews)
                pair.Value.OnResize(new SharpDX.Vector2(mWindow.Width, mWindow.Height));
        }

        public void OnFrame()
        {
            Surface.RenderFrame(rt =>
            {
                Root.OnRender(rt);
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
            mWindow.MouseMove += (sender, args) =>
            {
                var msg = new MouseMessage(MessageType.MouseMove, new SharpDX.Vector2(args.X, args.Y), GetButton(args.Button));
                Root.OnMessage(msg);
                mActiveView?.OnMessage(msg);
            };

            mWindow.MouseDown += (sender, args) =>
            {
                var msg = new MouseMessage(MessageType.MouseDown, new SharpDX.Vector2(args.X, args.Y), GetButton(args.Button));
                Root.OnMessage(msg);
                mActiveView?.OnMessage(msg);
            };

            mWindow.MouseUp += (sender, args) =>
            {
                var msg = new MouseMessage(MessageType.MouseUp, new SharpDX.Vector2(args.X, args.Y), GetButton(args.Button));
                Root.OnMessage(msg);
                mActiveView?.OnMessage(msg);
            };

            mWindow.KeyDown += (sender, args) =>
            {
                var c = KeyboardMessage.GetCharacter(args);
                var msg = new KeyboardMessage(MessageType.KeyDown, c, args.KeyCode);
                Root.OnMessage(msg);
                mActiveView?.OnMessage(msg);
            };

            mWindow.KeyUp += (sender, args) =>
            {
                var c = KeyboardMessage.GetCharacter(args);
                var msg = new KeyboardMessage(MessageType.KeyUp, c, args.KeyCode);
                Root.OnMessage(msg);
                mActiveView?.OnMessage(msg);
            };
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
