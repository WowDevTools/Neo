using System.Numerics;
using System.Runtime.InteropServices;

namespace Neo.IO.Files.Terrain
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AdtVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vector2 TexCoordAlpha;
        public uint Color;
        public uint AdditiveColor;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Modf
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
    public struct Mddf
    {
        public int Mmid;
        public int UniqueId;
        public Vector3 Position;
        public Vector3 Rotation;
        public ushort Scale;
        public ushort Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Mcly
    {
        public int TextureId;
        public uint Flags;
        public int OfsMcal;
        public short EffectId;
        public short Padding;
    }

	public struct LoadedModel
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
