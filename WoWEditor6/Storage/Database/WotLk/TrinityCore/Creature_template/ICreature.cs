namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    public interface ICreature
    {
        int EntryId { get; set; }
        int DifficultyEntry1 { get; set; }
        int DifficultyEntry2 { get; set; }
        int DifficultyEntry3 { get; set; }
        int KillCredit1 { get; set; }
        int KillCredit2 { get; set; }
        int ModelId1 { get; set; }
        int ModelId2 { get; set; }
        int ModelId3 { get; set; }
        int ModelId4 { get; set; }
        string Name { get; set; }
        string SubName { get; set; }
        string IconName { get; set; }
        int GossipMenuId { get; set; }
        int MinLevel { get; set; }
        int MaxLevel { get; set; }
        int Experience { get; set; }
        int Faction { get; set; }
        uint NpcFlag { get; set; }
        float SpeedWalk { get; set; }
        float SpeedRun { get; set; }
        float Scale { get; set; }
        Rank Rank { get; set; }
        DamageSchool DamageSchool { get; set; }
        int BaseAttackTime { get; set; }
        int RangeAttackTime { get; set; }
        float BaseVariance { get; set; }
        float RangeVariance { get; set; }
        UnitClass UnitClass { get; set; }
        uint UnitFlags { get; set; }
        uint UnitFlags2 { get; set; }
        uint DynamicFlags { get; set; }
        Family Family { get; set; }
        TrainerType TrainerType { get; set; }
        int TrainerSpell { get; set; }
        int TrainerClass { get; set; }
        int TrainerRace { get; set; }
        CreatureType Type { get; set; }
        uint TypeFlags { get; set; }
        int LootId { get; set; }
        int PickPocketLoot { get; set; }
        int SkinLoot { get; set; }
        int Resistance1 { get; set; }
        int Resistance2 { get; set; }
        int Resistance3 { get; set; }
        int Resistance4 { get; set; }
        int Resistance5 { get; set; }
        int Resistance6 { get; set; }
        int Spell1 { get; set; }
        int Spell2 { get; set; }
        int Spell3 { get; set; }
        int Spell4 { get; set; }
        int Spell5 { get; set; }
        int Spell6 { get; set; }
        int Spell7 { get; set; }
        int Spell8 { get; set; }
        int PetSpellDataId { get; set; }
        int VehicleId { get; set; }
        int MinGold { get; set; }
        int MaxGold { get; set; }
        string AiName { get; set; }
        MovementType MovementType { get; set; }
        int InhabitType { get; set; }
        float HoverHeight { get; set; }
        float HealthModifier { get; set; }
        float ManaModifier { get; set; }
        float ArmorModifier { get; set; }
        float DamageModifier { get; set; }
        float ExperienceModifier { get; set; }
        int RacialLeader { get; set; }
        int MovementId { get; set; }
        int RegenHealth { get; set; }
        uint MechanicImmuneMask { get; set; }
        uint FlagsExtra { get; set; }
        string ScriptName { get; set; }
        int VerifiedBuild { get; set; }
        string GetUpdateSqlQuery();
        string GetInsertSqlQuery();
    }

    public enum MovementType
    {
        NoMovement = 0,
        Random = 1,
        Path = 2
    }

    public enum NpcFlag : uint
    {
        None = 0x0,
        Gossip = 0x1,
        QuestGiver = 0x2,
        Trainer = 0x10,
        ClassTrainer = 0x20,
        ProfessionTrainer = 0x40,
        Vendor = 0x80,
        VendorAmmo = 0x100,
        VendorFood = 0x200,
        VendorPoison = 0x400,
        VendorReagent = 0x800,
        Repairer = 0x1000,
        FlightMaster = 0x2000,
        SpiritHealer = 0x4000,
        SpiritGuide = 0x8000,
        Innkeeper = 0x10000,
        Banker = 0x20000,
        Petitioner = 0x40000,
        TabardDesigner = 0x80000,
        Battlemaster = 0x100000,
        Auctioneer = 0x200000,
        StableMaster = 0x400000,
        GuildBanker = 0x800000,
        Spellclick = 0x1000000,
        Mailbox = 0x4000000
    }

    public enum UnitFlags : uint
    {
        None = 0x0,
        ServerControlled = 0x1,
        NonAttackable = 0x2,
        DisableMove = 0x4,
        PvPAttackable = 0x8,
        Rename = 0x10,
        Preparation = 0x20,
        Unknown6 = 0x40,
        NotAttackable1 = 0x80,
        ImmuneToPc = 0x100,
        ImmuneToNpc = 0x200,
        Looting = 0x400,
        PetInCombat = 0x800,
        PvP = 0x1000,
        Silenced = 0x2000,
        CannotSwim = 0x4000,
        Unknown15 = 0x8000,
        Unknown16 = 0x10000,
        Pacified = 0x20000,
        Stunned = 0x40000,
        InCombat = 0x80000,
        TaxiFlight = 0x100000,
        Disarmed = 0x200000,
        Confused = 0x400000,
        Fleeing = 0x800000,
        PlayerControlled = 0x1000000,
        NotSelectable = 0x2000000,
        Skinnable = 0x4000000,
        Mount = 0x8000000,
        Unknown28 = 0x10000000,
        Unknown29 = 0x20000000,
        Sheathe = 0x40000000,
        Unknown31 = 0x80000000
    }

    public enum UnitFlags2
    {
        None = 0x0,
        FeignDeath = 0x1,
        Unknown1 = 0x2,
        IgnoreReputation = 0x4,
        ComprehendLang = 0x8,
        MirrorImage = 0x10,
        ForceMove = 0x40,
        DisarmOffhand = 0x80,
        DisarmRanged = 0x400,
        RegeneratePower = 0x800,
        AllowEnemyInteract = 0x4000,
        AllowCheatSpells = 0x40000,
    }

    public enum DynamicFlags
    {
        None = 0x0,
        Lootable = 0x1,
        TrackUnit = 0x2,
        Tapped = 0x4,
        TappedByPlayer = 0x8,
        SpecialInfo = 0x10,
        Dead = 0x20,
        ReferAFriend = 0x40,
        TappedByAllThreatList = 0x80
    }

    public enum Rank
    {
        Normal = 0,
        Elite = 1,
        RareElite = 2,
        Boss = 3,
        Rare = 4
    }

    public enum DamageSchool
    {
        Normal = 0,
        Holy = 1,
        Fire = 2,
        Nature = 3,
        Frost = 4,
        Shadow = 5,
        Arcane = 6
    }

    public enum UnitClass
    {
        Warrior = 1,
        Paladin = 2,
        Rogue = 4,
        Mage = 8
    }

    public enum TrainerType
    {
        Class = 0,
        Mounts = 1,
        Tradeskills = 2,
        Pets = 3
    }

    public enum Family
    {
        None = 0x0,
        Wolf = 1,
        Cat = 2,
        Spider = 3,
        Bear = 4,
        Boar = 5,
        Crocolisk = 6,
        CarrionBird = 7,
        Crab = 8,
        Gorrila = 9,
        Raptor = 11,
        Tallstrider = 12,
        Felhunter = 15,
        Voidwalker = 16,
        Succubus = 117,
        Doomguard = 19,
        Scorpid = 20,
        Turtle = 21,
        Imp = 23,
        Bat = 24,
        Hyena = 25,
        Owl = 26,
        WindSerpent = 27,
        RemoteControl = 28,
        Felguard = 29,
        Dragonhawk = 30,
        Ravager = 31,
        WarpStalker = 32,
        Sporebat = 33,
        NetherRay = 34,
        Serpent = 35,
        Moth = 37,
        Chimaera = 38,
        Devilsaur = 39,
        Ghoul = 40,
        Silithid = 41,
        Worm = 42,
        Rhino = 43,
        Wasp = 44,
        CoreHound = 45,
        SpiritBeast = 46
    }

    public enum CreatureType
    {
        None = 0,
        Beast = 1,
        Dragonkin = 2,
        Demon = 3,
        Elemental = 4,
        Giant = 5,
        Undead = 6,
        Humanoid = 7,
        Critter = 8,
        Mechanical = 9,
        NotSpecified = 10,
        Totem = 11,
        NonCombatPet = 12,
        GasCloud = 13
    }

    public enum TypeFlags
    {
        None = 0x0,
        Tameable = 0x1,
        Ghost = 0x2,
        Boss = 0x4,
        DoNotPlayWoundParryAnimation = 0x8,
        HideFactionTooltip = 0x10,
        Unknown6 = 0x20,
        SpellAttackable = 0x40,
        DeadInteract = 0x80,
        HerbLoot = 0x100,
        MiningLoot = 0x200,
        DontLogDeath = 0x400,
        MountedCombat = 0x800,
        AidPlayers = 0x1000,
        IsPetBarUsed = 0x2000,
        MaskUid = 0x4000,
        EngineerLoot = 0x8000,
        Exotic = 0x10000,
        UseDefaultCollisionBox = 0x20000,
        IsSiegeWeapon = 0x40000,
        ProjectileCollision = 0x80000,
        HideNameplate = 0x100000,
        DoNotPlayMountedAnimation = 0x200000,
        IsLinkAll = 0x400000,
        InteractOnlyWithCreator = 0x800000,
        ForceGossip = 0x8000000
    }

    public enum InhabitType
    {
        Ground = 1,
        Water = 2,
        Flying = 4
    }

    public enum MechanicImmuneMask
    {
        None = 0x0,
        Charm = 0x1,
        Disoriented = 0x2,
        Disarm = 0x4,
        Distract = 0x8,
        Fear = 0x10,
        Grip = 0x20,
        Root = 0x40,
        Pacify = 0x80,
        Silence = 0x100,
        Sleep = 0x200,
        Snare = 0x400,
        Stun = 0x800,
        Freeze = 0x1000,
        Knockout = 0x2000,
        Bleed = 0x4000,
        Bandage = 0x8000,
        Polymorph = 0x10000,
        Banish = 0x20000,
        Shield = 0x40000,
        Shackle = 0x80000,
        Mount = 0x100000,
        Infected = 0x200000,
        Turn = 0x400000,
        Horror = 0x800000,
        Invulnerability = 0x1000000,
        Interrupt = 0x2000000,
        Daze = 0x4000000,
        Discovery = 0x8000000,
        ImmuneShield = 0x10000000,
        Sapped = 0x20000000,
        Enraged = 0x40000000
    }

    public enum FlagsExtra
    {
        None = 0x0,
        InstanceBind = 0x1,
        Civilian = 0x2,
        NoParry = 0x4,
        NoParryHasten = 0x8,
        NoBlock = 0x10,
        NoCrush = 0x20,
        NoXpAtKill = 0x40,
        Trigger = 0x80,
        NoTaunt = 0x100,
        Worldevent = 0x4000,
        Guard = 0x8000,
        NoCrit = 0x20000,
        NoSkillgain = 0x40000,
        TauntDiminish = 0x80000,
        AllDiminish = 0x100000,
        NoPlayerDamageReq = 0x200000,
        //DungeonBoss = 0x10000000 - Will crash the core if set
        IgnorePathFinding = 0x20000000,
        ImmunityKnockback = 0x40000000
    } 
}