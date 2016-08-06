namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    public class Item : IItem
    {
        public int EntryId { get; set; }
        public itemClass Class { get; set; }
        public int SubClass { get; set; }
        public int SoundOverrideSubclass { get; set; }
        public string name { get; set; }
        public int displayid { get; set; }
        public itemQuality Quality { get; set; }
        public uint ItemFlags { get; set; }
        public uint ItemFlagsExtra { get; set; }
        public int BuyCount { get; set; }
        public uint BuyPrice { get; set; }
        public uint SellPrice { get; set; }
        public itemInventoryType InventoryType { get; set; }
        public int AllowableClass { get; set; }
        public int AllowableRace { get; set; }
        public int ItemLevel { get; set; }
        public int RequiredLevel { get; set; }
        public int RequiredSkill { get; set; }
        public int RequiredSkillRank { get; set; }
        public int requiredspell { get; set; }
        public int requiredhonorrank { get; set; }
        public int RequiredCityRank { get; set; }
        public int RequiredReputationFaction { get; set; }
        public itemReputationRank RequiredReputationRank { get; set; }
        public int maxcount { get; set; }
        public int stackable { get; set; }
        public int ContainerSlots { get; set; } //only for bag
        public int StatsCount { get; set; }
        public itemStatType stat_type1 { get; set; }
        public int stat_value1 { get; set; }
        public itemStatType stat_type2 { get; set; }
        public int stat_value2 { get; set; }
        public itemStatType stat_type3 { get; set; }
        public int stat_value3 { get; set; }
        public itemStatType stat_type4 { get; set; }
        public int stat_value4 { get; set; }
        public itemStatType stat_type5 { get; set; }
        public int stat_value5 { get; set; }
        public itemStatType stat_type6 { get; set; }
        public int stat_value6 { get; set; }
        public itemStatType stat_type7 { get; set; }
        public int stat_value7 { get; set; }
        public itemStatType stat_type8 { get; set; }
        public int stat_value8 { get; set; }
        public itemStatType stat_type9 { get; set; }
        public int stat_value9 { get; set; }
        public itemStatType stat_type10 { get; set; }
        public int stat_value10 { get; set; }
        public int ScalingStatDistribution { get; set; }
        public int ScalingStatValue { get; set; }
        public float dmg_min1 { get; set; }
        public float dmg_max1 { get; set; }
        public itemDmgType dmg_type1 { get; set; }
        public float dmg_min2 { get; set; }
        public float dmg_max2 { get; set; }
        public itemDmgType dmg_type2 { get; set; }
        public int armor { get; set; }
        public int holy_res { get; set; }
        public int fire_res { get; set; }
        public int nature_res { get; set; }
        public int frost_res { get; set; }
        public int shadow_res { get; set; }
        public int arcane_res { get; set; }
        public int delay { get; set; }
        public itemAmmoType ammo_type { get; set; }
        public float RangedModRange { get; set; }
        public int spellid_1 { get; set; }
        public itemSpellTrigger spelltrigger_1 { get; set; }
        public int spellcharges_1 { get; set; }
        public float spellppmRate_1 { get; set; }
        public int spellcooldown_1 { get; set; }
        public int spellcategory_1 { get; set; }
        public int spellcategorycooldown_1 { get; set; }
        public int spellid_2 { get; set; }
        public itemSpellTrigger spelltrigger_2 { get; set; }
        public int spellcharges_2 { get; set; }
        public float spellppmRate_2 { get; set; }
        public int spellcooldown_2 { get; set; }
        public int spellcategory_2 { get; set; }
        public int spellcategorycooldown_2 { get; set; }
        public int spellid_3 { get; set; }
        public itemSpellTrigger spelltrigger_3 { get; set; }
        public int spellcharges_3 { get; set; }
        public float spellppmRate_3 { get; set; }
        public int spellcooldown_3 { get; set; }
        public int spellcategory_3 { get; set; }
        public int spellcategorycooldown_3 { get; set; }
        public int spellid_4 { get; set; }
        public itemSpellTrigger spelltrigger_4 { get; set; }
        public int spellcharges_4 { get; set; }
        public float spellppmRate_4 { get; set; }
        public int spellcooldown_4 { get; set; }
        public int spellcategory_4 { get; set; }
        public int spellcategorycooldown_4 { get; set; }
        public int spellid_5 { get; set; }
        public itemSpellTrigger spelltrigger_5 { get; set; }
        public int spellcharges_5 { get; set; }
        public float spellppmRate_5 { get; set; }
        public int spellcooldown_5 { get; set; }
        public int spellcategory_5 { get; set; }
        public int spellcategorycooldown_5 { get; set; }
        public itemBonding bonding { get; set; }
        public string description { get; set; }
        public int PageText { get; set; }
        public int LanguageID { get; set; }
        public int PageMaterial { get; set; }
        public int startquest { get; set; }
        public int lockid { get; set; }
        public itemMaterial Material { get; set; }
        public itemSheath sheath { get; set; }
        public int RandomProperty { get; set; }
        public int RandomSuffix { get; set; }
        public int block { get; set; }
        public int itemset { get; set; }
        public int MaxDurability { get; set; }
        public int area { get; set; }
        public int Map { get; set; }
        public int BagFamily { get; set; }
        public itemTotemCategory TotemCategory { get; set; }
        public itemSocketColor socketColor_1 { get; set; }
        public int socketContent_1 { get; set; }
        public itemSocketColor socketColor_2 { get; set; }
        public int socketContent_2 { get; set; }
        public itemSocketColor socketColor_3 { get; set; }
        public int socketContent_3 { get; set; }
        public int socketBonus { get; set; }
        public int GemProperties { get; set; }
        public int RequiredDisenchantSkill { get; set; }
        public float ArmorDamageModifier { get; set; }
        public int duration { get; set; }
        public int ItemLimitCategory { get; set; }
        public int HolidayId { get; set; }
        public string ScriptName { get; set; }
        public int DisenchantID { get; set; }
        public itemFoodType FoodType { get; set; }
        public int VerifiedBuild { get; set; }
        public int minMoneyLoot { get; set; }
        public int maxMoneyLoot { get; set; }
        public int flagsCustom { get; set; }

        public string GetUpdateSqlQuery()
        {
            //These line avoid sql error when the computer by-default separator is the comma ( like in belgium )
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            return "UPDATE item_template SET class = '"+ (int)this.Class + "', subclass = '"+ this.SubClass + "', SoundOverrideSubclass = '"+ this.SoundOverrideSubclass + "', name = '"+ this.name + "', displayid = '"+ this.displayid + "', Quality = '"+ (int)this.Quality + "', Flags = '"+ this.ItemFlags + "', FlagsExtra = '"+ this.ItemFlagsExtra + "', BuyCount = '"+ this.BuyCount + "', BuyPrice = '"+ this.BuyPrice + "', SellPrice = '"+ this.SellPrice + "', InventoryType = '"+ (int)this.InventoryType + "', AllowableClass = '"+ this.AllowableClass + "', AllowableRace = '"+ this.AllowableRace + "', ItemLevel = '"+ this.ItemLevel + "', RequiredLevel = '"+ this.RequiredLevel + "', RequiredSkill = '"+ this.RequiredSkill + "', RequiredSkillRank = '"+ this.RequiredSkillRank + "', requiredspell = '"+ this.requiredspell + "', requiredhonorrank = '"+this.requiredhonorrank + "', RequiredCityRank = '"+ this.RequiredCityRank + "', RequiredReputationFaction = '"+ this.RequiredReputationFaction + "', RequiredReputationRank = '"+ (int)this.RequiredReputationRank + "', maxcount = '"+ this.maxcount + "', stackable = '"+ this.stackable + "', ContainerSlots = '"+ this.ContainerSlots + "', StatsCount = '"+ this.StatsCount + "', stat_type1 = '"+ (int)this.stat_type1 + "', stat_value1 = '"+ this.stat_value1 + "', stat_type2 = '" + (int)this.stat_type2 + "', stat_value2 = '" + this.stat_value2 + "', stat_type3 = '" + (int)this.stat_type3 + "', stat_value3 = '" + this.stat_value3 + "', stat_type4 = '" + (int)this.stat_type4 + "', stat_value4 = '" + this.stat_value4 + "', stat_type5 = '" + (int)this.stat_type5 + "', stat_value5 = '" + this.stat_value5 + "', stat_type6 = '" + (int)this.stat_type6 + "', stat_value6 = '" + this.stat_value6 + "', stat_type7 = '" + (int)this.stat_type7 + "', stat_value7 = '" + this.stat_value7 + "', stat_type8 = '" + (int)this.stat_type8 + "', stat_value8 = '" + this.stat_value8 + "', stat_type9 = '" + (int)this.stat_type9 + "', stat_value9 = '" + this.stat_value9 + "', stat_type10 = '" + (int)this.stat_type10 + "', stat_value10 = '" + this.stat_value10 + "',  ScalingStatDistribution = '"+ this.ScalingStatDistribution + "', ScalingStatValue = '"+ this.ScalingStatValue + "', dmg_min1 = '"+ this.dmg_min1 + "', dmg_max1 = '"+ this.dmg_max1 + "', dmg_type1 = '"+ (int)this.dmg_type1 + "',  dmg_min2 = '" + this.dmg_min2 + "', dmg_max2 = '" + this.dmg_max2 + "', dmg_type2 = '" + (int)this.dmg_type2 + "', armor = '"+ this.armor + "', holy_res = '"+ this.holy_res + "', fire_res = '"+ this.fire_res + "', nature_res = '"+ this.nature_res + "', frost_res = '"+ this.frost_res + "', shadow_res = '"+ this.shadow_res + "', arcane_res = '"+ this.arcane_res + "', delay = '"+ this.delay + "', ammo_type = '"+ (int)this.ammo_type + "', RangedModRange = '"+ this.RangedModRange + "', spellid_1 = '"+ this.spellid_1 + "', spelltrigger_1 = '"+ (int)this.spelltrigger_1 + "', spellcharges_1 = '"+ this.spellcharges_1 + "', spellppmRate_1 = '"+ this.spellppmRate_1 + "', spellcooldown_1 = '"+ this.spellcooldown_1 + "', spellcategory_1 = '"+ this.spellcategory_1 + "', spellcategorycooldown_1 = '"+ this.spellcategorycooldown_1 + "', spellid_2 = '" + this.spellid_2 + "', spelltrigger_2 = '" + (int)this.spelltrigger_2 + "', spellcharges_2 = '" + this.spellcharges_2 + "', spellppmRate_2 = '" + this.spellppmRate_2 + "', spellcooldown_2 = '" + this.spellcooldown_2 + "', spellcategory_2 = '" + this.spellcategory_2 + "', spellcategorycooldown_2 = '" + this.spellcategorycooldown_2 + "', spellid_3 = '" + this.spellid_3 + "', spelltrigger_3 = '" + (int)this.spelltrigger_3 + "', spellcharges_3 = '" + this.spellcharges_3 + "', spellppmRate_3 = '" + this.spellppmRate_3 + "', spellcooldown_3 = '" + this.spellcooldown_3 + "', spellcategory_3 = '" + this.spellcategory_3 + "', spellcategorycooldown_3 = '" + this.spellcategorycooldown_3 + "', spellid_4 = '" + this.spellid_4 + "', spelltrigger_4 = '" + (int)this.spelltrigger_4 + "', spellcharges_4 = '" + this.spellcharges_4 + "', spellppmRate_4 = '" + this.spellppmRate_4 + "', spellcooldown_4 = '" + this.spellcooldown_4 + "', spellcategory_4 = '" + this.spellcategory_4 + "', spellcategorycooldown_4 = '" + this.spellcategorycooldown_4 + "', spellid_5 = '" + this.spellid_5 + "', spelltrigger_5 = '" + (int)this.spelltrigger_5 + "', spellcharges_5 = '" + this.spellcharges_5 + "', spellppmRate_5 = '" + this.spellppmRate_5 + "', spellcooldown_5 = '" + this.spellcooldown_5 + "', spellcategory_5 = '" + this.spellcategory_5 + "', spellcategorycooldown_5 = '" + this.spellcategorycooldown_5 + "', bonding = '"+ (int)this.bonding + "', description = '"+ this.description + "', PageText = '"+ this.PageText + "', LanguageID = '"+ this.LanguageID + "', PageMaterial = '"+ this.PageMaterial + "', startquest = '"+ this.startquest + "', lockid = '"+ this.lockid + "', Material = '"+ (int)this.Material + "', sheath = '"+ (int)this.sheath + "', RandomProperty = '"+ this.RandomProperty + "', RandomSuffix = '"+ this.RandomSuffix + "', block = '"+ this.block + "', itemset = '"+ this.itemset + "', MaxDurability = '"+ this.MaxDurability + "', area = '"+ this.area + "', Map = '"+ this.Map + "', BagFamily = '"+ this.BagFamily + "', TotemCategory = '"+ (int)this.TotemCategory + "', socketColor_1 = '"+ (int)this.socketColor_1 + "', socketContent_1 = '"+ this.socketContent_1 + "', socketColor_2 = '" + (int)this.socketColor_2 + "', socketContent_2 = '" + this.socketContent_2 + "', socketColor_3 = '" + (int)this.socketColor_3 + "', socketContent_3 = '" + this.socketContent_3 + "', socketBonus = '"+ this.socketBonus + "', GemProperties = '"+ this.GemProperties + "', RequiredDisenchantSkill = '"+ this.RequiredDisenchantSkill + "', ArmorDamageModifier = '"+ this.ArmorDamageModifier + "', duration = '"+ this.duration + "', ItemLimitCategory = '"+ this.ItemLimitCategory + "', HolidayId = '"+this.HolidayId+ "', ScriptName = '"+ this.ScriptName + "', DisenchantID = '"+ this.DisenchantID + "', FoodType = '"+ (int)this.FoodType + "', minMoneyLoot = '"+ this.minMoneyLoot + "', maxMoneyLoot = '"+ this.maxMoneyLoot + "', flagsCustom = '"+ this.flagsCustom + "', VerifiedBuild = '"+ this.VerifiedBuild + "' WHERE entry = '" + this.EntryId + "';";
        }
        public string GetInsertSqlQuery()
        {
            //These line avoid sql error when the computer by-default separator is the comma ( like in belgium )
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            return "";
        }
    }
}
