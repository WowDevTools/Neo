using OpenTK;
using SlimTK;

// ReSharper disable FunctionComplexityOverflow

namespace Neo.Scene
{
	internal static class Picking
    {
        public static Ray Build(ref Vector2 screenPos, ref Matrix4 invView, ref Matrix4 invProj)
        {
            var vp = WorldFrame.Instance.GraphicsContext.Viewport;
            var sx = (((2.0f * screenPos.X) / vp.Width) - 1);
            var sy = -(((2.0f * screenPos.Y) / vp.Height) - 1);
            var screenNear = new Vector3(sx, sy, 0.0f);
            var screenFar = new Vector3(sx, sy, 1.0f);

            var matrix = invProj * invView;

            Vector3 nearPos, farPos;
            Vector3.TransformVector(ref screenNear, ref matrix, out nearPos);
            Vector3.TransformVector(ref screenFar, ref matrix, out farPos);

            var dir = farPos - nearPos;
            dir.Normalize();
            return new Ray(nearPos, dir);
        }

        public static Ray Build(ref Vector2 screenPos, ref Matrix4 invView, ref Matrix4 invProj, ref Matrix4 invWorld)
        {
            var vp = WorldFrame.Instance.GraphicsContext.Viewport;
            var sx = (((2.0f * screenPos.X) / vp.Width) - 1);
            var sy = -(((2.0f * screenPos.Y) / vp.Height) - 1);
            var screenNear = new Vector3(sx, sy, 0.0f);
            var screenFar = new Vector3(sx, sy, 1.0f);
            var matrix = invProj * invView * invWorld;
            Vector3 nearPos, farPos;
            Vector3.TransformVector(ref screenNear, ref matrix, out nearPos);
            Vector3.TransformVector(ref screenFar, ref matrix, out farPos);

            var dir = farPos - nearPos;
            dir.Normalize();
            return new Ray(nearPos, dir);
        }
    }
}
