using System;
using SharpDX;

namespace WoWEditor6.Scene.Models
{
    interface IModelInstance : IDisposable
    {
        BoundingBox InstanceBoundingBox { get; }
        Vector3[] InstanceCorners { get; }

        bool IsSpecial { get; }

        void Rotate(float x, float y, float z);

        void UpdateScale(float s);

        void UpdatePosition(Vector3 position);

        Vector3 GetPosition();

        Vector3 GetNamePlatePosition();

        bool Intersects(IntersectionParams parameters, ref Ray globalRay, out float value);

        void CreateModelNameplate();

        void DestroyModelNameplate();
    }
}
