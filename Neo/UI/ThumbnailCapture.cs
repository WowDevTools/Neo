using System;
using System.Drawing;
using System.Drawing.Imaging;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Neo.Graphics;
using Neo.IO.Files.Models;
using Neo.Scene;
using Neo.Scene.Models.M2;
using Color = System.Drawing.Color;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Rectangle = System.Drawing.Rectangle;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace Neo.UI
{
    class ThumbnailCapture : IDisposable
    {
        private PerspectiveCamera mCamera;
        private UniformBuffer mMatrixBuffer;
        private RenderTarget mTarget;
        private Texture2D mResolveTexture;
        private Texture2D mMapTexture;
        private Timer renderTimer;
        private int mThumbnailCaptureFrame = CAPTURE_FRAME;
        private M2Renderer mRenderer;
        private ConcurrentQueue<string> mModels = new ConcurrentQueue<string>();

        private readonly int ImgWidth;
        private readonly int ImgHeight;
        private const int CAPTURE_FRAME = 12;

        public ThumbnailCapture(int Width, int Height)
        {
            ImgWidth = Width;
            ImgHeight = Height;

            if (WorldFrame.Instance == null || WorldFrame.Instance.GraphicsContext == null)
                return;

            mTarget = new RenderTarget(WorldFrame.Instance.GraphicsContext);
            mMatrixBuffer = new UniformBuffer(WorldFrame.Instance.GraphicsContext);

            mCamera = new PerspectiveCamera();
            mCamera.ViewChanged += delegate { mMatrixBuffer.UpdateData(mCamera.ViewProjection); };
            mCamera.ProjectionChanged += delegate { mMatrixBuffer.UpdateData(mCamera.ViewProjection); };
            mCamera.SetClip(0.2f, 1000.0f);
            mCamera.SetParameters(new Vector3(10, 0, 0), Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);

            renderTimer = new Timer();
            renderTimer.Interval = 10;
            renderTimer.Tick += OnRenderTimerTick;

            var texDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Height = ImgWidth,
                Width = ImgWidth,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1, 0),
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

            mTarget.Resize(ImgWidth, ImgHeight, true);
            mCamera.SetAspect((float)ImgWidth / ImgHeight);
        }

        public void AddModel(string model)
        {
            mModels.Enqueue(model); //Add to list
            if (!renderTimer.Enabled) //Check if it is ready to go
                LoadModel();
        }

        void LoadModel()
        {
            string model;
            mModels.TryDequeue(out model);

            var file = ModelFactory.Instance.CreateM2(model);
            if (file.Load() == false)
            {
                if (mModels.Count > 0)
                    LoadModel();
                else
                    return;
            }

            mRenderer = new M2Renderer(file);
            var bboxMin = file.BoundingBox.Minimum.Z;
            var bboxMax = file.BoundingBox.Maximum.Z;
            WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
            {
                mCamera.SetParameters(new Vector3(file.BoundingRadius * 1.5f, 0, bboxMin + (bboxMax - bboxMin) / 2),
                    new Vector3(0, 0, bboxMin + (bboxMax - bboxMin) / 2), Vector3.UnitZ, Vector3.UnitY);
            });

            mThumbnailCaptureFrame = CAPTURE_FRAME;
            renderTimer.Start();
        }

        void OnRenderTimerTick(object sender, EventArgs args)
        {
            if (WorldFrame.Instance.Dispatcher.InvokeRequired)
                WorldFrame.Instance.Dispatcher.BeginInvoke(OnRenderModel);
            else
                OnRenderModel();
        }

        unsafe void OnRenderModel()
        {
            if (mRenderer == null)
                return;

            mTarget.Clear();
            mTarget.Apply();

            var ctx = WorldFrame.Instance.GraphicsContext;
            var vp = ctx.Viewport;
            ctx.Context.Rasterizer.SetViewport(new Viewport(0, 0, ImgWidth, ImgHeight, 0.0f, 1.0f));

            ctx.Context.VertexShader.SetConstantBuffer(0, mMatrixBuffer.Native);
            mRenderer.RenderPortrait();

            mTarget.Remove();
            ctx.Context.Rasterizer.SetViewport(vp);

            ctx.Context.ResolveSubresource(mTarget.Texture, 0, mResolveTexture, 0, Format.B8G8R8A8_UNorm);
            ctx.Context.CopyResource(mResolveTexture, mMapTexture);

            var box = ctx.Context.MapSubresource(mMapTexture, 0, MapMode.Read, MapFlags.None);
            var bmp = new Bitmap(ImgWidth, ImgHeight, PixelFormat.Format32bppArgb);
            var bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            byte* ptrDst = (byte*)bmpd.Scan0.ToPointer();
            byte* ptrSrc = (byte*)box.DataPointer.ToPointer();

            for (var i = 0; i < bmp.Height; ++i)
            {
                UnsafeNativeMethods.CopyMemory(ptrDst + i * bmp.Width * 4, ptrSrc + i * box.RowPitch, bmp.Width * 4);
            }

            bmp.UnlockBits(bmpd);
            ctx.Context.UnmapSubresource(mMapTexture, 0);

            //Cache thumbnail
            if (mThumbnailCaptureFrame > 0 && --mThumbnailCaptureFrame == 0)
            {
                renderTimer.Stop();

                Bitmap thumbnail = new Bitmap(ImgWidth, ImgHeight);
                using (var g = System.Drawing.Graphics.FromImage(thumbnail))
                {
                    g.Clear(Color.Black);
                    g.DrawImage(bmp, new PointF(0, 0));
                }

                ThumbnailCache.Cache(mRenderer.Model.FileName, thumbnail);
                if (mModels.Count > 0) //More models so render next
                    LoadModel();
            }
        }

        public void Dispose()
        {
            ((IDisposable)renderTimer).Dispose();
        }
    }
}
