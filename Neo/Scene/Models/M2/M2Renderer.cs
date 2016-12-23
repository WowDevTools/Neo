using System;
using System.Collections.Generic;
using Neo.Graphics;
using Neo.IO.Files.Models;
using Neo.Storage;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Neo.Scene.Models.M2
{
    public class M2Renderer : IDisposable
    {
        private M2BatchRenderer mBatchRenderer;
        private M2PortraitRenderer mPortraitRenderer;
        private M2SingleRenderer mSingleRenderer;

        public VertexBuffer VertexBuffer { get; private set; }
        public IndexBuffer IndexBuffer { get; private set; }
        public UniformBuffer AnimBuffer { get; private set; }

        public M2File Model { get; private set; }

        private Matrix4[] mAnimationMatrices;
        private Dictionary<int, M2RenderInstance> mFullInstances = new Dictionary<int, M2RenderInstance>();

        public List<M2RenderInstance> VisibleInstances { get; private set; }

        private bool mIsSyncLoaded;
        private bool mSkipRendering;
        private object mSyncLoadToken;

        public IM2Animator Animator { get; private set; }
        public M2PortraitRenderer PortraitRenderer { get { return this.mPortraitRenderer; } }

        public M2Renderer(M2File model)
        {
	        this.Model = model;
	        this.VisibleInstances = new List<M2RenderInstance>();

            if (!model.NeedsPerInstanceAnimation)
            {
	            this.mAnimationMatrices = new Matrix4[model.GetNumberOfBones()];
	            this.Animator = ModelFactory.Instance.CreateAnimator(model);
                if (this.Animator.SetAnimation(AnimationType.Stand) == false)
                {
	                this.Animator.SetAnimationByIndex(0);
                }
	            StaticAnimationThread.Instance.AddAnimator(this.Animator);
            }

	        this.mBatchRenderer = new M2BatchRenderer(model);
	        this.mSingleRenderer = new M2SingleRenderer(model);
	        this.mPortraitRenderer = new M2PortraitRenderer(model);
        }

        public void RenderBatch()
        {
            if (this.mIsSyncLoaded == false)
            {
                if (!BeginSyncLoad())
                {
	                return;
                }
            }

	        if (this.mSkipRendering || this.Model.NeedsPerInstanceAnimation)
	        {
		        return;
	        }

	        if (this.Animator.GetBones(this.mAnimationMatrices))
	        {
		        this.AnimBuffer.BufferData(this.mAnimationMatrices);
	        }

	        if (!this.Model.HasOpaquePass)
	        {
		        return;
	        }

            // TODO: We *really* need to get rid of this!
	        if (IO.FileManager.Instance.Version == IO.FileDataVersion.Lichking)
	        {
		        this.mBatchRenderer.OnFrame_Old(this);
	        }
	        else
	        {
		        this.mBatchRenderer.OnFrame(this);
	        }
        }

        public void RenderSingleInstance(M2RenderInstance instance)
        {
            if (this.mIsSyncLoaded == false)
            {
	            if (!BeginSyncLoad())
	            {
		            return;
	            }
            }

            if (this.mSkipRendering)
            {
	            return;
            }

	        // TODO: We *really* need to get rid of this!
	        if (IO.FileManager.Instance.Version == IO.FileDataVersion.Lichking)
	        {
		        this.mSingleRenderer.OnFrame_Old(this, instance);
	        }
	        else
	        {
		        this.mSingleRenderer.OnFrame(this, instance);
	        }
        }

        public void RenderPortrait()
        {
            if (this.mIsSyncLoaded == false)
            {
	            if (!BeginSyncLoad())
	            {
		            return;
	            }
            }

            if (!this.mSkipRendering)
            {
	            this.mPortraitRenderer.OnFrame(this);
                //M2RenderInstance instance = new M2RenderInstance( 0, new Vector3( 0.0f, 0.0f, 0.0f ), new Vector3( 0.0f, 0.0f, 0.0f ), new Vector3( 0.0f, 0.0f, 0.0f ), this );

                //mSingleRenderer.OnFrame(this, instance );
            }
        }

        public bool RemoveInstance(int uuid)
        {
	        if (this.mFullInstances == null || this.VisibleInstances == null)
	        {
		        return false;
	        }

            lock (this.mFullInstances)
            {
                M2RenderInstance inst;
	            if (this.mFullInstances.TryGetValue(uuid, out inst) == false)
	            {
		            return false;
	            }

                --inst.NumReferences;
                if (inst.NumReferences > 0)
                {
                    ++inst.NumReferences;
                    return false;
                }

	            this.mFullInstances.Remove(uuid);
                inst.Dispose();
            }

	        lock (this.VisibleInstances)
	        {
		        this.VisibleInstances.RemoveAll(inst => inst.Uuid == uuid);
	        }

            return this.mFullInstances.Count == 0;
        }

        public M2RenderInstance AddInstance(int uuid, Vector3 position, Vector3 rotation, Vector3 scaling)
        {
            M2RenderInstance inst;
            // ReSharper disable once InconsistentlySynchronizedField
            if (this.mFullInstances.TryGetValue(uuid, out inst))
            {
                ++inst.NumReferences;
                return inst;
            }

            var instance = new M2RenderInstance(uuid, position, rotation, scaling, this);
            lock (this.mFullInstances)
            {
	            this.mFullInstances.Add(uuid, instance);
	            if (!instance.IsVisible(WorldFrame.Instance.ActiveCamera))
	            {
		            return instance;
	            }

	            lock (this.VisibleInstances)
	            {
		            this.VisibleInstances.Add(instance);
	            }
                return instance;
            }
        }

        public void PushMapReference(M2Instance instance)
        {
            var renderInstance = instance.RenderInstance;
	        if (this.Model.HasBlendPass)
	        {
		        renderInstance.UpdateDepth();
	        }

            renderInstance.IsUpdated = true;
	        lock (this.VisibleInstances)
	        {
		        this.VisibleInstances.Add(renderInstance);
	        }
        }

        public void ViewChanged()
        {
	        lock (this.VisibleInstances)
	        {
		        this.VisibleInstances.Clear();
	        }

            lock (this.mFullInstances)
            {
	            foreach (var pair in this.mFullInstances)
	            {
		            pair.Value.IsUpdated = false;
	            }
            }
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
                return true;
            }

	        this.mSyncLoadToken = WorldFrame.Instance.Dispatcher.BeginInvoke(SyncLoad);
            return false;
        }

        private void SyncLoad()
        {
	        this.mIsSyncLoaded = true;
	        this.mSyncLoadToken = null;

            if (this.Model.Vertices.Length == 0 || this.Model.Indices.Length == 0 || this.Model.Passes.Count == 0)
            {
	            this.mSkipRendering = true;
                return;
            }

	        this.VertexBuffer = new VertexBuffer();
	        this.IndexBuffer = new IndexBuffer(DrawElementsType.UnsignedShort);

	        this.VertexBuffer.BufferData(this.Model.Vertices);
	        this.IndexBuffer.BufferData(this.Model.Indices);

            if (this.Animator != null)
            {
	            this.AnimBuffer = new UniformBuffer();
	            this.AnimBuffer.BufferData(this.mAnimationMatrices);
            }

	        this.mBatchRenderer.OnSyncLoad();
	        this.mSingleRenderer.OnSyncLoad();
	        this.mPortraitRenderer.OnSyncLoad();
        }

        ~M2Renderer()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
	        this.mSkipRendering = true;
            if (this.mBatchRenderer != null)
            {
	            this.mBatchRenderer.Dispose();
	            this.mBatchRenderer = null;
            }

            if (this.mSingleRenderer != null)
            {
	            this.mSingleRenderer.Dispose();
	            this.mSingleRenderer = null;
            }

            if (this.mPortraitRenderer != null)
            {
	            this.mPortraitRenderer.Dispose();
	            this.mPortraitRenderer = null;
            }

            if (this.mFullInstances != null)
            {
                lock (this.mFullInstances)
                {
                    foreach (var inst in this.mFullInstances.Values)
                    {
	                    inst.Dispose();
                    }

	                this.mFullInstances.Clear();
	                this.mFullInstances = null;
                }
            }

            var vb = this.VertexBuffer;
            var ib = this.IndexBuffer;
            var ab = this.AnimBuffer;

            WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
            {
	            if (vb != null)
	            {
		            vb.Dispose();
	            }
	            if (ib != null)
	            {
		            ib.Dispose();
	            }
	            if (ab != null)
	            {
		            ab.Dispose();
	            }
            });

	        this.VertexBuffer = null;
	        this.IndexBuffer = null;
	        this.AnimBuffer = null;

            if (this.Animator != null)
            {
                StaticAnimationThread.Instance.RemoveAnimator(this.Animator);
	            this.Animator = null;
            }

            // Sync load can be called even after the object has been disposed.
            if (this.mSyncLoadToken != null)
            {
                WorldFrame.Instance.Dispatcher.Remove(this.mSyncLoadToken);
	            this.mSyncLoadToken = null;
            }

	        this.mAnimationMatrices = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
