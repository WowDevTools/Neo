using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.Storage.Database
{
    class Creature : ICreature
    {
        public int EntryID { get; set; }
        public int DifficultyEntry1 { get; set; }
        public int DifficultyEntry2 { get; set; }
        public int DifficultyEntry3 { get; set; }
        public int KillCredit1 { get; set; }
        public int KillCredit2 { get; set; }
        public int ModelID1 { get; set; }
        public int ModelID2 { get; set; }
        public int ModelID3 { get; set; }
        public int ModelID4 { get; set; }
        public string Name { get; set; }
        public string SubName { get; set; }
        public string IconName { get; set; }
        public int GossipMenuID { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public int Experience { get; set; }
        public int Faction { get; set; }
        public NPCFlag NPCFlag { get; set; }
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
        public enumType Type { get; set; }
        public TypeFlags TypeFlags { get; set; }
        public int LootID { get; set; }
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
        public int PetSpellDataID { get; set; }
        public int VehicleID { get; set; }
        public int MinGold { get; set; }
        public int MaxGold { get; set; }
        public AIName AIName { get; set; }
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
        public int MovementID { get; set; }
        public int RegenHealth { get; set; }
        public MechanicImmuneMask MechanicImmuneMask { get; set; }
        public FlagsExtra FlagsExtra { get; set; }
        public string ScriptName { get; set; }
        public int WDBVerified { get; set; }

        int ICreature.SubName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        int ICreature.IconName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string GetUpdateSQLQuery()
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
