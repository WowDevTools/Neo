using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using OpenTK;

namespace Neo.Storage.Database.WotLk.TrinityCore
{
	internal class CreatureManager : Singleton<CreatureManager>, ICreatureManager
    {
        private readonly List<Creature> mCreatures = new List<Creature>();
        private readonly List<SpawnedCreature> mSpawnedCreatures = new List<SpawnedCreature>();
        private readonly List<int> mLoadedMaps = new List<int>();

        public void LoadCreatures(DataTable pDataTable)
        {
            //These line avoid sql error when the computer by-default separator is the comma ( like in belgium )
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            foreach (DataRow dRow in pDataTable.Rows)
            {
                var creature = new Creature()
                {
                    EntryId = int.Parse(dRow[0].ToString()),
                    DifficultyEntry1 = int.Parse(dRow[1].ToString()),
                    DifficultyEntry2 = int.Parse(dRow[2].ToString()),
                    DifficultyEntry3 = int.Parse(dRow[3].ToString()),
                    KillCredit1 = int.Parse(dRow[4].ToString()),
                    KillCredit2 = int.Parse(dRow[5].ToString()),
                    ModelId1 = int.Parse(dRow[6].ToString()),
                    ModelId2 = int.Parse(dRow[7].ToString()),
                    ModelId3 = int.Parse(dRow[8].ToString()),
                    ModelId4 = int.Parse(dRow[9].ToString()),
                    Name = dRow[10].ToString(),
                    SubName = dRow[11].ToString(),
                    IconName = dRow[12].ToString(),
                    GossipMenuId = int.Parse(dRow[13].ToString()),
                    MinLevel = int.Parse(dRow[14].ToString()),
                    MaxLevel = int.Parse(dRow[15].ToString()),
                    Experience = int.Parse(dRow[16].ToString()),
                    Faction = int.Parse(dRow[17].ToString()),
                    NpcFlag = uint.Parse(dRow[18].ToString()),
                    SpeedWalk = float.Parse(dRow[19].ToString()),
                    SpeedRun = float.Parse(dRow[20].ToString()),
                    Scale = float.Parse(dRow[21].ToString()),
                    Rank = (Rank)Enum.Parse(typeof(Rank), dRow[22].ToString()),
                    DamageSchool = (DamageSchool)Enum.Parse(typeof(DamageSchool), dRow[23].ToString()),
                    BaseAttackTime = int.Parse(dRow[24].ToString()),
                    RangeAttackTime = int.Parse(dRow[25].ToString()),
                    BaseVariance = float.Parse(dRow[26].ToString()),
                    RangeVariance = float.Parse(dRow[27].ToString()),
                    UnitClass = (UnitClass)Enum.Parse(typeof(UnitClass), dRow[28].ToString()),
                    UnitFlags = uint.Parse(dRow[29].ToString()),
                    UnitFlags2 = uint.Parse(dRow[30].ToString()),
                    DynamicFlags = uint.Parse(dRow[31].ToString()),
                    Family = (Family)Enum.Parse(typeof(Family), dRow[32].ToString()),
                    TrainerType = (TrainerType)Enum.Parse(typeof(TrainerType), dRow[33].ToString()),
                    TrainerSpell = int.Parse(dRow[34].ToString()),
                    TrainerClass = int.Parse(dRow[35].ToString()),
                    TrainerRace = int.Parse(dRow[36].ToString()),
                    Type = (CreatureType)Enum.Parse(typeof(EnumType), dRow[37].ToString()),
                    TypeFlags = uint.Parse(dRow[38].ToString()),
                    LootId = int.Parse(dRow[39].ToString()),
                    PickPocketLoot = int.Parse(dRow[40].ToString()),
                    SkinLoot = int.Parse(dRow[41].ToString()),
                    Resistance1 = int.Parse(dRow[42].ToString()),
                    Resistance2 = int.Parse(dRow[43].ToString()),
                    Resistance3 = int.Parse(dRow[44].ToString()),
                    Resistance4 = int.Parse(dRow[45].ToString()),
                    Resistance5 = int.Parse(dRow[46].ToString()),
                    Resistance6 = int.Parse(dRow[47].ToString()),
                    Spell1 = int.Parse(dRow[48].ToString()),
                    Spell2 = int.Parse(dRow[49].ToString()),
                    Spell3 = int.Parse(dRow[50].ToString()),
                    Spell4 = int.Parse(dRow[51].ToString()),
                    Spell5 = int.Parse(dRow[52].ToString()),
                    Spell6 = int.Parse(dRow[53].ToString()),
                    Spell7 = int.Parse(dRow[54].ToString()),
                    Spell8 = int.Parse(dRow[55].ToString()),
                    PetSpellDataId = int.Parse(dRow[56].ToString()),
                    VehicleId = int.Parse(dRow[57].ToString()),
                    MinGold = int.Parse(dRow[58].ToString()),
                    MaxGold = int.Parse(dRow[59].ToString()),
                    AiName = dRow[60].ToString(),
                    MovementType = (MovementType)Enum.Parse(typeof(MovementType), dRow[61].ToString()),
                    InhabitType = int.Parse(dRow[62].ToString()),
                    HoverHeight = float.Parse(dRow[63].ToString()),
                    HealthModifier = float.Parse(dRow[64].ToString()),
                    ManaModifier = float.Parse(dRow[65].ToString()),
                    ArmorModifier = float.Parse(dRow[66].ToString()),
                    DamageModifier = float.Parse(dRow[67].ToString()),
                    ExperienceModifier = float.Parse(dRow[68].ToString()),
                    RacialLeader = int.Parse(dRow[69].ToString()),
                    MovementId = int.Parse(dRow[70].ToString()),
                    RegenHealth = int.Parse(dRow[71].ToString()),
                    MechanicImmuneMask = uint.Parse(dRow[72].ToString()),
                    FlagsExtra = uint.Parse(dRow[73].ToString()),
                    ScriptName = dRow[74].ToString(),
                    VerifiedBuild = int.Parse(dRow[75].ToString())
                };
	            this.mCreatures.Add(creature);
            }
        }

        public void LoadSpawnedCreatures(DataTable pDataTable, int pMapId)
        {
            if (!MapAlreadyLoaded(pMapId))
            {
                foreach (DataRow dRow in pDataTable.Rows)
                {
                    var position = new Vector3()
                    {
                        X = float.Parse(dRow[9].ToString()),
                        Y = float.Parse(dRow[10].ToString()),
                        Z = float.Parse(dRow[11].ToString())
                    };
                    var creature = new SpawnedCreature()
                    {
                        SpawnGuid = int.Parse(dRow[0].ToString()),
                        Creature = GetCreatureByEntry(int.Parse(dRow[1].ToString())),
                        Map = int.Parse(dRow[2].ToString()),
                        ZoneId = int.Parse(dRow[3].ToString()),
                        AreaId = int.Parse(dRow[4].ToString()),
                        SpawnMask = (npcSpawnMask)Enum.Parse(typeof(npcSpawnMask), dRow[5].ToString()),
                        cPhaseMask = int.Parse(dRow[6].ToString()),
                        ModelId = int.Parse(dRow[7].ToString()),
                        EquipmentId = int.Parse(dRow[8].ToString()),
                        Position = position,
                        Orientation = float.Parse(dRow[12].ToString()),
                        SpawnTimeSecs = int.Parse(dRow[13].ToString()),
                        SpawnDist = float.Parse(dRow[14].ToString()),
                        CurrentWayPoint = int.Parse(dRow[15].ToString()),
                        CurrentHealth = int.Parse(dRow[16].ToString()),
                        CurrentMana = int.Parse(dRow[17].ToString()),
                        MovementType = (MovementType)Enum.Parse(typeof(MovementType), dRow[18].ToString()),
                        NpcFlag = (NpcFlag)Enum.Parse(typeof(NpcFlag), dRow[19].ToString()),
                        UnitFlags = (UnitFlags)Enum.Parse(typeof(UnitFlags), dRow[20].ToString()),
                        DynamicFlags = (DynamicFlags)Enum.Parse(typeof(DynamicFlags), dRow[21].ToString()),
                        VerifiedBuild = int.Parse(dRow[22].ToString())
                    };
	                this.mSpawnedCreatures.Add(creature);
                }
	            this.mLoadedMaps.Add(pMapId);
            }
        }

        public void addCreatedCreature(Creature creature)
        {
	        this.mCreatures.Add(creature);
        }

        public Creature GetCreatureByEntry(int pEntryId)
        {
            return this.mCreatures.FirstOrDefault(creature => creature.EntryId == pEntryId);
        }

        private bool MapAlreadyLoaded(int pMapId)
        {
            try
            {
                var mapLoaded = this.mLoadedMaps.First(map => map == pMapId);
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

            var retVal = (from SpawnedCreature creature in this.mSpawnedCreatures
                where ((creature.Position.X <= camPosXPlus && creature.Position.X >= camPosXMinus) && (creature.Position.Y <= camPosYPlus && creature.Position.Y >= camPosYMinus))
                select creature).ToList<SpawnedCreature>();

            return retVal;
        }

    }
}
