using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
        public readonly int StartIndex;
        public readonly int NumIndices;
        public readonly int MaterialInt;
        public readonly int BlendMode;
        public readonly int MaterialFlags;
    }
}
