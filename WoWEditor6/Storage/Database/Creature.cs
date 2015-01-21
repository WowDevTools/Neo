using System;

namespace WoWEditor6.Storage.Database
{
    class Creature : ICreature
    {
        public int EntryId { get; set; }
        public int DifficultyEntry1 { get; set; }
        public int DifficultyEntry2 { get; set; }
        public int DifficultyEntry3 { get; set; }
        public int KillCredit1 { get; set; }
        public int KillCredit2 { get; set; }
        public int ModelId1 { get; set; }
        public int ModelId2 { get; set; }
        public int ModelId3 { get; set; }
        public int ModelId4 { get; set; }
        public string Name { get; set; }
        public string SubName { get; set; }
        public string IconName { get; set; }
        public int GossipMenuId { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public int Experience { get; set; }
        public int Faction { get; set; }
        public NpcFlag NpcFlag { get; set; }
        public float SpeedWalk { get; set; }
        public float SpeedRun { get; set; }
        public float Scale { get; set; }
        public Rank Rank { get; set; }
        public float MinDamage { get; set; }
        public float MaxDamage { get; set; }
        public DamageSchool DamageSchool { get; set; }
        public int AttackPower { get; set; }
        public float DamageMultiplier { get; set; }
        public int BaseAttackTime { get; set; }
        public int RangeAttackTime { get; set; }
        public UnitClass UnitClass { get; set; }
        public UnitFlags UnitFlags { get; set; }
        public UnitFlags2 UnitFlags2 { get; set; }
        public DynamicFlags DynamicFlags { get; set; }
        public Family Family { get; set; }
        public TrainerType TrainerType { get; set; }
        public int TrainerSpell { get; set; }
        public int TrainerClass { get; set; }
        public int TrainerRace { get; set; }
        public float MinRangedDamage { get; set; }
        public float MaxRangedDamage { get; set; }
        public int RangedAttackPower { get; set; }
        public EnumType Type { get; set; }
        public TypeFlags TypeFlags { get; set; }
        public int LootId { get; set; }
        public int PickPocketLoot { get; set; }
        public int SkinLoot { get; set; }
        public int Resistance1 { get; set; }
        public int Resistance2 { get; set; }
        public int Resistance3 { get; set; }
        public int Resistance4 { get; set; }
        public int Resistance5 { get; set; }
        public int Resistance6 { get; set; }
        public int Spell1 { get; set; }
        public int Spell2 { get; set; }
        public int Spell3 { get; set; }
        public int Spell4 { get; set; }
        public int Spell5 { get; set; }
        public int Spell6 { get; set; }
        public int Spell7 { get; set; }
        public int Spell8 { get; set; }
        public int PetSpellDataId { get; set; }
        public int VehicleId { get; set; }
        public int MinGold { get; set; }
        public int MaxGold { get; set; }
        public AiName AiName { get; set; }
        public MovementType MovementType { get; set; }
        public InhabitType InhabitType { get; set; }
        public float HoverHeight { get; set; }
        public float HealthMod { get; set; }
        public float ManaMod { get; set; }
        public float ArmorMod { get; set; }
        public int RacialLeader { get; set; }
        public int QuestItem1 { get; set; }
        public int QuestItem2 { get; set; }
        public int QuestItem3 { get; set; }
        public int QuestItem4 { get; set; }
        public int QuestItem5 { get; set; }
        public int QuestItem6 { get; set; }
        public int MovementId { get; set; }
        public int RegenHealth { get; set; }
        public MechanicImmuneMask MechanicImmuneMask { get; set; }
        public FlagsExtra FlagsExtra { get; set; }
        public string ScriptName { get; set; }
        public int WdbVerified { get; set; }

        public string GetUpdateSqlQuery()
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
