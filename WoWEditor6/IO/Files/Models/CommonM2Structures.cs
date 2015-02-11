using System.Runtime.InteropServices;
using SharpDX;

namespace WoWEditor6.IO.Files.Models
{
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct M2Vertex
    {
        public Vector3 position;
        public fixed byte boneWeights[4];
        public fixed byte boneIndices[4];
        public Vector3 normal;
        public readonly Vector2 texCoord1;
        public readonly Vector2 texCoord2;
    }
}
