using System;
using System.Collections.Generic;
using SharpDX;
using WoWEditor6.Graphics;
using WoWEditor6.IO.Files.Models;
using WoWEditor6.Storage;

namespace WoWEditor6.Scene.Models.M2
{
    class M2Renderer : IDisposable
    {
        private readonly M2BatchRenderer mBatchRenderer;
        private readonly M2PortraitRenderer mPortraitRenderer;
        private readonly M2SingleRenderer mSingleRenderer;

        public VertexBuffer VertexBuffer { get; private set; }
        public IndexBuffer IndexBuffer { get; private set; }
        public ConstantBuffer AnimBuffer { get; private set; }

        public M2File Model { get; private set; }

        private readonly Matrix[] mAnimationMatrices;
        private readonly Dictionary<int, M2RenderInstance> mFullInstances = new Dictionary<int, M2RenderInstance>();

        public List<M2RenderInstance> VisibleInstances { get; private set; }

        private bool mIsSyncLoaded;
        private bool mIsSyncLoadRequested;
        private bool mSkipRendering;

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
                if(Animator.SetAnimation(AnimationType.Stand) == false)
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

            if (Model.HasOpaquePass)
            {
                if (IO.FileManager.Instance.Version == IO.FileDataVersion.Lichking)
                {
                    mBatchRenderer.OnFrame_Old(this);
                }
                else
                {
                    mBatchRenderer.OnFrame(this);

                }
            }
        }

        public void RenderSingleInstance(M2RenderInstance instance)
        {
            if (mIsSyncLoaded == false)
            {
                if (!BeginSyncLoad())
                    return;
            }

            if (!mSkipRendering)
            {
                if (IO.FileManager.Instance.Version == IO.FileDataVersion.Lichking)
                {
                    mSingleRenderer.OnFrame_Old(this, instance);
                }
                else
                {
                    mSingleRenderer.OnFrame(this, instance);
                }
            }
        }

        public void RenderPortrait()
        {
            if (mIsSyncLoaded == false)
            {
                if (!BeginSyncLoad())
                    return;
            }

            if (!mSkipRendering)
                mPortraitRenderer.OnFrame(this);
        }

        public bool RemoveInstance(int uuid)
        {
            bool lastInstance = false;
            lock (mFullInstances)
            {
                M2RenderInstance inst;
                if (mFullInstances.TryGetValue(uuid, out inst) == false)
                    return false;

                --inst.NumReferences;
                if (inst.NumReferences > 0)
                    return false;

                mFullInstances.Remove(uuid);
                if (mFullInstances.Count == 0)
                    lastInstance = true;
            }

            lock (VisibleInstances)
            {
                for (var i = 0; i < VisibleInstances.Count; ++i)
                {
                    if (VisibleInstances[i].Uuid == uuid)
                    {
                        VisibleInstances.RemoveAt(i);
                        break;
                    }
                }
            }

            return lastInstance;
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
                if (!WorldFrame.Instance.ActiveCamera.Contains(ref instance.BoundingBox))
                    return instance;

                lock (VisibleInstances)
                    VisibleInstances.Add(instance);
                return instance;
            }
        }

        public void PushMapReference(M2Instance instance)
        {
            M2RenderInstance renderInstance = instance.RenderInstance;
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
            if (mIsSyncLoadRequested)
                return false;

            if (WorldFrame.Instance.MapManager.IsInitialLoad)
            {
                SyncLoad();
                return true;
            }

            WorldFrame.Instance.Dispatcher.BeginInvoke(SyncLoad);
            mIsSyncLoadRequested = true;
            return false;
        }

        private void SyncLoad()
        {
            mIsSyncLoaded = true;

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

        public virtual void Dispose()
        {
            mSkipRendering = true;
            if (mBatchRenderer != null)
                mBatchRenderer.Dispose();

            if (mSingleRenderer != null)
                mSingleRenderer.Dispose();

            if (mPortraitRenderer != null)
                mPortraitRenderer.Dispose();

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

            if (Animator != null)
                StaticAnimationThread.Instance.RemoveAnimator(Animator);
        }
    }
}
