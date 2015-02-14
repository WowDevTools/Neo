using System.Collections.Generic;
using SharpDX;

namespace WoWEditor6.IO.Files.Models
{
    abstract class M2File
    {
        public M2Vertex[] Vertices { get; protected set; }
        public List<M2RenderPass> Passes { get; private set; }
        public ushort[] Indices { get; protected set; }

        public BoundingBox BoundingBox { get; protected set; }
        public BoundingSphere BoundingSphere { get; protected set; }
        public bool HasBlendPass { get; protected set; }

        protected M2File()
        {
            Vertices = new M2Vertex[0];
            Passes = new List<M2RenderPass>();
            Indices = new ushort[0];
            HasBlendPass = false;
        }

        public abstract bool Load();
    }
}
