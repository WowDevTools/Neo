using System.Runtime.InteropServices;
using OpenTK;

namespace Neo.IO.Files.Models
{
	internal interface IInterpolator<TSource, out TDest>
    {
        TDest Interpolate(float fac, ref TSource v1, ref TSource v2);
        TDest Interpolate(ref TSource v1);
    }

	internal class VectorInterpolator : IInterpolator<Vector3, Vector3>, IInterpolator<Vector4, Vector4>, IInterpolator<Vector2, Vector2>
    {
        public Vector3 Interpolate(float fac, ref Vector3 v1, ref Vector3 v2)
        {
            return v1 * (1.0f - fac) + fac * v2;
        }

        public Vector4 Interpolate(float fac, ref Vector4 v1, ref Vector4 v2)
        {
            return v1 * (1.0f - fac) + fac * v2;
        }

        public Vector2 Interpolate(float fac, ref Vector2 v1, ref Vector2 v2)
        {
            return v1 * (1.0f - fac) + fac * v2;
        }

        public Vector3 Interpolate(ref Vector3 v1)
        {
            return v1;
        }

        public Vector4 Interpolate(ref Vector4 v1)
        {
            return v1;
        }

        public Vector2 Interpolate(ref Vector2 v1)
        {
            return v1;
        }
    }

	internal class QuaternionInterpolator : IInterpolator<Quaternion16, Quaternion>, IInterpolator<InvQuaternion16, Quaternion>
    {
        public Quaternion Interpolate(float fac, ref Quaternion16 v1, ref Quaternion16 v2)
        {
            var q1 = v1.ToQuaternion();
            var q2 = v2.ToQuaternion();
            Quaternion ret = Quaternion.Slerp(q1, q2, fac);
            return ret;
        }

        public Quaternion Interpolate(ref Quaternion16 v1)
        {
            return v1.ToQuaternion();
        }

        public Quaternion Interpolate(float fac, ref InvQuaternion16 v1, ref InvQuaternion16 v2)
        {
            var q1 = v1.ToQuaternion();
            var q2 = v2.ToQuaternion();
	        Quaternion ret = Quaternion.Slerp(q1, q2, fac);
	        return ret;
        }

        public Quaternion Interpolate(ref InvQuaternion16 v1)
        {
            return v1.ToQuaternion();
        }
    }

	internal class NoInterpolateAlpha16 : IInterpolator<short, float>
    {
        public float Interpolate(float fac, ref short v1, ref short v2)
        {
            return v1 / 32768.0f;
        }

        public float Interpolate(ref short v1)
        {
            return v1 / 32768.0f;
        }
    }

	internal class InterpolateAlpha16 : IInterpolator<short, float>
    {
        public float Interpolate(float fac, ref short v1, ref short v2)
        {
            return ((1.0f - fac) * v1 + fac * v2) / 32768.0f;
        }

        public float Interpolate(ref short v1)
        {
            return v1 / 32768.0f;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Quaternion16
    {
        private readonly short x;
        private readonly short y;
        private readonly short z;
        private readonly short w;

        public Quaternion ToQuaternion()
        {
            return new Quaternion(
                    (x < 0 ? x + 32768 : x - 32767) / 32767.0f,
                    (y < 0 ? y + 32768 : y - 32767) / 32767.0f,
                    (z < 0 ? z + 32768 : z - 32767) / 32767.0f,
                    (w < 0 ? w + 32768 : w - 32767) / 32767.0f);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct InvQuaternion16
    {
        private readonly short x;
        private readonly short y;
        private readonly short z;
        private readonly short w;

        public Quaternion ToQuaternion()
        {
            return new Quaternion(
                    (x < 0 ? x + 32768 : x - 32767) / -32767.0f,
                    (y < 0 ? y + 32768 : y - 32767) / -32767.0f,
                    (z < 0 ? z + 32768 : z - 32767) / -32767.0f,
                    (w < 0 ? w + 32768 : w - 32767) / 32767.0f);
        }
    }
}
