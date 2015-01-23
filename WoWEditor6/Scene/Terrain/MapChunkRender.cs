using System;
using System.Linq;
using SharpDX;
using WoWEditor6.Graphics;

namespace WoWEditor6.Scene.Terrain
{
    class MapChunkRender : IDisposable
    {
        public static Sampler ColorSampler { get; private set; }
        public static Sampler AlphaSampler { get; private set; }
        public static ShaderProgram BlendNew { get; private set; }
        public static ShaderProgram BlendOld { get; private set; }

        private IO.Files.Terrain.MapChunk mData;
        private bool mAsyncLoaded;
        private bool mSyncLoaded;
        private bool mSyncLoadRequested;
        private Graphics.Texture mAlphaTexture;
        private BoundingBox mBoundingBox;
        private ConstantBuffer mScaleBuffer;
        private Graphics.Texture[] mShaderTextures;

        public static Mesh ChunkMesh { get; private set; }

        public void Dispose()
        {
            mShaderTextures = null;
            var alphaTex = mAlphaTexture;
            var constBuffer = mScaleBuffer;
            WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
            {
                constBuffer?.Dispose();
                alphaTex?.Dispose();
            });
        }

        public void OnFrame()
        {
            if (mAsyncLoaded == false)
                return;

            if(WorldFrame.Instance.MapManager.IsInitialLoad == false)
            {
                if (WorldFrame.Instance.ActiveCamera.Contains(ref mBoundingBox) == false)
                    return;
            }

            if(mSyncLoaded == false)
            {
                if (mSyncLoadRequested)
                    return;

                if (WorldFrame.Instance.MapManager.IsInitialLoad)
                {
                    SyncLoad();
                    WorldFrame.Instance.MapManager.OnLoadProgress();
                }
                else
                {
                    WorldFrame.Instance.Dispatcher.BeginInvoke(SyncLoad);
                    mSyncLoadRequested = true;
                    return;
                }
            }

            ChunkMesh.StartVertex = mData.StartVertex;
            ChunkMesh.Program.SetPixelTexture(0, mAlphaTexture);
            ChunkMesh.Program.SetPixelTextures(1, mShaderTextures);

            ChunkMesh.Program.SetPixelConstantBuffer(2, mScaleBuffer);

            ChunkMesh.Draw();
        }

        public void OnAsyncLoad(IO.Files.Terrain.MapChunk chunk)
        {
            mData = chunk;
            mBoundingBox = chunk.BoundingBox;
            mAsyncLoaded = true;
        }

        private void SyncLoad()
        {
            mAlphaTexture = new Graphics.Texture(WorldFrame.Instance.GraphicsContext);
            mAlphaTexture.UpdateMemory(64, 64, SharpDX.DXGI.Format.R8G8B8A8_UNorm, mData.AlphaValues, 4 * 64);
            mScaleBuffer = new ConstantBuffer(WorldFrame.Instance.GraphicsContext);
            mScaleBuffer.UpdateData(mData.TextureScales);
            mShaderTextures = mData.Textures.ToArray();
            mSyncLoaded = true;
        }

        public static void Initialize(GxContext context)
        {
            ChunkMesh = new Mesh(context);
            InitIndices();
            InitMesh(context);
        }

        private static void InitMesh(GxContext context)
        {
            // all tiles will supply their own vertex buffer
            ChunkMesh.VertexBuffer.Dispose();

            ChunkMesh.AddElement("POSITION", 0, 3);
            ChunkMesh.AddElement("NORMAL", 0, 3);
            ChunkMesh.AddElement("TEXCOORD", 0, 2);
            ChunkMesh.AddElement("TEXCOORD", 1, 2);
            ChunkMesh.AddElement("COLOR", 0, 4, DataType.Byte, true);

            ChunkMesh.IndexCount = 768;
            ChunkMesh.Stride = IO.SizeCache<IO.Files.Terrain.AdtVertex>.Size;
            ChunkMesh.BlendState.BlendEnabled = false;
            ChunkMesh.DepthState.DepthEnabled = true;
            ChunkMesh.RasterizerState.CullEnabled = true;

            BlendNew = new ShaderProgram(context);
            BlendNew.SetVertexShader(Resources.Shaders.TerrainVertex, "main");
            BlendNew.SetPixelShader(Resources.Shaders.TerrainPixelNew, "main");

            ChunkMesh.Program = BlendNew;

            BlendOld = new ShaderProgram(context);
            BlendOld.SetVertexShader(Resources.Shaders.TerrainVertex, "main");
            BlendOld.SetPixelShader(Resources.Shaders.TerrainPixel, "main");

            ColorSampler = new Sampler(context)
            {
                AddressMode = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                Filter = SharpDX.Direct3D11.Filter.MinMagMipLinear
            };
            AlphaSampler = new Sampler(context) {AddressMode = SharpDX.Direct3D11.TextureAddressMode.Clamp};
        }

        private static void InitIndices()
        {
            var indices = new uint[768];
            for (uint y = 0; y < 8; ++y)
            {
                for (uint x = 0; x < 8; ++x)
                {
                    var i = y * 8 * 12 + x * 12;
                    indices[i + 0] = y * 17 + x;
                    indices[i + 2] = y * 17 + x + 1;
                    indices[i + 1] = y * 17 + x + 9;

                    indices[i + 3] = y * 17 + x + 1;
                    indices[i + 5] = y * 17 + x + 18;
                    indices[i + 4] = y * 17 + x + 9;

                    indices[i + 6] = y * 17 + x + 18;
                    indices[i + 8] = y * 17 + x + 17;
                    indices[i + 7] = y * 17 + x + 9;

                    indices[i + 9] = y * 17 + x + 17;
                    indices[i + 11] = y * 17 + x;
                    indices[i + 10] = y * 17 + x + 9;
                }
            }

            ChunkMesh.IndexBuffer.UpdateData(indices);
            ChunkMesh.IndexBuffer.IndexFormat = SharpDX.DXGI.Format.R32_UInt;
        }
    }
}
