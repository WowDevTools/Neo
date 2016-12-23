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
            mRender = new WmoBatchRender(file);
            mRender.AddInstance(1, Vector3.Zero, Vector3.Zero);

            mCamera.SetParameters(file.BoundingBox.Maximum, Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            if (mPaintBitmap != null)
            {
                e.Graphics.DrawImage(mPaintBitmap, new PointF(0, 0));
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

	        mTarget = new RenderTarget(WorldFrame.Instance.GraphicsContext);
            mMatrixBuffer = new ConstantBuffer(WorldFrame.Instance.GraphicsContext);

            mCamera = new PerspectiveCamera();
            mCamera.ViewChanged += ViewChanged;
            mCamera.ProjectionChanged += ProjChanged;
            mCamera.SetClip(0.2f, 1000.0f);
            mCamera.SetParameters(Vector3.Zero, Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            mCamControl = new CameraControl(this)
            {
                TurnFactor = 0.1f,
                SpeedFactor = 20.0f
            };

            MouseDown += OnClick;
            Resize += OnResize;
            renderTimer.Tick += OnRenderTimerTick;

            OnResize(this, null);

            renderTimer.Start();
        }

        void OnResize(object sender, EventArgs args)
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

            if (mResolveTexture != null)
            {
	            this.mResolveTexture.Dispose();
            }

	        mResolveTexture = new Texture2D(WorldFrame.Instance.GraphicsContext.Device, texDesc);

            if (mMapTexture != null)
            {
	            this.mMapTexture.Dispose();
            }

	        texDesc.CpuAccessFlags = CpuAccessFlags.Read;
            texDesc.Usage = ResourceUsage.Staging;
            mMapTexture = new Texture2D(WorldFrame.Instance.GraphicsContext.Device, texDesc);

            mTarget.Resize(ClientSize.Width, ClientSize.Height, true);
        }

        void OnClick(object sender, MouseEventArgs args)
        {
            Focus();
        }

        void OnRenderTimerTick(object sender, EventArgs args)
        {
            mCamControl.Update(mCamera, false);
            if (WorldFrame.Instance.Dispatcher.InvokeRequired)
            {
	            WorldFrame.Instance.Dispatcher.BeginInvoke(OnRenderModel);
            }
            else
            {
	            OnRenderModel();
            }
        }

        void ViewChanged(Camera cam, Matrix matView)
        {
            mMatrixBuffer.UpdateData(cam.ViewProjection);
        }

        void ProjChanged(Camera cam, Matrix matProj)
        {
            mMatrixBuffer.UpdateData(cam.ViewProjection);
        }

        unsafe void OnRenderModel()
        {
            mTarget.Clear();
            mTarget.Apply();

            var ctx = WorldFrame.Instance.GraphicsContext;
            var vp = ctx.Viewport;
            ctx.Context.Rasterizer.SetViewport(new Viewport(0, 0, ClientSize.Width, ClientSize.Height, 0.0f, 1.0f));
            ctx.Context.VertexShader.SetConstantBuffer(0, mMatrixBuffer.Native);

            WmoGroupRender.Mesh.BeginDraw();
            WmoGroupRender.Mesh.Program.SetPixelSampler(0, WmoGroupRender.Sampler);
            mRender.OnFrame();

            mTarget.Remove();
            ctx.Context.Rasterizer.SetViewport(vp);

            ctx.Context.ResolveSubresource(mTarget.Texture, 0, mResolveTexture, 0, Format.B8G8R8A8_UNorm);
            ctx.Context.CopyResource(mResolveTexture, mMapTexture);

            var box = ctx.Context.MapSubresource(mMapTexture, 0, MapMode.Read, MapFlags.None);
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
            if (mPaintBitmap != null)
            {
	            this.mPaintBitmap.Dispose();
            }

	        mPaintBitmap = bmp;
            ctx.Context.UnmapSubresource(mMapTexture, 0);
            Invalidate();
        }
    }
}
