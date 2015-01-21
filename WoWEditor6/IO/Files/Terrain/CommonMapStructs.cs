using System.Runtime.InteropServices;
using SharpDX;

namespace WoWEditor6.IO.Files.Terrain
{
    [StructLayout(LayoutKind.Sequential)]
    struct AdtVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vector2 TexCoordAlpha;
        public uint Color;
    }
}
