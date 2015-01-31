using System;
using System.Collections.Generic;
using SharpDX;

namespace WoWEditor6.IO.Files.Models
{
    abstract class M2File : IDisposable
    {
        public M2Vertex[] Vertices { get; protected set; } = new M2Vertex[0];
        public List<M2RenderPass> Passes { get; } = new List<M2RenderPass>();
        public ushort[] Indices { get; protected set; } = new ushort[0];

        public BoundingBox BoundingBox { get; protected set; }
        public BoundingSphere BoundingSphere { get; protected set; }

        public abstract void Dispose();

        public abstract bool Load();
    }
}
