using System;
using OpenTK;
using SlimTK;

namespace Neo.Graphics
{
    class ViewFrustum
    {
        private static readonly float[] Clip = new float[16];
        private static readonly Vector3[] BoxCorners = new Vector3[8];
        private readonly Plane[] mPlanes = new Plane[6];

        public ContainmentType Contains(ref BoundingBox box)
        {
            var result = ContainmentType.Contains;
            box.GetCorners(BoxCorners);
            // ReSharper disable TooWideLocalVariableScope
            int k, nIn, nOut;
            float distance;

            for(var i = 0; i < 6; ++i)
            {
                nIn = 0;
                nOut = 0;
                for(k = 0; k < 8 && (nIn == 0 || nOut == 0); ++k)
                {
                    Plane.DotCoordinate(ref mPlanes[i], ref BoxCorners[k], out distance);
                    if (distance < 0)
                    {
	                    ++nOut;
                    }
                    else
                    {
	                    ++nIn;
                    }
                }

                if (nIn == 0)
                {
	                return ContainmentType.Disjoint;
                }
	            if (nOut != 0)
	            {
		            result = ContainmentType.Intersects;
	            }
            }

            return result;
        }

        public ContainmentType Contains(ref BoundingSphere sphere)
        {
            float distance;

            for(var i = 0; i < 6; ++i)
            {
                Plane.DotNormal(ref mPlanes[i], ref sphere.Center, out distance);
                if (distance < sphere.Radius)
                {
	                return ContainmentType.Disjoint;
                }

	            if (Math.Abs(distance) < sphere.Radius)
	            {
		            return ContainmentType.Intersects;
	            }
            }

            return ContainmentType.Contains;
        }

        // ReSharper disable once FunctionComplexityOverflow
        public void Update(Matrix4 matView, Matrix4 matProj)
        {
            Clip[0] = matView.M11 * matProj.M11 + matView.M12 * matProj.M21 + matView.M13 * matProj.M31
                      + matView.M14 * matProj.M41;
            Clip[1] = matView.M11 * matProj.M12 + matView.M12 * matProj.M22 + matView.M13 * matProj.M32
                      + matView.M14 * matProj.M42;
            Clip[2] = matView.M11 * matProj.M13 + matView.M12 * matProj.M23 + matView.M13 * matProj.M33
                      + matView.M14 * matProj.M43;
            Clip[3] = matView.M11 * matProj.M14 + matView.M12 * matProj.M24 + matView.M13 * matProj.M34
                      + matView.M14 * matProj.M44;
            Clip[4] = matView.M21 * matProj.M11 + matView.M22 * matProj.M21 + matView.M23 * matProj.M31
                      + matView.M24 * matProj.M41;
            Clip[5] = matView.M21 * matProj.M12 + matView.M22 * matProj.M22 + matView.M23 * matProj.M32
                      + matView.M24 * matProj.M42;
            Clip[6] = matView.M21 * matProj.M13 + matView.M22 * matProj.M23 + matView.M23 * matProj.M33
                      + matView.M24 * matProj.M43;
            Clip[7] = matView.M21 * matProj.M14 + matView.M22 * matProj.M24 + matView.M23 * matProj.M34
                      + matView.M24 * matProj.M44;
            Clip[8] = matView.M31 * matProj.M11 + matView.M32 * matProj.M21 + matView.M33 * matProj.M31
                      + matView.M34 * matProj.M41;
            Clip[9] = matView.M31 * matProj.M12 + matView.M32 * matProj.M22 + matView.M33 * matProj.M32
                      + matView.M34 * matProj.M42;
            Clip[10] = matView.M31 * matProj.M13 + matView.M32 * matProj.M23 + matView.M33
                       * matProj.M33 + matView.M34 * matProj.M43;
            Clip[11] = matView.M31 * matProj.M14 + matView.M32 * matProj.M24 + matView.M33
                       * matProj.M34 + matView.M34 * matProj.M44;
            Clip[12] = matView.M41 * matProj.M11 + matView.M42 * matProj.M21 + matView.M43
                       * matProj.M31 + matView.M44 * matProj.M41;
            Clip[13] = matView.M41 * matProj.M12 + matView.M42 * matProj.M22 + matView.M43
                       * matProj.M32 + matView.M44 * matProj.M42;
            Clip[14] = matView.M41 * matProj.M13 + matView.M42 * matProj.M23 + matView.M43
                       * matProj.M33 + matView.M44 * matProj.M43;
            Clip[15] = matView.M41 * matProj.M14 + matView.M42 * matProj.M24 + matView.M43
                       * matProj.M34 + matView.M44 * matProj.M44;

            mPlanes[0] = new Plane(
                Clip[3] - Clip[0],
                Clip[7] - Clip[4],
                Clip[11] - Clip[8],
                Clip[15] - Clip[12]);

            mPlanes[1] = new Plane(
                Clip[3] + Clip[0],
                Clip[7] + Clip[4],
                Clip[11] + Clip[8],
                Clip[15] + Clip[12]);

            mPlanes[2] = new Plane(
                Clip[3] + Clip[1],
                Clip[7] + Clip[5],
                Clip[11] + Clip[9],
                Clip[15] + Clip[13]);

            mPlanes[3] = new Plane(
                Clip[3] - Clip[1],
                Clip[7] - Clip[5],
                Clip[11] - Clip[9],
                Clip[15] - Clip[13]);

            mPlanes[4] = new Plane(
                Clip[3] - Clip[2],
                Clip[7] - Clip[6],
                Clip[11] - Clip[10],
                Clip[15] - Clip[14]);

            mPlanes[5] = new Plane(
                Clip[3] + Clip[2],
                Clip[7] + Clip[6],
                Clip[11] + Clip[10],
                Clip[15] + Clip[14]);

            for (var i = 0; i < 6; ++i)
            {
	            this.mPlanes[i].Normalize();
            }
        }
    }
}
