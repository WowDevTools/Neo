using System;

namespace WoWEditor6.Storage.Database
{
    class SpawnedCreature : ISpawnedCreature
    {
        public int SpawnGUID { get; set; }
        public Creature Creature { get; set; }
        public int ModellID { get; set; }
        // XYZ aka Vector3D
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Orientation { get; set; }
        public int Map { get; set; }
        public SpawnMask SpawnMask { get; set; }
        public int PhaseMask { get; set; }
    }
}
