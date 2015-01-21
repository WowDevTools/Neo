using System;
using System.Linq;
using SharpDX;

namespace WoWEditor6.Utils
{
    class Polygon
    {
        private Vector2[] mPoints = new Vector2[0];

        private static sbyte CrossTest(ref Vector2 a, ref Vector2 b, ref Vector2 c)
        {
            var vb = b;
            var vc = c;
            var va = a;
            if(Math.Abs(va.Y - vb.Y) < 1e-5 && Math.Abs(va.Y - vc.Y) < 1e-5)
            {
                if ((vb.X <= va.X && va.X <= vc.Y) || (vc.X <= va.X && va.X <= vb.X))
                    return 0;
                return 1;
            }

            if(vb.Y > c.Y)
            {
                var tmp = vb;
                vb = vc;
                vc = tmp;
            }

            if (Math.Abs(va.Y - vb.Y) < 1e-5 && Math.Abs(va.X - vb.X) < 1e-5)
                return 0;

            if (va.Y <= vb.Y || va.Y > vc.Y)
                return 1;

            var d = (vb.X - va.X) * (vc.Y - va.Y) - (vb.Y - va.Y) * (vc.X - va.X);
            if (d > 1e-5)
                return -1;
            return (d < -1e-5) ? (sbyte)1 : (sbyte)0;
        }

        public void SetCoeffs(Vector2[] points)
        {
            mPoints = points.Length > 0 ? points.Concat(new[] {points[0]}).ToArray() : new Vector2[0];
        }

        public void AddPoint(ref Vector2 point)
        {
            mPoints = mPoints.Concat(new[] {point}).ToArray();
        }

        public bool IsInside(ref Vector2 point)
        {
            if (mPoints.Length == 0)
                return false;

            var t = -1;
            for(var i = 0; i < mPoints.Length - 1; ++i)
                t *= CrossTest(ref point, ref mPoints[i], ref mPoints[i + 1]);

            return t >= 0;
        }

        public float Distance(ref Vector2 point)
        {
            if (mPoints.Length == 0)
                return float.MaxValue;

            var dist = float.MaxValue;
            for(var i = 0; i < mPoints.Length - 1; ++i)
            {
                var p1 = mPoints[i];
                var p2 = mPoints[i + 1];
                if ((point - p1).LengthSquared() < 1e-5)
                    return 0.0f;

                if ((point - p2).LengthSquared() < 1e-5)
                    return 0.0f;

                var l2 = (p2 - p1).LengthSquared();
                if(l2 < 1e-5)
                {
                    dist = Math.Min(dist, (point - p1).Length());
                    continue;
                }

                var t = Vector2.Dot(point - p1, p2 - p1) / l2;
                if (t < 0)
                    dist = Math.Min(dist, (point - p1).Length());
                else if (t > 1.0f)
                    dist = Math.Min(dist, (point - p2).Length());
                else
                {
                    var proj = p1 + (p2 - p1) * t;
                    dist = Math.Min(dist, (point - proj).Length());
                }
            }

            return dist;

        }
    }
}
