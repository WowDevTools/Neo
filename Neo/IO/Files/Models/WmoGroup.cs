using System.Collections.Generic;
using SlimTK;

namespace Neo.IO.Files.Models
{
    public abstract class WmoGroup
    {
        public BoundingBox BoundingBox { get; protected set; }
        public IList<ushort> Indices { get; protected set; }
        public IList<WmoBatch> Batches { get; protected set; }
        public WmoVertex[] Vertices { get; protected set; }
        public bool IsIndoor { get; protected set; }
        public string Name { get; set; }
        public bool DisableRendering { get; protected set; }
    }
}
