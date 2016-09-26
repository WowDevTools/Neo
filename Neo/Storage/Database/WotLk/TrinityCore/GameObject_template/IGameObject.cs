namespace WoWEditor6.Storage.Database.WotLk.TrinityCore
{
    public interface IGameObject
    {
        int EntryId { get; set; }
        EnumType Type { get; set; }
        int DisplayId { get; set; }
        string Name { get; set; }
        string IconName { get; set; }
        string CastBarCaption { get; set; }
        string Unknown1 { get; set; }
        int Faction { get; set; }
        uint Flags { get; set; }
        float Size { get; set; }
        int Data0 { get; set; }
        int Data1 { get; set; }
        int Data2 { get; set; }
        int Data3 { get; set; }
        int Data4 { get; set; }
        int Data5 { get; set; }
        int Data6 { get; set; }
        int Data7 { get; set; }
        int Data8 { get; set; }
        int Data9 { get; set; }
        int Data10 { get; set; }
        int Data11 { get; set; }
        int Data12 { get; set; }
        int Data13 { get; set; }
        int Data14 { get; set; }
        int Data15 { get; set; }
        int Data16 { get; set; }
        int Data17 { get; set; }
        int Data18 { get; set; }
        int Data19 { get; set; }
        int Data20 { get; set; }
        int Data21 { get; set; }
        int Data22 { get; set; }
        int Data23 { get; set; }
        string AiName { get; set; }
        string ScriptName { get; set; }
        int VerifiedBuild { get; set; }
        string GetUpdateSqlQuery();
        string GetInsertSqlQuery();
    }

    public enum EnumType
    {
        Door = 0,
        Button = 1,
        QuestGiver = 2,
        Chest = 3,
        Binder = 4,
        Generic = 5,
        Trap = 6,
        Chair = 7,
        SpellFocus = 8,
        Text = 9,
        Goober = 10,
        Transport = 11,
        AreaDamage = 12,
        Camera = 13,
        MapObject = 14,
        MoTransport = 15,
        DuelArbiter = 16,
        FishingNode = 17,
        Ritual = 18,
        Mailbox = 19,
        AuctionHouse = 20,
        Guardpost = 21,
        Spellcaster = 22,
        MeetingStone = 23,
        FlagStand = 24,
        FishingHole = 25,
        FlagDrop = 26,
        MiniGame = 27,
        LotteryKiosk = 28,
        CapturePoint = 29,
        AuraGenerator = 30,
        DungeonDifficulty = 31,
        BarberChair = 32,
        DestructibleBuilding = 33,
        GuildBank = 34,
        TrapDoor = 35
    }

    public enum Flags
    {
        InUse = 0x1,
        Locked = 0x2,
        InteractCond = 0x4,
        Transport = 0x8,
        NotSelectable = 0x10,
        NoDespawn = 0x20,
        Triggered = 0x40,
        Damaged = 0x200,
        Destroyed = 0x400
    }
}
