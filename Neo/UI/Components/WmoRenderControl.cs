using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Neo.Graphics;
using Neo.IO;
using Neo.IO.Files;
using Neo.IO.Files.Models;
using Neo.Scene;
using Neo.Scene.Models.M2;
using Neo.Scene.Texture;
using Neo.Storage;
using Color = System.Drawing.Color;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Rectangle = System.Drawing.Rectangle;
using Neo.Scene.Models.WMO;
using Neo.Scene.Models;

namespace Neo.UI.Components
{
    public partial class WmoRenderControl : UserControl
    {
        private PerspectiveCamera mCamera;
        private CameraControl mCamControl;
        private ConstantBuffer mMatrixBuffer;
        private RenderTarget mTarget;
        private Texture2D mResolveTexture;
        private Texture2D mMapTexture;
        private Bitmap mPaintBitmap;

        private WmoBatchRender mRender;

        public override bool Focused
        {
            get
            {
                return base.Focused;
            }
        }

        public WmoRenderControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.Selectable, true);
            InitializeComponent();
        }

        public void SetModel(string model, int variation = 0)
        {
            var root = ModelFactory.Instance.CreateWmo();
            if (root.Load(model) == false)
            {
	            return;
            }

	        var file = new WmoRootRender();
            file.OnAsyncLoad(root);
	        this.mRender = new WmoBatchRender(file);
	        this.mRender.AddInstance(1, Vector3.Zero, Vector3.Zero);

	        this.mCamera.SetParameters(file.BoundingBox.Maximum, Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            if (this.mPaintBitmap != null)
            {
                e.Graphics.DrawImage(this.mPaintBitmap, new PointF(0, 0));
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Site != null && Site.DesignMode)
            {
	            return;
            }

	        if (WorldFrame.Instance == null || WorldFrame.Instance.GraphicsContext == null)
	        {
		        return;
	        }

	        this.mTarget = new RenderTarget(WorldFrame.Instance.GraphicsContext);
	        this.mMatrixBuffer = new ConstantBuffer(WorldFrame.Instance.GraphicsContext);

	        this.mCamera = new PerspectiveCamera();
	        this.mCamera.ViewChanged += ViewChanged;
	        this.mCamera.ProjectionChanged += ProjChanged;
	        this.mCamera.SetClip(0.2f, 1000.0f);
	        this.mCamera.SetParameters(Vector3.Zero, Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
	        this.mCamControl = new CameraControl(this)
            {
                TurnFactor = 0.1f,
                SpeedFactor = 20.0f
            };

            MouseDown += OnClick;
            Resize += OnResize;
	        this.renderTimer.Tick += OnRenderTimerTick;

            OnResize(this, null);

	        this.renderTimer.Start();
        }

	    private void OnResize(object sender, EventArgs args)
        {
            var texDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Height = ClientSize.Height,
                Width = ClientSize.Width,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1, 0),
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1
            };

            if (this.mResolveTexture != null)
            {
	            this.mResolveTexture.Dispose();
            }

	        this.mResolveTexture = new Texture2D(WorldFrame.Instance.GraphicsContext.Device, texDesc);

            if (this.mMapTexture != null)
            {
	            this.mMapTexture.Dispose();
            }

	        texDesc.CpuAccessFlags = CpuAccessFlags.Read;
            texDesc.Usage = ResourceUsage.Staging;
	        this.mMapTexture = new Texture2D(WorldFrame.Instance.GraphicsContext.Device, texDesc);

	        this.mTarget.Resize(ClientSize.Width, ClientSize.Height, true);
        }

	    private void OnClick(object sender, MouseEventArgs args)
        {
            Focus();
        }

	    private void OnRenderTimerTick(object sender, EventArgs args)
        {
	        this.mCamControl.Update(this.mCamera, false);
            if (WorldFrame.Instance.Dispatcher.InvokeRequired)
            {
	            WorldFrame.Instance.Dispatcher.BeginInvoke(OnRenderModel);
            }
            else
            {
	            OnRenderModel();
            }
        }

	    private void ViewChanged(Camera cam, Matrix matView)
        {
	        this.mMatrixBuffer.UpdateData(cam.ViewProjection);
        }

	    private void ProjChanged(Camera cam, Matrix matProj)
        {
	        this.mMatrixBuffer.UpdateData(cam.ViewProjection);
        }

	    private unsafe void OnRenderModel()
        {
	        this.mTarget.Clear();
	        this.mTarget.Apply();

            var ctx = WorldFrame.Instance.GraphicsContext;
            var vp = ctx.Viewport;
            ctx.Context.Rasterizer.SetViewport(new Viewport(0, 0, ClientSize.Width, ClientSize.Height, 0.0f, 1.0f));
            ctx.Context.VertexShader.SetConstantBuffer(0, this.mMatrixBuffer.Native);

            WmoGroupRender.Mesh.BeginDraw();
            WmoGroupRender.Mesh.Program.SetPixelSampler(0, WmoGroupRender.Sampler);
	        this.mRender.OnFrame();

	        this.mTarget.Remove();
            ctx.Context.Rasterizer.SetViewport(vp);

            ctx.Context.ResolveSubresource(this.mTarget.Texture, 0, this.mResolveTexture, 0, Format.B8G8R8A8_UNorm);
            ctx.Context.CopyResource(this.mResolveTexture, this.mMapTexture);

            var box = ctx.Context.MapSubresource(this.mMapTexture, 0, MapMode.Read, MapFlags.None);
            var bmp = new Bitmap(ClientSize.Width, ClientSize.Height, PixelFormat.Format32bppArgb);
            var bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            byte* ptrDst = (byte*)bmpd.Scan0.ToPointer();
            byte* ptrSrc = (byte*)box.DataPointer.ToPointer();

            for (var i = 0; i < bmp.Height; ++i)
            {
                UnsafeNativeMethods.CopyMemory(ptrDst + i * bmp.Width * 4, ptrSrc + i * box.RowPitch, bmp.Width * 4);
            }

            bmp.UnlockBits(bmpd);
            if (this.mPaintBitmap != null)
            {
	            this.mPaintBitmap.Dispose();
            }

	        this.mPaintBitmap = bmp;
            ctx.Context.UnmapSubresource(this.mMapTexture, 0);
            Invalidate();
        }
    }
}
