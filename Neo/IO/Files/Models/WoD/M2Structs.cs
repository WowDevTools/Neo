using System.Runtime.InteropServices;
using OpenTK;

#pragma warning disable 649

namespace Neo.IO.Files.Models.WoD
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct M2Header
    {
        public readonly int Magic;
        public readonly int Version;
        public readonly int LenName;
        public readonly int OfsName;
        public readonly int GlobalFlags;
        public readonly int NGlobalSequences;
        public readonly int OfsGlobalSequences;
        public readonly int NAnimations;
        public readonly int OfsAnimations;
        public readonly int NAnimLookup;
        public readonly int OfsAnimLookup;
        public readonly int NBones;
        public readonly int OfsBones;
        public readonly int NKeyBoneLookup;
        public readonly int OfsKeyBoneLookup;
        public readonly int NVertices;
        public readonly int OfsVertices;
        public readonly int NViews;
        public readonly int NSubmeshAnimations;
        public readonly int OfsSubmeshAnimations;
        public readonly int NTextures;
        public readonly int OfsTextures;
        public readonly int NTransparencies;
        public readonly int OfsTransparencies;
        public readonly int NUvAnimation;
        public readonly int OfsUvAnimation;
        public readonly int NTexReplace;
        public readonly int OfsTexReplace;
        public readonly int NRenderFlags;
        public readonly int OfsRenderFlags;
        public readonly int NBoneLookupTable;
        public readonly int OfsBoneLookupTable;
        public readonly int NTexLookup;
        public readonly int OfsTexLookup;
        public readonly int NTexUnits;
        public readonly int OfsTexUnits;
        public readonly int NTransLookup;
        public readonly int OfsTransLookup;
        public readonly int NUvAnimLookup;
        public readonly int OfsUvAnimLookup;
        public readonly Vector3 VertexBoxMin;
        public readonly Vector3 VertexBoxMax;
        public readonly float VertexRadius;
        public readonly Vector3 BoundingBoxMin;
        public readonly Vector3 BoundingBoxMax;
        public readonly float BoundingRadius;
        public readonly int NBoundingTriangles;
        public readonly int OfsBoundingTriangles;
        public readonly int NBoundingVertices;
        public readonly int OfsBoundingVertices;
        public readonly int NBoundingNormals;
        public readonly int OfsBoundingNormals;
        public readonly int NAttachments;
        public readonly int OfsAttachments;
        public readonly int NAttachLookup;
        public readonly int OfsAttachLookup;
        public readonly int NEvents;
        public readonly int OfsEvents;
        public readonly int NLights;
        public readonly int OfsLights;
        public readonly int NCameras;
        public readonly int OfsCameras;
        public readonly int NCameraLookup;
        public readonly int OfsCameraLookup;
        public readonly int NRibbonEmitters;
        public readonly int OfsRibbonEmitters;
        public readonly int NParticleEmitters;
        public readonly int OfsParticleEmitters;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct AnimationEntry
    {
        public readonly ushort animationID;
        public readonly ushort animationSubID;
        public readonly uint length;
        public readonly float movingSpeed;
        public readonly uint flags;
        public readonly short probability;
        private readonly short unk1;
        private readonly uint unk2, unk3;
        public readonly uint playbackSpeed;
        public readonly Vector3 minExtent;
        public readonly Vector3 maxExtent;
        public readonly float boundingRadius;
        public readonly short nextAnimation;
        public readonly ushort index;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AnimationBlock
    {
        public readonly ushort interpolation;
        public readonly short globalSequence;
        public readonly int numTimeStamps;
        public readonly int ofsTimeStamps;
        public readonly int numValues;
        public readonly int ofsValues;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ShortAnimationBlock
    {
        public readonly int numTimeStamps;
        public readonly int ofsTimeStamps;
        public readonly int numValues;
        public readonly int ofsValues;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct M2Bone
    {
        public readonly int keyBoneId;
        public readonly uint flags;
        public readonly short parentBone;
        public readonly ushort submeshid;
        private readonly ushort unk1, unk2;
        public readonly AnimationBlock translation;
        public readonly AnimationBlock rotation;
        public readonly AnimationBlock scaling;
        public readonly Vector3 pivot;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct M2Texture
    {
        public readonly int type;
        public readonly uint flags;
        public readonly int lenName;
        public readonly int ofsName;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct M2TexAnim
    {
        public readonly AnimationBlock translation;
        public readonly AnimationBlock rotation;
        public readonly AnimationBlock scaling;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct M2Skin
    {
        public readonly uint id;
        public readonly int nIndices;
        public readonly int ofsIndices;
        public readonly int nTriangles;
        public readonly int ofsTriangles;
        public readonly int nProperties;
        public readonly int ofsProperties;
        public readonly int nSubmeshes;
        public readonly int ofsSubmeshes;
        public readonly int nTexUnits;
        public readonly int ofsTexUnits;
        public readonly int bones;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct M2SubMesh
    {
        public readonly ushort meshPartId;
        public readonly ushort unk1;
        public readonly ushort startVertex;
        public readonly ushort nVertices;
        public readonly ushort startTriangle;
        public readonly ushort nTriangles;
        public readonly ushort nBones;
        public readonly ushort startBone;
        private readonly ushort unk2;
        public readonly ushort rootBone;
        public readonly Vector3 centerMass;
        public readonly Vector3 centerBoundingBox;
        public readonly float radius;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct M2TexUnit
    {
        public readonly ushort flags;
        public readonly short shaderId;
        public readonly ushort submeshIndex;
        public readonly ushort submeshIndex2;
        public readonly short colorIndex;
        public readonly ushort renderFlags;
        public readonly ushort texUnitNumber;
        public readonly ushort op_count;
        public readonly ushort texture;
        public readonly ushort texUnitNumber2;
        public readonly short transparencyIndex;
        public readonly short textureAnimIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct M2ColorAnim
    {
        public readonly AnimationBlock color;
        public readonly AnimationBlock alpha;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct M2Particle
    {
        private readonly int unk1;
        public readonly uint flags;
        public readonly Vector3 position;
        public readonly ushort boneIndex;
        public readonly ushort textureIndex;
        public readonly int lenModelName;
        public readonly int ofsModelName;
        public readonly int lenParticleName;
        public readonly int ofsParticleName;
        public readonly byte blendingType;
        public readonly byte emitterType;
        public readonly ushort particleColorIndex;
        public readonly byte particleType;
        public readonly byte headOrTail;
        public readonly ushort textureTileRotation;
        public readonly ushort texRows;
        public readonly ushort texCols;
        public readonly AnimationBlock emissionSpeed;
        public readonly AnimationBlock speedVariation;
        public readonly AnimationBlock verticalRange;
        public readonly AnimationBlock horizontalRange;
        public readonly AnimationBlock gravity;
        public readonly AnimationBlock lifespan;
        private readonly int padding;
        public readonly AnimationBlock emissionRate;
        private readonly int padding2;
        public readonly AnimationBlock emissionAreaLength;
        public readonly AnimationBlock emissionAreaWidth;
        public readonly AnimationBlock gravity2;
        public readonly ShortAnimationBlock particleColor;
        public readonly ShortAnimationBlock particleOpacity;
        public readonly ShortAnimationBlock particleSize;
        private readonly int unk2, unk3, unk4, unk5, unk6, unk7, unk8;
        public readonly ShortAnimationBlock intensity;
        private readonly ShortAnimationBlock unk9;
        private readonly float unk10, unk11, unk12;
        public readonly Vector3 scale;
        public readonly float drag;
        private readonly float unk13, unk14;
        public readonly float rotation;
        private readonly float unk15, unk16, unk17;
        public readonly Vector3 modelRotation1;
        public readonly Vector3 modelRotation2;
        public readonly Vector3 modelTranslation;
        private readonly float unk18, unk19, unk20, unk21;
        private readonly int unk22, unk23;
        public readonly AnimationBlock enabledIn;
    }
}
