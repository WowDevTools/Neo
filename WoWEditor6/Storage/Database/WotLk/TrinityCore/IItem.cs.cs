using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    public interface IItem
    {
        int EntryId { get; set; }
        itemClass Class { get; set; }
        int SubClass { get; set; }
        int SoundOverrideSubclass { get; set; }
        string name { get; set; }
        int displayid { get; set; }
        itemQuality Quality { get; set; }
        uint ItemFlags { get; set; }
        uint ItemFlagsExtra { get; set; }
        int BuyCount { get; set; }
        uint BuyPrice { get; set; }
        uint SellPrice { get; set; }
        itemInvenotryType InventoryType { get; set; }
        int AllowableClass { get; set; }
        int AllowableRace { get; set; }
        int ItemLevel { get; set; }
        int RequiredLevel { get; set; }
        int RequiredSkill { get; set; }
        int RequiredSkillRank { get; set; }
        int requiredspell { get; set; }
        int requiredhonorrank { get; set; }
        int RequiredCityRank { get; set; }
        int RequiredReputationFaction { get; set; }
        itemReputationRank RequiredReputationRank { get; set; }
        int maxcount { get; set; }
        int stackable { get; set; }
        int ContainerSlots { get; set; }
        int StatsCount { get; set; }
        itemStatType stat_type1 { get; set; }
        int stat_value1 { get; set; }
        itemStatType stat_type2 { get; set; }
        int stat_value2 { get; set; }
        itemStatType stat_type3 { get; set; }
        int stat_value3 { get; set; }
        itemStatType stat_type4 { get; set; }
        int stat_value4 { get; set; }
        itemStatType stat_type5 { get; set; }
        int stat_value5 { get; set; }
        itemStatType stat_type6 { get; set; }
        int stat_value6 { get; set; }
        itemStatType stat_type7 { get; set; }
        int stat_value7 { get; set; }
        itemStatType stat_type8 { get; set; }
        int stat_value8 { get; set; }
        itemStatType stat_type9 { get; set; }
        int stat_value9 { get; set; }
        itemStatType stat_type10 { get; set; }
        int stat_value10 { get; set; }
        int ScalingStatDistribution { get; set; }
        int ScalingStatValue { get; set; }
        float dmg_min1 { get; set; }
        float dmg_max1 { get; set; }
        itemDmgType dmg_type1 { get; set; }
        float dmg_min2 { get; set; }
        float dmg_max2 { get; set; }
        itemDmgType dmg_type2 { get; set; }
        int armor { get; set; }
        int holy_res { get; set; }
        int fire_res { get; set; }
        int nature_res { get; set; }
        int frost_res { get; set; }
        int shadow_res { get; set; }
        int arcane_res { get; set; }
        int delay { get; set; }
        itemAmmoType ammo_type { get; set; }
        float RangedModRange { get; set; }
        int spellid_1 { get; set; }
        itemSpellTrigger spelltrigger_1 { get; set; }
        int spellcharges_1 { get; set; }
        float spellppmRate_1 { get; set; }
        int spellcooldown_1 { get; set; }
        int spellcategory_1 { get; set; }
        int spellcategorycooldown_1 { get; set; }
        int spellid_2 { get; set; }
        itemSpellTrigger spelltrigger_2 { get; set; }
        int spellcharges_2 { get; set; }
        float spellppmRate_2 { get; set; }
        int spellcooldown_2 { get; set; }
        int spellcategory_2 { get; set; }
        int spellcategorycooldown_2 { get; set; }
        int spellid_3 { get; set; }
        itemSpellTrigger spelltrigger_3 { get; set; }
        int spellcharges_3 { get; set; }
        float spellppmRate_3 { get; set; }
        int spellcooldown_3 { get; set; }
        int spellcategory_3 { get; set; }
        int spellcategorycooldown_3 { get; set; }
        int spellid_4 { get; set; }
        itemSpellTrigger spelltrigger_4 { get; set; }
        int spellcharges_4 { get; set; }
        float spellppmRate_4 { get; set; }
        int spellcooldown_4 { get; set; }
        int spellcategory_4 { get; set; }
        int spellcategorycooldown_4 { get; set; }
        int spellid_5 { get; set; }
        itemSpellTrigger spelltrigger_5 { get; set; }
        int spellcharges_5 { get; set; }
        float spellppmRate_5 { get; set; }
        int spellcooldown_5 { get; set; }
        int spellcategory_5 { get; set; }
        int spellcategorycooldown_5 { get; set; }
        itemBonding bonding { get; set; }
        string description { get; set; }
        int PageText { get; set; }
        int LanguageID { get; set; }
        int PageMaterial { get; set; }
        int startquest { get; set; }
        int lockid { get; set; }
        itemMaterial Material { get; set; }
        itemSheath sheath { get; set; }
        int RandomProperty { get; set; }
        int RandomSuffix { get; set; }
        int block { get; set; }
        int itemset { get; set; }
        int MaxDurability { get; set; }
        int area { get; set; }
        int Map { get; set; }
        int BagFamily { get; set; }
        itemTotemCategory TotemCategory { get; set; }
        itemSocketColor socketColor_1 { get; set; }
        int socketContent_1 { get; set; }
        itemSocketColor socketColor_2 { get; set; }
        int socketContent_2 { get; set; }
        itemSocketColor socketColor_3 { get; set; }
        int socketContent_3 { get; set; }
        int socketBonus { get; set; }
        int GemProperties { get; set; }
        int RequiredDisenchantSkill { get; set; }
        float ArmorDamageModifier { get; set; }
        int duration { get; set; }
        int ItemLimitCategory { get; set; }
        int HolidayId { get; set; }
        string ScriptName { get; set; }
        int DisenchantID { get; set; }
        itemFoodType FoodType { get; set; }
        int VerifiedBuild { get; set; }
        int minMoneyLoot { get; set; }
        int maxMoneyLoot { get; set; }
        uint flagsCustom { get; set; }
        string GetUpdateSqlQuery();
        string GetInsertSqlQuery();
    }

    public enum itemClass
    {
        Consumable = 0,
        Container = 1,
        Weapon = 2,
        Gem = 3,
        Armor = 4,
        Reagent = 5,
        Projectile = 6,
        TradeGoods = 7,
        Generic = 8, //OBSOLETE
        Recipe = 9,
        Money = 10, //OBSOLETE
        Quiver = 11,
        Quest = 12,
        Key = 13,
        Permanent = 14, //OBSOLETE
        Miscellaneous = 15,
        Glyph = 16
    }

    public enum itemQuality
    {
        Poor = 0,
        Common = 1,
        Uncommon = 2,
        Rare = 3,
        Epic = 4,
        Legendary = 5,
        Artifact = 6,
        BindToAccount = 7
    }

    public enum itemFlags : uint
    {
        UNK1 = 0x1,
        ConjuredItem = 0x2,
        Openable = 0x4,
        Heroic = 0x8,
        Deprecated = 0x10,
        Indestructible = 0x20,
        UNK2 = 0x40,
        NoCooldownOnEquip = 0x80,
        UNK3 = 0x100,
        Wrapper = 0x200,
        UNK4 = 0x400,
        PartyLoot = 0x800,
        Refundable = 0x1000,
        Charter = 0x2000,
        UNK5 = 0x4000,
        UNK6 = 0x8000,
        UNK7 = 0x10000,
        UNK8 = 0x20000,
        Prospectable = 0x40000,
        UniqueEquipped = 0x80000,
        UNK9 = 0x100000,
        CanBeUsedInArena = 0x200000,
        Throwable = 0x400000,
        CanBeUsedInShapeshiftForms = 0x800000,
        UNK10 = 0x1000000,
        ProfessionRecipes = 0x2000000,
        CannotBeUsedInArena = 0x4000000,
        BindToAccount = 0x8000000,
        CastSpellWithTriggeredFlag = 0x10000000,
        Millable = 0x20000000,
        UNK11 = 0x40000000,
        BindOnPickupTradeable = 0x80000000
    }

    public enum itemFlagsExtra
    {
        HordeOnly = 0x1,
        AllianceOnly = 0x2,
        ExtendedCostPlusGold = 0x4,
        NeedRollDisabled = 0x100
    }

    public enum itemFlagsCustom
    {
        DurationRealTime = 0x1,
        IgnoreQuestStatus = 0x2,
        FollowLootRules = 0x4
    }

    public enum itemInvenotryType
    {
        NonEquipable = 0,
        Head = 1,
        Neck = 2,
        Shoulder = 3,
        Shirt = 4,
        Chest = 5,
        Waist = 6,
        Legs = 7,
        Feet = 8,
        Wrists = 9,
        Hands = 10,
        Finger = 11,
        Trinket = 12,
        Weapon = 13,
        Shield = 14,
        Ranged = 15, //Bow
        Back = 16,
        woHand = 17,
        Bag = 18,
        Tabard = 19,
        Robe = 20,
        MainHand = 21,
        OffHand = 22,
        Holdable = 23, //Tome
        Ammo = 24,
        Thrown = 25,
        RangedRight = 26, //Wands, Guns
        Quiver = 27,
        Relic = 28
    }

    public enum itemAllowableClass
    {
        All = -1,
        Warrior = 0x1,
        Paladin = 0x2,
        Hunter = 0x4,
        Rogue = 0x8,
        Priest = 0x10,
        DeathKnight = 0x20,
        Shaman = 0x40,
        Mage = 0x80,
        Warlock = 0x100,
        Druid = 0x400
    }

    public enum itemAllowableRace
    {
        All = -1,
        Human = 0x1,
        Orc = 0x2,
        Dwarf = 0x4,
        NightElf = 0x8,
        Undead = 0x10,
        Tauren = 0x20,
        Gnome = 0x40,
        Troll = 0x80,
        BloodElf = 0x200,
        Draenei = 0x400
    }

    public enum itemReputationRank
    {
        Hated = 0,
        Hostile = 1,
        Unfirendly = 2,
        Neutral = 3,
        Friendly = 4,
        Honored = 5,
        Revered = 6,
        Exalted = 7
    }

    public enum itemStatType
    {
        Mana = 0,
        Health = 1,
        Agility = 3,
        Strength = 4,
        Intellect = 5,
        Spirit = 6,
        Stamina = 7,
        DefenseSkillRating = 12,
        DodgeRating = 13,
        ParryRating = 14,
        BlockRating = 15,
        HitMeleeRating = 16,
        HitRangedRating = 17,
        HitSpellRating = 18,
        CritMeleeRating = 19,
        CritRangedRating = 20,
        CritSpellRating = 21,
        HitTakenMeleeRating = 22,
        HitTakenRangedRating = 23,
        HitTakenSpellRating = 24,
        CritTakenMeleeRating = 25,
        CritTakenRangedRating = 26,
        CritTakenSpellRating = 27,
        HasteMeleeRating = 28,
        HasteRangedRating = 29,
        HasteSpellRating = 30,
        HitRating = 31,
        CritRating = 32,
        HitTakenRating = 33,
        CritTakenRating = 34,
        ResilienceRating = 35,
        HasteRating = 36,
        ExpertiseRating = 37,
        AttackPower = 38,
        RangedAttackPower = 39,
        FeralAttackPower = 40,
        SpellHealingDone = 41,
        SpellDamageDone = 42,
        ManaRegeneration = 43,
        ArmorPenetrationRating = 44,
        SpellPower = 45,
        HealthRegen = 46,
        SpellPenetration = 47,
        BlockValue = 48
    }

    public enum itemDmgType
    {
        Physical = 0,
        Holy = 1,
        Fire = 2,
        Nature = 3,
        Frost = 4,
        Shadow = 5,
        Arcane = 6
    }

    public enum itemAmmoType
    {
        Arrows = 2,
        Bullets = 3
    }

    public enum itemSpellTrigger
    {
        Use = 0,
        OnEquip = 1,
        ChanceOnHit = 2,
        Soulstone = 4,
        UseWithNoDelay = 5,
        LearnSpellId
    }

    public enum itemBonding
    {
        NoBounds = 0,
        BindsWhenPickedUp = 1,
        BindWhenEquipped = 2,
        BindsWhenUsed = 3,
        QuestItem = 4,
        QuestItem1= 5
    }

    public enum itemMaterial
    {
        Consumables = -1,
        NotDefined = 0,
        Metal = 1,
        Wood = 2,
        Liquid = 3,
        Jewelry = 4,
        Chain = 5,
        Palte = 6,
        Cloth = 7,
        Leather = 8
    }

    public enum itemSheath
    {
        TwoHandedWeapon = 1,
        Staff = 2,
        OneHanded = 3,
        Shield = 4,
        EnchantersRod = 5,
        OffHand = 6
    }

    public enum itemBagFamily
    {
        None = 0x0,
        Arrows = 0x1,
        Bullets = 0x2,
        SoulShards = 0x4,
        LeatherWorkingSupplies = 0x8,
        InscriptionSupplies = 0x10,
        Herbs = 0x20,
        EnchantingSupplies = 0x40,
        EngineeringSupplies = 0x80,
        Keys = 0x100,
        Gems = 0x200,
        MiningSupplies = 0x400,
        SoulboundEquipment = 0x800,
        VanityPets = 0x1000,
        CurrecyTokens = 0x2000,
        QuestItems = 0x4000
    }

    public enum itemTotemCategory
    {
        //SkinningKnife = 1, OLD
        EarthTotem = 2,
        AirTotem = 3,
        FireTotem = 4,
        WaterTotem = 5,
        RunedCopperRod = 6,
        RunedSilverRod = 7,
        RunedGoldenRod = 8,
        RunedTruesilverRod = 9,
        RunedArcaniteRod = 10,
        //MiningPick = 11, OLD
        PhilosophersStone = 12,
        //BlacksmithHammer = 13, OLD
        ArclightSpanner = 14,
        GyromaticMicroAdjustor = 15,
        MasterTotem = 21,
        RunedFelIronRod = 41,
        RunedAdamantiteRod = 62,
        RunedEterniumRod = 63,
        HollowQuill = 81,
        RunedAzuriteRod = 101,
        VirtuosoInkingSet = 121,
        Drums = 141,
        GnomishArmyKnife = 161,
        MiningPick = 165,
        SkinningKnife = 166,
        HammerPick = 167,
        BladedPickaxe = 168,
        FlintAndTinder = 169,
        RunedCobaltRod = 189,
        RunedTitaniumRod = 190
    }

    public enum itemSocketColor
    {
        Meta = 1,
        Red = 2,
        Yellow = 4,
        Blue = 8
    }

    public enum itemFoodType
    {
        Meat = 1,
        Fish = 2,
        Cheese = 3,
        Bread = 4,
        Fungus = 5,
        Fruit = 6,
        RawMeat = 7,
        RawFish = 8
    }
}
