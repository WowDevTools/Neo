using System;
using System.Runtime.InteropServices;

namespace Neo.IO.Files.Models.Wotlk
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CreatureDisplayInfoEntry
    {
        public readonly uint Id;
        public readonly uint ModelId;
        public readonly uint SoundId;
        public readonly uint ExtendedDisplayInfoId;
        public readonly float CreatureModelScale;
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
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CreatureDisplayInfoExtraEntry
    {
        public readonly uint ID;
        public readonly int DisplayRaceID;
        public readonly int DisplaySexID;
        public readonly int SkinID;
        public readonly int FaceID;
        public readonly int HairStyleID;
        public readonly int HairColorID;
        public readonly int FacialHairID;
        public readonly int Helm;
        public readonly int Shoulder;
        public readonly int Shirt;
        public readonly int Cuirass;
        public readonly int Belt;
        public readonly int Legs;
        public readonly int Boots;
        public readonly int Wrist;
        public readonly int Gloves;
        public readonly int Tabard;
        public readonly int Cape;
        public readonly int CanEquip;
        public readonly string Texture;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CreatureModelDataEntry
    {
        public readonly uint ID;
        public readonly uint Flags;
        public readonly string ModelPath;
        public readonly uint sizeClass;
        public readonly float modelScale;
        public readonly uint BloodLevel;
        public readonly uint Footprint;
        public readonly float footprintTextureLength;
        public readonly float footprintTextureWidth;
        public readonly float footprintParticleScale;
        public readonly uint foleyMaterialId;
        public readonly uint footstepShakeSize;
        public readonly uint deathThudShakeSize;
        public readonly uint SoundData;
        public readonly float CollisionWidth;
        public readonly float CollisionHeight;
        public readonly float mountHeight;
        public readonly float geoBoxMin1;
        public readonly float geoBoxMin2;
        public readonly float geoBoxMin3;
        public readonly float geoBoxMax1;
        public readonly float geoBoxMax2;
        public readonly float geoBoxMax3;
        public readonly float worldEffectScale;
        public readonly float attachedEffectScale;
        public readonly float Unknown5;
        public readonly float Unknown6;
    }
}
