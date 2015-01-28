using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using WoWEditor6.IO.Files.Models;
using WoWEditor6.IO.Files.Terrain;
using WoWEditor6.Scene.Models.M2;
using WoWEditor6.Scene.Models.WMO;

namespace WoWEditor6.Scene
{
    class IntersectionParams
    {
        public M2File M2Model { get; set; }
        public M2RenderInstance M2Instance { get; set; }
        public WmoGroup WmoModel { get; set; }
        public WmoInstance WmoInstance { get; set; }
        public MapChunk ChunkHit { get; set; }
        public Vector3 TerrainPosition { get; set; }
        public Vector3 M2Position { get; set; }
        public Vector3 WmoPosition { get; set; }

        public bool WmoHit { get; set; }
        public bool M2Hit { get; set; }
        public bool TerrainHit { get; set; }
    }
}
