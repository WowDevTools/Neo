using System.Security.RightsManagement;
using SharpDX;
using Neo.IO.Files.Models;
using Neo.IO.Files.Terrain;
using Neo.Scene.Models.M2;
using Neo.Scene.Models.WMO;

namespace Neo.Scene
{
    class IntersectionParams
    {
        public M2File M2Model { get; set; }
        public M2RenderInstance M2Instance { get; set; }
        public WmoRoot WmoModel { get; set; }
        public WmoInstance WmoInstance { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public MapChunk ChunkHit { get; set; }
        public Vector3 TerrainPosition { get; set; }
        public Vector3 M2Position { get; set; }
        public Vector3 WmoPosition { get; set; }

        public float TerrainDistance { get; set; }
        public float M2Distance { get; set; }
        public float WmoDistance { get; set; }

        public bool WmoHit { get; set; }
        public bool M2Hit { get; set; }
        public bool TerrainHit { get; set; }

        public Matrix InverseView;
        public Matrix InverseProjection;
        public Vector2 ScreenPosition;

        public IntersectionParams(Matrix inverseView, Matrix inverseProjection, Vector2 screenPosition)
        {
            InverseView = inverseView;
            InverseProjection = inverseProjection;
            ScreenPosition = screenPosition;
        }
    }
}
