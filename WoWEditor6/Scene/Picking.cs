using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.Scene
{
    static class Picking
    {
        public static Ray Build(ref Vector2 screenPos, ref Matrix invView, ref Matrix invProj, ref Matrix invWorld)
        {
            var screenNear = new Vector3(screenPos, 0.0f);
            var screenFar = new Vector3(screenPos, 1.0f);
            var matrix = invProj * invView * invWorld;
            Vector3 nearPos, farPos;
            Vector3.TransformCoordinate(ref screenNear, ref matrix, out nearPos);
            Vector3.TransformCoordinate(ref screenFar, ref matrix, out farPos);

            var dir = farPos - nearPos;
            dir.Normalize();
            return new Ray(nearPos, dir);
        }
    }
}
