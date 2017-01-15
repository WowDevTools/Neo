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
using OpenTK;

namespace Neo.UI
{
	internal class ThumbnailCapture : IDisposable
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
	        this.ImgWidth = Width;
	        this.ImgHeight = Height;

            if (WorldFrame.Instance == null || WorldFrame.Instance.GraphicsContext == null)
            {
	            return;
            }

	        this.mTarget = new RenderTarget(WorldFrame.Instance.GraphicsContext);
	        this.mMatrixBuffer = new UniformBuffer();

	        this.mCamera = new PerspectiveCamera();
	        this.mCamera.ViewChanged += delegate { this.mMatrixBuffer.BufferData(this.mCamera.ViewProjection); };
	        this.mCamera.ProjectionChanged += delegate { this.mMatrixBuffer.BufferData(this.mCamera.ViewProjection); };
	        this.mCamera.SetClip(0.2f, 1000.0f);
	        this.mCamera.SetParameters(new Vector3(10, 0, 0), Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);

	        this.renderTimer = new Timer();
	        this.renderTimer.Interval = 10;
	        this.renderTimer.Tick += OnRenderTimerTick;

            var texDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Height = this.ImgWidth,
                Width = this.ImgWidth,
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

	        this.mTarget.Resize(this.ImgWidth, this.ImgHeight, true);
	        this.mCamera.SetAspect((float) this.ImgWidth / this.ImgHeight);
        }

        public void AddModel(string model)
        {
	        this.mModels.Enqueue(model); //Add to list
            if (!this.renderTimer.Enabled) //Check if it is ready to go
            {
	            LoadModel();
            }
        }

	    private void LoadModel()
        {
            string model;
	        this.mModels.TryDequeue(out model);

            var file = ModelFactory.Instance.CreateM2(model);
            if (file.Load() == false)
            {
                if (this.mModels.Count > 0)
                {
	                LoadModel();
                }
                else
                {
	                return;
                }
            }

	        this.mRenderer = new M2Renderer(file);
            var bboxMin = file.BoundingBox.Minimum.Z;
            var bboxMax = file.BoundingBox.Maximum.Z;
            WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
            {
	            this.mCamera.SetParameters(new Vector3(file.BoundingRadius * 1.5f, 0, bboxMin + (bboxMax - bboxMin) / 2),
                    new Vector3(0, 0, bboxMin + (bboxMax - bboxMin) / 2), Vector3.UnitZ, Vector3.UnitY);
            });

	        this.mThumbnailCaptureFrame = CAPTURE_FRAME;
	        this.renderTimer.Start();
        }

	    private void OnRenderTimerTick(object sender, EventArgs args)
        {
            if (WorldFrame.Instance.Dispatcher.InvokeRequired)
            {
	            WorldFrame.Instance.Dispatcher.BeginInvoke(OnRenderModel);
            }
            else
            {
	            OnRenderModel();
            }
        }

	    private unsafe void OnRenderModel()
        {
            if (this.mRenderer == null)
            {
	            return;
            }

	        this.mTarget.Clear();
	        this.mTarget.Apply();

            var ctx = WorldFrame.Instance.GraphicsContext;
            var vp = ctx.Viewport;
            ctx.Context.Rasterizer.SetViewport(new Viewport(0, 0, this.ImgWidth, this.ImgHeight, 0.0f, 1.0f));

            ctx.Context.VertexShader.SetConstantBuffer(0, this.mMatrixBuffer.Native);
	        this.mRenderer.RenderPortrait();

	        this.mTarget.Remove();
            ctx.Context.Rasterizer.SetViewport(vp);

            ctx.Context.ResolveSubresource(this.mTarget.Texture, 0, this.mResolveTexture, 0, Format.B8G8R8A8_UNorm);
            ctx.Context.CopyResource(this.mResolveTexture, this.mMapTexture);

            var box = ctx.Context.MapSubresource(this.mMapTexture, 0, MapMode.Read, MapFlags.None);
            var bmp = new Bitmap(this.ImgWidth, this.ImgHeight, PixelFormat.Format32bppArgb);
            var bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            byte* ptrDst = (byte*)bmpd.Scan0.ToPointer();
            byte* ptrSrc = (byte*)box.DataPointer.ToPointer();

            for (var i = 0; i < bmp.Height; ++i)
            {
                UnsafeNativeMethods.CopyMemory(ptrDst + i * bmp.Width * 4, ptrSrc + i * box.RowPitch, bmp.Width * 4);
            }

            bmp.UnlockBits(bmpd);
            ctx.Context.UnmapSubresource(this.mMapTexture, 0);

            //Cache thumbnail
            if (this.mThumbnailCaptureFrame > 0 && --this.mThumbnailCaptureFrame == 0)
            {
	            this.renderTimer.Stop();

                Bitmap thumbnail = new Bitmap(this.ImgWidth, this.ImgHeight);
                using (var g = System.Drawing.Graphics.FromImage(thumbnail))
                {
                    g.Clear(Color.Black);
                    g.DrawImage(bmp, new PointF(0, 0));
                }

                ThumbnailCache.Cache(this.mRenderer.Model.FileName, thumbnail);
                if (this.mModels.Count > 0) //More models so render next
                {
	                LoadModel();
                }
            }
        }

        public void Dispose()
        {
            ((IDisposable) this.renderTimer).Dispose();
        }
    }
}
