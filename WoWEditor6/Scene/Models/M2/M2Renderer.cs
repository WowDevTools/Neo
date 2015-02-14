using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpDX;
using WoWEditor6.Graphics;
using WoWEditor6.IO.Files.Models;

namespace WoWEditor6.Scene.Models.M2
{
    class M2Renderer : IDisposable
    {
        public M2BatchRenderer BatchRenderer { get; private set; }
        public M2PortraitRenderer PortraitRenderer { get; private set; }
        public M2AlphaRenderer AlphaRenderer { get; private set; }

        public VertexBuffer VertexBuffer { get; private set; }
        public IndexBuffer IndexBuffer { get; private set; }

        public M2File Model { get; private set; }

        public Dictionary<int, M2RenderInstance> FullInstances { get; private set; }
        public List<M2RenderInstance> VisibleInstances { get; private set; }

        private bool mIsSyncLoaded;
        private bool mIsSyncLoadRequested;
        private bool mSkipRendering;

        public M2Renderer(M2File model)
        {
            Model = model;
            FullInstances = new Dictionary<int, M2RenderInstance>();
            VisibleInstances = new List<M2RenderInstance>();
            BatchRenderer = new M2BatchRenderer(model);
            AlphaRenderer = new M2AlphaRenderer(model);
            PortraitRenderer = new M2PortraitRenderer(model);
        }

        public void RenderBatch()
        {
            if (mIsSyncLoaded == false)
            {
                if (!BeginSyncLoad())
                    return;
            }

            if (!mSkipRendering)
                BatchRenderer.OnFrame(this);
        }

        public void RenderPortrait()
        {
            if (mIsSyncLoaded == false)
            {
                if (!BeginSyncLoad())
                    return;
            }

            if (!mSkipRendering)
                PortraitRenderer.OnFrame(this);
        }

        public void RenderAlphaInstance(M2RenderInstance instance)
        {
            if (mIsSyncLoaded == false)
            {
                if (!BeginSyncLoad())
                    return;
            }

            if (!mSkipRendering)
                AlphaRenderer.OnFrame(this, instance);
        }

        public bool RemoveInstance(int uuid)
        {
            bool lastInstance = false;
            lock (FullInstances)
            {
                M2RenderInstance inst;
                if (FullInstances.TryGetValue(uuid, out inst) == false)
                    return false;

                --inst.NumReferences;
                if (inst.NumReferences > 0)
                    return false;

                FullInstances.Remove(uuid);
                if (FullInstances.Count == 0)
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
            if (FullInstances.TryGetValue(uuid, out inst))
            {
                ++inst.NumReferences;
                return inst;
            }

            var instance = new M2RenderInstance(uuid, position, rotation, scaling, this);
            lock (FullInstances)
            {
                FullInstances.Add(uuid, instance);
                if (!WorldFrame.Instance.ActiveCamera.Contains(ref instance.BoundingBox))
                    return instance;

                lock (VisibleInstances)
                    VisibleInstances.Add(instance);
                return instance;
            }
        }

        public void PushMapReference(M2Instance instance)
        {
            M2RenderInstance inst;
            lock (FullInstances)
            {
                if (FullInstances.TryGetValue(instance.Uuid, out inst) == false)
                    return;
            }

            inst.IsUpdated = true;
            inst.UpdateDepth();

            lock (VisibleInstances)
                VisibleInstances.Add(inst);
        }

        public void ViewChanged()
        {
            lock (VisibleInstances)
                VisibleInstances.Clear();

            lock (FullInstances)
            {
                foreach (var pair in FullInstances)
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

            BatchRenderer.OnSyncLoad();
            AlphaRenderer.OnSyncLoad();
            PortraitRenderer.OnSyncLoad();
        }

        public virtual void Dispose()
        {
            mSkipRendering = true;
            if (BatchRenderer != null)
                BatchRenderer.Dispose();

            if (PortraitRenderer != null)
                PortraitRenderer.Dispose();

            var vb = VertexBuffer;
            var ib = IndexBuffer;

            WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
            {
                if (vb != null)
                    vb.Dispose();
                if (ib != null)
                    ib.Dispose();
            });
        }
    }
}
