using System.Runtime.InteropServices;
using OpenTK;

namespace Neo.IO.Files.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WmoVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public uint Color;
    }

	public class WmoBatch
    {
        public int StartIndex;
        public int NumIndices;
        public int MaterialId;
        public int BlendMode;
    }
}
