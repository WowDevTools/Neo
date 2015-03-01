using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using SharpDX;

namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    class CreatureManager : Singleton<CreatureManager>, ICreatureManager
    {
        private readonly List<Creature> mCreatures = new List<Creature>();
        private readonly List<SpawnedCreature> mSpawnedCreatures = new List<SpawnedCreature>();
        private readonly List<int> mLoadedMaps = new List<int>();

        // ReSharper disable once FunctionComplexityOverflow
        public void LoadCreatures(DataTable pDataTable)
        {
            foreach (DataRow dRow in pDataTable.Rows)
            {
                var creature = new Creature();
                creature.EntryId = int.Parse(dRow[0].ToString());
                creature.DifficultyEntry1 = int.Parse(dRow[1].ToString());
                creature.DifficultyEntry2 = int.Parse(dRow[2].ToString());
                creature.DifficultyEntry3 = int.Parse(dRow[3].ToString());
                creature.KillCredit1 = int.Parse(dRow[4].ToString());
                creature.KillCredit2 = int.Parse(dRow[5].ToString());
                creature.ModelId1 = int.Parse(dRow[6].ToString());
                creature.ModelId2 = int.Parse(dRow[7].ToString());
                creature.ModelId3 = int.Parse(dRow[8].ToString());
                creature.ModelId4 = int.Parse(dRow[9].ToString());
                creature.Name = dRow[10].ToString();
                creature.SubName = dRow[11].ToString();
                creature.IconName = dRow[12].ToString();
                creature.GossipMenuId = int.Parse(dRow[13].ToString());
                creature.MinLevel = int.Parse(dRow[14].ToString());
                creature.MaxLevel = int.Parse(dRow[15].ToString());
                creature.Experience = int.Parse(dRow[16].ToString());
                creature.Faction = int.Parse(dRow[17].ToString());
                creature.NpcFlag = (NpcFlag)Enum.Parse(typeof(NpcFlag), dRow[18].ToString());
                creature.SpeedWalk = float.Parse(dRow[19].ToString());
                creature.SpeedRun = float.Parse(dRow[20].ToString());
                creature.Scale = float.Parse(dRow[21].ToString());
                creature.Rank = (Rank)Enum.Parse(typeof(Rank), dRow[22].ToString());
                creature.DamageSchool = (DamageSchool)Enum.Parse(typeof(DamageSchool), dRow[23].ToString());
                creature.BaseAttackTime = int.Parse(dRow[24].ToString());
                creature.RangeAttackTime = int.Parse(dRow[25].ToString());
                creature.BaseVariance = float.Parse(dRow[26].ToString());
                creature.RangeVariance = float.Parse(dRow[27].ToString());
                creature.UnitClass = (UnitClass)Enum.Parse(typeof(UnitClass), dRow[28].ToString());
                creature.UnitFlags = (UnitFlags)Enum.Parse(typeof(UnitFlags), dRow[29].ToString());
                creature.UnitFlags2 = (UnitFlags2)Enum.Parse(typeof(UnitFlags2), dRow[30].ToString());
                creature.DynamicFlags = (DynamicFlags)Enum.Parse(typeof(DynamicFlags), dRow[31].ToString());
                creature.Family = (Family)Enum.Parse(typeof(Family), dRow[32].ToString());
                creature.TrainerType = (TrainerType)Enum.Parse(typeof(TrainerType), dRow[33].ToString());
                creature.TrainerSpell = int.Parse(dRow[34].ToString());
                creature.TrainerClass = int.Parse(dRow[35].ToString());
                creature.TrainerRace = int.Parse(dRow[36].ToString());
                creature.Type = (CreatureType)Enum.Parse(typeof(EnumType), dRow[37].ToString());
                creature.TypeFlags = (TypeFlags)Enum.Parse(typeof(TypeFlags), dRow[38].ToString());
                creature.LootId = int.Parse(dRow[39].ToString());
                creature.PickPocketLoot = int.Parse(dRow[40].ToString());
                creature.SkinLoot = int.Parse(dRow[41].ToString());
                creature.Resistance1 = int.Parse(dRow[42].ToString());
                creature.Resistance2 = int.Parse(dRow[43].ToString());
                creature.Resistance3 = int.Parse(dRow[44].ToString());
                creature.Resistance4 = int.Parse(dRow[45].ToString());
                creature.Resistance5 = int.Parse(dRow[46].ToString());
                creature.Resistance6 = int.Parse(dRow[47].ToString());
                creature.Spell1 = int.Parse(dRow[48].ToString());
                creature.Spell2 = int.Parse(dRow[49].ToString());
                creature.Spell3 = int.Parse(dRow[50].ToString());
                creature.Spell4 = int.Parse(dRow[51].ToString());
                creature.Spell5 = int.Parse(dRow[52].ToString());
                creature.Spell6 = int.Parse(dRow[53].ToString());
                creature.Spell7 = int.Parse(dRow[54].ToString());
                creature.Spell8 = int.Parse(dRow[55].ToString());
                creature.PetSpellDataId = int.Parse(dRow[56].ToString());
                creature.VehicleId = int.Parse(dRow[57].ToString());
                creature.MinGold = int.Parse(dRow[58].ToString());
                creature.MaxGold = int.Parse(dRow[59].ToString());
                creature.AiName = dRow[60].ToString();
                creature.MovementType = (MovementType)Enum.Parse(typeof(MovementType), dRow[61].ToString());
                creature.InhabitType = (InhabitType)Enum.Parse(typeof(InhabitType), dRow[62].ToString());
                creature.HoverHeight = float.Parse(dRow[63].ToString());
                creature.HealthModifier = float.Parse(dRow[64].ToString());
                creature.ManaModifier = float.Parse(dRow[65].ToString());
                creature.ArmorModifier = float.Parse(dRow[66].ToString());
                creature.DamageModifier = float.Parse(dRow[67].ToString());
                creature.ExperienceModifier = float.Parse(dRow[68].ToString());
                creature.RacialLeader = int.Parse(dRow[69].ToString());
                creature.QuestItem1 = int.Parse(dRow[70].ToString());
                creature.QuestItem2 = int.Parse(dRow[71].ToString());
                creature.QuestItem3 = int.Parse(dRow[72].ToString());
                creature.QuestItem4 = int.Parse(dRow[73].ToString());
                creature.QuestItem5 = int.Parse(dRow[74].ToString());
                creature.QuestItem6 = int.Parse(dRow[75].ToString());
                creature.MovementId = int.Parse(dRow[76].ToString());
                creature.RegenHealth = int.Parse(dRow[77].ToString());
                creature.MechanicImmuneMask = (MechanicImmuneMask)Enum.Parse(typeof(MechanicImmuneMask), dRow[78].ToString());
                creature.FlagsExtra = (FlagsExtra)Enum.Parse(typeof(FlagsExtra), dRow[79].ToString());
                creature.ScriptName = dRow[80].ToString();
                creature.VerifiedBuild = int.Parse(dRow[81].ToString());
                mCreatures.Add(creature);
            }
        }

        // ReSharper disable once FunctionComplexityOverflow
        public void LoadSpawnedCreatures(DataTable pDataTable, int pMapId)
        {
            if (!MapAlreadyLoaded(pMapId))
            {
                foreach (DataRow dRow in pDataTable.Rows)
                {
                    var creature = new SpawnedCreature();
                    var position = new Vector3();
                    creature.SpawnGuid = int.Parse(dRow[0].ToString());
                    creature.Creature = GetCreatureByEntry(int.Parse(dRow[1].ToString()));
                    creature.Map = int.Parse(dRow[2].ToString());
                    creature.ZoneId = int.Parse(dRow[3].ToString());
                    creature.AreaId = int.Parse(dRow[4].ToString());
                    creature.SpawnMask = (npcSpawnMask)Enum.Parse(typeof(npcSpawnMask), dRow[5].ToString());
                    creature.cPhaseMask = int.Parse(dRow[6].ToString());
                    creature.ModelId = int.Parse(dRow[7].ToString());
                    creature.EquipmentId = int.Parse(dRow[8].ToString());
                    position.X = float.Parse(dRow[9].ToString());
                    position.Y = float.Parse(dRow[10].ToString());
                    position.Z = float.Parse(dRow[11].ToString());
                    creature.Position = position;
                    creature.Orientation = float.Parse(dRow[12].ToString());
                    creature.SpawnTimeSecs = int.Parse(dRow[13].ToString());
                    creature.SpawnDist = int.Parse(dRow[14].ToString());
                    creature.CurrentWayPoint = int.Parse(dRow[15].ToString());
                    creature.CurrentHealth = int.Parse(dRow[16].ToString());
                    creature.CurrentMana = int.Parse(dRow[17].ToString());
                    creature.MovementType = (MovementType)Enum.Parse(typeof(MovementType), dRow[18].ToString());
                    creature.NpcFlag = (NpcFlag)Enum.Parse(typeof(NpcFlag), dRow[19].ToString());
                    creature.UnitFlags = (UnitFlags)Enum.Parse(typeof(UnitFlags), dRow[20].ToString());
                    creature.DynamicFlags = (DynamicFlags)Enum.Parse(typeof(DynamicFlags), dRow[21].ToString());
                    creature.VerifiedBuild = int.Parse(dRow[22].ToString());
                    mSpawnedCreatures.Add(creature);
                }
                mLoadedMaps.Add(pMapId);
            }
        }

        public Creature GetCreatureByEntry(int pEntryId)
        {
            return mCreatures.First(creature => creature.EntryId == pEntryId);
        }

        private bool MapAlreadyLoaded(int pMapId)
        {
            try
            {
                var mapLoaded = mLoadedMaps.First(map => map == pMapId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<SpawnedCreature> GetSpawnedCreaturesInRadius(Vector3 pPosition, double pRadius)
        {
            var camPosXPlus = pPosition.X + pRadius;
            var camPosXMinus = pPosition.X - pRadius;
            var camPosYPlus = pPosition.Y + pRadius;
            var camPosYMinus = pPosition.Y - pRadius;

            var retVal = (from SpawnedCreature creature in mSpawnedCreatures
                where ((creature.Position.X <= camPosXPlus && creature.Position.X >= camPosXMinus) && (creature.Position.Y <= camPosYPlus && creature.Position.Y >= camPosYMinus))
                select creature).ToList<SpawnedCreature>();

            return retVal;
        }

    }
}
