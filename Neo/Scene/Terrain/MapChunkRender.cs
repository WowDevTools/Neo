using System;
using System.Linq;
using System.Runtime.InteropServices;
using Neo.Editing;
using Neo.Graphics;
using Neo.IO.Files.Models;
using Neo.Scene.Models;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SlimTK;
using DataType = Neo.Graphics.DataType;

namespace Neo.Scene.Terrain
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TexAnimBuffer
    {
        public Matrix4 Layer0;
        public Matrix4 Layer1;
        public Matrix4 Layer2;
        public Matrix4 Layer3;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TexParamsBuffer
    {
        public Vector4 TextureScales;
        public Vector4 SpecularFactors;
        public Vector4 AreaColour;
        public Vector4 ChunkLine;
    }

    [Flags]
    public enum ChunkRenderFlags
    {
        ShowArea = 0x1,
        ShowLines = 0x2,
        HideArea = 0x4,
        HideLines = 0x8
    }

	internal class MapChunkRender : IDisposable
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

        private UniformBuffer mTexAnimBuffer;
        private UniformBuffer mScaleBuffer;
        private Graphics.Texture[] mShaderTextures;
        private Graphics.Texture[] mShaderSpecularTextures;
        private M2Instance[] mReferences;
        private WeakReference<MapAreaRender> mParent;

        public static Mesh ChunkMesh { get; private set; }

        public static bool WireFrame
        {
            get { return ChunkMesh.RasterizerState.Wireframe; }
            set { ChunkMesh.RasterizerState.Wireframe = value; }
        }

        public void UpdateBoundingBox()
        {
	        this.mBoundingBox = this.mData.BoundingBox;
        }

        public MapChunkRender()
        {
            ChunkEditManager.Instance.OnChunkRenderModeChange += SetRenderMode;
            ChunkEditManager.Instance.ForceRenderUpdate += ForceRenderMode;
        }

        private void ForceRenderMode(IO.Files.Terrain.MapChunk chunk, bool updateHoles)
        {
            if(chunk == this.mData)
            {
	            if (updateHoles)
	            {
		            this.mHoleTexture.UpdateMemory(8, 8, Format.R8_UNorm, this.mData.HoleValues, 8);
	            }
                else
                {
	                this.mTexParams.AreaColour = ChunkEditManager.Instance.GetAreaColour(this.mData.AreaId, chunk.HasImpassFlag);
                    SetRenderMode(ChunkEditManager.Instance.ChunkRenderMode);
	                this.mScaleBuffer.BufferData(this.mTexParams);
                }
            }
        }

        private void SetRenderMode(ChunkRenderFlags flags)
        {
            if (this.mScaleBuffer == null)
            {
	            return;
            }

	        if (flags.HasFlag(ChunkRenderFlags.ShowLines) || flags.HasFlag(ChunkRenderFlags.HideLines))
            {
	            this.mTexParams.ChunkLine.W = (flags.HasFlag(ChunkRenderFlags.HideLines) ? 0f : 1f);
	            this.mScaleBuffer.BufferData(this.mTexParams);
            }

            if (flags.HasFlag(ChunkRenderFlags.ShowArea) || flags.HasFlag(ChunkRenderFlags.HideArea))
            {
	            this.mTexParams.AreaColour.W = (flags.HasFlag(ChunkRenderFlags.HideArea) ? 0f : 1f);
	            this.mScaleBuffer.BufferData(this.mTexParams);
            }
        }

        ~MapChunkRender()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            var alphaTex = this.mAlphaTexture;
            var holeTex = this.mHoleTexture;
            var constBuffer = this.mScaleBuffer;
            var tanim = this.mTexAnimBuffer;

            WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
            {
                if (holeTex != null)
                {
	                holeTex.Dispose();
                }
	            if (alphaTex != null)
	            {
		            alphaTex.Dispose();
	            }
	            if (constBuffer != null)
	            {
		            constBuffer.Dispose();
	            }
	            if (tanim != null)
	            {
		            tanim.Dispose();
	            }
            });

            lock (this)
            {
                // Sync load can be called even after the object has been disposed.
                if (this.mSyncLoadToken != null)
                {
                    WorldFrame.Instance.Dispatcher.Remove(this.mSyncLoadToken);
	                this.mSyncLoadToken = null;
                }
            }

            ChunkEditManager.Instance.OnChunkRenderModeChange -= SetRenderMode;
            ChunkEditManager.Instance.ForceRenderUpdate -= ForceRenderMode;

	        this.mAlphaTexture = null;
	        this.mHoleTexture = null;
	        this.mScaleBuffer = null;
	        this.mShaderTextures = null;
	        this.mReferences = null;
	        this.mParent = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void PushDoodadReferences()
        {
            if (this.mData.DoodadsChanged)
            {
                MapAreaRender parent;
	            this.mParent.TryGetTarget(out parent);
                if (parent != null)
                {
	                this.mReferences = new M2Instance[this.mData.DoodadReferences.Length];
                    for (var i = 0; i < this.mReferences.Length; ++i)
                    {
	                    this.mReferences[i] = parent.AreaFile.DoodadInstances[this.mData.DoodadReferences[i]];
                    }
                }

	            this.mData.DoodadsChanged = false;
	            this.mModelBox = this.mData.ModelBox;
            }

            if (this.mReferences.Length == 0)
            {
	            return;
            }

	        if (WorldFrame.Instance.ActiveCamera.Contains(ref this.mModelBox))
	        {
		        WorldFrame.Instance.M2Manager.PushMapReferences(this.mReferences);
	        }
        }

        public void OnFrame()
        {
            if (this.mIsAsyncLoaded == false)
            {
	            return;
            }

	        if(WorldFrame.Instance.MapManager.IsInitialLoad == false)
            {
                if (WorldFrame.Instance.MapManager.SkySphere.BoundingSphere.Intersects(ref this.mBoundingBox) == false)
                {
                    if (M2Manager.IsViewDirty == false)
                    {
	                    return;
                    }

	                if (WorldFrame.Instance.MapManager.SkySphere.BoundingSphere.Intersects(ref this.mModelBox) == false)
	                {
		                return;
	                }
                }

                if (WorldFrame.Instance.ActiveCamera.Contains(ref this.mBoundingBox) == false)
                {
                    if (M2Manager.IsViewDirty == false)
                    {
	                    return;
                    }

	                PushDoodadReferences();
                    return;
                }
            }

            if (this.mIsSyncLoaded == false)
            {
                if (!BeginSyncLoad())
                {
	                return;
                }
            }

            if (M2Manager.IsViewDirty)
            {
	            PushDoodadReferences();
            }

	        if (this.mData.IsAlphaChanged)
            {
	            this.mAlphaTexture.UpdateMemory(64, 64, Format.R8G8B8A8_UNorm, this.mData.AlphaValues, 4 * 64);
	            this.mData.IsAlphaChanged = false;
            }

            if (this.mData.TexturesChanged)
            {
	            this.mShaderTextures = this.mData.Textures.ToArray();
	            this.mShaderSpecularTextures = this.mData.SpecularTextures.ToArray();
	            this.mData.TexturesChanged = false;
            }

            UpdateTextureAnimations();

            ChunkMesh.StartVertex = this.mData.StartVertex;
            ChunkMesh.Program.SetFragmentTexture(0, this.mAlphaTexture);
            ChunkMesh.Program.SetFragmentTexture(1, this.mHoleTexture);
            ChunkMesh.Program.SetFragmentTextures(2, this.mShaderTextures);
            ChunkMesh.Program.SetFragmentTextures(6, this.mShaderSpecularTextures);
            ChunkMesh.Program.SetVertexUniformBuffer(1, this.mTexAnimBuffer);

            ChunkMesh.Program.SetFragmentUniformBuffer(2, this.mScaleBuffer);

            ChunkMesh.Draw();
        }

        public void OnAsyncLoad(IO.Files.Terrain.MapChunk chunk, MapAreaRender parent)
        {
	        this.mData = chunk;
	        this.mBoundingBox = chunk.BoundingBox;
	        this.mModelBox = chunk.ModelBox;
	        this.mReferences = new M2Instance[chunk.DoodadReferences.Length];
	        for (var i = 0; i < this.mReferences.Length; ++i)
	        {
		        this.mReferences[i] = parent.AreaFile.DoodadInstances[chunk.DoodadReferences[i]];
	        }

            for (var i = 0; i < this.mData.Layers.Length; ++i)
            {
                if ((this.mData.Layers[i].Flags & 0x40) != 0)
                {
	                this.mHasTexAnim = true;

                    var rotation = 0.0f;
                    if ((this.mData.Layers[i].Flags & 1) != 0)
                    {
	                    rotation += (float)Math.PI / 4.0f;
                    }
	                if ((this.mData.Layers[i].Flags & 2) != 0)
	                {
		                rotation += (float)Math.PI / 2.0f;
	                }
	                if((this.mData.Layers[i].Flags & 4) != 0)
	                {
		                rotation += (float)Math.PI;
	                }

	                var quat = Quaternion.FromAxisAngle(Vector3.UnitZ, rotation);
                    var dir = Vector2.Transform(new Vector2(0, 1), quat);
	                this.mTexAnimDirections[i] = dir;

	                this.mTexAnimDirections[i].Normalize();
	                if ((this.mData.Layers[i].Flags & 8) != 0)
	                {
		                this.mTexAnimDirections[i] *= 1.2f;
	                }
                    else if ((this.mData.Layers[i].Flags & 0x10) != 0)
                    {
	                    this.mTexAnimDirections[i] *= 1.44f;
                    }
                    else if ((this.mData.Layers[i].Flags & 0x20) != 0)
                    {
	                    this.mTexAnimDirections[i] *= 1.728f;
                    }
                }
            }

	        this.mIsAsyncLoaded = true;
	        this.mParent = new WeakReference<MapAreaRender>(parent);
        }

        private void UpdateTextureAnimations()
        {
	        if (this.mHasTexAnim == false)
	        {
		        return;
	        }

            var curTime = (int)(Utils.TimeManager.Instance.GetTime().TotalMilliseconds / 15.0f);

	        this.mTexAnimStore.Layer0 = Matrix4.CreateTranslation(this.mTexAnimDirections[0].X * curTime / 1000.0f, this.mTexAnimDirections[0].Y * curTime / 1000.0f, 0.0f);
	        this.mTexAnimStore.Layer1 = Matrix4.CreateTranslation(this.mTexAnimDirections[1].X * curTime / 1000.0f, this.mTexAnimDirections[1].Y * curTime / 1000.0f, 0.0f);
	        this.mTexAnimStore.Layer2 = Matrix4.CreateTranslation(this.mTexAnimDirections[2].X * curTime / 1000.0f, this.mTexAnimDirections[2].Y * curTime / 1000.0f, 0.0f);
	        this.mTexAnimStore.Layer3 = Matrix4.CreateTranslation(this.mTexAnimDirections[3].X * curTime / 1000.0f, this.mTexAnimDirections[3].Y * curTime / 1000.0f, 0.0f);

	        this.mTexAnimBuffer.BufferData(this.mTexAnimStore);
        }

        private bool BeginSyncLoad()
        {
	        if (this.mSyncLoadToken != null)
	        {
		        return false;
	        }

            if (WorldFrame.Instance.MapManager.IsInitialLoad)
            {
                SyncLoad();
                WorldFrame.Instance.MapManager.OnLoadProgress();
                return true;
            }

	        lock (this)
	        {
		        this.mSyncLoadToken = WorldFrame.Instance.Dispatcher.BeginInvoke(SyncLoad);
	        }
            return false;
        }

        private void SyncLoad()
        {
	        this.mSyncLoadToken = null;

	        this.mTexParams.TextureScales = new Vector4(this.mData.TextureScales);
	        this.mTexParams.SpecularFactors = new Vector4(this.mData.SpecularFactors);
	        this.mTexParams.ChunkLine = new Vector4(0.0f, 0.7f, 0.0f, 0.0f);
	        this.mTexParams.AreaColour = ChunkEditManager.Instance.GetAreaColour(this.mData.AreaId, this.mData.HasImpassFlag);

	        this.mTexAnimBuffer = new UniformBuffer();
	        this.mTexAnimStore.Layer0 = this.mTexAnimStore.Layer1 = this.mTexAnimStore.Layer2 = this.mTexAnimStore.Layer3 = Matrix4.Identity;
	        this.mTexAnimBuffer.BufferData(this.mTexAnimStore);

	        this.mAlphaTexture = new Graphics.Texture();
	        this.mAlphaTexture.UpdateMemory(64, 64, Format.R8G8B8A8_UNorm, this.mData.AlphaValues, 4 * 64);

	        this.mHoleTexture = new Graphics.Texture();
	        this.mHoleTexture.UpdateMemory(8, 8, Format.R8_UNorm, this.mData.HoleValues, 8);

	        this.mScaleBuffer = new UniformBuffer();
	        this.mScaleBuffer.BufferData(this.mTexParams);

	        this.mShaderTextures = this.mData.Textures.ToArray();
	        this.mShaderSpecularTextures = this.mData.SpecularTextures.ToArray();

            SetRenderMode(ChunkEditManager.Instance.ChunkRenderMode); //Set current render mode

	        this.mIsSyncLoaded = true;
        }

        public static void Initialize()
        {
            ChunkMesh = new Mesh();
            InitMesh();
        }

        private static void InitMesh()
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
            ChunkMesh.RasterizerState.BackfaceCullingEnabled = true;

            BlendNew = new ShaderProgram();
            BlendNew.SetVertexShader(Resources.Shaders.TerrainVertex);
            BlendNew.SetPixelShader(Resources.Shaders.TerrainFragmentNew);

            ChunkMesh.Program = BlendNew;
            ChunkMesh.InitLayout(BlendNew);

            BlendOld = new ShaderProgram();
            BlendOld.SetVertexShader(Resources.Shaders.TerrainVertex);
	        BlendOld.SetPixelShader(Resources.Shaders.TerrainFragment);

            ColorSampler = new Sampler()
            {
                AddressU = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                AddressV = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                AddressW = SharpDX.Direct3D11.TextureAddressMode.Clamp,
                Filter = SharpDX.Direct3D11.Filter.Anisotropic,
                MaximumAnisotropy = 16,
            };
            AlphaSampler = new Sampler()
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

            ChunkMesh.IndexBuffer.BufferData(indices);
            ChunkMesh.IndexBuffer.IndexFormat = DrawElementsType.UnsignedInt;
        }
    }
}
