using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.Storage.Database
{
    interface ISpawnedCreature
    {
        int SpawnGUID { get; set; }
        // Also known as the field "id"
        Creature Creature { get; set; }
        int Map { get; set; }
        int ZoneID{ get; set; }
        int AreaID{ get; set; }
        SpawnMask SpawnMask { get; set; }
        int PhaseMask { get; set; }
        int ModellID { get; set; }
        int EquipmentID { get; set; }
        Vector3 Position{ get; set; }
        float Orientation { get; set; }
        int SpawnTimeSecs{ get; set; }
        int SpawnDist{ get; set; }
        int CurrentWayPoint{ get; set; }
        int CurrentHealth{ get; set; }
        int CurrentMana{ get; set; }
        MovementType MovementType { get; set; }
        NPCFlag NPCFlag{ get; set; }
        UnitFlags UnitFlags{ get; set; }
        DynamicFlags DynamicFlags{ get; set; }
        int VerifiedBuild{ get; set; }
        string GetUpdateSQLQuery();
    }

    public enum SpawnMask
    {
        NotSpawned = 0,
        Normal10 = 1,
        Normal25 = 2,
        Heroic10 = 4,
        Heroic25 = 8,
        All = 15
    };
}
