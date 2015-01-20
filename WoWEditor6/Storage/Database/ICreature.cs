using System;

namespace WoWEditor6.Storage.Database
{
    interface ICreature
    {
        public int EntryID { get; set; }
        public int DifficultyEntry1 { get; set; }
        public int DifficultyEntry2 { get; set; }
        public int DifficultyEntry3 { get; set; }
        public int KillCredit1{ get; set; }
        public int KillCredit2{ get; set; }
        public int ModelID1 { get; set; }
        public int ModelID2 { get; set; }
        public int ModelID3 { get; set; }
        public int ModelID4 { get; set; }
        public string Name { get; set; }
        public int SubName { get; set; }
        public int IconName { get; set; }
        public int GossipMenuID { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public int Experience { get; set; }
        public int Faction{ get; set; }
        public NPCFlag NPCFlag { get; set; }
        public float SpeedWalk { get; set; }
        public float SpeedRun { get; set; }
        public float Scale { get; set; }
        public Rank Rank { get; set; }
        public float MinDamage { get; set; }
        public float MaxDamage { get; set; }
        public DamageSchool DamageSchool{ get; set; }
        public int AttackPower{ get; set; }
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
        public int RacialLeader{ get; set; }
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
        public string ScriptName{ get; set; }
        public int WDBVerified { get; set; }
        public string GetUpdateSQLQuery();
    }

    public enum MovementType
    {
        NoMovement = 0,
        Random = 1,
        Path = 2
    }

    public enum NPCFlag
    {
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

    public enum UnitFlags
    {
        ServerControlled = 0x1,
        NonAttackable = 0x2,
        DisableMove = 0x4,
        PvPAttackable = 0x8,
        Rename = 0x10,
        Preparation = 0x20,
        Unknown6 = 0x40,
        NotAttackable1 = 0x80,
        ImmuneToPC = 0x100,
        ImmuneToNPC = 0x200,
        Looting = 0x400,
        PetInCombat = 0x800,
        PvP = 0x1000,
        Silenced = 0x2000,
        Unknown14 = 0x4000,
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

    public enum enumType
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
        Tameable = 0x1,
        Ghost = 0x2,
        Unknown3 = 0x4,
        Unknown4 = 0x8,
        Unknown5 = 0x10,
        Unknown6 = 0x20,
        Unknown7 = 0x40,
        DeadInteract = 0x80,
        HerbLoot = 0x100,
        MiningLoot = 0x200,
        Unknown11 = 0x400,
        MountedCombat = 0x800,
        AidPlayers = 0x1000,
        Unknown14 = 0x2000,
        Unknown15 = 0x4000,
        EngineerLoot = 0x8000,
        Exotic = 0x10000,
        Unknown18 = 0x20000,
        Unknown19 = 0x40000,
        Unknown20 = 0x80000,
        Unknown21 = 0x100000,
        Unknown22 = 0x200000,
        Unknown23 = 0x400000,
        Unknown24 = 0x800000,
    }

    public enum AIName
    {
        NullAI = "NullAI",
        AggressorAI = "AggressorAI",
        ReactorAI = "ReactorAI",
        GuardAI = "GuardAI",
        PetAI = "PetAI",
        TotemAI = "TotemAI",
        EventAI = "EventAI",
        SmartAI = "SmartAI"
    }

    public enum InhabitType
    {
        Ground = 1,
        Water = 2,
        Flying = 4
    }

    public enum MechanicImmuneMask
    {
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
        InstanceBind = 0x1,
        Civilian = 0x2,
        NoParry = 0x4,
        NoParryHasten = 0x8,
        NoBlock = 0x10,
        NoCrush = 0x20,
        NoXPAtKill = 0x40,
        Trigger = 0x80,
        NoTaunt = 0x100,
        Worldevent = 0x4000,
        Guard = 0x8000,
        NoCrit = 0x20000,
        NoSkillgain = 0x40000,
        TauntDiminish = 0x80000,
        AllDiminish = 0x100000
        //DungeonBoss - Will crash the core if set
    } 
}