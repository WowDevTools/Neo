using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.IO.Files.Terrain.WoD
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

    [StructLayout(LayoutKind.Sequential)]
    struct MCNK
    {
        public readonly int Flags;
        public readonly int IndexX;
        public readonly int IndexY;
        public readonly int NumLayers;
        public readonly int NumDoodadRefs;
        public readonly int Mcvt;
        public readonly int Mcnr;
        public readonly int Mcly;
        public readonly int Mcrf;
        public readonly int Mcal;
        public readonly int SizeAlpha;
        public readonly int Mcsh;
        public readonly int SizeShadow;
        public readonly int AreaId;
        public readonly int NumMapObjRefs;
        public readonly int Holes;
        public readonly ulong Low1;
        public readonly ulong Low2;
        public readonly int PredTex;
        public readonly int NoEffectDoodad;
        public readonly int Mcse;
        public readonly int NumSoundEmitters;
        public readonly int Mclq;
        public readonly int SizeLiquid;
        public readonly Vector3 Position;
        public readonly int Mccv;
        public readonly int Mclv;
        public readonly int Unused;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MCLY
    {
        public readonly int TextureId;
        public readonly int Flags;
        public readonly int OfsMcal;
        public readonly int EffectId;
    }
}
