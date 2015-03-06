using System;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.DXGI;
using WoWEditor6.Graphics;
using WoWEditor6.IO.Files.Models;
using WoWEditor6.Scene.Models;

namespace WoWEditor6.Scene.Terrain
{
    [StructLayout(LayoutKind.Sequential)]
    struct TexAnimBuffer
    {
        public Matrix Layer0;
        public Matrix Layer1;
        public Matrix Layer2;
        public Matrix Layer3;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct TexParamsBuffer
    {
        public Vector4 TextureScales;
        public Vector4 SpecularFactors;
    }

    class MapChunkRender : IDisposable
    {
        public static Sampler ColorSampler { get; private set; }
        public static Sampler AlphaSampler { get; private set; }
        public static ShaderProgram BlendNew { get; private set; }
        public static ShaderProgram BlendOld { get; private set; }

        private IO.Files.Terrain.MapChunk mData;

        private bool mIsAsyncLoaded;
        private bool mIsSyncLoaded;
        private object mSyncLoadToken;

        private Graphics.Texture mAlphaTexture;
        private Graphics.Texture mHoleTexture;

        private BoundingBox mBoundingBox;
        private BoundingBox mModelBox;

        private TexAnimBuffer mTexAnimStore;
        private readonly Vector2[] mTexAnimDirections = new Vector2[4];
        private bool mHasTexAnim;

        private TexParamsBuffer mTexParams;

        private ConstantBuffer mTexAnimBuffer;
        private ConstantBuffer mScaleBuffer;
        private Graphics.Texture[] mShaderTextures;
        private Graphics.Texture[] mShaderSpecularTextures;
        private M2Instance[] mReferences;
        private WeakReference<MapAreaRender> mParent;

        public static Mesh ChunkMesh { get; private set; }

        public void UpdateBoundingBox()
        {
            mBoundingBox = mData.BoundingBox;
        }

        ~MapChunkRender()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            var alphaTex = mAlphaTexture;
            var holeTex = mHoleTexture;
            var constBuffer = mScaleBuffer;
            var tanim = mTexAnimBuffer;

            WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
            {
                if (holeTex != null)
                    holeTex.Dispose();
                if (alphaTex != null)
                    alphaTex.Dispose();
                if (constBuffer != null)
                    constBuffer.Dispose();
                if (tanim != null)
                    tanim.Dispose();
            });

            lock (this)
            {
                // Sync load can be called even after the object has been disposed.
                if (mSyncLoadToken != null)
                {
                    WorldFrame.Instance.Dispatcher.Remove(mSyncLoadToken);
                    mSyncLoadToken = null;
                }
            }

            mAlphaTexture = null;
            mHoleTexture = null;
            mScaleBuffer = null;
            mShaderTextures = null;
            mReferences = null;
            mParent = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void PushDoodadReferences()
        {
            if (mData.DoodadsChanged)
            {
                MapAreaRender parent;
                mParent.TryGetTarget(out parent);
                if (parent != null)
                {
                    mReferences = new M2Instance[mData.DoodadReferences.Length];
                    for (var i = 0; i < mReferences.Length; ++i)
                        mReferences[i] = parent.AreaFile.DoodadInstances[mData.DoodadReferences[i]];
                }

                mData.DoodadsChanged = false;
            }

            if (mReferences.Length == 0)
                return;

            if (WorldFrame.Instance.ActiveCamera.Contains(ref mModelBox))
                WorldFrame.Instance.M2Manager.PushMapReferences(mReferences);
        }

        public void OnFrame()
        {
            if (mIsAsyncLoaded == false)
                return;

            if(WorldFrame.Instance.MapManager.IsInitialLoad == false)
            {
                if (WorldFrame.Instance.ActiveCamera.Contains(ref mBoundingBox) == false)
                {
                    if (M2Manager.IsViewDirty == false)
                        return;

                    PushDoodadReferences();
                    return;
                }
            }

            if (mIsSyncLoaded == false)
            {
                if (!BeginSyncLoad())
                    return;
            }

            if (M2Manager.IsViewDirty)
                PushDoodadReferences();

            if (mData.IsAlphaChanged)
            {
                mAlphaTexture.UpdateMemory(64, 64, Format.R8G8B8A8_UNorm, mData.AlphaValues, 4 * 64);
                mData.IsAlphaChanged = false;
            }

            if (mData.TexturesChanged)
            {
                mShaderTextures = mData.Textures.ToArray();
                mShaderSpecularTextures = mData.SpecularTextures.ToArray();
                mData.TexturesChanged = false;
            }

            UpdateTextureAnimations();

            ChunkMesh.StartVertex = mData.StartVertex;
            ChunkMesh.Program.SetPixelTexture(0, mAlphaTexture);
            ChunkMesh.Program.SetPixelTexture(1, mHoleTexture);
            ChunkMesh.Program.SetPixelTextures(2, mShaderTextures);
            ChunkMesh.Program.SetPixelTextures(6, mShaderSpecularTextures);
            ChunkMesh.Program.SetVertexConstantBuffer(1, mTexAnimBuffer);

            ChunkMesh.Program.SetPixelConstantBuffer(2, mScaleBuffer);

            ChunkMesh.Draw();
        }

        public void OnAsyncLoad(IO.Files.Terrain.MapChunk chunk, MapAreaRender parent)
        {
            mData = chunk;
            mBoundingBox = chunk.BoundingBox;
            mModelBox = chunk.ModelBox;
            mReferences = new M2Instance[chunk.DoodadReferences.Length];
            for (var i = 0; i < mReferences.Length; ++i)
                mReferences[i] = parent.AreaFile.DoodadInstances[chunk.DoodadReferences[i]];

            for (var i = 0; i < mData.Layers.Length; ++i)
            {
                if ((mData.Layers[i].Flags & 0x40) != 0)
                {
                    mHasTexAnim = true;

                    var rotation = 0.0f;
                    if ((mData.Layers[i].Flags & 1) != 0)
                        rotation += (float)Math.PI / 4.0f;
                    if ((mData.Layers[i].Flags & 2) != 0)
                        rotation += (float)Math.PI / 2.0f;
                   if((mData.Layers[i].Flags & 4) != 0)
                        rotation += (float)Math.PI;

                    var matrix = Matrix.RotationZ(rotation);
                    var dir = Vector2.TransformCoordinate(new Vector2(0, 1), matrix);
                    mTexAnimDirections[i] = dir;

                    mTexAnimDirections[i].Normalize();
                    if ((mData.Layers[i].Flags & 8) != 0)
                        mTexAnimDirections[i] *= 1.2f;
                    else if ((mData.Layers[i].Flags & 0x10) != 0)
                        mTexAnimDirections[i] *= 1.44f;
                    else if ((mData.Layers[i].Flags & 0x20) != 0)
                        mTexAnimDirections[i] *= 1.728f;
                }
            }

            mIsAsyncLoaded = true;
            mParent = new WeakReference<MapAreaRender>(parent);
        }

        private void UpdateTextureAnimations()
        {
            if (mHasTexAnim == false)
                return;

            var curTime = (int)(Utils.TimeManager.Instance.GetTime().TotalMilliseconds / 15.0f);

            mTexAnimStore.Layer0 = Matrix.Translation(mTexAnimDirections[0].X * curTime / 1000.0f,
                mTexAnimDirections[0].Y * curTime / 1000.0f, 0.0f);
            mTexAnimStore.Layer1 = Matrix.Translation(mTexAnimDirections[1].X * curTime / 1000.0f,
                mTexAnimDirections[1].Y * curTime / 1000.0f, 0.0f);
            mTexAnimStore.Layer2 = Matrix.Translation(mTexAnimDirections[2].X * curTime / 1000.0f,
                mTexAnimDirections[2].Y * curTime / 1000.0f, 0.0f);
            mTexAnimStore.Layer3 = Matrix.Translation(mTexAnimDirections[3].X * curTime / 1000.0f,
                mTexAnimDirections[3].Y * curTime / 1000.0f, 0.0f);

            mTexAnimBuffer.UpdateData(mTexAnimStore);
        }

        private bool BeginSyncLoad()
        {
            if (mSyncLoadToken != null)
                return false;

            if (WorldFrame.Instance.MapManager.IsInitialLoad)
            {
                SyncLoad();
                WorldFrame.Instance.MapManager.OnLoadProgress();
                return true;
            }

            lock (this)
                mSyncLoadToken = WorldFrame.Instance.Dispatcher.BeginInvoke(SyncLoad);
            return false;
        }

        private void SyncLoad()
        {
            mSyncLoadToken = null;

            mTexParams.TextureScales = new Vector4(mData.TextureScales);
            mTexParams.SpecularFactors = new Vector4(mData.SpecularFactors);

            mTexAnimBuffer = new ConstantBuffer(WorldFrame.Instance.GraphicsContext);
            mTexAnimStore.Layer0 = mTexAnimStore.Layer1 = mTexAnimStore.Layer2 = mTexAnimStore.Layer3 = Matrix.Identity;
            mTexAnimBuffer.UpdateData(mTexAnimStore);
            mAlphaTexture = new Graphics.Texture(WorldFrame.Instance.GraphicsContext);
            mAlphaTexture.UpdateMemory(64, 64, Format.R8G8B8A8_UNorm, mData.AlphaValues, 4 * 64);
            mHoleTexture = new Graphics.Texture(WorldFrame.Instance.GraphicsContext);
            mHoleTexture.UpdateMemory(8, 8, Format.R8_UNorm, mData.HoleValues, 8);
            mScaleBuffer = new ConstantBuffer(WorldFrame.Instance.GraphicsContext);
            mScaleBuffer.UpdateData(mTexParams);
            mShaderTextures = mData.Textures.ToArray();
            mShaderSpecularTextures = mData.SpecularTextures.ToArray();

            mIsSyncLoaded = true;
        }

        public static void Initialize(GxContext context)
        {
            ChunkMesh = new Mesh(context);
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
            ChunkMesh.AddElement("COLOR", 1, 4, DataType.Byte, true);

            ChunkMesh.IndexCount = 768;
            ChunkMesh.Stride = IO.SizeCache<IO.Files.Terrain.AdtVertex>.Size;
            ChunkMesh.BlendState.BlendEnabled = false;
            ChunkMesh.DepthState.DepthEnabled = true;
            ChunkMesh.RasterizerState.CullEnabled = true;

            BlendNew = new ShaderProgram(context);
            BlendNew.SetVertexShader(Resources.Shaders.TerrainVertex);
            BlendNew.SetPixelShader(Resources.Shaders.TerrainPixelNew);

            ChunkMesh.Program = BlendNew;
            ChunkMesh.InitLayout(BlendNew);

            BlendOld = new ShaderProgram(context);
            BlendOld.SetVertexShader(Resources.Shaders.TerrainVertex);
            BlendOld.SetPixelShader(Resources.Shaders.TerrainPixel);

            ColorSampler = new Sampler(context)
            {
                AddressU = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                AddressV = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                AddressW = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                Filter = SharpDX.Direct3D11.Filter.Anisotropic,
                MaximumAnisotropy = 16,
            };
            AlphaSampler = new Sampler(context)
            {
                AddressU = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                AddressV = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                AddressW = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                Filter = SharpDX.Direct3D11.Filter.Anisotropic,
                MaximumAnisotropy = 16,
            };
        }

        public static void InitIndices()
        {
            InitIndicesDefault();
        }

        private static void InitIndicesDefault()
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
            ChunkMesh.IndexBuffer.IndexFormat = Format.R32_UInt;
        }
    }
}
