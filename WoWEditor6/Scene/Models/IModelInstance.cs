using System;
using SharpDX;

namespace WoWEditor6.Scene.Models
{
    interface IModelInstance : IDisposable
    {
        bool Intersects(IntersectionParams parameters, ref Ray globalRay, out float value);

        void CreateModelNameplate();

        void DestroyModelNameplate();
    }
}
