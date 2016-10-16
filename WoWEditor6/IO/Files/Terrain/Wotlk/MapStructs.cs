using System.Runtime.InteropServices;
using SharpDX;

namespace WoWEditor6.IO.Files.Terrain.Wotlk
{
    [StructLayout(LayoutKind.Sequential)]
    struct Mhdr
    {
        public uint Flags;
        public int ofsMcin;
        public int ofsMtex;
        public int ofsMmdx;
        public int ofsMmid;
        public int ofsMwmo;
        public int ofsMwid;
        public int ofsMddf;
        public int ofsModf;
        public int ofsMfbo;
        public int ofsMh2o;
        public int ofsMtxf;
        public int unused0, unused1, unused2, unused3;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Mcin
    {
        public int OfsMcnk;
        public int SizeMcnk;
        public int Flags;
        public int AsyncId;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Mcnk
    {
        public uint Flags;
        public readonly int IndexX;
        public readonly int IndexY;
        public int NumLayers;
        public int NumDoodadRefs;
        public int Mcvt;
        public int Mcnr;
        public int Mcly;
        public int Mcrf;
        public int Mcal;
        public int SizeAlpha;
        public int Mcsh;
        public int SizeShadow;
        public int AreaId;
        public readonly int NumMapObjRefs;
        public int Holes;
        public readonly ulong Low1;
        public readonly ulong Low2;
        public readonly int PredTex;
        public readonly int NoEffectDoodad;
        public int Mcse;
        public int NumSoundEmitters;
        public int Mclq;
        public int SizeLiquid;
        public Vector3 Position;
        public int Mccv;
        public int Mclv;
        public readonly int Unused;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AreaTable
    {
        public readonly int Id;
        public readonly int Continentid;
        public readonly int Parentareaid;
        public readonly int Areabit;
        public readonly int Flags;
        public readonly int Soundproviderpref;
        public readonly int Soundproviderprefunderwater;
        public readonly int Ambienceid;
        public readonly int Zonemusic;
        public readonly int Introsound;
        public readonly int Explorationlevel;
        public readonly LocalisedString Areaname_Lang;
        public readonly int Factiongroupmask;
        public readonly int Liquidtypeid1;
        public readonly int Liquidtypeid2;
        public readonly int Liquidtypeid3;
        public readonly int Liquidtypeid4;
        public readonly float Minelevation;
        public readonly float Ambient_Multiplier;
        public readonly int Lightid;
    }
}
