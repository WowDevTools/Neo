using System.Collections.Generic;
using System.Linq;
using SharpDX;
using WoWEditor6.Graphics;
using WoWEditor6.IO.Files.Models;

namespace WoWEditor6.Scene.Models.WMO
{
    class WmoRootRender
    {
        public WmoRoot Data { get; private set; }

        private bool mAsyncLoaded;
        private bool mSyncLoaded;
        private bool mSyncLoadRequested;
        private BoundingBox mBoundingBox;
        private WmoVertex[] mVertices;
        private ushort[] mIndices;

        private VertexBuffer mVertexBuffer;
        private IndexBuffer mIndexBuffer;

        public BoundingBox BoundingBox => mBoundingBox;
        public IList<WmoGroupRender> Groups { get; private set; }

        public void OnFrame(List<WmoInstance> instances)
        {
            if (mAsyncLoaded == false)
                return;

            if(WorldFrame.Instance.MapManager.IsInitialLoad == false)
            {
                if (WorldFrame.Instance.ActiveCamera.Contains(ref mBoundingBox) == false)
                    return;
            }

            if(mSyncLoaded == false)
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
            mesh.Program.SetVertexConstantBuffer(2, WmoGroupRender.InstanceBuffer);

            foreach (var instance in instances)
            {
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
            Data = root;
            var groups = root.Groups.Select(@group => new WmoGroupRender(@group, this)).ToList();
            Groups = groups.AsReadOnly();
            mBoundingBox = Data.BoundingBox;

            var indices = new List<ushort>();
            var vertices = new List<WmoVertex>();
            foreach(var group in Groups)
            {
                group.BaseIndex = indices.Count;
                group.BaseVertex = vertices.Count;
                indices.AddRange(group.Data.Indices);
                vertices.AddRange(group.Data.Vertices);
            }

            mVertices = vertices.ToArray();
            mIndices = indices.ToArray();

            mAsyncLoaded = true;
        }

        private void SyncLoad()
        {
            mVertexBuffer = new VertexBuffer(WorldFrame.Instance.GraphicsContext);
            mIndexBuffer = new IndexBuffer(WorldFrame.Instance.GraphicsContext);
            mVertexBuffer.UpdateData(mVertices);
            mIndexBuffer.UpdateData(mIndices);

            foreach (var group in Groups)
                group.SyncLoad();

            mSyncLoaded = true;
        }
    }
}
