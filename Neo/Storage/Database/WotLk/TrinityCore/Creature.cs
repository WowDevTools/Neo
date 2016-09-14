using System;

namespace Neo.Storage.Database.WotLk.TrinityCore
{
    public class Creature : ICreature
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
        public uint NpcFlag { get; set; }
        public float SpeedWalk { get; set; }
        public float SpeedRun { get; set; }
        public float Scale { get; set; }
        public Rank Rank { get; set; }
        public DamageSchool DamageSchool { get; set; }
        public int BaseAttackTime { get; set; }
        public int RangeAttackTime { get; set; }
        public float BaseVariance { get; set; }
        public float RangeVariance { get; set; }
        public UnitClass UnitClass { get; set; }
        public uint UnitFlags { get; set; }
        public uint UnitFlags2 { get; set; }
        public uint DynamicFlags { get; set; }
        public Family Family { get; set; }
        public TrainerType TrainerType { get; set; }
        public int TrainerSpell { get; set; }
        public int TrainerClass { get; set; }
        public int TrainerRace { get; set; }
        public CreatureType Type { get; set; }
        public uint TypeFlags { get; set; }
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
        public string AiName { get; set; }
        public MovementType MovementType { get; set; }
        public int InhabitType { get; set; }
        public float HoverHeight { get; set; }
        public float HealthModifier { get; set; }
        public float ManaModifier { get; set; }
        public float ArmorModifier { get; set; }
        public float DamageModifier { get; set; }
        public float ExperienceModifier { get; set; }
        public int RacialLeader { get; set; }
        public int MovementId { get; set; }
        public int RegenHealth { get; set; }
        public uint MechanicImmuneMask { get; set; }
        public uint FlagsExtra { get; set; }
        public string ScriptName { get; set; }
        public int VerifiedBuild { get; set; }

        public string GetUpdateSqlQuery()
        {
            //These line avoid sql error when the computer by-default separator is the comma ( like in belgium )
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            return "UPDATE creature_template SET difficulty_entry_1 = '" + this.DifficultyEntry1 + "', difficulty_entry_2 = '" + this.DifficultyEntry2 + "', difficulty_entry_3 = '" + this.DifficultyEntry3 + "', KillCredit1 = '" + this.KillCredit1 + "', KillCredit2 = '" + this.KillCredit2 + "', modelid1 = '" + this.ModelId1 + "', modelid2 = '" + this.ModelId2 + "', modelid3 = '" + this.ModelId3 + "', modelid4 = '" + this.ModelId4 + "', name = '" + this.Name + "', subname = '" + this.SubName + "', IconName = '" + this.IconName + "', gossip_menu_id = '" + this.GossipMenuId + "', minlevel = '" + this.MinLevel + "', maxlevel = '" + this.MaxLevel + "', exp = '" + this.Experience + "', faction = '" + this.Faction + "', npcflag = '" + this.NpcFlag + "', speed_walk = '" + this.SpeedWalk + "', speed_run = '" + this.SpeedRun + "', scale = '" + this.Scale + "', rank = '" + (int)this.Rank + "', dmgschool = '" + (int)this.DamageSchool + "', BaseAttackTime = '" + this.BaseAttackTime + "', RangeAttackTime = '" + this.RangeAttackTime + "', BaseVariance = '" + this.BaseVariance + "', RangeVariance = '" + this.RangeVariance + "', unit_class = '" + (int)this.UnitClass + "', unit_flags = '" + this.UnitFlags + "', unit_flags2 = '" + this.UnitFlags2 + "', dynamicflags = '" + this.DynamicFlags + "', family = '" + (int)this.Family + "', trainer_type = '" + (int)this.TrainerType + "', trainer_spell = '" + this.TrainerSpell + "', trainer_class = '" + this.TrainerClass + "', trainer_race = '" + this.TrainerRace + "', type = '" + (int)this.Type + "', type_flags = '" + this.TypeFlags + "', lootid = '" + this.LootId + "', pickpocketloot = '" + this.PickPocketLoot + "', skinloot = '" + this.SkinLoot + "', resistance1 = '" + this.Resistance1 + "', resistance2 = '" + this.Resistance2 + "', resistance3 = '" + this.Resistance3 + "', resistance4 = '" + this.Resistance4 + "', resistance5 = '" + this.Resistance5 + "', resistance6 = '" + this.Resistance6 + "', spell1 = '" + this.Spell1 + "', spell2 = '" + this.Spell2 + "', spell3 = '" + this.Spell3 + "', spell4 = '" + this.Spell4 + "', spell5 = '" + this.Spell5 + "', spell6 = '" + this.Spell6 + "', spell7 = '" + this.Spell7 + "', spell8 = '" + this.Spell8 + "', PetSpellDataId = '" + this.PetSpellDataId + "', VehicleId = '" + this.VehicleId + "', mingold = '" + this.MinGold + "', maxgold = '" + this.MaxGold + "', AIName = '" + this.AiName + "', MovementType = '" + (int)this.MovementType + "', InhabitType = '" + this.InhabitType + "', HoverHeight = '" + this.HoverHeight + "', HealthModifier = '" + this.HealthModifier + "', ManaModifier = '" + this.ManaModifier + "', ArmorModifier = '" + this.ArmorModifier + "', DamageModifier = '" + this.DamageModifier + "', ExperienceModifier = '" + this.ExperienceModifier + "', RacialLeader = '" + this.RacialLeader + "', movementId = '" + this.MovementId + "', RegenHealth = '" + this.RegenHealth + "', mechanic_immune_mask = '" + this.MechanicImmuneMask + "', flags_extra = '" + this.FlagsExtra + "', ScriptName = '" + this.ScriptName + "', VerifiedBuild = '" + this.VerifiedBuild + "' WHERE entry = '" + this.EntryId + "';";
        }

        public string GetInsertSqlQuery()
        {
            //These line avoid sql error when the computer by-default separator is the comma ( like in belgium )
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            return "INSERT INTO creature_template VALUES ('" + this.EntryId + "', '" + this.DifficultyEntry1 + "', '" + this.DifficultyEntry2 + "', '" + this.DifficultyEntry3 + "', '" + this.KillCredit1 + "', '" + this.KillCredit2 + "', '" + this.ModelId1 + "', '" + this.ModelId2 + "', '" + this.ModelId3 + "', '" + this.ModelId4 + "', '" + this.Name + "', '" + this.SubName + "', '" + this.IconName + "', '" + this.GossipMenuId + "', '" + this.MinLevel + "', '" + this.MaxLevel + "', '" + this.Experience + "', '" + this.Faction + "', '" + this.NpcFlag + "', '" + this.SpeedWalk + "', '" + this.SpeedRun + "', '" + this.Scale + "', '" + (int)this.Rank + "', '" + (int)this.DamageSchool + "', '" + this.BaseAttackTime + "', '" + this.RangeAttackTime + "', '" + this.BaseVariance + "', '" + this.RangeVariance + "', '" + (int)this.UnitClass + "', '" + this.UnitFlags + "', '" + this.UnitFlags2 + "', '" + this.DynamicFlags + "', '" + (int)this.Family + "', '" + (int)this.TrainerType + "', '" + this.TrainerSpell + "', '" + this.TrainerClass + "', '" + this.TrainerRace + "', '" + (int)this.Type + "', '" + this.TypeFlags + "', '" + this.LootId + "', '" + this.PickPocketLoot + "', '" + this.SkinLoot + "', '" + this.Resistance1 + "', '" + this.Resistance2 + "', '" + this.Resistance3 + "', '" + this.Resistance4 + "', '" + this.Resistance5 + "', '" + this.Resistance6 + "', '" + this.Spell1 + "', '" + this.Spell2 + "', '" + this.Spell3 + "', '" + this.Spell4 + "', '" + this.Spell5 + "', '" + this.Spell6 + "', '" + this.Spell7 + "', '" + this.Spell8 + "', '" + this.PetSpellDataId + "', '" + this.VehicleId + "', '" + this.MinGold + "', '" + this.MaxGold + "', '" + this.AiName + "', '" + (int)this.MovementType + "', '" + this.InhabitType + "', '" + this.HoverHeight + "', '" + this.HealthModifier + "', '" + this.ManaModifier + "', '" + this.ArmorModifier + "', '" + this.DamageModifier + "', '" + this.ExperienceModifier + "', '" + this.RacialLeader + "', '" + this.MovementId + "', '" + this.RegenHealth + "', '" + this.MechanicImmuneMask + "', '" + this.FlagsExtra + "', '" + this.ScriptName + "', '" + this.VerifiedBuild + "');";
        }
    }
}
