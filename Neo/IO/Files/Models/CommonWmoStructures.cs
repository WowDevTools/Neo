using System.Runtime.InteropServices;
using SharpDX;

namespace Neo.IO.Files.Models
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
