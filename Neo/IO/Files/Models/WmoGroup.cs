using System.Collections.Generic;
using Warcraft.Core;

namespace Neo.IO.Files.Models
{
    abstract class WmoGroup
    {
        public Box BoundingBox { get; protected set; }
        public IList<ushort> Indices { get; protected set; }
        public IList<WmoBatch> Batches { get; protected set; }
        public WmoVertex[] Vertices { get; protected set; }
        public bool IsIndoor { get; protected set; }
        public string Name { get; set; }
        public bool DisableRendering { get; protected set; }
    }
}
