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
        public WmoRoot Data { get; private set; }

        private bool mAsyncLoaded;
        private bool mIsSyncLoaded;
        private object mSyncLoadToken;

        private BoundingBox mBoundingBox;
        private WmoVertex[] mVertices;
        private uint[] mIndices;

        private VertexBuffer mVertexBuffer;
        private IndexBuffer mIndexBuffer;

        public BoundingBox BoundingBox { get { return mBoundingBox; } }

        public List<WmoGroupRender> Groups { get; private set; }

        ~WmoRootRender()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            mIndices = null;
            mVertices = null;
            mAsyncLoaded = false;

            var vb = mVertexBuffer;
            var ib = mIndexBuffer;
            WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
            {
                if (vb != null)
                    vb.Dispose();
                if (ib != null)
                    ib.Dispose();
            });

            mVertexBuffer = null;
            mIndexBuffer = null;

            if (Groups != null)
            {
                foreach (var group in Groups)
                    group.Dispose();

                Groups.Clear();
                Groups = null;
            }

            if (Data != null)
            {
                Data.Dispose();
                Data = null;
            }

            // Sync load can be called even after the object has been disposed.
            if (mSyncLoadToken != null)
            {
                WorldFrame.Instance.Dispatcher.Remove(mSyncLoadToken);
                mSyncLoadToken = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void OnFrame(IEnumerable<WmoInstance> instances)
        {
	        if (mAsyncLoaded == false)
	        {
		        return;
	        }

	        if (mIndices.Length == 0 || mVertices.Length == 0)
	        {
		        return;
	        }

            if (mIsSyncLoaded == false)
            {
                if (!BeginSyncLoad())
                    return;
            }

            var mesh = WmoGroupRender.Mesh;
            mesh.UpdateVertexBuffer(mVertexBuffer);
            mesh.UpdateIndexBuffer(mIndexBuffer);
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

                for(var i = 0; i < Groups.Count; ++i)
                {
	                if (WorldFrame.Instance.ActiveCamera.Contains(ref instance.GroupBoxes[i]) == false)
	                {
		                continue;
	                }

                    Groups[i].OnFrame();
                }
            }
        }

        private bool BeginSyncLoad()
        {
	        if (mSyncLoadToken != null)
	        {
		        return false;
	        }

            if (WorldFrame.Instance.MapManager.IsInitialLoad)
            {
                SyncLoad();
                return true;
            }

            mSyncLoadToken = WorldFrame.Instance.Dispatcher.BeginInvoke(SyncLoad);
            return false;
        }

        public void OnAsyncLoad(WmoRoot root)
        {
            mAsyncLoaded = true;

            var indices = new List<ushort>();
            var vertices = new List<WmoVertex>();

            Data = root;

            Groups = root.Groups.Select(group => new WmoGroupRender(group, this)).ToList();
            mBoundingBox = Data.BoundingBox;

            foreach (var group in Groups)
            {
                group.BaseIndex = indices.Count;
                group.BaseVertex = vertices.Count;
                indices.AddRange(group.Data.Indices);
                vertices.AddRange(group.Data.Vertices);
            }

            mVertices = vertices.ToArray();
            mIndices = indices.Select(i => (uint) i).ToArray();
        }

        private void SyncLoad()
        {
            mIsSyncLoaded = true;
            mSyncLoadToken = null;

	        if (mVertices == null || mIndices == null || Groups == null)
	        {
		        return;
	        }

            mVertexBuffer = new VertexBuffer();
	        mIndexBuffer = new IndexBuffer(DrawElementsType.UnsignedInt);

            if (mVertices.Length != 0 && mIndices.Length != 0)
            {
                mVertexBuffer.BufferData(mVertices);
                mIndexBuffer.BufferData(mIndices);
            }

	        foreach (var group in Groups)
	        {
		        group.SyncLoad();
	        }
        }
    }
}
