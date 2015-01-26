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
        private static RasterState gNoCullState;
        private static RasterState gCullState;

        public static Sampler Sampler { get; private set; }
        public static ConstantBuffer InstanceBuffer { get; private set; }

        private readonly List<WmoRenderBatch> mBatches = new List<WmoRenderBatch>();

        private bool mLoaded;

        public int BaseIndex { get; set; }
        public int BaseVertex { get; set; }

        public IO.Files.Models.WmoGroup Data { get; }

        public BoundingBox BoundingBox => Data.BoundingBox;

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
            foreach(var batch in mBatches)
            {
                System.IO.File.AppendAllText("Group" + this.GetHashCode() + ".txt", "StartIndex: " + batch.Batch.StartIndex + " - NumIndices: " +
                                             batch.Batch.NumIndices + "\r\n");
            }
            mLoaded = true;
        }

        private static void SetupBatch(WmoRenderBatch batch)
        {
            var hasCull = (batch.Material.MaterialFlags & 0x04) != 0;
            Mesh.UpdateRasterizerState(hasCull ? gCullState : gNoCullState);
            Mesh.UpdateBlendState((batch.Batch.BlendMode != 0) ? gAlphaBlendState : gNoBlendState);
            Mesh.Program = (batch.Batch.BlendMode != 0) ? gBlendProgram : gNoBlendProgram;
            Mesh.Program.Bind();
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
            gNoBlendProgram.SetVertexShader(Resources.Shaders.WmoVertex, "main");
            gNoBlendProgram.SetPixelShader(Resources.Shaders.WmoPixel, "main");

            gBlendProgram = new ShaderProgram(context);
            gBlendProgram.SetVertexShader(Resources.Shaders.WmoVertex, "main");
            gBlendProgram.SetPixelShader(Resources.Shaders.WmoPixel, "main_blend");

            Mesh.Program = gNoBlendProgram;
        }
    }
}
