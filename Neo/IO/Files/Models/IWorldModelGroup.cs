using System.Collections.Generic;
using SlimTK;

namespace Neo.IO.Files.Models
{
    public interface IWorldModelGroup
    {
        BoundingBox BoundingBox { get; }
        IList<ushort> Indices { get; }
        IList<WmoBatch> Batches { get; }
        WmoVertex[] Vertices { get; }
        bool IsIndoor { get; }
        string Name { get; }
        bool DisableRendering { get; }
    }
}
