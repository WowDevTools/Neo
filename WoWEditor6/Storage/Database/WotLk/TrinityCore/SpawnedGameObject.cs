using System;
using System.Data;
using System.Collections.Generic;
using SharpDX;

namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    class SpawnedGameObject : ISpawnedGameObject
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
            throw new NotImplementedException();
        }

        public string GetInsertSqlQuery()
        {
            throw new NotImplementedException();
        }
    }
}
