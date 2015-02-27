using System;
using System.Collections.Generic;
using SharpDX;
using WoWEditor6.Graphics;

namespace WoWEditor6.Scene.Models.WMO
{
    class WmoGroupRender : IDisposable
    {
        class WmoRenderBatch
        {
            public IO.Files.Models.WmoMaterial Material;
            public IO.Files.Models.WmoBatch Batch;
        }

        public static Mesh Mesh { get; private set; }

        private static BlendState gNoBlendState;
        private static BlendState gAlphaBlendState;
        private static ShaderProgram gBlendProgram;
        private static ShaderProgram gNoBlendProgram;
        private static ShaderProgram gIndoorBlendProgram;
        private static ShaderProgram gIndoorNoBlendProgram;
        private static RasterState gNoCullState;
        private static RasterState gCullState;

        public static Sampler Sampler { get; private set; }
        public static ConstantBuffer InstanceBuffer { get; private set; }

        private readonly List<WmoRenderBatch> mBatches = new List<WmoRenderBatch>();

        private bool mLoaded;

        public int BaseIndex { get; set; }
        public int BaseVertex { get; set; }

        public IO.Files.Models.WmoGroup Data { get; private set; }

        public BoundingBox BoundingBox { get { return Data.BoundingBox; } }

        public WmoGroupRender(IO.Files.Models.WmoGroup group, WmoRootRender root)
        {
            Data = group;
            foreach(var batch in Data.Batches)
            {
                mBatches.Add(new WmoRenderBatch
                {
                    Batch = batch,
                    Material = root.Data.GetMaterial(batch.MaterialId)
                });
            }
        }

        public void Dispose()
        {
            
        }

        public void OnFrame()
        {
            if (mLoaded == false)
                return;

            Mesh.StartVertex = BaseVertex;

            foreach(var batch in mBatches)
            {
                SetupBatch(batch);
                Mesh.StartIndex = BaseIndex + batch.Batch.StartIndex;
                Mesh.IndexCount = batch.Batch.NumIndices;

                Mesh.Draw();
            }
        }

        public void SyncLoad()
        {
            mLoaded = true;
        }

        private void SetupBatch(WmoRenderBatch batch)
        {
            var cullingDisabled = (batch.Material.MaterialFlags & 0x04) != 0;
            Mesh.UpdateRasterizerState(cullingDisabled ? gNoCullState : gCullState);
            Mesh.UpdateBlendState((batch.Batch.BlendMode != 0) ? gAlphaBlendState : gNoBlendState);
            ShaderProgram newProgram;
            if(Data.IsIndoor)
                newProgram = (batch.Batch.BlendMode != 0) ? gIndoorBlendProgram : gIndoorNoBlendProgram;
            else
                newProgram = (batch.Batch.BlendMode != 0) ? gBlendProgram : gNoBlendProgram;

            if(newProgram != Mesh.Program)
            {
                Mesh.Program = newProgram;
                Mesh.Program.Bind();
            }
            Mesh.Program.SetPixelTextures(0, batch.Material.Textures);
        }

        public static void Initialize(GxContext context)
        {
            gNoBlendState = new BlendState(context) { BlendEnabled = false };
            gAlphaBlendState = new BlendState(context) { BlendEnabled = true };

            gNoCullState = new RasterState(context) {CullEnabled = false};
            gCullState = new RasterState(context) {CullEnabled = true};

            Sampler = new Sampler(context);

            InstanceBuffer = new ConstantBuffer(context);
            InstanceBuffer.UpdateData(Matrix.Identity); // preallocate space so the underlying buffer wont change anymore

            Mesh = new Mesh(context)
            {
                DepthState = { DepthEnabled = true },
                Stride = IO.SizeCache<IO.Files.Models.WmoVertex>.Size
            };

            Mesh.AddElement("POSITION", 0, 3);
            Mesh.AddElement("NORMAL", 0, 3);
            Mesh.AddElement("TEXCOORD", 0, 2);
            Mesh.AddElement("COLOR", 0, 4, DataType.Byte, true);

            gNoBlendProgram = new ShaderProgram(context);
            gNoBlendProgram.SetVertexShader(Resources.Shaders.WmoVertex);
            gNoBlendProgram.SetPixelShader(Resources.Shaders.WmoPixel);

            gBlendProgram = new ShaderProgram(context);
            gBlendProgram.SetVertexShader(Resources.Shaders.WmoVertex);
            gBlendProgram.SetPixelShader(Resources.Shaders.WmoPixelBlend);

            gIndoorNoBlendProgram = new ShaderProgram(context);
            gIndoorNoBlendProgram.SetVertexShader(Resources.Shaders.WmoVertex);
            gIndoorNoBlendProgram.SetPixelShader(Resources.Shaders.WmoPixelIndoor);

            gIndoorBlendProgram = new ShaderProgram(context);
            gIndoorBlendProgram.SetVertexShader(Resources.Shaders.WmoVertex);
            gIndoorBlendProgram.SetPixelShader(Resources.Shaders.WmoPixelBlendIndoor);

            Mesh.Program = gNoBlendProgram;

            Mesh.InitLayout(gNoBlendProgram);

        }
    }
}
