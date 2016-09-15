using Neo.IO.Files.Models;
using Neo.IO.Files.Terrain;
using Neo.Scene.Models.M2;
using Neo.Scene.Models.WMO;
using OpenTK;

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

        public Matrix4 InverseView;
        public Matrix4 InverseProjection;
        public Vector2 ScreenPosition;

        public IntersectionParams(Matrix4 inverseView, Matrix4 inverseProjection, Vector2 screenPosition)
        {
            InverseView = inverseView;
            InverseProjection = inverseProjection;
            ScreenPosition = screenPosition;
        }
    }
}
