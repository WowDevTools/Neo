using SharpDX;

namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    interface ISpawnedCreature
    {
        int SpawnGuid { get; set; }
        // Also known as the field "id"
        Creature Creature { get; set; }
        int Map { get; set; }
        int ZoneId{ get; set; }
        int AreaId{ get; set; }
        npcSpawnMask SpawnMask { get; set; }
        int cPhaseMask { get; set; }
        int ModelId { get; set; }
        int EquipmentId { get; set; }
        Vector3 Position{ get; set; }
        float Orientation { get; set; }
        int SpawnTimeSecs{ get; set; }
        float SpawnDist{ get; set; }
        int CurrentWayPoint{ get; set; }
        int CurrentHealth{ get; set; }
        int CurrentMana{ get; set; }
        MovementType MovementType { get; set; }
        NpcFlag NpcFlag{ get; set; }
        UnitFlags UnitFlags{ get; set; }
        DynamicFlags DynamicFlags{ get; set; }
        int VerifiedBuild{ get; set; }
        string GetUpdateSqlQuery();
        string GetInsertSqlQuery();
    }

    public enum npcSpawnMask
    {
        NotSpawned = 0,
        Normal10 = 1,
        Normal25 = 2,
        Heroic10 = 4,
        Heroic25 = 8,
        All = 15
    };
}
