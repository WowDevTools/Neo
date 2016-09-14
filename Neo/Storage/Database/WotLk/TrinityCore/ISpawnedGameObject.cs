using SharpDX;

namespace Neo.Storage.Database.WotLk.TrinityCore
{
    interface ISpawnedGameObject
    {
        int SpawnGuid { get; set; }
        // Also known as the field "id"
        GameObject GameObject { get; set; }
        int Map { get; set; }
        int ZoneId { get; set; }
        int AreaId { get; set; }
        goSpawnMask SpawnMask { get; set; }
        int PhaseMask { get; set; }
        Vector3 Position { get; set; }
        float Orientation { get; set; }
        float Rotation0 { get; set; }
        float Rotation1 { get; set; }
        float Rotation2 { get; set; }
        float Rotation3 { get; set; }
        int SpawnTimeSecs { get; set; }
        int AnimProgress { get; set; }
        int State { get; set; }
        string GetUpdateSqlQuery();
        string GetInsertSqlQuery();
    }

    public enum goSpawnMask
    {
        NotSpawned = 0,
        Normal10 = 1,
        Normal25 = 2,
        Heroic10 = 4,
        Heroic25 = 8,
        All = 15
    };
}
