using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D11;
using WoWEditor6.Graphics;
using WoWEditor6.Scene;

namespace WoWEditor6.UI.Components
{
    public partial class ModelRenderControl : UserControl
    {
        private PerspectiveCamera mCamera;
        private CameraControl mCamControl;
        private ConstantBuffer mMatrixBuffer;
        private RenderTarget mTarget;
        private Texture2D mResolveTexture;
        private Texture2D mMapTexture;
        private Bitmap mPaintBitmap;

        private Scene.Models.M2.M2ModelRenderer mRenderer;


        public ModelRenderControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque | ControlStyles.UserPaint, true);
            InitializeComponent();
        }

        public void SetModel(string model)
        {
            var file = IO.Files.Models.ModelFactory.Instance.CreateM2(model);
            if (file.Load() == false)
                return;

            mRenderer = new Scene.Models.M2.M2ModelRenderer(file);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(System.Drawing.Color.Black);
            if (mPaintBitmap != null)
                e.Graphics.DrawImage(mPaintBitmap, new PointF(0, 0));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Site != null && Site.DesignMode)
                return;

            if (WorldFrame.Instance == null || WorldFrame.Instance.GraphicsContext == null)
                return;

            mTarget = new RenderTarget(WorldFrame.Instance.GraphicsContext);
            mMatrixBuffer = new ConstantBuffer(WorldFrame.Instance.GraphicsContext);

            mCamera = new PerspectiveCamera();
            mCamera.ViewChanged += ViewChanged;
            mCamera.ProjectionChanged += ProjChanged;
            mCamera.SetClip(0.2f, 1000.0f);
            mCamera.SetParameters(new Vector3(10, 0, 0), Vector3.Zero, Vector3.UnitZ, -Vector3.UnitY);
            mCamControl = new CameraControl(this);


            MouseClick += OnClick;
            Resize += OnResize;
            renderTimer.Tick += OnRenderTimerTick;

            OnResize(this, null);

            renderTimer.Start();
        }

        void OnResize(object sender, EventArgs args)
        {
            var texDesc = new Texture2DDescription
            {
                ArraySize = 1, BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                Height = ClientSize.Height,
                Width = ClientSize.Width,
                Usage = ResourceUsage.Default,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1
            };

            if (mResolveTexture != null)
                mResolveTexture.Dispose();

            mResolveTexture = new Texture2D(WorldFrame.Instance.GraphicsContext.Device, texDesc);

            if (mMapTexture != null) mMapTexture.Dispose();

            texDesc.CpuAccessFlags = CpuAccessFlags.Read;
            texDesc.Usage = ResourceUsage.Staging;
            mMapTexture = new Texture2D(WorldFrame.Instance.GraphicsContext.Device, texDesc);

            mTarget.Resize(ClientSize.Width, ClientSize.Height, true);
            mCamera.SetAspect((float) ClientSize.Width / ClientSize.Height);
        }

        void OnClick(object sender, MouseEventArgs args)
        {
            Focus();
        }

        void OnRenderTimerTick(object sender, EventArgs args)
        {
            mCamControl.Update(mCamera, false);
            WorldFrame.Instance.Dispatcher.BeginInvoke(OnRenderModel);
        }

        void ViewChanged(Camera cam, Matrix matView)
        {
            mMatrixBuffer.UpdateData(new[] {cam.View, cam.Projection});

        }

        void ProjChanged(Camera cam, Matrix matProj)
        {
            mMatrixBuffer.UpdateData(new[] {cam.View, cam.Projection});
        }

        unsafe void OnRenderModel()
        {
            if (mRenderer == null)
                return;

            mTarget.Clear();
            mTarget.Apply();

            var ctx = WorldFrame.Instance.GraphicsContext;
            var vp = ctx.Viewport;
            ctx.Context.Rasterizer.SetViewport(new Viewport(0, 0, ClientSize.Width, ClientSize.Height, 0.0f, 1.0f));

            ctx.Context.VertexShader.SetConstantBuffer(0, mMatrixBuffer.Native);

            mRenderer.OnFrame();

            mTarget.Remove();
            ctx.Context.Rasterizer.SetViewport(vp);

            ctx.Context.ResolveSubresource(mTarget.Texture, 0, mResolveTexture, 0, SharpDX.DXGI.Format.B8G8R8A8_UNorm);
            ctx.Context.CopyResource(mResolveTexture, mMapTexture);

            var box = ctx.Context.MapSubresource(mMapTexture, 0, MapMode.Read, MapFlags.None);
            var bmp = new Bitmap(ClientSize.Width, ClientSize.Height, PixelFormat.Format32bppArgb);
            var bmpd = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            byte* ptrDst = (byte*) bmpd.Scan0.ToPointer();
            byte* ptrSrc = (byte*) box.DataPointer.ToPointer();

            for(var i = 0; i < bmp.Height; ++i)
            {
                UnsafeNativeMethods.CopyMemory(ptrDst + i * bmp.Width * 4, ptrSrc + i * box.RowPitch, bmp.Width * 4);
            }

            bmp.UnlockBits(bmpd);
            if (mPaintBitmap != null)
                mPaintBitmap.Dispose();

            mPaintBitmap = bmp;
            ctx.Context.UnmapSubresource(mMapTexture, 0);
            Invalidate();
        }
    }
}
