using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using WoWEditor6.Graphics;
using WoWEditor6.IO.Files.Models;

namespace WoWEditor6.Scene.Models.M2
{
    class M2BatchRenderer
    {
        private static Mesh gMesh;

        private IM2Animator mAnimator;
        private readonly M2File mModel;
        private bool mIsSyncLoaded;
        private bool mIsSyncLoadRequested;

        private VertexBuffer mVertexBuffer;
        private VertexBuffer mInstanceBuffer;
        private IndexBuffer mIndexBuffer;

        private readonly List<M2RenderInstance> mInstances = new List<M2RenderInstance>();
        private Matrix[] mActiveInstances;
        private bool mUpdateBuffer;

        public BoundingBox BoundingBox => mModel.BoundingBox;

        public M2BatchRenderer(M2File model)
        {
            mModel = model;
            mAnimator = ModelFactory.Instance.CreateAnimator(model);
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

            CheckBuffer();
        }

        public void AddInstance(int uuid, Vector3 position, Quaternion rotation, Vector3 scaling)
        {
            var instance = new M2RenderInstance(uuid, position, rotation, scaling, this);
            lock (mInstances)
            {
                mInstances.Add(instance);
                if (!WorldFrame.Instance.ActiveCamera.Contains(ref instance.BoundingBox))
                    return;

                lock (mInstanceBuffer)
                    mActiveInstances = mActiveInstances.Concat(new[] {instance.InstanceMatrix}).ToArray();
                mUpdateBuffer = true;
            }
        }

        public void ViewChanged()
        {
            lock(mInstances)
            {
                lock (mInstanceBuffer)
                {
                    mActiveInstances =
                        mInstances.Where(i => WorldFrame.Instance.ActiveCamera.Contains(ref i.BoundingBox))
                            .Select(i => i.InstanceMatrix)
                            .ToArray();

                    mUpdateBuffer = true;
                }
            }
        }

        private void CheckBuffer()
        {
            if (mUpdateBuffer == false)
                return;

            mUpdateBuffer = false;
            lock (mInstanceBuffer)
                mInstanceBuffer.UpdateData(mActiveInstances);
        }

        private void SyncLoad()
        {
            var ctx = WorldFrame.Instance.GraphicsContext;
            mVertexBuffer = new VertexBuffer(ctx);
            // ReSharper disable once InconsistentlySynchronizedField
            mInstanceBuffer = new VertexBuffer(ctx);
            mIndexBuffer = new IndexBuffer(ctx);

            mVertexBuffer.UpdateData(mModel.Vertices);
            mIndexBuffer.UpdateData(mModel.Indices);

            mIsSyncLoaded = true;
        }

        public static void Initialize(GxContext context)
        {
            gMesh = new Mesh(context)
            {
                Stride = IO.SizeCache<M2Vertex>.Size,
                InstanceStride = 64,
                DepthState = {DepthEnabled = true}
            };

            // 4 * 4 matrix entries, 4 bytes per entry
            gMesh.BlendState.Dispose();
            gMesh.IndexBuffer.Dispose();
            gMesh.VertexBuffer.Dispose();

            gMesh.AddElement("POSITION", 0, 3);
            gMesh.AddElement("BLENDWEIGHT", 0, 4, DataType.Byte, true);
            gMesh.AddElement("BLENDINDEX", 0, 4, DataType.Byte);
            gMesh.AddElement("NORMAL", 0, 3);
            gMesh.AddElement("TEXCOORD", 0, 2);
            gMesh.AddElement("TEXCOORD", 1, 2);

            gMesh.AddElement("TEXCOORD", 2, 4, DataType.Float, false, 1, true);
            gMesh.AddElement("TEXCOORD", 3, 4, DataType.Float, false, 1, true);
            gMesh.AddElement("TEXCOORD", 4, 4, DataType.Float, false, 1, true);
            gMesh.AddElement("TEXCOORD", 5, 4, DataType.Float, false, 1, true);

            var program = new ShaderProgram(context);
            program.SetVertexShader(Resources.Shaders.M2VertexInstanced, "main");
            program.SetPixelShader(Resources.Shaders.M2Pixel, "main");

            gMesh.Program = program;
        }
    }
}
