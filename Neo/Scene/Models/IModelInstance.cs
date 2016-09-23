using System;
using OpenTK;
using SlimTK;

namespace Neo.Scene.Models
{
    interface IModelInstance : IDisposable
    {
	    BoundingBox InstanceBoundingBox { get; }
        Vector3[] InstanceCorners { get; }

        bool IsSpecial { get; }

        void Rotate(float x, float y, float z);

        Vector3 GetRotation();

        string GetModelName();

        void UpdateScale(float s);

        void SetPosition(Vector3 position);

        Vector3 GetPosition();

        void Remove();

        Vector3 GetNamePlatePosition();

        bool Intersects(IntersectionParams parameters, ref Ray globalRay, out float value);

        void CreateModelNameplate();

        void DestroyModelNameplate();
    }
}
