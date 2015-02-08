using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using WoWEditor6.Graphics;
using WoWEditor6.IO.Files.Models;

namespace WoWEditor6.Scene.Models.WMO
{
    class WmoRootRender : IDisposable
    {
        public WmoRoot Data { get; private set; }

        private bool mAsyncLoaded;
        private bool mSyncLoaded;
        private bool mSyncLoadRequested;
        private BoundingBox mBoundingBox;
        private WmoVertex[] mVertices;
        private uint[] mIndices;

        private VertexBuffer mVertexBuffer;
        private IndexBuffer mIndexBuffer;

        public BoundingBox BoundingBox { get { return mBoundingBox; } }

        public IList<WmoGroupRender> Groups { get; private set; }

        public void Dispose()
        {
            mIndices = null;
            mVertices = null;
            mAsyncLoaded = false;
            WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
            {
                if (mVertexBuffer != null)
                    mVertexBuffer.Dispose();
                if (mIndexBuffer != null)
                    mIndexBuffer.Dispose();
            });

            foreach (var group in Groups)
                group.Dispose();

            Groups = new List<WmoGroupRender>();
            if(Data != null)
                Data.Dispose();
        }

        public void OnFrame(IEnumerable<WmoInstance> instances)
        {
            if (mAsyncLoaded == false)
                return;

            if (mIndices.Length == 0 || mVertices.Length == 0)
                return;

            if (mSyncLoaded == false)
            {
                if (mSyncLoadRequested)
                    return;

                if (WorldFrame.Instance.MapManager.IsInitialLoad == false)
                {
                    WorldFrame.Instance.Dispatcher.BeginInvoke(SyncLoad);
                    mSyncLoadRequested = true;
                    return;
                }

                SyncLoad();
            }

            var mesh = WmoGroupRender.Mesh;
            mesh.UpdateVertexBuffer(mVertexBuffer);
            mesh.UpdateIndexBuffer(mIndexBuffer);
            mesh.Program.SetVertexConstantBuffer(1, WmoGroupRender.InstanceBuffer);

            foreach (var instance in instances)
            {
                if (WorldFrame.Instance.MapManager.IsInitialLoad == false)
                {
                    if (WorldFrame.Instance.ActiveCamera.Contains(ref instance.BoundingBox) == false)
                        continue;
                }

                WmoGroupRender.InstanceBuffer.UpdateData(instance.InstanceMatrix);

                for(var i = 0; i < Groups.Count; ++i)
                {
                    if (WorldFrame.Instance.ActiveCamera.Contains(ref instance.GroupBoxes[i]) == false)
                        continue;

                    Groups[i].OnFrame();
                }
            }
        }

        public void OnAsyncLoad(WmoRoot root)
        {
            var indices = new List<ushort>();
            var vertices = new List<WmoVertex>();

            Data = root;

            var groups = root.Groups.Select(@group => new WmoGroupRender(@group, this)).ToList();
            Groups = groups.AsReadOnly();
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

            mAsyncLoaded = true;
        }

        private void SyncLoad()
        {
            if (mVertices == null || mIndices == null || Groups == null)
                return;

            mVertexBuffer = new VertexBuffer(WorldFrame.Instance.GraphicsContext);
            mIndexBuffer = new IndexBuffer(WorldFrame.Instance.GraphicsContext)
            {
                IndexFormat = SharpDX.DXGI.Format.R32_UInt
            };

            if (mVertices.Length != 0 && mIndices.Length != 0)
            {
                mVertexBuffer.UpdateData(mVertices);
                mIndexBuffer.UpdateData(mIndices);
            }

            foreach (var group in Groups)
                group.SyncLoad();

            mSyncLoaded = true;
        }
    }
}
