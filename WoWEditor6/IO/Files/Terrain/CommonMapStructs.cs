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
        public uint AdditiveColor;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Modf
    {
        public readonly int Mwid;
        public readonly int UniqueId;
        public readonly Vector3 Position;
        public Vector3 Rotation;
        public readonly Vector3 BboxMax;
        public readonly Vector3 BboxMin;
        public readonly ushort Flags;
        public readonly short DoodadSet;
        public readonly short NameSet;
        private readonly short padding;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Mddf
    {
        public readonly int Mmid;
        public readonly int UniqueId;
        public Vector3 Position;
        public readonly Vector3 Rotation;
        public readonly ushort Scale;
        public readonly ushort Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Mcly
    {
        public int TextureId;
        public uint Flags;
        public int OfsMcal;
        public short EffectId;
        public short Padding;
    }

    struct LoadedModel
    {
        public readonly string FileName;
        public readonly int Uuid;

        public LoadedModel(string file, int uuid)
        {
            FileName = file;
            Uuid = uuid;
        }
    }
}
