using System;
using System.Collections.Generic;
using SharpDX;
using Neo.Graphics;
using Neo.IO.Files.Models;
using Neo.Storage;

namespace Neo.Scene.Models.M2
{
    class M2Renderer : IDisposable
    {
        private M2BatchRenderer mBatchRenderer;
        private M2PortraitRenderer mPortraitRenderer;
        private M2SingleRenderer mSingleRenderer;

        public VertexBuffer VertexBuffer { get; private set; }
        public IndexBuffer IndexBuffer { get; private set; }
        public ConstantBuffer AnimBuffer { get; private set; }

        public M2File Model { get; private set; }

        private Matrix[] mAnimationMatrices;
        private Dictionary<int, M2RenderInstance> mFullInstances = new Dictionary<int, M2RenderInstance>();

        public List<M2RenderInstance> VisibleInstances { get; private set; }

        private bool mIsSyncLoaded;
        private bool mSkipRendering;
        private object mSyncLoadToken;

        public IM2Animator Animator { get; private set; }
        public M2PortraitRenderer PortraitRenderer { get { return mPortraitRenderer; } }

        public M2Renderer(M2File model)
        {
            Model = model;
            VisibleInstances = new List<M2RenderInstance>();

            if (!model.NeedsPerInstanceAnimation)
            {
                mAnimationMatrices = new Matrix[model.GetNumberOfBones()];
                Animator = ModelFactory.Instance.CreateAnimator(model);
                if (Animator.SetAnimation(AnimationType.Stand) == false)
                    Animator.SetAnimationByIndex(0);
                StaticAnimationThread.Instance.AddAnimator(Animator);
            }

            mBatchRenderer = new M2BatchRenderer(model);
            mSingleRenderer = new M2SingleRenderer(model);
            mPortraitRenderer = new M2PortraitRenderer(model);
        }

        public void RenderBatch()
        {
            if (mIsSyncLoaded == false)
            {
                if (!BeginSyncLoad())
                    return;
            }

            if (mSkipRendering || Model.NeedsPerInstanceAnimation)
                return;

            if (Animator.GetBones(mAnimationMatrices))
                AnimBuffer.UpdateData(mAnimationMatrices);

            if (!Model.HasOpaquePass)
                return;

            // TODO: We *really* need to get rid of this!
            if (IO.FileManager.Instance.Version == IO.FileDataVersion.Lichking)
                mBatchRenderer.OnFrame_Old(this);
            else
                mBatchRenderer.OnFrame(this);
        }

        public void RenderSingleInstance(M2RenderInstance instance)
        {
            if (mIsSyncLoaded == false)
            {
                if (!BeginSyncLoad())
                    return;
            }

            if (mSkipRendering)
                return;

            // TODO: We *really* need to get rid of this!
            if (IO.FileManager.Instance.Version == IO.FileDataVersion.Lichking)
                mSingleRenderer.OnFrame_Old(this, instance);
            else
                mSingleRenderer.OnFrame(this, instance);
        }

        public void RenderPortrait()
        {
            if (mIsSyncLoaded == false)
            {
                if (!BeginSyncLoad())
                    return;
            }

            if (!mSkipRendering)
            {
                mPortraitRenderer.OnFrame(this);
                //M2RenderInstance instance = new M2RenderInstance( 0, new Vector3( 0.0f, 0.0f, 0.0f ), new Vector3( 0.0f, 0.0f, 0.0f ), new Vector3( 0.0f, 0.0f, 0.0f ), this );

                //mSingleRenderer.OnFrame(this, instance );
            }
        }

        public bool RemoveInstance(int uuid)
        {
            if (mFullInstances == null || VisibleInstances == null)
                return false;

            lock (mFullInstances)
            {
                M2RenderInstance inst;
                if (mFullInstances.TryGetValue(uuid, out inst) == false)
                    return false;

                --inst.NumReferences;
                if (inst.NumReferences > 0)
                {
                    ++inst.NumReferences;
                    return false;
                }

                mFullInstances.Remove(uuid);
                inst.Dispose();
            }

            lock (VisibleInstances)
                VisibleInstances.RemoveAll(inst => inst.Uuid == uuid);

            return mFullInstances.Count == 0;
        }

        public M2RenderInstance AddInstance(int uuid, Vector3 position, Vector3 rotation, Vector3 scaling)
        {
            M2RenderInstance inst;
            // ReSharper disable once InconsistentlySynchronizedField
            if (mFullInstances.TryGetValue(uuid, out inst))
            {
                ++inst.NumReferences;
                return inst;
            }

            var instance = new M2RenderInstance(uuid, position, rotation, scaling, this);
            lock (mFullInstances)
            {
                mFullInstances.Add(uuid, instance);
                if (!instance.IsVisible(WorldFrame.Instance.ActiveCamera))
                    return instance;

                lock (VisibleInstances)
                    VisibleInstances.Add(instance);
                return instance;
            }
        }

        public void PushMapReference(M2Instance instance)
        {
            var renderInstance = instance.RenderInstance;
            if (Model.HasBlendPass)
                renderInstance.UpdateDepth();

            renderInstance.IsUpdated = true;
            lock (VisibleInstances)
                VisibleInstances.Add(renderInstance);
        }

        public void ViewChanged()
        {
            lock (VisibleInstances)
                VisibleInstances.Clear();

            lock (mFullInstances)
            {
                foreach (var pair in mFullInstances)
                    pair.Value.IsUpdated = false;
            }
        }

        private bool BeginSyncLoad()
        {
            if (mSyncLoadToken != null)
                return false;

            if (WorldFrame.Instance.MapManager.IsInitialLoad)
            {
                SyncLoad();
                return true;
            }

            mSyncLoadToken = WorldFrame.Instance.Dispatcher.BeginInvoke(SyncLoad);
            return false;
        }

        private void SyncLoad()
        {
            mIsSyncLoaded = true;
            mSyncLoadToken = null;

            if (Model.Vertices.Length == 0 || Model.Indices.Length == 0 || Model.Passes.Count == 0)
            {
                mSkipRendering = true;
                return;
            }

            var ctx = WorldFrame.Instance.GraphicsContext;
            VertexBuffer = new VertexBuffer(ctx);
            IndexBuffer = new IndexBuffer(ctx);

            VertexBuffer.UpdateData(Model.Vertices);
            IndexBuffer.UpdateData(Model.Indices);

            if (Animator != null)
            {
                AnimBuffer = new ConstantBuffer(ctx);
                AnimBuffer.UpdateData(mAnimationMatrices);
            }

            mBatchRenderer.OnSyncLoad();
            mSingleRenderer.OnSyncLoad();
            mPortraitRenderer.OnSyncLoad();
        }

        ~M2Renderer()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            mSkipRendering = true;
            if (mBatchRenderer != null)
            {
                mBatchRenderer.Dispose();
                mBatchRenderer = null;
            }

            if (mSingleRenderer != null)
            {
                mSingleRenderer.Dispose();
                mSingleRenderer = null;
            }

            if (mPortraitRenderer != null)
            {
                mPortraitRenderer.Dispose();
                mPortraitRenderer = null;
            }

            if (mFullInstances != null)
            {
                lock (mFullInstances)
                {
                    foreach (var inst in mFullInstances.Values)
                        inst.Dispose();

                    mFullInstances.Clear();
                    mFullInstances = null;
                }
            }

            var vb = VertexBuffer;
            var ib = IndexBuffer;
            var ab = AnimBuffer;

            WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
            {
                if (vb != null)
                    vb.Dispose();
                if (ib != null)
                    ib.Dispose();
                if (ab != null)
                    ab.Dispose();
            });

            VertexBuffer = null;
            IndexBuffer = null;
            AnimBuffer = null;

            if (Animator != null)
            {
                StaticAnimationThread.Instance.RemoveAnimator(Animator);
                Animator = null;
            }

            // Sync load can be called even after the object has been disposed.
            if (mSyncLoadToken != null)
            {
                WorldFrame.Instance.Dispatcher.Remove(mSyncLoadToken);
                mSyncLoadToken = null;
            }

            mAnimationMatrices = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
