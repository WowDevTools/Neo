using System;
using System.Data;
using System.Collections.Generic;
using SharpDX;

namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    public class SpawnedGameObject : ISpawnedGameObject
    {
        public int SpawnGuid { get; set; }
        // Also known as the field "id"
        public GameObject GameObject { get; set; }
        public int Map { get; set; }
        public int ZoneId { get; set; }
        public int AreaId { get; set; }
        public goSpawnMask SpawnMask { get; set; }
        public int PhaseMask { get; set; }
        public Vector3 Position { get; set; }
        public float Orientation { get; set; }
        public float Rotation0 { get; set; }
        public float Rotation1 { get; set; }
        public float Rotation2 { get; set; }
        public float Rotation3 { get; set; }
        public int SpawnTimeSecs { get; set; }
        public int AnimProgress { get; set; }
        public int State { get; set; }

        public string GetUpdateSqlQuery()
        {
            return "UPDATE creature SET id = '" + this.GameObject.EntryId + "', map = '" + this.Map + "', zoneId = '" + this.ZoneId + "', areaId = '" + this.AreaId + "', spawnMask = '" + this.SpawnMask + "', phaseMask = '" + this.PhaseMask + "', position_x = '" + this.Position.X + "', position_y = '" + this.Position.Y + "', position_z = '" + this.Position.Z + "', orientation = '" + this.Orientation + "', rotation0 = '" + this.Rotation0 + "', rotation1 = '" + this.Rotation1 + "', rotation 2 = '" + this.Rotation2 + "', rotation 3 = '" + this.Rotation3 + "', spawntimesecs = '" + this.SpawnTimeSecs + "', animprogress = '" + this.AnimProgress + "', state = '" + this.State + "' WHERE guid = '" + this.SpawnGuid + "';";
        }

        public string GetInsertSqlQuery()
        {
            return "INSERT INTO creature VALUES ('" + this.SpawnGuid + "', '" + this.GameObject.EntryId + "', '" + this.Map + "', '" + this.ZoneId + "', '" + this.AreaId + "', '" + this.SpawnMask + "', '" + this.PhaseMask + "', '" + this.Position.X + "', '" + this.Position.Y + "', '" + this.Position.Z + "', '" + this.Orientation + "', '" + this.Rotation0 + "', '" + this.Rotation1 + "', '" + this.Rotation2 + "', '" + this.Rotation3 + "', '" + this.SpawnTimeSecs + "', '" + this.AnimProgress + "', '" + this.State + "');";
        }
    }
}
