namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    class Item : IItem
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
        public int BuyPrice { get; set; }
        public int SellPrice { get; set; }
        public itemInvenotryType InventoryType { get; set; }
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
        public int ContainerSlots { get; set; }
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
        public int dmg_min1 { get; set; }
        public int dmg_max1 { get; set; }
        public itemDmgType dmg_type1 { get; set; }
        public int dmg_min2 { get; set; }
        public int dmg_max2 { get; set; }
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
        public int RangedModRange { get; set; }
        public int spellid_1 { get; set; }
        public itemSpellTrigger spelltrigger_1 { get; set; }
        public int spellcharges_1 { get; set; }
        public int spellppmRate_1 { get; set; }
        public int spellcooldown_1 { get; set; }
        public int spellcategory_1 { get; set; }
        public int spellcategorycooldown_1 { get; set; }
        public int spellid_2 { get; set; }
        public itemSpellTrigger spelltrigger_2 { get; set; }
        public int spellcharges_2 { get; set; }
        public int spellppmRate_2 { get; set; }
        public int spellcooldown_2 { get; set; }
        public int spellcategory_2 { get; set; }
        public int spellcategorycooldown_2 { get; set; }
        public int spellid_3 { get; set; }
        public itemSpellTrigger spelltrigger_3 { get; set; }
        public int spellcharges_3 { get; set; }
        public int spellppmRate_3 { get; set; }
        public int spellcooldown_3 { get; set; }
        public int spellcategory_3 { get; set; }
        public int spellcategorycooldown_3 { get; set; }
        public int spellid_4 { get; set; }
        public itemSpellTrigger spelltrigger_4 { get; set; }
        public int spellcharges_4 { get; set; }
        public int spellppmRate_4 { get; set; }
        public int spellcooldown_4 { get; set; }
        public int spellcategory_4 { get; set; }
        public int spellcategorycooldown_4 { get; set; }
        public int spellid_5 { get; set; }
        public itemSpellTrigger spelltrigger_5 { get; set; }
        public int spellcharges_5 { get; set; }
        public int spellppmRate_5 { get; set; }
        public int spellcooldown_5 { get; set; }
        public int spellcategory_5 { get; set; }
        public int spellcategorycooldown_5 { get; set; }
        public itemBonding bonding { get; set; }
        public int description { get; set; }
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
        public int ArmorDamageModifier { get; set; }
        public int duration { get; set; }
        public int ItemLimitCategory { get; set; }
        public int HolidayId { get; set; }
        public int ScriptName { get; set; }
        public int DisenchantID { get; set; }
        public itemFoodType FoodType { get; set; }
        public int VerifiedBuild { get; set; }
        public int minMoneyLoot { get; set; }
        public int maxMoneyLoot { get; set; }
        public uint flagsCustom { get; set; }

        public string GetUpdateSqlQuery()
        {
            //These line avoid sql error when the computer by-default separator is the comma ( like in belgium )
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            return "";
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
