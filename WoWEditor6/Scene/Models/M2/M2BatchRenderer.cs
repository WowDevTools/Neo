using System.Collections.Generic;
using System.Linq;
using SharpDX;
using WoWEditor6.Graphics;
using WoWEditor6.IO.Files.Models;

namespace WoWEditor6.Scene.Models.M2
{
    class M2BatchRenderer
    {
        public static Mesh Mesh { get; private set; }
        public static Sampler Sampler { get; private set; }

        private static readonly BlendState[] gBlendStates = new BlendState[7];

        private static ShaderProgram gNoBlendProgram;
        private static ShaderProgram gBlendProgram;

        private readonly IM2Animator mAnimator;
        private readonly M2File mModel;
        private bool mIsSyncLoaded;
        private bool mIsSyncLoadRequested;

        private VertexBuffer mVertexBuffer;
        private VertexBuffer mInstanceBuffer;
        private IndexBuffer mIndexBuffer;
        private readonly object mInstanceBufferLock = new object();

        private int mInstanceCount;

        private Matrix[] mActiveInstances = new Matrix[0];
        private bool mUpdateBuffer;
        private readonly Matrix[] mAnimationMatrices = new Matrix[256];

        private readonly Dictionary<int, M2RenderInstance> mFullInstances = new Dictionary<int, M2RenderInstance>();
        private readonly List<M2RenderInstance> mVisibleInstances = new List<M2RenderInstance>();
        private readonly List<int> mUpdatedEntries = new List<int>();

        private bool mSkipRendering;

        private ConstantBuffer mAnimBuffer;

        public BoundingBox BoundingBox => mModel.BoundingBox;

        public M2BatchRenderer(M2File model)
        {
            mModel = model;
            mAnimator = ModelFactory.Instance.CreateAnimator(model);
            mAnimator.SetAnimationByIndex(0);
            StaticAnimationThread.Instance.AddAnimator(mAnimator);
        }

        public void OnFrame()
        {
            if(mIsSyncLoaded == false)
            {
                if (mIsSyncLoadRequested)
                    return;

                if (WorldFrame.Instance.MapManager.IsInitialLoad)
                    SyncLoad();
                else
                {
                    WorldFrame.Instance.Dispatcher.BeginInvoke(SyncLoad);
                    mIsSyncLoadRequested = true;
                    return;
                }
            }

            if (mSkipRendering)
                return;

            CheckBuffer();

            if (mInstanceCount == 0)
                return;

            Mesh.UpdateIndexBuffer(mIndexBuffer);
            Mesh.UpdateVertexBuffer(mVertexBuffer);
            Mesh.UpdateInstanceBuffer(mInstanceBuffer);

            if (mAnimator.GetBones(mAnimationMatrices))
                mAnimBuffer.UpdateData(mAnimationMatrices);

            Mesh.Program.SetVertexConstantBuffer(2, mAnimBuffer);

            foreach (var pass in mModel.Passes)
            {
                Mesh.UpdateBlendState(gBlendStates[pass.BlendMode]);
                var oldProgram = Mesh.Program;
                Mesh.Program = (pass.BlendMode > 0 ? gBlendProgram : gNoBlendProgram);
                if (Mesh.Program != oldProgram)
                    Mesh.Program.Bind();

                Mesh.StartVertex = 0;
                Mesh.StartIndex = pass.StartIndex;
                Mesh.IndexCount = pass.IndexCount;
                Mesh.Program.SetPixelTexture(0, pass.Textures.First());
                Mesh.Draw(mInstanceCount);
            }
        }

        public bool RemoveInstance(int uuid)
        {
            lock(mFullInstances)
            {
                if (mFullInstances.ContainsKey(uuid) == false)
                    return false;

                mFullInstances.Remove(uuid);
                lock(mInstanceBuffer)
                {
                    for(var i = 0; i < mVisibleInstances.Count; ++i)
                    {
                        if(mVisibleInstances[i].Uuid == uuid)
                        {
                            mVisibleInstances.RemoveAt(i);
                            mUpdateBuffer = true;
                            break;
                        }
                    }

                    return mFullInstances.Count == 0;
                }
            }
        }

        public BoundingBox AddInstance(int uuid, Vector3 position, Vector3 rotation, Vector3 scaling)
        {
            var instance = new M2RenderInstance(uuid, position, rotation, scaling, this);
            lock (mFullInstances)
            {
                if (mFullInstances.ContainsKey(uuid))
                    return instance.BoundingBox;

                mFullInstances.Add(uuid, instance);
                if (!WorldFrame.Instance.ActiveCamera.Contains(ref instance.BoundingBox))
                    return instance.BoundingBox;

                lock (mInstanceBufferLock)
                {
                    mVisibleInstances.Add(instance);
                    if (mVisibleInstances.Count > mActiveInstances.Length)
                        mActiveInstances = new Matrix[mVisibleInstances.Count];

                    for (var i = 0; i < mVisibleInstances.Count; ++i)
                        mActiveInstances[i] = mVisibleInstances[i].InstanceMatrix;

                    mInstanceCount = mVisibleInstances.Count;

                }

                mUpdateBuffer = true;
                return instance.BoundingBox;
            }
        }

        public void PushMapReference(M2Instance instance)
        {
            lock(mInstanceBufferLock)
            {
                if (mUpdatedEntries.Contains(instance.Uuid))
                    return;

                var inst = mFullInstances[instance.Uuid];
                if (WorldFrame.Instance.ActiveCamera.Contains(ref inst.BoundingBox))
                    mVisibleInstances.Add(inst);

                mUpdatedEntries.Add(instance.Uuid);
            }
        }

        public void ViewChanged()
        {
            mVisibleInstances.Clear();
            mUpdatedEntries.Clear();
        }

        private void CheckBuffer()
        {
            if(M2Manager.IsViewDirty)
            {
                lock(mInstanceBufferLock)
                {
                    if (mActiveInstances.Length < mVisibleInstances.Count)
                        mActiveInstances = new Matrix[mVisibleInstances.Count];

                    for (var i = 0; i < mVisibleInstances.Count; ++i)
                        mActiveInstances[i] = mVisibleInstances[i].InstanceMatrix;

                    mInstanceCount = mVisibleInstances.Count;
                    if (mInstanceCount == 0)
                        return;

                    mInstanceBuffer.UpdateData(mActiveInstances);
                }
            }
            else if(mUpdateBuffer)
            {
                lock(mInstanceBuffer)
                {
                    mUpdateBuffer = false;
                    if (mInstanceCount == 0)
                        return;

                    mInstanceBuffer.UpdateData(mActiveInstances);
                }
            }
        }

        private void SyncLoad()
        {
            if(mModel.Vertices.Length ==0 || mModel.Indices.Length == 0 || mModel.Passes.Count == 0)
            {
                mIsSyncLoaded = true;
                mSkipRendering = true;
                return;
            }

            var ctx = WorldFrame.Instance.GraphicsContext;
            mVertexBuffer = new VertexBuffer(ctx);
            // ReSharper disable once InconsistentlySynchronizedField
            mInstanceBuffer = new VertexBuffer(ctx);
            mIndexBuffer = new IndexBuffer(ctx);

            mVertexBuffer.UpdateData(mModel.Vertices);
            mIndexBuffer.UpdateData(mModel.Indices);

            mAnimBuffer = new ConstantBuffer(ctx);
            mAnimBuffer.UpdateData(mAnimationMatrices);

            mIsSyncLoaded = true;
        }

        public static void Initialize(GxContext context)
        {
            Mesh = new Mesh(context)
            {
                Stride = IO.SizeCache<M2Vertex>.Size,
                InstanceStride = 64,
                DepthState = {DepthEnabled = true}
            };

            Mesh.BlendState.Dispose();
            Mesh.IndexBuffer.Dispose();
            Mesh.VertexBuffer.Dispose();

            Mesh.AddElement("POSITION", 0, 3);
            Mesh.AddElement("BLENDWEIGHT", 0, 4, DataType.Byte, true);
            Mesh.AddElement("BLENDINDEX", 0, 4, DataType.Byte);
            Mesh.AddElement("NORMAL", 0, 3);
            Mesh.AddElement("TEXCOORD", 0, 2);
            Mesh.AddElement("TEXCOORD", 1, 2);

            Mesh.AddElement("TEXCOORD", 2, 4, DataType.Float, false, 1, true);
            Mesh.AddElement("TEXCOORD", 3, 4, DataType.Float, false, 1, true);
            Mesh.AddElement("TEXCOORD", 4, 4, DataType.Float, false, 1, true);
            Mesh.AddElement("TEXCOORD", 5, 4, DataType.Float, false, 1, true);

            var program = new ShaderProgram(context);
            program.SetVertexShader(Resources.Shaders.M2VertexInstanced, "main");
            program.SetPixelShader(Resources.Shaders.M2Pixel, "main");

            Mesh.Program = program;

            Sampler = new Sampler(context)
            {
                AddressMode = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                Filter = SharpDX.Direct3D11.Filter.MinMagMipLinear
            };

            for (var i = 0; i < gBlendStates.Length; ++i)
                gBlendStates[i] = new BlendState(context);

            gBlendStates[0] = new BlendState(context)
            {
                BlendEnabled = false
            };

            gBlendStates[1] = new BlendState(context)
            {
                BlendEnabled = true,
                SourceBlend = SharpDX.Direct3D11.BlendOption.One,
                DestinationBlend = SharpDX.Direct3D11.BlendOption.Zero,
                SourceAlphaBlend = SharpDX.Direct3D11.BlendOption.One,
                DestinationAlphaBlend = SharpDX.Direct3D11.BlendOption.Zero
            };

            gBlendStates[2] = new BlendState(context)
            {
                BlendEnabled = true,
                SourceBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha,
                DestinationBlend = SharpDX.Direct3D11.BlendOption.InverseSourceAlpha,
                SourceAlphaBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha,
                DestinationAlphaBlend = SharpDX.Direct3D11.BlendOption.InverseSourceAlpha
            };

            gBlendStates[3] = new BlendState(context)
            {
                BlendEnabled = true,
                SourceBlend = SharpDX.Direct3D11.BlendOption.SourceColor,
                DestinationBlend = SharpDX.Direct3D11.BlendOption.DestinationColor,
                SourceAlphaBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha,
                DestinationAlphaBlend = SharpDX.Direct3D11.BlendOption.DestinationAlpha
            };

            gBlendStates[4] = new BlendState(context)
            {
                BlendEnabled = true,
                SourceBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha,
                DestinationBlend = SharpDX.Direct3D11.BlendOption.One,
                SourceAlphaBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha,
                DestinationAlphaBlend = SharpDX.Direct3D11.BlendOption.One
            };

            gBlendStates[5] = new BlendState(context)
            {
                BlendEnabled = true,
                SourceBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha,
                DestinationBlend = SharpDX.Direct3D11.BlendOption.InverseSourceAlpha,
                SourceAlphaBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha,
                DestinationAlphaBlend = SharpDX.Direct3D11.BlendOption.InverseSourceAlpha
            };

            gBlendStates[6] = new BlendState(context)
            {
                BlendEnabled = true,
                SourceBlend = SharpDX.Direct3D11.BlendOption.DestinationColor,
                DestinationBlend = SharpDX.Direct3D11.BlendOption.SourceColor,
                SourceAlphaBlend = SharpDX.Direct3D11.BlendOption.DestinationAlpha,
                DestinationAlphaBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha
            };

            gNoBlendProgram = program;

            gBlendProgram = new ShaderProgram(context);
            gBlendProgram.SetPixelShader(Resources.Shaders.M2Pixel, "main_blend");
            gBlendProgram.SetVertexShader(Resources.Shaders.M2VertexInstanced, "main");
        }
    }
}
