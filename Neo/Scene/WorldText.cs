using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using Neo.Graphics;
using Neo.IO.Files.Texture;
using Neo.Resources;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Rectangle = System.Drawing.Rectangle;

namespace Neo.Scene
{
    class WorldText : IDisposable
    {
        public enum TextDrawMode
        {
            TextDraw2D,
            TextDraw2D_World,
            TextDraw3D,
            TextDraw3D_NoDepthTest
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PerDrawCallBuffer
        {
            public Matrix4 matTransform;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct WorldTextVertex
        {
            public WorldTextVertex(Vector3 pos, float u, float v)
            {
                position = pos;
                texCoord.X = u;
                texCoord.Y = v;
            }

            public Vector3 position;
            public Vector2 texCoord;
        }

        private static Mesh gMesh;
        private static VertexBuffer gVertexBuffer;

        private static RasterState gRasterState;
        private static DepthState gDepthState;
        private static DepthState gNoDepthState;

        private static ShaderProgram gWorldTextShader;
        private static ShaderProgram gWorldTextShader2D;
        private static BlendState gBlendState;

        private static UniformBuffer gPerDrawCallBuffer;

        public Vector3 Position { get; set; }
        public float Scaling { get; set; }
        public TextDrawMode DrawMode { get; set; }

        public Font Font
        {
            get { return mFont; }
            set { UpdateFont(value); }
        }

        public Brush Brush
        {
            get { return mBrush; }
            set { UpdateBrush(value); }
        }

        public string Text
        {
            get { return mText; }
            set { UpdateText(value); }
        }

        private string mText;
        private float mWidth;
        private float mHeight;

        private Font mFont = UI.FontCollection.GetFont("Friz Quadrata TT", 30);
        private Brush mBrush = Brushes.AntiqueWhite;

        private static readonly Bitmap Bitmap = new Bitmap(1, 1);
        private static System.Drawing.Graphics gGraphics;

        private Graphics.Texture mTexture;
	    private Graphics.Texture mTexture2D;

        private bool mIsDirty = true;
        private bool mShouldDraw = false;
        private bool mIsInitialized = false;

        public WorldText(Font font = null, Brush brush = null)
        {
            if (font != null)
                mFont = font;

            if (brush != null)
                mBrush = brush;

            Scaling = 1.0f;
            DrawMode = TextDrawMode.TextDraw3D;
        }

        ~WorldText()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (mTexture != null)
            {
                mTexture.Dispose();
                mTexture = null;
            }

            mFont = null;
            mBrush = null;
            mText = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static void BeginDraw()
        {
            gMesh.BeginDraw();
            gMesh.Program.SetVertexUniformBuffer(1, gPerDrawCallBuffer);
        }

        public void OnFrame(Camera camera)
        {
            if (!mIsInitialized)
            {
                OnSyncLoad();
                mIsInitialized = true;
            }

            if (mIsDirty)
            {
                OnRenderText();
                mIsDirty = false;
            }

            if (!mShouldDraw)
                return;

            switch (DrawMode)
            {
                case TextDrawMode.TextDraw2D:
                    DrawText2D(camera, false);
                    break;

                case TextDrawMode.TextDraw2D_World:
                    DrawText2D(camera, true);
                    break;

                case TextDrawMode.TextDraw3D:
                    DrawText3D(camera, false);
                    break;

                case TextDrawMode.TextDraw3D_NoDepthTest:
                    DrawText3D(camera, true);
                    break;
            }
        }

        private static Vector3 WorldToScreenCoords(Camera camera, Vector3 world)
        {
            var viewport = WorldFrame.Instance.GraphicsContext.Viewport;
            var screenCoords = Vector3.TransformVector(world, camera.ViewProjection);

            screenCoords.X = viewport.X + (1.0f + screenCoords.X) * viewport.Width / 2.0f;
            screenCoords.Y = viewport.Y + (1.0f - screenCoords.Y) * viewport.Height / 2.0f;
            screenCoords.Z = viewport.MinDepth + screenCoords.Z * (viewport.MaxDepth - viewport.MinDepth);
            return screenCoords;
        }

        private void DrawText2D(Camera camera, bool world)
        {
            var center = Position;
            if (world)
            {
                center = WorldToScreenCoords(camera, center);
                if (center.Z >= 1.0f)
                    return;
            }

            var scale = Scaling;
            var right = new Vector3(mWidth, 0, 0) * 0.5f;
            var up = new Vector3(0, mHeight, 0) * 0.5f;

            gVertexBuffer.BufferData(new[]
            {
                new WorldTextVertex(center - (right + up) * scale, 0.0f, 0.0f),
                new WorldTextVertex(center + (right - up) * scale, 1.0f, 0.0f),
                new WorldTextVertex(center - (right - up) * scale, 0.0f, 1.0f),
                new WorldTextVertex(center + (right + up) * scale, 1.0f, 1.0f)
            });

            gMesh.UpdateDepthState(gNoDepthState);

            gMesh.IndexCount = 4;
            gMesh.StartVertex = 0;
            gMesh.StartIndex = 0;
            gMesh.Topology = BeginMode.TriangleStrip;

            if (gMesh.Program != gWorldTextShader2D)
            {
                gMesh.Program = gWorldTextShader2D;
                gMesh.Program.Bind();
            }

            gMesh.Program.SetFragmentTexture(0, mTexture);
            gMesh.DrawNonIndexed();
        }

        private void DrawText3D(Camera camera, bool noDepth)
        {
            var center = Position;
            var scale = Scaling / 10.0f;
            var right = camera.Right * mWidth * 0.5f;
            var up = camera.Up * mHeight * 0.5f;

            if (camera.LeftHanded)
                right = -right;

            gVertexBuffer.BufferData(new[]
            {
                new WorldTextVertex(center - (right + up) * scale, 0.0f, 0.0f),
                new WorldTextVertex(center + (right - up) * scale, 1.0f, 0.0f),
                new WorldTextVertex(center - (right - up) * scale, 0.0f, 1.0f),
                new WorldTextVertex(center + (right + up) * scale, 1.0f, 1.0f)
            });

            gMesh.UpdateDepthState(noDepth ? gNoDepthState : gDepthState);

            gMesh.IndexCount = 4;
            gMesh.StartVertex = 0;
            gMesh.StartIndex = 0;
            gMesh.Topology = BeginMode.TriangleStrip;

            if (gMesh.Program != gWorldTextShader)
            {
                gMesh.Program = gWorldTextShader;
                gMesh.Program.Bind();
            }

            gMesh.Program.SetFragmentTexture(0, mTexture);
            gMesh.DrawNonIndexed();
        }

        private void UpdateText(string text)
        {
            if (text.Equals(mText))
                return;

            mText = text;
            mIsDirty = true;
        }

        private void UpdateFont(Font font)
        {
            if (font.Equals(mFont))
                return;

            mFont = font;
            mIsDirty = true;
        }

        private void UpdateBrush(Brush brush)
        {
            if (brush.Equals(mBrush))
                return;

            mBrush = brush;
            mIsDirty = true;
        }

	    // INVESTIGATE: Is this creating an internal BLP from a bitmap?
        private unsafe void OnRenderText()
        {
            var size = gGraphics.MeasureString(mText, mFont);
            var width = (int) (size.Width + 0.5f);
            var height = (int) (size.Height + 0.5f);

            mWidth = size.Width;
            mHeight = size.Height;

            if (width == 0 || height == 0)
            {
                mShouldDraw = false;
                return;
            }

            byte[] buffer;
            using (var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
                {
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                    graphics.DrawString(mText, mFont, mBrush, 0, 0);
                }

                var rect = new Rectangle(0, 0, width, height);
                var bits = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

                width = bits.Width;
                height = bits.Height;

                var destPitch = width * 4;
                var scanline = (byte*) bits.Scan0;
                buffer = new byte[destPitch * height];

                for (var y = 0; y < height; ++y)
                {
                    var scanline32 = (uint*) scanline;
                    for (var x = 0; x < width; ++x)
                    {
                        var color = *scanline32++;
                        var a = (color >> 24) & 0xff;
                        var r = (color >> 16) & 0xff;
                        var g = (color >> 8) & 0xff;
                        var b = (color >> 0) & 0xff;

                        var offset = destPitch * y + x * 4;
                        buffer[offset + 0] = (byte) r;
                        buffer[offset + 1] = (byte) g;
                        buffer[offset + 2] = (byte) b;
                        buffer[offset + 3] = (byte) a;
                    }

                    scanline += bits.Stride;
                }

                bitmap.UnlockBits(bits);
            }

            var texLoadInfo = new TextureLoadInfo
            {
                Format = Format.R8G8B8A8_UNorm,
                Usage = ResourceUsage.Immutable,
                Width = width,
                Height = height,
                GenerateMipMaps = true
            };

            texLoadInfo.Layers.Add(buffer);
            texLoadInfo.RowPitchs.Add(width * 4);
            mTexture.LoadFromLoadInfo(texLoadInfo);
            mShouldDraw = true;
        }

        private void OnSyncLoad()
        {
            mTexture = new Graphics.Texture();
        }

        public static void Initialize()
        {
            gGraphics = System.Drawing.Graphics.FromImage(Bitmap);

            gTexture = new Sampler()
            {
                AddressU = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                AddressV = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                AddressW = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                Filter = Filter.MinMagMipLinear
            };

            gTexture2D = new Sampler()
            {
                AddressU = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                AddressV = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                AddressW = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                Filter = Filter.MinMagMipPoint
            };

            gBlendState = new BlendState
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                SourceAlphaBlend = BlendOption.SourceAlpha,
                DestinationAlphaBlend = BlendOption.InverseSourceAlpha
            };

            gRasterState = new RasterState
            {
                BackfaceCullingEnabled = false
            };

            gDepthState = new DepthState
            {
                DepthEnabled = true,
                DepthWriteEnabled = false
            };

            gNoDepthState = new DepthState
            {
                DepthEnabled = false,
                DepthWriteEnabled = false
            };

            gVertexBuffer = new VertexBuffer();

            gMesh = new Mesh
            {
                Stride = IO.SizeCache<WorldTextVertex>.Size
            };

            gMesh.IndexBuffer.Dispose();
            gMesh.VertexBuffer.Dispose();

            gMesh.RasterizerState = gRasterState;
            gMesh.BlendState = gBlendState;
            gMesh.VertexBuffer = gVertexBuffer;
            gMesh.DepthState = gDepthState;

            gMesh.AddElement("POSITION", 0, 3);
            gMesh.AddElement("TEXCOORD", 0, 2);

            gWorldTextShader = new ShaderProgram();
            gWorldTextShader.SetVertexShader(Shaders.WorldTextVertex);
            gWorldTextShader.SetFragmentShader(Shaders.WorldTextFragment);

            gWorldTextShader2D = new ShaderProgram();
            gWorldTextShader2D.SetVertexShader(Shaders.WorldTextVertexOrtho);
            gWorldTextShader2D.SetFragmentShader(Shaders.WorldTextFragment);
            gMesh.Program = gWorldTextShader;
            gMesh.InitLayout(gWorldTextShader);

            gPerDrawCallBuffer = new UniformBuffer();
            gPerDrawCallBuffer.BufferData(new PerDrawCallBuffer
            {
                matTransform = Matrix4.Identity
            });
        }
    }
}
