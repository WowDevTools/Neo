using System.Collections.Generic;
using System.Linq;
using Neo.Utils;
using OpenTK;

namespace Neo.IO.Files.Sky.WoD
{
    public class ZoneLight
    {
        private readonly Polygon mInnerPolygon = new Polygon();
        private readonly List<ZoneLightPoint> mPoints = new List<ZoneLightPoint>();
        private DbcZoneLight mZoneLightEntry;

        public int Id { get { return this.mZoneLightEntry.Id; } }
        public MapLight Light { get; set; }


        public void SetDbcZoneLight(ref DbcZoneLight e) {
	        this.mZoneLightEntry = e; }

        public void CreatePolygon()
        {
            if (this.mPoints.Count > 0)
            {
	            this.mPoints.Sort((p1, p2) => p1.Counter.CompareTo(p2.Counter));
            }

	        var polyPoints = this.mPoints.Select(p =>
            {
                var x = Metrics.MapMidPoint - p.Z;
                var y = Metrics.MapMidPoint - p.X;
                return new Vector2(x, 64.0f * Metrics.TileSize - y);
            }).ToArray();

	        this.mInnerPolygon.SetCoeffs(polyPoints);
        }

        public void AddPolygonPoint(ref ZoneLightPoint point)
        {
	        this.mPoints.Add(point);
        }

        public bool IsInside(ref Vector2 point)
        {
            return this.mInnerPolygon.IsInside(ref point);
        }

        public float GetDistance(ref Vector2 point)
        {
            return this.mInnerPolygon.Distance(ref point);
        }
    }
}
