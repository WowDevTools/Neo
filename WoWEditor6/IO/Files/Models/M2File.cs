using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.IO.Files.Models
{
    abstract class M2File
    {
        public M2Vertex[] Vertices { get; protected set; } = new M2Vertex[0];
        public List<M2RenderPass> Passes { get; } = new List<M2RenderPass>();
        public ushort[] Indices { get; protected set; } = new ushort[0];

        public BoundingBox BoundingBox { get; protected set; }

        public abstract bool Load();
    }
}
