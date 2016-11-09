using System;
using System.Data;

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
        public int minMoneyLoot { get; set; }
        public int maxMoneyLoot { get; set; }
        public int flagsCustom { get; set; }
        public int VerifiedBuild { get; set; }

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

            return "INSERT INTO item_template VALUES ('"+ this.EntryId +"','"+ (int)this.Class + "','"+ this.SubClass+ "','"+ this.SoundOverrideSubclass+ "','"+ this.name + "','"+ this.displayid + "','"+ (int)this.Quality + "','"+ this.ItemFlags+ "','"+ this.ItemFlagsExtra + "','"+ this.BuyCount + "','"+ this.BuyPrice+ "','"+ this.SellPrice+ "','"+ (int)this.InventoryType+ "','"+ this.AllowableClass+ "','"+ this.AllowableRace+ "','"+ this.ItemLevel+ "','"+ this.RequiredLevel+ "','"+ this.RequiredSkill+ "','"+this.RequiredSkillRank+ "','"+this.requiredspell+ "','"+ this.requiredhonorrank+ "','"+ this.RequiredCityRank+ "','"+ this.RequiredReputationFaction+ "','"+ (int)this.RequiredReputationRank+ "','"+ this.maxcount+ "','"+this.stackable+ "','"+this.ContainerSlots+ "','"+this.StatsCount+ "','"+ (int)this.stat_type1+ "','"+this.stat_value1+ "','" + (int)this.stat_type2 + "','" + this.stat_value2 + "','" + (int)this.stat_type3 + "','" + this.stat_value3 + "','" + (int)this.stat_type4 + "','" + this.stat_value4 + "','" + (int)this.stat_type5 + "','" + this.stat_value5 + "','" + (int)this.stat_type6 + "','" + this.stat_value6 + "','" + (int)this.stat_type7 + "','" + this.stat_value7 + "','" + (int)this.stat_type8 + "','" + this.stat_value8 + "','" + (int)this.stat_type9 + "','" + this.stat_value9 + "','" + (int)this.stat_type10 + "','" + this.stat_value10 + "', '"+ this.ScalingStatDistribution+ "', '"+ this.ScalingStatValue+ "', '"+ this.dmg_min1+ "', '"+this.dmg_max1+ "', '"+(int)this.dmg_type1+ "', '"+this.dmg_min2+ "', '"+ this.dmg_max2+ "', '"+(int)this.dmg_type2+ "', '"+this.armor+ "', '"+this.holy_res+ "', '"+this.fire_res+ "', '"+this.nature_res+ "', '"+this.frost_res+ "', '"+this.shadow_res+ "', '"+this.arcane_res+ "', '"+this.delay+ "', '"+(int)this.ammo_type+"', '"+ this.RangedModRange+ "', '"+this.spellid_1+ "', '"+(int)this.spelltrigger_1+ "', '"+this.spellcharges_1+ "', '"+this.spellppmRate_1+ "', '"+this.spellcooldown_1+ "', '"+this.spellcategory_1+ "', '"+this.spellcategorycooldown_1+ "', '" + this.spellid_2 + "', '" + (int)this.spelltrigger_2 + "', '" + this.spellcharges_2 + "', '" + this.spellppmRate_2 + "', '" + this.spellcooldown_2 + "', '" + this.spellcategory_2 + "', '" + this.spellcategorycooldown_2 + "', '" + this.spellid_3 + "', '" + (int)this.spelltrigger_3 + "', '" + this.spellcharges_3 + "', '" + this.spellppmRate_3 + "', '" + this.spellcooldown_3 + "', '" + this.spellcategory_3 + "', '" + this.spellcategorycooldown_3 + "', '" + this.spellid_4 + "', '" + (int)this.spelltrigger_4 + "', '" + this.spellcharges_4 + "', '" + this.spellppmRate_4 + "', '" + this.spellcooldown_4 + "', '" + this.spellcategory_4 + "', '" + this.spellcategorycooldown_4 + "', '" + this.spellid_5 + "', '" + (int)this.spelltrigger_5 + "', '" + this.spellcharges_5 + "', '" + this.spellppmRate_5 + "', '" + this.spellcooldown_5 + "', '" + this.spellcategory_5 + "', '" + this.spellcategorycooldown_5 + "', '"+(int)this.bonding+ "', '"+this.description+ "', '"+this.PageText+ "', '"+this.LanguageID+ "', '"+this.PageMaterial+ "', '"+this.startquest+ "', '"+this.lockid+ "', '"+(int)this.Material+ "', '"+(int)this.sheath+ "', '"+this.RandomProperty+ "', '"+this.RandomSuffix+ "', '"+this.block+ "', '"+this.itemset+ "', '"+this.MaxDurability+ "', '"+this.area+ "', '"+this.Map+ "', '"+this.BagFamily+ "', '"+(int)this.TotemCategory+ "', '"+(int)this.socketColor_1+ "', '"+this.socketContent_1+ "', '" + (int)this.socketColor_2 + "', '" + this.socketContent_2 + "', '" + (int)this.socketColor_3 + "', '" + this.socketContent_3 + "', '"+ this.socketBonus +"', '"+this.GemProperties+ "', '"+this.RequiredDisenchantSkill+ "', '"+this.ArmorDamageModifier+ "', '"+this.duration+ "', '"+this.ItemLimitCategory+ "', '"+this.HolidayId+ "', '"+this.ScriptName+ "', '"+this.DisenchantID+ "', '"+(int)this.FoodType+ "', '"+this.minMoneyLoot+ "', '"+this.maxMoneyLoot+ "', '"+this.flagsCustom+ "', '"+this.VerifiedBuild+"');";
        }

        public static Item loadItem(DataTable pDataTable)
        {
            var dRow = pDataTable.Rows[0];

            var item = new Item()
            {
                EntryId = int.Parse(dRow[0].ToString()),
                Class = (itemClass)Enum.Parse(typeof(itemClass), dRow[1].ToString()),
                SubClass = int.Parse(dRow[2].ToString()),
                SoundOverrideSubclass = int.Parse(dRow[3].ToString()),
                name = dRow[4].ToString(),
                displayid = int.Parse(dRow[5].ToString()),
                Quality = (itemQuality)Enum.Parse(typeof(itemQuality), dRow[6].ToString()),
                ItemFlags = uint.Parse(dRow[7].ToString()),
                ItemFlagsExtra = uint.Parse(dRow[8].ToString()),
                BuyCount = int.Parse(dRow[9].ToString()),
                BuyPrice = uint.Parse(dRow[10].ToString()),
                SellPrice = uint.Parse(dRow[11].ToString()),
                InventoryType = (itemInventoryType)Enum.Parse(typeof(itemInventoryType), dRow[12].ToString()),
                AllowableClass = int.Parse(dRow[13].ToString()),
                AllowableRace = int.Parse(dRow[14].ToString()),
                ItemLevel = int.Parse(dRow[15].ToString()),
                RequiredLevel = int.Parse(dRow[16].ToString()),
                RequiredSkill = int.Parse(dRow[17].ToString()),
                RequiredSkillRank = int.Parse(dRow[18].ToString()),
                requiredspell = int.Parse(dRow[19].ToString()),
                requiredhonorrank = int.Parse(dRow[20].ToString()),
                RequiredCityRank = int.Parse(dRow[21].ToString()),
                RequiredReputationFaction = int.Parse(dRow[22].ToString()),
                RequiredReputationRank = (itemReputationRank)Enum.Parse(typeof(itemReputationRank), dRow[23].ToString()),
                maxcount = int.Parse(dRow[24].ToString()),
                stackable = int.Parse(dRow[25].ToString()),
                ContainerSlots = int.Parse(dRow[26].ToString()),
                StatsCount = int.Parse(dRow[27].ToString()),
                stat_type1 = (itemStatType)Enum.Parse(typeof(itemStatType), dRow[28].ToString()),
                stat_value1 = int.Parse(dRow[29].ToString()),
                stat_type2 = (itemStatType)Enum.Parse(typeof(itemStatType), dRow[30].ToString()),
                stat_value2 = int.Parse(dRow[31].ToString()),
                stat_type3 = (itemStatType)Enum.Parse(typeof(itemStatType), dRow[32].ToString()),
                stat_value3 = int.Parse(dRow[33].ToString()),
                stat_type4 = (itemStatType)Enum.Parse(typeof(itemStatType), dRow[34].ToString()),
                stat_value4 = int.Parse(dRow[35].ToString()),
                stat_type5 = (itemStatType)Enum.Parse(typeof(itemStatType), dRow[36].ToString()),
                stat_value5 = int.Parse(dRow[37].ToString()),
                stat_type6 = (itemStatType)Enum.Parse(typeof(itemStatType), dRow[38].ToString()),
                stat_value6 = int.Parse(dRow[39].ToString()),
                stat_type7 = (itemStatType)Enum.Parse(typeof(itemStatType), dRow[40].ToString()),
                stat_value7 = int.Parse(dRow[41].ToString()),
                stat_type8 = (itemStatType)Enum.Parse(typeof(itemStatType), dRow[42].ToString()),
                stat_value8 = int.Parse(dRow[43].ToString()),
                stat_type9 = (itemStatType)Enum.Parse(typeof(itemStatType), dRow[44].ToString()),
                stat_value9 = int.Parse(dRow[45].ToString()),
                stat_type10 = (itemStatType)Enum.Parse(typeof(itemStatType), dRow[46].ToString()),
                stat_value10 = int.Parse(dRow[47].ToString()),
                ScalingStatDistribution = int.Parse(dRow[48].ToString()),
                ScalingStatValue = int.Parse(dRow[49].ToString()),
                dmg_min1 = float.Parse(dRow[50].ToString()),
                dmg_max1 = float.Parse(dRow[51].ToString()),
                dmg_type1 = (itemDmgType)Enum.Parse(typeof(itemDmgType), dRow[52].ToString()),
                dmg_min2 = float.Parse(dRow[53].ToString()),
                dmg_max2 = float.Parse(dRow[54].ToString()),
                dmg_type2 = (itemDmgType)Enum.Parse(typeof(itemDmgType), dRow[55].ToString()),
                armor = int.Parse(dRow[56].ToString()),
                holy_res = int.Parse(dRow[57].ToString()),
                fire_res = int.Parse(dRow[58].ToString()),
                nature_res = int.Parse(dRow[59].ToString()),
                frost_res = int.Parse(dRow[60].ToString()),
                shadow_res = int.Parse(dRow[61].ToString()),
                arcane_res = int.Parse(dRow[62].ToString()),
                delay = int.Parse(dRow[63].ToString()),
                ammo_type = (itemAmmoType)Enum.Parse(typeof(itemAmmoType), dRow[64].ToString()),
                RangedModRange = float.Parse(dRow[65].ToString()),
                spellid_1 = int.Parse(dRow[66].ToString()),
                spelltrigger_1 = (itemSpellTrigger)Enum.Parse(typeof(itemSpellTrigger), dRow[67].ToString()),
                spellcharges_1 = int.Parse(dRow[68].ToString()),
                spellppmRate_1 = float.Parse(dRow[69].ToString()),
                spellcooldown_1 = int.Parse(dRow[70].ToString()),
                spellcategory_1 = int.Parse(dRow[71].ToString()),
                spellcategorycooldown_1 = int.Parse(dRow[72].ToString()),
                spellid_2 = int.Parse(dRow[73].ToString()),
                spelltrigger_2 = (itemSpellTrigger)Enum.Parse(typeof(itemSpellTrigger), dRow[74].ToString()),
                spellcharges_2 = int.Parse(dRow[75].ToString()),
                spellppmRate_2 = float.Parse(dRow[76].ToString()),
                spellcooldown_2 = int.Parse(dRow[77].ToString()),
                spellcategory_2 = int.Parse(dRow[78].ToString()),
                spellcategorycooldown_2 = int.Parse(dRow[79].ToString()),
                spellid_3 = int.Parse(dRow[80].ToString()),
                spelltrigger_3 = (itemSpellTrigger)Enum.Parse(typeof(itemSpellTrigger), dRow[81].ToString()),
                spellcharges_3 = int.Parse(dRow[82].ToString()),
                spellppmRate_3 = float.Parse(dRow[83].ToString()),
                spellcooldown_3 = int.Parse(dRow[84].ToString()),
                spellcategory_3 = int.Parse(dRow[85].ToString()),
                spellcategorycooldown_3 = int.Parse(dRow[86].ToString()),
                spellid_4 = int.Parse(dRow[87].ToString()),
                spelltrigger_4 = (itemSpellTrigger)Enum.Parse(typeof(itemSpellTrigger), dRow[88].ToString()),
                spellcharges_4 = int.Parse(dRow[89].ToString()),
                spellppmRate_4 = float.Parse(dRow[90].ToString()),
                spellcooldown_4 = int.Parse(dRow[91].ToString()),
                spellcategory_4 = int.Parse(dRow[92].ToString()),
                spellcategorycooldown_4 = int.Parse(dRow[93].ToString()),
                spellid_5 = int.Parse(dRow[94].ToString()),
                spelltrigger_5 = (itemSpellTrigger)Enum.Parse(typeof(itemSpellTrigger), dRow[95].ToString()),
                spellcharges_5 = int.Parse(dRow[96].ToString()),
                spellppmRate_5 = float.Parse(dRow[97].ToString()),
                spellcooldown_5 = int.Parse(dRow[98].ToString()),
                spellcategory_5 = int.Parse(dRow[99].ToString()),
                spellcategorycooldown_5 = int.Parse(dRow[100].ToString()),
                bonding = (itemBonding)Enum.Parse(typeof(itemBonding), dRow[101].ToString()),
                description = dRow[102].ToString(),
                PageText = int.Parse(dRow[103].ToString()),
                LanguageID = int.Parse(dRow[104].ToString()),
                PageMaterial = int.Parse(dRow[105].ToString()),
                startquest = int.Parse(dRow[106].ToString()),
                lockid = int.Parse(dRow[107].ToString()),
                Material = (itemMaterial)Enum.Parse(typeof(itemMaterial), dRow[108].ToString()),
                sheath = (itemSheath)Enum.Parse(typeof(itemSheath), dRow[109].ToString()),
                RandomProperty = int.Parse(dRow[110].ToString()),
                RandomSuffix = int.Parse(dRow[111].ToString()),
                block = int.Parse(dRow[112].ToString()),
                itemset = int.Parse(dRow[113].ToString()),
                MaxDurability = int.Parse(dRow[114].ToString()),
                area = int.Parse(dRow[115].ToString()),
                Map = int.Parse(dRow[116].ToString()),
                BagFamily = int.Parse(dRow[117].ToString()),
                TotemCategory = (itemTotemCategory)Enum.Parse(typeof(itemTotemCategory), dRow[118].ToString()),
                socketColor_1 = (itemSocketColor)Enum.Parse(typeof(itemSocketColor), dRow[119].ToString()),
                socketContent_1 = int.Parse(dRow[120].ToString()),
                socketColor_2 = (itemSocketColor)Enum.Parse(typeof(itemSocketColor), dRow[121].ToString()),
                socketContent_2 = int.Parse(dRow[122].ToString()),
                socketColor_3 = (itemSocketColor)Enum.Parse(typeof(itemSocketColor), dRow[123].ToString()),
                socketContent_3 = int.Parse(dRow[124].ToString()),
                socketBonus = int.Parse(dRow[125].ToString()),
                GemProperties = int.Parse(dRow[126].ToString()),
                RequiredDisenchantSkill = int.Parse(dRow[127].ToString()),
                ArmorDamageModifier = float.Parse(dRow[128].ToString()),
                duration = int.Parse(dRow[129].ToString()),
                ItemLimitCategory = int.Parse(dRow[130].ToString()),
                HolidayId = int.Parse(dRow[131].ToString()),
                ScriptName = dRow[132].ToString(),
                DisenchantID = int.Parse(dRow[133].ToString()),
                FoodType = (itemFoodType)Enum.Parse(typeof(itemFoodType), dRow[134].ToString()),
                minMoneyLoot = int.Parse(dRow[135].ToString()),
                maxMoneyLoot = int.Parse(dRow[136].ToString()),
                flagsCustom = int.Parse(dRow[137].ToString()),
                VerifiedBuild = int.Parse(dRow[138].ToString())
            };

            return item;
        }
    }
}
