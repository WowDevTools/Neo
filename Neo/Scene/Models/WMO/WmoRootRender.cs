using System;
using System.Collections.Generic;
using System.Linq;
using Neo.Graphics;
using Neo.IO.Files.Models;
using OpenTK.Graphics.OpenGL;
using SlimTK;

namespace Neo.Scene.Models.WMO
{
    public class WmoRootRender : IDisposable
    {
        public IWorldModelRoot Data { get; private set; }

        private bool mAsyncLoaded;
        private bool mIsSyncLoaded;
        private object mSyncLoadToken;

        private BoundingBox mBoundingBox;
        private WmoVertex[] mVertices;
        private uint[] mIndices;

        private VertexBuffer mVertexBuffer;
        private IndexBuffer mIndexBuffer;

        public BoundingBox BoundingBox { get { return this.mBoundingBox; } }

        public List<WmoGroupRender> Groups { get; private set; }

        ~WmoRootRender()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
	        this.mIndices = null;
	        this.mVertices = null;
	        this.mAsyncLoaded = false;

            var vb = this.mVertexBuffer;
            var ib = this.mIndexBuffer;
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
            });

	        this.mVertexBuffer = null;
	        this.mIndexBuffer = null;

            if (this.Groups != null)
            {
                foreach (var group in this.Groups)
                {
	                @group.Dispose();
                }

	            this.Groups.Clear();
	            this.Groups = null;
            }

            if (this.Data != null)
            {
	            this.Data.Dispose();
	            this.Data = null;
            }

            // Sync load can be called even after the object has been disposed.
            if (this.mSyncLoadToken != null)
            {
                WorldFrame.Instance.Dispatcher.Remove(this.mSyncLoadToken);
	            this.mSyncLoadToken = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void OnFrame(IEnumerable<WmoInstance> instances)
        {
	        if (this.mAsyncLoaded == false)
	        {
		        return;
	        }

	        if (this.mIndices.Length == 0 || this.mVertices.Length == 0)
	        {
		        return;
	        }

            if (this.mIsSyncLoaded == false)
            {
                if (!BeginSyncLoad())
                {
	                return;
                }
            }

            var mesh = WmoGroupRender.Mesh;
            mesh.UpdateVertexBuffer(this.mVertexBuffer);
            mesh.UpdateIndexBuffer(this.mIndexBuffer);
            mesh.Program.SetVertexUniformBuffer(1, WmoGroupRender.InstanceBuffer);

            foreach (var instance in instances)
            {
                if (WorldFrame.Instance.MapManager.IsInitialLoad == false)
                {
	                if (WorldFrame.Instance.ActiveCamera.Contains(ref instance.BoundingBox) == false)
	                {
		                continue;
	                }
                }

                WmoGroupRender.InstanceBuffer.BufferData(instance.InstanceMatrix);

                for(var i = 0; i < this.Groups.Count; ++i)
                {
	                if (WorldFrame.Instance.ActiveCamera.Contains(ref instance.GroupBoxes[i]) == false)
	                {
		                continue;
	                }

	                this.Groups[i].OnFrame();
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

        public void OnAsyncLoad(IWorldModelRoot root)
        {
	        this.mAsyncLoaded = true;

            var indices = new List<ushort>();
            var vertices = new List<WmoVertex>();

	        this.Data = root;

	        this.Groups = root.Groups.Select(group => new WmoGroupRender(group, this)).ToList();
	        this.mBoundingBox = this.Data.BoundingBox;

            foreach (var group in this.Groups)
            {
                group.BaseIndex = indices.Count;
                group.BaseVertex = vertices.Count;
                indices.AddRange(group.Data.Indices);
                vertices.AddRange(group.Data.Vertices);
            }

	        this.mVertices = vertices.ToArray();
	        this.mIndices = indices.Select(i => (uint) i).ToArray();
        }

        private void SyncLoad()
        {
	        this.mIsSyncLoaded = true;
	        this.mSyncLoadToken = null;

	        if (this.mVertices == null || this.mIndices == null || this.Groups == null)
	        {
		        return;
	        }

	        this.mVertexBuffer = new VertexBuffer();
	        this.mIndexBuffer = new IndexBuffer(DrawElementsType.UnsignedInt);

            if (this.mVertices.Length != 0 && this.mIndices.Length != 0)
            {
	            this.mVertexBuffer.BufferData(this.mVertices);
	            this.mIndexBuffer.BufferData(this.mIndices);
            }

	        foreach (var group in this.Groups)
	        {
		        group.SyncLoad();
	        }
        }
    }
}
