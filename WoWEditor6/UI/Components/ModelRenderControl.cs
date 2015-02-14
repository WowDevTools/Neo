using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D11;
using WoWEditor6.Graphics;
using WoWEditor6.IO;
using WoWEditor6.IO.Files;
using WoWEditor6.Scene;
using WoWEditor6.Scene.Models.M2;
using WoWEditor6.Scene.Texture;

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

        public void SetCreatureDisplayEntry(int entry)
        {
            var displayInfo = Storage.DbcStorage.CreatureDisplayInfo.GetRowById(entry);
            if (displayInfo == null)
                return;

            var modelId = displayInfo.GetInt32(1);
            var modelData = Storage.DbcStorage.CreatureModelData.GetRowById(modelId);
            if (modelData == null)
                return;

            var modelPath = string.Empty;
            if (FileManager.Instance.Version <= FileDataVersion.Warlords)
            {
                var fileDataId = modelData.GetInt32(2);
                var modelPathEntry = Storage.DbcStorage.FileData.GetRowById(fileDataId);
                if (modelPathEntry != null)
                    modelPath = Path.Combine(modelPathEntry.GetString(2), modelPathEntry.GetString(1));
            }
            else
                modelPath = modelData.GetString(2);

            if (string.IsNullOrEmpty(modelPath))
                return;

            if (modelPath.ToUpperInvariant().EndsWith(".MDX"))
                modelPath = Path.ChangeExtension(modelPath, ".m2");

            if (FileManager.Instance.Provider.Exists(modelPath) == false)
                return;

            var file = IO.Files.Models.ModelFactory.Instance.CreateM2(modelPath);
            try
            {
                if (file.Load() == false)
                    return;
            }
            catch (Exception)
            {
                return;
            }

            mRenderer = new M2ModelRenderer(file);
            for (var i = 0; i < mRenderer.Textures.Length; ++i)
            {
                var tex = mRenderer.Textures[i];
                var root = mRenderer.Model.ModelRoot;

                switch (tex.TextureType)
                {
                    case 13:
                    case 12:
                    case 11:
                        tex.Texture = TextureManager.Instance.GetTexture(GetSkinName(root, displayInfo, tex.TextureType - 11));
                        break;
                }
            }
        }

        private string GetSkinName(string root, DbcRecord displayInfo, int index)
        {
            var skinString = displayInfo.GetString(6 + index);
            return string.IsNullOrEmpty(skinString)
                ? "default_texture"
                : string.Format("{0}\\{1}.blp", root, skinString);
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
