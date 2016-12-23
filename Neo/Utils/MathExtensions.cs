using OpenTK;
using SlimTK;

namespace Neo.Utils
{
    static class MathExtensions
    {
        public static BoundingBox Transform(this BoundingBox box, ref Matrix4 matrix)
        {
            var corners = box.GetCorners();

	        var newMin = new Vector3(float.MaxValue);
            var newMax = new Vector3(float.MinValue);

            for(var i = 0; i < corners.Length; ++i)
            {
                Vector3 v;
                Vector3.TransformVector(ref corners[i], ref matrix, out v);
                TakeMin(ref newMin, ref v);
                TakeMax(ref newMax, ref v);
            }

            return new BoundingBox(newMin, newMax);
        }

        public static void TakeMin(ref Vector3 v , ref Vector3 other)
        {
            if (v.X > other.X)
            {
	            v.X = other.X;
            }
	        if (v.Y > other.Y)
	        {
		        v.Y = other.Y;
	        }
	        if (v.Z > other.Z)
	        {
		        v.Z = other.Z;
	        }
        }

        public static void TakeMax(ref Vector3 v, ref Vector3 other)
        {
            if (v.X < other.X)
            {
	            v.X = other.X;
            }
	        if (v.Y < other.Y)
	        {
		        v.Y = other.Y;
	        }
	        if (v.Z < other.Z)
	        {
		        v.Z = other.Z;
	        }
        }
    }
}
