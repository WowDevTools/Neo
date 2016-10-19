using System.Runtime.InteropServices;
using OpenTK;

namespace Neo.IO.Files.Terrain.WoD
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Mcnk
    {
        public uint Flags;
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
        public int AreaId;
        public readonly int NumMapObjRefs;
        public int Holes;
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
    public struct AreaTable
    {
        public readonly int Id;
        public readonly int Continentid;
        public readonly int Parentareaid;
        public readonly int Areabit;
        public readonly int Flags;
        public readonly int Flags2;
        public readonly int Soundproviderpref;
        public readonly int Soundproviderprefunderwater;
        public readonly int Ambienceid;
        public readonly int Zonemusic;
        public readonly string Zonename;
        public readonly int Introsound;
        public readonly int Explorationlevel;
        public readonly string Areaname_Lang;
        public readonly int Factiongroupmask;
        public readonly int Liquidtypeid1;
        public readonly int Liquidtypeid2;
        public readonly int Liquidtypeid3;
        public readonly int Liquidtypeid4;
        public readonly float Ambient_Multiplier;
        public readonly int Mountflags;
        public readonly int Uwintrosound;
        public readonly int Uwzonemusic;
        public readonly int Uwambience;
        public readonly int World_Pvp_Id;
        public readonly int Pvpcombatworldstateid;
        public readonly int Wildbattlepetlevelmin;
        public readonly int Wildbattlepetlevelmax;
        public readonly int Windsettingsid;

    }
}
