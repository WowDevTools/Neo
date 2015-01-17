using WoWEditor6.Graphics;
using WoWEditor6.Resources;

namespace WoWEditor6.UI
{
    class InterfaceManager
    {
        public static InterfaceManager Instance { get; } = new InterfaceManager();

        private DrawSurface mSurface;
        private MainWindow mWindow;
        private Mesh mMesh;
        private GxContext mContext;
        private Sampler mQuadSampler;

        public void Initialize(MainWindow window, GxContext context)
        {
            mContext = context;
            mWindow = window;
            mSurface = new DrawSurface(context);
            mSurface.GraphicsInit();
            mSurface.OnResize(window.ClientSize.Width, window.ClientSize.Height);
            mQuadSampler = new Sampler(context)
            {
                AddressMode = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                Filter = SharpDX.Direct3D11.Filter.MinMagMipPoint
            };

            mWindow.Text = Strings.MainWindowTitle;

            InitMesh();
        }

        public void OnFrame()
        {
            mSurface.RenderFrame(null);
            mMesh.Program.SetPixelTexture(0, mSurface.NativeView);
            mMesh.Program.SetPixelSampler(0, mQuadSampler);

            mMesh.BeginDraw();
            mMesh.Draw();

            mSurface.EndFrame();
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
