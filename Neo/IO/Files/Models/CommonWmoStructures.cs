using System.Runtime.InteropServices;
using SharpDX;

namespace WoWEditor6.IO.Files.Models
{
    [StructLayout(LayoutKind.Sequential)]
    struct WmoVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public uint Color;
    }

    class WmoBatch
    {
        public int StartIndex;
        public int NumIndices;
        public int MaterialId;
        public int BlendMode;
    }
}
