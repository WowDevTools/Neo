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
                foreach (DataColumn dColumn in pDataTable.Columns)
                    switch (dColumn.ColumnName)
                    {
                        case "entry":
                            creature.EntryId = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "difficulty_entry_1":
                            creature.DifficultyEntry1 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "difficulty_entry_2":
                            creature.DifficultyEntry2 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "difficulty_entry_3":
                            creature.DifficultyEntry3 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "KillCredit1":
                            creature.KillCredit1 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "KillCredit2":
                            creature.KillCredit2 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "modelid1":
                            creature.ModelId1 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "modelid2":
                            creature.ModelId2 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "modelid3":
                            creature.ModelId3 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "modelid4":
                            creature.ModelId4 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "name":
                            creature.Name = dRow[dColumn].ToString();
                            break;
                        case "subname":
                            creature.SubName = dRow[dColumn].ToString();
                            break;
                        case "IconName":
                            creature.IconName = dRow[dColumn].ToString();
                            break;
                        case "gossip_menu_id":
                            creature.GossipMenuId = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "minlevel":
                            creature.MinLevel = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "maxlevel":
                            creature.MaxLevel = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "exp":
                            creature.Experience = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "faction":
                            creature.Faction = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "npcflag":
                            creature.NpcFlag = (NpcFlag)Enum.Parse(typeof(NpcFlag), dRow[dColumn].ToString());
                            break;
                        case "speed_walk":
                            creature.SpeedWalk = float.Parse(dRow[dColumn].ToString());
                            break;
                        case "speed_run":
                            creature.SpeedRun = float.Parse(dRow[dColumn].ToString());
                            break;
                        case "scale":
                            creature.Scale = float.Parse(dRow[dColumn].ToString());
                            break;
                        case "rank":
                            creature.Rank = (Rank)Enum.Parse(typeof(Rank), dRow[dColumn].ToString());
                            break;
                        case "dmgschool":
                            creature.DamageSchool = (DamageSchool)Enum.Parse(typeof(DamageSchool), dRow[dColumn].ToString());
                            break;
                        case "baseattacktime":
                            creature.BaseAttackTime = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "rangeattacktime":
                            creature.RangeAttackTime = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "BaseVariance":
                            creature.BaseVariance = float.Parse(dRow[dColumn].ToString());
                            break;
                        case "RangeVariance":
                            creature.RangeVariance = float.Parse(dRow[dColumn].ToString());
                            break;
                        case "unit_class":
                            creature.UnitClass = (UnitClass)Enum.Parse(typeof(UnitClass), dRow[dColumn].ToString());
                            break;
                        case "unit_flags":
                            creature.UnitFlags = (UnitFlags)Enum.Parse(typeof(UnitFlags), dRow[dColumn].ToString());
                            break;
                        case "unit_flags2":
                            creature.UnitFlags2 = (UnitFlags2)Enum.Parse(typeof(UnitFlags2), dRow[dColumn].ToString());
                            break;
                        case "dynamicflags":
                            creature.DynamicFlags = (DynamicFlags)Enum.Parse(typeof(DynamicFlags), dRow[dColumn].ToString());
                            break;
                        case "family":
                            creature.Family = (Family)Enum.Parse(typeof(Family), dRow[dColumn].ToString());
                            break;
                        case "trainer_type":
                            creature.TrainerType = (TrainerType)Enum.Parse(typeof(TrainerType), dRow[dColumn].ToString());
                            break;
                        case "trainer_spell":
                            creature.TrainerSpell = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "trainer_class":
                            creature.TrainerClass = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "trainer_race":
                            creature.TrainerRace = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "rangedattackpower":
                            creature.RangedAttackPower = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "type":
                            creature.Type = (EnumType)Enum.Parse(typeof(EnumType), dRow[dColumn].ToString());
                            break;
                        case "type_flags":
                            creature.TypeFlags = (TypeFlags)Enum.Parse(typeof(TypeFlags), dRow[dColumn].ToString());
                            break;
                        case "lootid":
                            creature.LootId = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "pickpocketloot":
                            creature.PickPocketLoot = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "skinloot":
                            creature.SkinLoot = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "resistance1":
                            creature.Resistance1 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "resistance2":
                            creature.Resistance2 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "resistance3":
                            creature.Resistance3 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "resistance4":
                            creature.Resistance4 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "resistance5":
                            creature.Resistance5 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "resistance6":
                            creature.Resistance6 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "spell1":
                            creature.Spell1 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "spell2":
                            creature.Spell2 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "spell3":
                            creature.Spell3 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "spell4":
                            creature.Spell4 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "spell5":
                            creature.Spell5 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "spell6":
                            creature.Spell6 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "spell7":
                            creature.Spell7 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "spell8":
                            creature.Spell8 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "PetSpellDataId":
                            creature.PetSpellDataId = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "VehicleId":
                            creature.VehicleId = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "mingold":
                            creature.MinGold = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "maxgold":
                            creature.MaxGold = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "AIName":
                            creature.AiName = (AiName)Enum.Parse(typeof(AiName), dRow[dColumn].ToString());
                            break;
                        case "MovementType":
                            creature.MovementType = (MovementType)Enum.Parse(typeof(MovementType), dRow[dColumn].ToString());
                            break;
                        case "InhabitType":
                            creature.InhabitType = (InhabitType)Enum.Parse(typeof(InhabitType), dRow[dColumn].ToString());
                            break;
                        case "HoverHeight":
                            creature.HoverHeight = float.Parse(dRow[dColumn].ToString());
                            break;
                        case "HealthModifier":
                            creature.HealthModifier = float.Parse(dRow[dColumn].ToString());
                            break;
                        case "ManaModifier":
                            creature.ManaModifier = float.Parse(dRow[dColumn].ToString());
                            break;
                        case "ArmorModifier":
                            creature.ArmorModifier = float.Parse(dRow[dColumn].ToString());
                            break;
                        case "DamageModifier":
                            creature.DamageModifier = float.Parse(dRow[dColumn].ToString());
                            break;
                        case "ExperienceModifier":
                            creature.ExperienceModifier = float.Parse(dRow[dColumn].ToString());
                            break;
                        case "RacialLeader":
                            creature.RacialLeader = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "QuestItem1":
                            creature.QuestItem1 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "QuestItem2":
                            creature.QuestItem2 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "QuestItem3":
                            creature.QuestItem3 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "QuestItem4":
                            creature.QuestItem4 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "QuestItem5":
                            creature.QuestItem5 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "QuestItem6":
                            creature.QuestItem6 = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "movementID":
                            creature.MovementId = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "RegenHealth":
                            creature.RegenHealth = int.Parse(dRow[dColumn].ToString());
                            break;
                        case "mechanic_immune_mask":
                            creature.MechanicImmuneMask = (MechanicImmuneMask)Enum.Parse(typeof(MechanicImmuneMask), dRow[dColumn].ToString());
                            break;
                        case "flags_extra":
                            creature.FlagsExtra = (FlagsExtra)Enum.Parse(typeof(FlagsExtra), dRow[dColumn].ToString());
                            break;
                        case "ScriptName":
                            creature.ScriptName = dRow[dColumn].ToString();
                            break;
                        case "VerifiedBuild":
                            creature.VerifiedBuild = int.Parse(dRow[dColumn].ToString());
                            break;
                    }
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
                    foreach (DataColumn dColumn in pDataTable.Columns)
                        switch (dColumn.ColumnName)
                        {
                            case "guid":
                                creature.SpawnGuid = int.Parse(dRow[dColumn].ToString());
                                break;
                            case "id":
                                creature.Creature = GetCreatureByEntry(int.Parse(dRow[dColumn].ToString()));
                                break;
                            case "map":
                                creature.Map = int.Parse(dRow[dColumn].ToString());
                                break;
                            case "spawnMask":
                                creature.SpawnMask = (SpawnMask)Enum.Parse(typeof(SpawnMask), dRow[dColumn].ToString());
                                break;
                            case "phaseMask":
                                creature.PhaseMask = int.Parse(dRow[dColumn].ToString());
                                break;
                            case "modelid":
                                creature.ModelId = int.Parse(dRow[dColumn].ToString());
                                break;
                            case "equipment_id":
                                creature.EquipmentId = int.Parse(dRow[dColumn].ToString());
                                break;
                            case "position_x":
                                position.X = float.Parse(dRow[dColumn].ToString());
                                break;
                            case "position_y":
                                position.Y = float.Parse(dRow[dColumn].ToString());
                                break;
                            case "position_z":
                                position.Z = float.Parse(dRow[dColumn].ToString());
                                break;
                            case "orientation":
                                creature.Orientation = float.Parse(dRow[dColumn].ToString());
                                break;
                            case "spawntimesecs":
                                creature.SpawnTimeSecs = int.Parse(dRow[dColumn].ToString());
                                break;
                            case "spawndist":
                                creature.SpawnDist = int.Parse(dRow[dColumn].ToString());
                                break;
                            case "currentwaypoint":
                                creature.CurrentWayPoint = int.Parse(dRow[dColumn].ToString());
                                break;
                            case "curhealth":
                                creature.CurrentHealth = int.Parse(dRow[dColumn].ToString());
                                break;
                            case "curmana":
                                creature.CurrentMana = int.Parse(dRow[dColumn].ToString());
                                break;
                            case "MovementType":
                                creature.MovementType = (MovementType)Enum.Parse(typeof(MovementType), dRow[dColumn].ToString());
                                break;
                            case "npcflag":
                                creature.NpcFlag = (NpcFlag)Enum.Parse(typeof(NpcFlag), dRow[dColumn].ToString());
                                break;
                            case "unit_flags":
                                creature.UnitFlags = (UnitFlags)Enum.Parse(typeof(UnitFlags), dRow[dColumn].ToString());
                                break;
                            case "dynamicflags":
                                creature.DynamicFlags = (DynamicFlags)Enum.Parse(typeof(DynamicFlags), dRow[dColumn].ToString());
                                break;
                        }
                    creature.Position = position;
                    mSpawnedCreatures.Add(creature);
                }
                mLoadedMaps.Add(pMapId);
            }
        }

        public Creature GetCreatureByEntry(int pEntryId)
        {
            var retVal = mCreatures.First(creature => creature.EntryId == pEntryId);
            return retVal;
        }

        public bool MapAlreadyLoaded(int pMapId)
        {
            try
            {
                // ReSharper disable once UnusedVariable
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
