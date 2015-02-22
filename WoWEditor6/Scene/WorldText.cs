using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using WoWEditor6.Graphics;
using WoWEditor6.IO.Files.Texture;

namespace WoWEditor6.Scene
{
    class WorldText : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        struct PerDrawCallBuffer
        {
            public Matrix matTransform;
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
        private static Sampler gSampler;
        private static VertexBuffer gVertexBuffer;

        private static RasterState gRasterState;
        private static DepthState gDepthState;

        private static ShaderProgram gWorldTextShader;
        private static Graphics.BlendState gBlendState;

        private static ConstantBuffer gPerDrawCallBuffer;

        public Vector3 Position { get; set; }
        public float Scaling { get; set; }

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

        private Font mFont;
        private Brush mBrush;

        private static readonly Bitmap Bitmap = new Bitmap(1, 1);
        private static System.Drawing.Graphics gGraphics;

        private Graphics.Texture mTexture;

        private bool mIsDirty = true;
        private bool mShouldDraw = false;
        private bool mIsInitialized = false;

        public WorldText(Font font, Brush brush)
        {
            mBrush = brush;
            mFont = font;
        }

        public void Dispose()
        {
            if (mTexture != null)
                mTexture.Dispose();
        }

        public static void BeginDraw()
        {
            gMesh.BeginDraw();
            gMesh.Program.SetPixelSampler(0, gSampler);
            gMesh.Program.SetVertexConstantBuffer(1, gPerDrawCallBuffer);
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

            var center = Position;
            var scale = Scaling * 0.01f;
            var right = camera.Right * mWidth * 0.5f;
            var up = camera.Up * mHeight * 0.5f;

            if (camera.LeftHanded)
                right = -right;

            gVertexBuffer.UpdateData(new[]
            {
                new WorldTextVertex(center - (right + up) * scale, 0.0f, 1.0f),
                new WorldTextVertex(center + (right - up) * scale, 1.0f, 1.0f),
                new WorldTextVertex(center - (right - up) * scale, 0.0f, 0.0f),
                new WorldTextVertex(center + (right + up) * scale, 1.0f, 0.0f) 
            });

            gMesh.IndexCount = 4;
            gMesh.StartVertex = 0;
            gMesh.StartIndex = 0;
            gMesh.Topology = SharpDX.Direct3D.PrimitiveTopology.TriangleStrip;

            gMesh.Program.SetPixelTexture(0, mTexture);
            gMesh.DrawNonIndexed();
        }

        private void UpdateText(string text)
        {
            if (mText != null && mText.Equals(text))
                return;

            mText = text;
            mIsDirty = true;
        }

        private void UpdateFont(Font font)
        {
            if (mFont != null && mFont.Equals(font))
                return;

            mFont = font;
            mIsDirty = true;
        }

        private void UpdateBrush(Brush brush)
        {
            if (mBrush != null && mBrush.Equals(brush))
                return;

            mBrush = brush;
            mIsDirty = true;
        }

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
            using (var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
                {
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                    graphics.DrawString(mText, mFont, mBrush, 0, 0);
                }

                var rect = new System.Drawing.Rectangle(0, 0, width, height);
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
            var ctx = WorldFrame.Instance.GraphicsContext;
            mTexture = new Graphics.Texture(ctx);
        }

        public static void Initialize(GxContext context)
        {
            gGraphics = System.Drawing.Graphics.FromImage(Bitmap);

            gSampler = new Sampler(context)
            {
                AddressMode = TextureAddressMode.Wrap,
                Filter = Filter.MinMagMipLinear
            };

            gBlendState = new Graphics.BlendState(context)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                SourceAlphaBlend = BlendOption.SourceAlpha,
                DestinationAlphaBlend = BlendOption.InverseSourceAlpha
            };

            gRasterState = new RasterState(context)
            {
                CullEnabled = false
            };

            gDepthState = new DepthState(context)
            {
                DepthEnabled = true,
                DepthWriteEnabled = false
            };

            gVertexBuffer = new VertexBuffer(context);

            gMesh = new Mesh(context)
            {
                Stride = IO.SizeCache<WorldTextVertex>.Size
            };

            gMesh.DepthState.Dispose();
            gMesh.BlendState.Dispose();
            gMesh.IndexBuffer.Dispose();
            gMesh.VertexBuffer.Dispose();

            gMesh.AddElement("POSITION", 0, 3);
            gMesh.AddElement("TEXCOORD", 0, 2);

            gMesh.DepthState = gDepthState;
            gMesh.RasterizerState = gRasterState;
            gMesh.BlendState = gBlendState;
            gMesh.VertexBuffer = gVertexBuffer;

            gWorldTextShader = new ShaderProgram(context);
            gWorldTextShader.SetVertexShader(Resources.Shaders.WorldTextVertex);
            gWorldTextShader.SetPixelShader(Resources.Shaders.WorldTextPixel);
            gMesh.Program = gWorldTextShader;

            gPerDrawCallBuffer = new ConstantBuffer(context);
            gPerDrawCallBuffer.UpdateData(new PerDrawCallBuffer
            {
                matTransform = Matrix.Identity
            });
        }
    }
}
