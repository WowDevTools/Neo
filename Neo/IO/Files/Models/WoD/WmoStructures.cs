using System.Numerics;
using System.Runtime.InteropServices;

namespace Neo.IO.Files.Models.WoD
{
    [StructLayout(LayoutKind.Sequential)]
    struct Mohd
    {
        public readonly int nMaterials;
        public readonly int nGroups;
        public readonly int nPortals;
        public readonly int nLights;
        public readonly int nModels;
        public readonly int nDoodads;
        public readonly int nSets;
        public readonly uint ambientColor;
        public readonly int wmoAreaTable;
        public readonly Vector3 bboxMin;
        public readonly Vector3 bboxMax;
        public readonly int flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Momt
    {
        public readonly uint flags;
        public readonly int shader;
        public readonly int blendMode;
        public readonly int texture1;
        public readonly uint color1;
        public readonly uint flags1;
        public readonly int texture2;
        public readonly uint color2;
        public readonly uint flags2;
        public readonly int texture3;
        public readonly uint color3;
        private readonly int padding1, padding2, padding3, padding4, padding5;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Mogi
    {
        public readonly uint flags;
        public readonly Vector3 bboxMin;
        public readonly Vector3 bboxMax;
        public readonly int nameOffset;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Mopt
    {
        public readonly short baseVertex;
        public readonly short numVertices;
        public readonly Vector3 normal;
        private readonly float unk;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Mopr
    {
        public readonly short portalIndex;
        public readonly short wmoGroup;
        public readonly short relation;
        private readonly short padding;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Molt
    {
        public readonly byte lightType;
        public readonly byte type;
        public readonly byte useAttenuation;
        private readonly byte padding;
        public readonly uint color;
        public readonly Vector3 position;
        public readonly float intensity;
        public readonly float attenuationStart;
        public readonly float attenuationEnd;
        private readonly float unk1, unk2, unk3, unk4;
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct Mods
    {
        public fixed byte name [20];
        public readonly int firstInstance;
        public readonly int numDoodads;
        private readonly int padding;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Modd
    {
        public readonly int nameIndex;
        public readonly Vector3 position;
        public readonly Vector4 rotation;
        public readonly float scale;
        public readonly float color;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Mfog
    {
        public readonly uint flags;
        public readonly Vector3 position;
        public readonly float innerRadius;
        public readonly float outerRadius;
        public readonly float fogEnd;
        public readonly float fogStart;
        public readonly uint color;
        private readonly float unk1, unk2;
        public readonly uint color2;
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct Mogp
    {
        public readonly uint groupName;
        public readonly uint descGroupName;
        public readonly uint flags;
        public readonly Vector3 bboxMin;
        public readonly Vector3 bboxMax;
        public readonly short ofsMopr;
        public readonly short numMopr;
        public readonly short numBatchesA;
        public readonly short numBatchesB;
        public readonly int numBatchesC;
        public fixed byte fogIndices [4];
        public readonly int liquidType;
        public readonly int wmoGroupId;
        private readonly int unk1, unk2;
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct Moba
    {
        public fixed uint a [3];
        public readonly int firstFace;
        public readonly ushort numFaces;
        public readonly ushort firstVertex;
        public readonly ushort lastVertex;
        private readonly byte unused;
        public readonly byte material;
    }
}
