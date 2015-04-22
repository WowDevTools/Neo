using System;
using SharpDX;

namespace WoWEditor6.Scene.Models
{
    interface IModelInstance : IDisposable
    {
        BoundingBox InstanceBoundingBox { get; }
        Vector3[] InstanceCorners { get; }

        bool IsSpecial { get; }

        bool Intersects(IntersectionParams parameters, ref Ray globalRay, out float value);

        void CreateModelNameplate();

        void DestroyModelNameplate();
    }
}
