using System.Runtime.InteropServices;
using SharpDX;

namespace WoWEditor6.IO.Files.Terrain.Wotlk
{
    [StructLayout(LayoutKind.Sequential)]
    struct Mcnk
    {
        public int Flags;
        public readonly int IndexX;
        public readonly int IndexY;
        public readonly int NumLayers;
        public readonly int NumDoodadRefs;
        public int Mcvt;
        public int Mcnr;
        public int Mcly;
        public int Mcrf;
        public int Mcal;
        public readonly int SizeAlpha;
        public int Mcsh;
        public readonly int SizeShadow;
        public readonly int AreaId;
        public readonly int NumMapObjRefs;
        public readonly int Holes;
        public readonly ulong Low1;
        public readonly ulong Low2;
        public readonly int PredTex;
        public readonly int NoEffectDoodad;
        public int Mcse;
        public readonly int NumSoundEmitters;
        public int Mclq;
        public readonly int SizeLiquid;
        public Vector3 Position;
        public int Mccv;
        public int Mclv;
        public readonly int Unused;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Mcly
    {
        public readonly int TextureId;
        public readonly int Flags;
        public readonly int OfsMcal;
        public readonly short EffectId;
        public readonly short Padding;
    }
}
