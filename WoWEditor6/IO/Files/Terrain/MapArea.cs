using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.IO.Files.Terrain
{
    abstract class MapArea
    {
        public int IndexX { get; protected set; }
        public int IndexY { get; protected set; }
        public string Continent { get; protected set; }

        public AdtVertex[] FullVertices { get; } = new AdtVertex[145 * 256];

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public BoundingBox BoundingBox { get; protected set; }

        public abstract Graphics.Texture GetTexture(int index);
        public abstract void AsyncLoad();
        public abstract MapChunk GetChunk(int index);
    }
}
