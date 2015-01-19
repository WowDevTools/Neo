using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.Storage.Database
{
    interface ISpawnedCreature
    {
        class SpawnedCreature
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

    public enum SpawnMask
    {
        NotSpawned = 0,
        Normal10 = 1,
        Normal25 = 2,
        Heroic10 = 4,
        Heroic25 = 8,
        All = 15
    }
}
