using System.Runtime.InteropServices;
using OpenTK;

namespace Neo.IO.Files.Models.WoD
{
    [StructLayout(LayoutKind.Sequential)]
    struct CreatureDisplayInfoEntry
    {
        public readonly uint Id;
        public readonly uint ModelId;
        public readonly uint SoundId;
        public readonly uint ExtendedDisplayInfoId;
        public readonly float CreatureModelScale;
        public readonly float PlayerModelScale;
        public readonly uint CreatureModelAlpha;
        public readonly string TextureVariation1;
        public readonly string TextureVariation2;
        public readonly string TextureVariation3;
        public readonly string PortraitTextureName;
        public readonly uint BloodId;
        public readonly uint NpcSoundId;
        public readonly uint ParticleColorId;
        public readonly uint CreatureGeosetData;
        public readonly uint ObjectEffectPackageId;
        public readonly uint AnimReplacementSetID;
        public readonly uint Flags;
        public readonly uint Gender;
        public readonly uint StateSpellVisualKitID;
        public readonly uint Unknown;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct CreatureModelDataEntry
    {
        public readonly uint Id;
        public readonly int Flags;
        public readonly int Filedataid;
        public readonly int Sizeclass;
        public readonly float Modelscale;
        public readonly int Bloodid;
        public readonly int Footprinttextureid;
        public readonly float Footprinttexturelength;
        public readonly float Footprinttexturewidth;
        public readonly float Footprintparticlescale;
        public readonly int Foleymaterialid;
        public readonly int Footstepshakesize;
        public readonly int Deaththudshakesize;
        public readonly int Soundid;
        public readonly float Collisionwidth;
        public readonly float Collisionheight;
        public readonly float Mountheight;
        public readonly Vector3 Geoboxmin;
        public readonly Vector3 Geoboxmax;
        public readonly float Worldeffectscale;
        public readonly float Attachedeffectscale;
        public readonly float Missilecollisionradius;
        public readonly float Missilecollisionpush;
        public readonly float Missilecollisionraise;
        public readonly float Overridelooteffectscale;
        public readonly float Overridenamescale;
        public readonly float Overrideselectionradius;
        public readonly float Tamedpetbasescale;
        public readonly int Creaturegeosetdataid;
        public readonly float Hoverheight;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FileDataIDEntry
    {
        public readonly uint ID;
        public readonly string FileName;
        public readonly string FilePath;
    }
}
