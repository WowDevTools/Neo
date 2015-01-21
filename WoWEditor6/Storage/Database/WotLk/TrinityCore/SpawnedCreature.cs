using System;
using SharpDX;

namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    class SpawnedCreature : ISpawnedCreature
    {
        public int SpawnGuid { get; set; }
        // Also known as the field "id"
        public Creature Creature { get; set; }
        public int Map { get; set; }
        public int ZoneId { get; set; }
        public int AreaId { get; set; }
        public SpawnMask SpawnMask { get; set; }
        public int PhaseMask { get; set; }
        public int ModelId { get; set; }
        public int EquipmentId { get; set; }
        public Vector3 Position { get; set; }
        public float Orientation { get; set; }
        public int SpawnTimeSecs { get; set; }
        public int SpawnDist { get; set; }
        public int CurrentWayPoint { get; set; }
        public int CurrentHealth { get; set; }
        public int CurrentMana { get; set; }
        public MovementType MovementType { get; set; }
        public NpcFlag NpcFlag { get; set; }
        public UnitFlags UnitFlags { get; set; }
        public DynamicFlags DynamicFlags { get; set; }
        public int VerifiedBuild { get; set; }

        public string GetUpdateSqlQuery()
        {
            return "UPDATE creature SET id = '" + this.Creature.EntryId + "', map = '" + this.Map + "', zoneId = '" + this.ZoneId + "', areaId = '" + this.AreaId + "', spawnMask = '" + this.SpawnMask + "', phaseMask = '" + this.PhaseMask + "', modelid = '" + this.ModelId + "', equipment_id = '" + this.EquipmentId + "', position_x = '" + this.Position.X + "', position_y = '" + this.Position.Y + "', position_z = '" + this.Position.Z + "', orientation = '" + this.Orientation + "', spawntimesecs = '" + this.SpawnTimeSecs + "', spawndist = '" + this.SpawnDist + "', currentwaypoint = '" + this.CurrentWayPoint + "', curhealth = '" + this.CurrentHealth + "', curmana = '" + this.CurrentMana + "', MovementType = '" + this.MovementType + "', npcflag = '" + this.NpcFlag + "', unit_flags = '" + this.UnitFlags + "', dynamicflags = '" + this.DynamicFlags + "', VerifiedBuild = '" + this.VerifiedBuild + "' WHERE guid = '" + this.SpawnGuid + "';";
        }

        public string GetInsertSqlQuery()
        {
            return "INSERT INTO creature VALUES ('" + this.SpawnGuid + "', '" + this.Creature.EntryId + "', '" + this.Map + "', '" + this.ZoneId + "', '" + this.AreaId + "', '" + this.SpawnMask + "', '" + this.PhaseMask + "', '" + this.ModelId + "', '" + this.EquipmentId + "', '" + this.Position.X + "', '" + this.Position.Y + "', '" + this.Position.Z + "', '" + this.Orientation + "', '" + this.SpawnTimeSecs + "', '" + this.SpawnDist + "', '" + this.CurrentWayPoint + "', '" + this.CurrentHealth + "', '" + this.CurrentMana + "', '" + this.MovementType + "', '" + this.NpcFlag + "', '" + this.UnitFlags + "', '" + this.DynamicFlags + "', '" + this.VerifiedBuild + "');";
        }
    }
}
