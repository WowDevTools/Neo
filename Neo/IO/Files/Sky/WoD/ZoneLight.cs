using System.Collections.Generic;
using System.Linq;
using SharpDX;
using WoWEditor6.Utils;

namespace WoWEditor6.IO.Files.Sky.WoD
{
    class ZoneLight
    {
        private readonly Polygon mInnerPolygon = new Polygon();
        private readonly List<ZoneLightPoint> mPoints = new List<ZoneLightPoint>();
        private DbcZoneLight mZoneLightEntry;

        public int Id { get { return mZoneLightEntry.Id; } }
        public MapLight Light { get; set; }


        public void SetDbcZoneLight(ref DbcZoneLight e) { mZoneLightEntry = e; }

        public void CreatePolygon()
        {
            if (mPoints.Count > 0)
                mPoints.Sort((p1, p2) => p1.Counter.CompareTo(p2.Counter));

            var polyPoints = mPoints.Select(p =>
            {
                var x = Metrics.MapMidPoint - p.Z;
                var y = Metrics.MapMidPoint - p.X;
                return new Vector2(x, 64.0f * Metrics.TileSize - y);
            }).ToArray();

            mInnerPolygon.SetCoeffs(polyPoints);
        }

        public void AddPolygonPoint(ref ZoneLightPoint point)
        {
            mPoints.Add(point);
        }

        public bool IsInside(ref Vector2 point)
        {
            return mInnerPolygon.IsInside(ref point);
        }

        public float GetDistance(ref Vector2 point)
        {
            return mInnerPolygon.Distance(ref point);
        }
    }
}
