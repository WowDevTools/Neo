using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Neo.Storage.Database.WotLk.TrinityCore
{
    class ItemManager : Singleton<ItemManager>, IItemManager
    {
        private readonly List<Item> mItem = new List<Item>();

        public void LoadItem(DataTable pDataTable)
        {
            //These line avoid sql error when the computer by-default separator is the comma ( like in belgium )
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            foreach (DataRow dRow in pDataTable.Rows)
            {
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
                mItem.Add(item);
            }
        }

        public void addCreatedItem(Item item)
        {
            mItem.Add(item);
        }

        public Item GetItemByEntry(int pEntryId)
        {
            return mItem.FirstOrDefault(item => item.EntryId == pEntryId);
        }
    }
}
