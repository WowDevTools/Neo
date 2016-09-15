using System;
using OpenTK;
using Warcraft.Core;

namespace Neo.Graphics
{
    class ViewFrustum
    {
        private static readonly float[] Clip = new float[16];
        private static readonly Vector3[] BoxCorners = new Vector3[8];
        private readonly Plane[] mPlanes = new Plane[6];

        public ContainmentType Contains(ref Box box)
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
                        ++nOut;
                    else
                        ++nIn;
                }

                if (nIn == 0)
                    return ContainmentType.Disjoint;
                if (nOut != 0)
                    result = ContainmentType.Intersects;
            }

            return result;
        }

        public ContainmentType Contains(ref Sphere sphere)
        {
            float distance;

            for(var i = 0; i < 6; ++i)
            {
                Plane.DotNormal(ref mPlanes[i], ref sphere.Center, out distance);
                if (distance < sphere.Radius)
                    return ContainmentType.Disjoint;

                if (Math.Abs(distance) < sphere.Radius)
                    return ContainmentType.Intersects;
            }

            return ContainmentType.Contains;
        }

        // ReSharper disable once FunctionComplexityOverflow
        public void Update(Matrix4 matView, Matrix4 matProj)
        {
            Clip[0] = matView[0] * matProj[0] + matView[1] * matProj[4] + matView[2] * matProj[8]
                      + matView[3] * matProj[12];
            Clip[1] = matView[0] * matProj[1] + matView[1] * matProj[5] + matView[2] * matProj[9]
                      + matView[3] * matProj[13];
            Clip[2] = matView[0] * matProj[2] + matView[1] * matProj[6] + matView[2] * matProj[10]
                      + matView[3] * matProj[14];
            Clip[3] = matView[0] * matProj[3] + matView[1] * matProj[7] + matView[2] * matProj[11]
                      + matView[3] * matProj[15];
            Clip[4] = matView[4] * matProj[0] + matView[5] * matProj[4] + matView[6] * matProj[8]
                      + matView[7] * matProj[12];
            Clip[5] = matView[4] * matProj[1] + matView[5] * matProj[5] + matView[6] * matProj[9]
                      + matView[7] * matProj[13];
            Clip[6] = matView[4] * matProj[2] + matView[5] * matProj[6] + matView[6] * matProj[10]
                      + matView[7] * matProj[14];
            Clip[7] = matView[4] * matProj[3] + matView[5] * matProj[7] + matView[6] * matProj[11]
                      + matView[7] * matProj[15];
            Clip[8] = matView[8] * matProj[0] + matView[9] * matProj[4] + matView[10] * matProj[8]
                      + matView[11] * matProj[12];
            Clip[9] = matView[8] * matProj[1] + matView[9] * matProj[5] + matView[10] * matProj[9]
                      + matView[11] * matProj[13];
            Clip[10] = matView[8] * matProj[2] + matView[9] * matProj[6] + matView[10]
                       * matProj[10] + matView[11] * matProj[14];
            Clip[11] = matView[8] * matProj[3] + matView[9] * matProj[7] + matView[10]
                       * matProj[11] + matView[11] * matProj[15];
            Clip[12] = matView[12] * matProj[0] + matView[13] * matProj[4] + matView[14]
                       * matProj[8] + matView[15] * matProj[12];
            Clip[13] = matView[12] * matProj[1] + matView[13] * matProj[5] + matView[14]
                       * matProj[9] + matView[15] * matProj[13];
            Clip[14] = matView[12] * matProj[2] + matView[13] * matProj[6] + matView[14]
                       * matProj[10] + matView[15] * matProj[14];
            Clip[15] = matView[12] * matProj[3] + matView[13] * matProj[7] + matView[14]
                       * matProj[11] + matView[15] * matProj[15];

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
                mPlanes[i].Normalize();
        }
    }
}
