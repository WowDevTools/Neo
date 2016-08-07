using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBCLib.Structures335
{
    public class AchievementEntry
    {
        public uint Id;                             // 0
        public int FactionFlag;                     // 1 -1=all, 0=horde, 1=alliance
        public int MapId;                           // 2 -1=none / Only set if achievement is related to a zone
        public uint ParentAchievement;              // 3 its Achievement parent (can`t start while parent uncomplete, use its Criteria if don`t have own, use its progress on begin)
        public LocalizedString Name;                // 4-20
        public LocalizedString Description;         // 21-37 If Description is empty, it's not an Achievement but part of the statistics tab
        public uint CategoryId;                     // 38
        public uint Points;                         // 39 (0,5,10,15,20,25,30,50)
        public uint OrderInCategory;                // 40
        public uint Flags;                          // 41 0-768, if it's 256 only one person per Realm can reach that achievement and if it's 768 it's only reachable for one raid per realm
        public uint Icon;                           // 42
        public LocalizedString TitleReward;         // 43-59
        public uint Count;                          // 60 need this count of completed criterias (own or referenced achievement criterias)
        public uint RefAchievement;                 // 61 referenced achievement (counting of all completed criterias)

        public override string ToString()
        {
            return Name;
        }
    }

    public class AchievementCategoryEntry
    {
        public uint Id;                             // 0
        public int ParentCategory;                  // 1 -1 for main category
        public LocalizedString Name;                // 2-18
        public uint SortOrder;                      // 19

        public override string ToString()
        {
            return Name;
        }
    }

    public class AchievementCriteriaEntry
    {
        public uint Id;                             // 0
        public uint ReferredAchievement;            // 1 References the achievement this criteria is needed for. 
        public uint RequiredType;                   // 2
        public uint ReqType0;                       // 3
        public uint ReqValue0;                      // 4
        public uint ReqType1;                       // 5
        public uint ReqValue1;                      // 6
        public uint ReqType2;                       // 7
        public uint ReqValue2;                      // 8
        public LocalizedString Name;                // 9-25
        public uint Flags;                          // 26
        public uint TimedType;                      // 27
        public uint TimerStartEvent;                // 28
        public uint TimeLimit;                      // 29 Complete quest in %i seconds. 
        public uint ShowOrder;                      // 30

        public override string ToString()
        {
            return Name;
        }
    }

    public class AnimationDataEntry
    {
        public uint Id;                             // 0
        public string Name;                         // 1
        public uint WeaponFlags;                    // 2 32 = pull weapons out during animation. 16 and 4 weapons are put back. 
        public uint BodyFlags;                      // 3 Flags of some sort. 
        public uint Flags;                          // 4
        public uint FallBack;                       // 5 The animation, preceeding this one. 
        public uint BehaviorId;                     // 6 Same as ID for normal animations.
        public uint BehaviorTier;                   // 7 0 for normal, 3 for fly.

        public override string ToString()
        {
            return Name;
        }
    }

    public class AreaGroupEntry
    {
        public uint AreaGroupId;                    // 0
        public uint[] AreaId = new uint[6];         // 1-6
        public uint NextGroup;                      // 7 And if these 6 aren't enough, at this ID there are more entries

        public override string ToString()
        {
            return AreaGroupId.ToString();
        }
    }

    public class AreaPOIEntry
    {
        public uint Id;                             // 0
        public uint Importance;                     // 1
        public uint NormalIcon;                     // 2 This is getting displayed normally.
        public uint NormalIcon50p;                  // 3 Destructible building being neutral at 50%. 
        public uint NormalIcon0p;                   // 4 Destroyed neutral building. 
        public uint HordeIcon;                      // 5 Building at 100% captured by the horde. 
        public uint HordeIcon50p;                   // 6 Destructible building being neutral at 50%. 
        public uint HordeIcon0p;                    // 7 Destroyed horde building. 
        public uint AllianceIcon;                   // 8 Building at 100% captured by the alliance. 
        public uint AllianceIcon50p;                // 9 Destructible building being neutral at 50%. 
        public uint AllianceIcon0p;                 // 10 Destroyed alliance building. 
        public uint FactionId;                      // 11 This being an icon would make no sense. Walls for cities? Oo 
        public float X;                             // 12 Coordinates of the POI. Global ones. 
        public float Y;                             // 13
        public float Z;                             // 14
        public uint ContinentId;                    // 15 And on which map that POI is. 
        public uint Flags;                          // 16 Flags defining, where this icon is shown. &4: Zone, &128: BG, &512: showInBattle(mini)Map
        public uint Area;                           // 17
        public LocalizedString Name;                // 18-34
        public LocalizedString Description;         // 35-51 The alert triggered on the zone (World Event/PvP states mostly). 
        public uint WorldState;                     // 52 Most likely the value defines the icon. 
        public uint WorldMapLink;                   // 53

        public override string ToString()
        {
            return Name;
        }
    }

    public class AreaTableEntry
    {
        public uint Id;                             // 0
        public uint MapId;                          // 1 Map.dbc ref
        public uint ParentId;                       // 2 Recursive. If this is a sub-area, it will link to its parent area.
        public uint ExploreFlag;                    // 3
        public uint Flags;                          // 4
        public uint RefSoundPref;                   // 5 
        public uint RefSoundPrefUWater;             // 6
        public uint RefSoundAmbi;                   // 7
        public uint RefZoneMusic;                   // 8
        public uint RefZoneIntro;                   // 9
        public int AreaLevel;                       // 10
        public LocalizedString AreaName;            // 11-27
        public uint RefFactionGroup;                // 28
        public uint[] LiquidType = new uint[4];     // 29-32 [0]: Water, [1]: Ocean, [2]: Magma, [3]: Slime
        public float MinElevation;                  // 33
        public float AmbientMultiplier;             // 34
        public uint LightId;                        // 35

        public override string ToString()
        {
            return AreaName;
        }
    }

    public class AreaTriggerEntry
    {
        public uint Id;                             // 0
        public uint MapId;                          // 1
        public float x;                             // 2
        public float y;                             // 3
        public float z;                             // 4
        public float Radius;                        // 5
        public float BoxX;                          // 6
        public float BoxY;                          // 7
        public float BoxZ;                          // 8
        public float BoxOrientation;                // 9

        public override string ToString()
        {
            return Id.ToString();
        }
    }

    public class AttackAnimKitsEntry
    {
        public uint Id;                             // 0
        public uint Animation;                      // 1 Non-combat animid most of the time. 
        public uint Type;                           // 2
        public uint Flags;                          // 3
        public uint Unknown;                        // 4 Only 1 for type being OffH_*. 

        public override string ToString()
        {
            return Id.ToString();
        }
    }

    public class AttackAnimTypesEntry
    {
        public uint Id;                             // 0
        public string Name;                         // 1

        public override string ToString()
        {
            return Name;
        }
    }

    public class AuctionHouseEntry
    {
        public uint HouseId;                        // 0
        public uint Faction;                        // 1
        public uint DepositPercent;                 // 2
        public uint CutPercent;                     // 3
        public LocalizedString Name;                // 4-20

        public override string ToString()
        {
            return Name;
        }
    }

    public class BankBagSlotPricesEntry
    {
        public uint Id;                             // 0
        public uint Price;                          // 1
    }

    public class BannedAddOnsEntry
    {
        public uint Id;                             // 0
        //public ? NameMD5;                         // 1
        //public ? VersionMD5;                      // 2
        public uint LastModified;                   // 3
        public uint Flags;                          // 4
    }

    public class BarberShopStyleEntry
    {
        public uint Id;                             // 0
        public uint Type;                           // 1
        public string Name;                         // 2-18
        public string UnkName;                      // 19-35
        public float CostMultiplier;                // 36
        public uint Race;                           // 37
        public uint Gender;                         // 38
        public uint HairId;                         // 39 (real ID to hair/facial hair)

        public override string ToString()
        {
            return Name;
        }
    }

    public class BattlemasterListEntry
    {
        public uint Id;                             // 0
        public uint[] Instance = new uint[8];       // 1-8
        public uint InstanceType;                   // 9 3 - BG, 4 - arena
        public uint JoinAsGroup;                    // 10
        public LocalizedString Name;                // 11-27
        public uint MaxGroupSize;                   // 28
        public uint HolidayWorldStateId;            // 29
        public uint MinLevel;                       // 30
        public uint MaxLevel;                       // 31

        public override string ToString()
        {
            return Name;
        }
    }

    public class CameraShakesEntry
    {
        public uint Id;                             // 0
        public uint ShakeType;                      // 1
        public uint Direction;                      // 2
        public float Amplitude;                     // 3
        public float Frequency;                     // 4
        public float Duration;                      // 5
        public float Phase;                         // 6
        public float Coefficient;                   // 7

        public override string ToString()
        {
            return Id.ToString();
        }
    }

    public class CfgCategoriesEntry
    {
        public uint Index;                          // 0 categoryId (sent in RealmList packet)
        public uint LocalMask;                      // 1
        public uint CharsetMask;                    // 2
        public uint IsTournamentRealm;              // 3
        public LocalizedString CategoryName;        // 4-20

        public override string ToString()
        {
            return CategoryName;
        }
    }

    public class CfgConfigsEntry
    {
        public uint Id;                             // 0
        public uint Type;                           // 1 (sent in RealmList packet)
        public uint IsPvp;                          // 2
        public uint IsRp;                           // 3
    }

    public class CharacterFacialHairStylesEntry
    {

    }

    [NoPrimary]
    public class CharBaseInfoEntry
    {
        public byte RaceId;
        public byte ClasseId;
    }

    public class CharHairGeosetsEntry
    {

    }

    public class CharHairTexturesEntry
    {

    }

    public class CharSectionsEntry
    {

    }

    public class CharStartOutfitEntry
    {
        public uint Id;                             // 0
        public uint RaceClassGender;                // 1
        public int[] ItemId = new int[24];          // 2-25
        public int[] ItemDisplayId = new int[24];   // 26-49
        public int[] ItemInventtorySlot = new int[24];// 50-73
        public uint Unk1;                           // 74 (index-like with gaps ordered in other way as ids)
        public uint Unk2;                           // 75
        public uint Unk3;                           // 76
    }

    public class CharTitlesEntry
    {
        public uint Id;                             // 0
        public uint ConditionId;                    // 1 This is never used by the client. Still looks like pointing somewhere. Serverside?
        public LocalizedString NameMale;            // 2-18
        public LocalizedString NameFemale;          // 19-35
        public uint TitleMaskId;                    // 36 Used ingame in the drop down menu.

        public override string ToString()
        {
            return NameMale;
        }
    }

    public class CharVariationsEntry
    {

    }

    public class ChatChannelsEntry
    {

    }

    public class ChatProfanityEntry
    {
        public uint Id;
        public String DirtyWord;
        public uint LanguageID;                     // Col Added in 2.0.0.5849. English = 0, AsianLang = 1, French=2, German = 3, etc...?
    }

    public class ChrClassesEntry
    {
        public uint ClassId;                        // 0
        public uint Flags;                          // 1
        public uint PowerType;                      // 2
        public string PetNameToken;                 // 3
        public LocalizedString Name;                // 4-20
        public LocalizedString NameFemale;          // 21-37
        public LocalizedString NameNeutral;         // 38-54
        public string Filename;                     // 55
        public uint SpellClassSet;                  // 56
        public uint Flags2;                         // 57
        public uint CinematicSequenceId;            // 58
        public uint RequiredExpansion;              // 59

        public override string ToString()
        {
            return Name;
        }
    }

    public class ChrRacesEntry
    {
        public uint RaceId;                         // 0
        public uint Flags;                          // 1
        public uint FactionId;                      // 2
        public uint ExplorationSoundId;             // 3
        public uint ModelM;                         // 4
        public uint ModelF;                         // 5
        public string ClientPrefix;                 // 6
        public uint BaseLanguage;                   // 7
        public uint CreatureType;                   // 8
        public uint ResSicknessSpellId;             // 9
        public uint SplashSoundId;                  // 10
        public string ClientFileString;             // 11
        public uint CinematicSequenceId;            // 12
        public uint Alliance;                       // 13
        public LocalizedString RaceNameMale;        // 14-30
        public LocalizedString RaceNameFemale;      // 31-47
        public LocalizedString RaceNameNeutral;     // 48-64
        public string FacialHairCustomization;      // 65
        public string FacialHairCustomization2;     // 66
        public string HairCustomization;            // 67
        public uint RequiredExpansion;              // 68

        public override string ToString()
        {
            return RaceNameMale;
        }
    }

    public class CinematicCameraEntry
    {
        public uint Id;                             // 0
        public string Filename;                     // 1
        public uint SoundId;                        // 2
        public float StartX;                        // 3
        public float StartY;                        // 4
        public float StartZ;                        // 5
        public float StartFacing;                   // 6
    }

    public class CinematicSequencesEntry
    {
        public uint Id;                             // 0
        public uint SoundId;                        // 1
        public uint[] CinematicCamera = new uint[8];// 2-9
    }

    public class CreatureDisplayInfoEntry
    {

    }

    public class CreatureDisplayInfoExtraEntry
    {

    }

    public class CreatureFamilyEntry
    {

    }

    public class CreatureModelDataEntry
    {

    }

    public class CreatureMovementInfoEntry
    {

    }

    public class CreatureSoundDataEntry
    {

    }

    public class CreatureSpellDataEntry
    {

    }

    public class CreatureTypeEntry
    {
        public uint Id;                             // 0
        public LocalizedString Name;                // 1-17
        public uint Flags;                          // 18

        public override string ToString()
        {
            return Name;
        }
    }

    public class CurrencyCategoryEntry
    {
        public uint Id;                             // 0
        public uint Flags;                          // 1
        public LocalizedString Name;                // 2-18

        public override string ToString()
        {
            return Name;
        }
    }

    public class CurrencyTypesEntry
    {
        public uint Id;                             // 0
        public uint ItemId;                         // 1 used as real index
        public uint Category;                       // 2 may be category
        public uint BitIndex;                       // 3 bit index in PLAYER_FIELD_KNOWN_CURRENCIES (1 << (index-1))
    }

    public class DanceMovesEntry
    {

    }

    public class DeathThudLookupsEntry
    {

    }

    public class DeclinedWordEntry
    {

    }

    public class DeclinedWordCasesEntry
    {

    }

    public class DestructibleModelDataEntry
    {

    }

    public class DungeonEncounterEntry
    {

    }

    public class DungeonMapEntry
    {
        public uint Id;                             // 0
        public uint Map;                            // 1
        public uint Layer;                          // 2 for instances with multiple floors
        public float[] Coordonates = new float[4];  // 3-6 in which range in x and y is this shown. used for scaling.
        public uint Area;                           // 7 if only used in a specific area. see the northrend one.

        public override string ToString()
        {
            return Id.ToString();
        }
    }

    public class DungeonMapChunkEntry
    {

    }

    public class DurabilityCostsEntry
    {
        public uint ItemLvl;                        // 0
        public uint[] Multiplier = new uint[29];    // 1-29
    }

    public class DurabilityQualityEntry
    {
        public uint Id;                             // 0
        public float QualityMod;                    // 1
    }

    public class EmotesEntry
    {

    }

    public class EmotesTextEntry
    {

    }

    public class EmotesTextDataEntry
    {

    }

    public class EmotesTextSoundEntry
    {

    }

    public class EnvironmentalDamageEntry
    {

    }

    public class ExhaustionEntry
    {

    }

    public class FactionEntry
    {
        public uint Id;                             // 0
        public int ReputationListId;                // 1 Each faction that has gainable rep has a unique number. All factions that you can not gain rep with have -1. 
        public uint[] BaseRepRaceMask = new uint[4];// 2-5
        public uint[] BaseRepClassMask = new uint[4];// 6-9
        public int[] BaseRepValue = new int[4];     // 10-13 Based on 0 = Neutral
        public uint[] ReputationFlags = new uint[4];// 14-17
        public uint ParentFactionId;                // 18
        public float SpilloverRateIn;               // 19 Faction gains incoming rep * spilloverRateIn
        public float SpilloverRateOut;              // 20 Faction outputs rep * spilloverRateOut as spillover reputation
        public uint SpilloverMaxRankIn;             // 21 The highest rank the faction will profit from incoming spillover
        public uint SpilloverRankUnk;               // 22 It does not seem to be the max standing at which a faction outputs spillover ...so no idea
        public LocalizedString Name;                // 23-39
        public LocalizedString Description;         // 40-56

        public override string ToString()
        {
            return Id.ToString() + " - " + Name;
        }
    }

    public class FactionGroupEntry
    {
        public uint Id;                             // 0
        public uint MaskId;                         // 1
        public string InternalName;                 // 2
        public LocalizedString Name;                // 3-19

        public override string ToString()
        {
            return InternalName;
        }
    }

    public class FactionTemplateEntry
    {
        public uint Id;                             // 0
        public uint Faction;                        // 1 
        public uint FactionFlags;                   // 2
        public uint OurMask;                        // 3
        public uint FriendlyMask;                   // 4
        public uint HostileMask;                    // 5
        public uint[] EnemyFaction = new uint[4];   // 6-9
        public uint[] FriendFaction = new uint[4];  // 10-13

        public override string ToString()
        {
            return "FactionTemplate n°" + Id;
        }
    }

    public class FileDataEntry
    {

    }

    public class FootprintTexturesEntry
    {

    }

    public class FootstepTerrainLookupEntry
    {

    }

    public class GameObjectArtKitEntry
    {

    }

    public class GameObjectDisplayInfoEntry
    {

    }

    public class GameTablesEntry
    {

    }

    public class GameTipsEntry
    {
        public uint Id;                              // 0
        public LocalizedString Tips;                 // 1-17

        public override string ToString()
        {
            return Id + ":" + Tips;
        }
    }

    public class GemPropertiesEntry
    {
        public uint Id;
        public uint iRefID_SpellItemEnchantment;
        public uint MaxcountInv;
        public uint MaxcountItem;
        public uint Type;
    }

    public class GlyphPropertiesEntry
    {
        public uint Id;                             // 0
        public uint SpellId;                        // 1
        public uint GlyphSlotFlags;                 // 2
        public uint SpellIconId;                    // 3
    }

    public class GlyphSlotEntry
    {
        public uint Id;                             // 0
        public uint TypeFlags;                      // 1
        public uint Order;                          // 2
    }

    public class GMSurveyAnswersEntry
    {

    }

    public class GMSurveyCurrentSurveyEntry
    {

    }

    public class GMSurveyQuestionsEntry
    {

    }

    public class GMSurveySurveysEntry
    {

    }

    public class GMTicketCategoryEntry
    {

    }

    public class GroundEffectDoodadEntry
    {

    }

    public class GroundEffectTextureEntry
    {

    }

    [NoPrimary]
    public class gtBarberShopCostBaseEntry
    {
        public float Cost;                          // 0
    }

    [NoPrimary]
    public class gtChanceToMeleeCritEntry
    {
        public float Ratio;                         // 0
    }

    [NoPrimary]
    public class gtChanceToMeleeCritBaseEntry
    {
        public float Base;                          // 0
    }

    [NoPrimary]
    public class gtChanceToSpellCritEntry
    {
        public float Ratio;                         // 0
    }

    [NoPrimary]
    public class gtChanceToSpellCritBaseEntry
    {
        public float Base;                          // 0
    }

    [NoPrimary]
    public class gtCombatRatingsEntry
    {
        public float Ratio;                         // 0
    }

    public class gtNPCManaCostScalerEntry
    {

    }

    [NoPrimary]
    public class gtOCTClassCombatRatingScalarEntry
    {
        public float Ratio;                         // 0
    }

    [NoPrimary]
    public class gtOCTRegenHPEntry
    {
        public float Ratio;                         // 0
    }

    [NoPrimary]
    public class gtOCTRegenMPEntry
    {
        public float Ratio;                         // 0
    }

    [NoPrimary]
    public class gtRegenHPPerSptEntry
    {
        public float Ratio;                         // 0
    }

    [NoPrimary]
    public class gtRegenMPPerSptEntry
    {
        public float Ratio;                         // 0
    }

    public class HelmetGeosetVisDataEntry
    {

    }

    public class HolidayDescriptionsEntry
    {
        public uint Id;                             // 0
        public LocalizedString Name;                // 1-17

        public override string ToString()
        {
            return Name;
        }
    }

    public class HolidayNamesEntry
    {
        public uint Id;                             // 0
        public LocalizedString Name;                // 1-17

        public override string ToString()
        {
            return Name;
        }
    }

    public class HolidaysEntry
    {
        public uint Id;                             // 0
        public uint[] Duration = new uint[10];      // 1-10
        public uint[] Date = new uint[26];          // 11-36 (dates in unix time starting at January, 1, 2000)
        public uint Region;                         // 37 (wow region)
        public uint Looping;                        // 38
        public uint[] CalendarFlags = new uint[10]; // 39-48
        public uint HolidayNameId;                  // 49
        public uint HolidayDescriptionId;           // 50
        public string TextureFilename;              // 51
        public uint Priority;                       // 52
        public uint CalendarFilterType;             // 53
        public uint Flags;                          // 54
    }

    public class ItemEntry
    {
        public uint Id;                             // 0
        public uint Class;                          // 1
        public uint SubClass;                       // 2
        public int SoundOverrideSubClassId;         // 3
        public int Material;                        // 4
        public uint DisplayId;                      // 5
        public uint InventoryType;                  // 6
        public uint Sheath;                         // 7
    }

    public class ItemBagFamilyEntry
    {
        public uint Id;                             // 0
        public LocalizedString Name;                // 1-17

        public override string ToString()
        {
            return Name;
        }
    }

    public class ItemClassEntry
    {
        public uint Id;                             // 0
        public uint Unk1;                           // 1
        public uint Unk2;                           // 2 only weapon have 1 in field, other 0
        public string Name;                         // 3-19
    }

    public class ItemCondExtCostsEntry
    {

    }

    public class ItemDisplayInfoEntry
    {

    }

    public class ItemExtendedCostEntry
    {
        public uint Id;                             // 0
        public uint ReqHonorPoints;                 // 1
        public uint ReqArenaPoints;                 // 2
        public uint ReqArenaSlot;                   // 3
        public uint[] ReqItem = new uint[5];        // 4-8
        public uint[] ReqItemCount = new uint[5];   // 9-13
        public uint ReqPersonalArenaRating;         // 14
        public uint ItemPurchaseGroup;              // 15
    }

    public class ItemGroupSoundsEntry
    {

    }

    public class ItemLimitCategoryEntry
    {
        public uint Id;                             // 0
        public LocalizedString Name;                // 1-17
        public uint MaxCount;                       // 18 max allowed equipped as item or in gem slot
        public uint Mode;                           // 19 0 = have, 1 = equip (enum ItemLimitCategoryMode)

        public override string ToString()
        {
            return Name;
        }
    }

    public class ItemPetFoodEntry
    {

    }

    public class ItemPurchaseGroupEntry
    {

    }

    public class ItemRandomPropertiesEntry
    {
        public uint Id;                             // 0
        public string InternalName;                 // 1
        public uint[] EnchantId = new uint[5];      // 2-6
        public LocalizedString NameSuffix;          // 7-23

        public override string ToString()
        {
            return NameSuffix;
        }
    }

    public class ItemRandomSuffixEntry
    {
        public uint Id;                             // 0
        public LocalizedString NameSuffix;          // 1-17
        public string InternalName;                 // 18
        public uint[] EnchantId = new uint[5];      // 19-21
        public uint[] Prefix = new uint[5];         // 22-26
    }

    public class ItemSetEntry
    {
        public uint Id;                                 // 0
        public LocalizedString sRefName;                // 1-17
        public uint[] items = new uint[17];
        public uint[] spells = new uint[8];
        public uint[] nbItemSpells = new uint[8];
        public uint RequiredSkill;
        public uint RequiredLevelSkill;

        public override string ToString()
        {
            return sRefName;
        } 

    }

    public class ItemSubClassEntry
    {

    }

    public class ItemSubClassMaskEntry
    {

    }

    public class ItemVisualEffectsEntry
    {

    }

    public class ItemVisualsEntry
    {

    }

    public class LanguagesEntry
    {

    }

    public class LanguageWordsEntry
    {

    }

    public class LFGDungeonExpansionEntry
    {

    }

    public class LFGDungeonGroupEntry
    {

    }

    public class LFGDungeonsEntry
    {

    }

    public class LightEntry
    {
        public uint ID;
        public uint MapID;
        public float x;
        public float y;
        public float z;
        public float falloff;
        public float falloffEnd;
        public uint skyParam;
        public uint waterParam;
        public uint sunsetParam;
        public uint otherParam;
        public uint deathParam;
        public uint unk1, unk2, unk3;
    }

    public class LightFloatBandEntry
    {
        public uint ID;
        public uint NumEntries;
        public uint[] Times = new uint[16];
        public float[] Values = new float[16];
    }

    public class LightIntBandEntry
    {
        public uint ID;
        public uint NumEntries;
        public uint[] Times = new uint[16];
        public uint[] Values = new uint[16];
    }

    public class LightParamsEntry
    {
        public uint ID;
        public uint HighlightSky;
        public uint skyboxID;
        public uint cloudID;
        public float glow;
        public float waterShallowAlpha;
        public float waterDeepAlpha;
        public float oceanShallowAlpha;
        public float oceanDeepAlpha;
    }

    public class LightSkyboxEntry
    {
        public uint ID;
        public string Path;
        public uint Flags;
    }

    public class LiquidMaterialEntry
    {

    }

    public class LiquidTypeEntry
    {

    }

    public class LoadingScreensEntry
    {
        public uint ID;
        public string Name;
        public string Path;
        public uint wscreen;

        public override string ToString()
        {
            return Name;
        }
    }

    public class LoadingScreenTaxiSplinesEntry
    {

    }

    public class LockEntry
    {

    }

    public class LockTypeEntry
    {

    }

    public class MailTemplateEntry
    {

    }

    public class MapEntry
    {
        public uint ID;
        public string InternalName;
        public uint MapType;
        public uint AreaTable;
        public uint IsBattleground;
        public LocalizedString Name;
        public LocalizedString DescAlliance;
        public LocalizedString DescHorde;
        public uint TimeOfDay;
        public uint LoadScreen;
        public float BattleFieldScale;
        public float EntranceX;
        public float EntranceY;
        public uint Expansion;
        public uint ParentArea;
        public uint Unk;
        public uint NumPlayers;
        public uint Unk2;

        public override string ToString()
        {
            return Name;
        }
    }

    public class MapDifficultyEntry
    {

    }

    public class MaterialEntry
    {

    }

    public class MovieEntry
    {

    }

    public class MovieFileDataEntry
    {

    }

    public class MovieVariationEntry
    {

    }

    public class NameGenEntry
    {

    }

    public class NamesProfanityEntry
    {
        public uint Id;                             // 0
        public String Name;                         // 2-18
        public uint LanguageId;                     // 19-35

        public override string ToString()
        {
            return Name;
        }
    }

    public class NamesReservedEntry
    {
        public uint Id;                             // 0
        public String Name;                         // 2-18
        public uint LanguageId;                     // 19-35

        public override string ToString()
        {
            return Name;
        }
    }

    public class NPCSoundsEntry
    {

    }

    public class ObjectEffectEntry
    {

    }

    public class ObjectEffectGroupEntry
    {

    }

    public class ObjectEffectModifierEntry
    {

    }

    public class ObjectEffectPackageEntry
    {

    }

    public class ObjectEffectPackageElemEntry
    {

    }

    public class OverrideSpellDataEntry
    {

    }

    public class PackageEntry
    {

    }

    public class PageTextMaterialEntry
    {

    }

    public class PaperDollItemFrameEntry
    {

    }

    public class ParticleColorEntry
    {

    }

    public class PetitionTypeEntry
    {

    }

    public class PetPersonalityEntry
    {

    }

    public class PowerDisplayEntry
    {

    }

    public class PvpDifficultyEntry
    {

    }

    public class QuestFactionRewardEntry
    {

    }

    public class QuestInfoEntry
    {

    }

    public class QuestSortEntry
    {

    }

    public class QuestXPEntry
    {

    }

    public class RandPropPointsEntry
    {

    }

    public class ResistancesEntry
    {

    }

    public class ScalingStatDistributionEntry
    {

    }

    public class ScalingStatValuesEntry
    {

    }

    public class ScreenEffectEntry
    {

    }

    public class ServerMessagesEntry
    {

    }

    public class SheatheSoundLookupsEntry
    {

    }

    public class SkillCostsDataEntry
    {

    }

    public class SkillLineEntry
    {
        public uint Id;                             // 0
        public uint CategoryId;                     // 1
        public uint SkillCostId;                    // 2
        public LocalizedString Name;                // 3-19
        public LocalizedString Description;         // 20-36
        public uint SpellIcon;                      // 37
        public LocalizedString AlternateVerb;       // 38-54
        public uint CanLink;                        // 55

        public override string ToString()
        {
            return Name;
        }
    }

    public class SkillLineAbilityEntry
    {
        public uint Id;                             // 0
        public uint SkillId;                        // 1
        public uint SpellId;                        // 2
        public uint RequiredRaces;                  // 3
        public uint RequiredClasses;                // 4
        public uint ExcludedRaces;                  // 5
        public uint ExcludedClasses;                // 6
        public uint MinSkillLineRank;               // 7
        public uint SpellParent;                    // 8
        public uint AcquireMethod;                  // 9
        public uint SkillGreyLevel;                 // 10
        public uint SkillGreenLevel;                // 11
        public uint CharacterPoint1;                // 12
        public uint CharacterPoint2;                // 13
    }

    public class SkillLineCategoryEntry
    {

    }

    public class SkillRaceClassInfoEntry
    {
        public uint Id;                             // 0
        public uint SkillId;                        // 1
        public uint RaceMask;                       // 2
        public uint ClassMask;                      // 3
        public uint Flags;                          // 4
        public uint ReqLevel;                       // 5
        public uint SkillTierId;                    // 6
        public uint SkillCostId;                    // 7
    }

    public class SkillTiersEntry
    {

    }

    public class SoundAmbienceEntry
    {

    }

    public class SoundEmittersEntry
    {

    }

    public class SoundEntriesEntry
    {

    }

    public class SoundEntriesAdvancedEntry
    {

    }

    public class SoundFilterEntry
    {

    }

    public class SoundFilterElemEntry
    {

    }

    public class SoundProviderPreferencesEntry
    {

    }

    public class SoundSamplePreferencesEntry
    {

    }

    public class SoundWaterTypeEntry
    {

    }

    public class SpamMessagesEntry
    {

    }

    public class SpellEntry
    {
        public uint Id;                             // 0
        public uint Category;                       // 1
        public uint Dispel;                         // 2
        public uint Mechanic;                       // 3
        public uint Attributes;                     // 4
        public uint AttributesEx;                   // 5
        public uint AttributesEx2;                  // 6
        public uint AttributesEx3;                  // 7
        public uint AttributesEx4;                  // 8
        public uint AttributesEx5;                  // 9
        public uint AttributesEx6;                  // 10
        public uint AttributesEx7;                  // 11
        public uint Stances;                        // 12
        public uint Unk3201;                        // 13
        public uint StancesNot;                     // 14
        public uint Unk3202;                        // 15
        public uint Targets;                        // 16
        public uint TargetCreatureType;             // 17
        public uint RequiresSpellFocus;             // 18
        public uint FacingCasterFlags;              // 19
        public uint CasterAuraState;                // 20
        public uint TargetAuraState;                // 21
        public uint CasterAuraStateNot;             // 22
        public uint TargetAuraStateNot;             // 23
        public uint CasterAuraSpell;                // 24
        public uint TargetAuraSpell;                // 25
        public uint ExcludeCasterAuraSpell;         // 26
        public uint ExcludeTargetAuraSpell;         // 27
        public uint CastingTimeIndex;               // 28
        public uint RecoveryTime;                   // 29
        public uint CategoryRecoveryTime;           // 30
        public uint InterruptFlags;                 // 31
        public uint AuraInterruptFlags;             // 32
        public uint ChannelInterruptFlags;          // 33
        public uint ProcFlags;                      // 34
        public uint ProcChance;                     // 35
        public uint ProcCharges;                    // 36
        public uint MaxLevel;                       // 37
        public uint BaseLevel;                      // 38
        public uint SpellLevel;                     // 39
        public uint DurationIndex;                  // 40
        public uint PowerType;                      // 41
        public uint ManaCost;                       // 42
        public uint ManaCostPerLevel;               // 43
        public uint ManaPerSecond;                  // 44
        public uint ManaPerSecondPerLevel;          // 45
        public uint RangeIndex;                     // 46
        public float Speed;                         // 47
        public uint ModalNextSpell;                 // 48
        public uint StackAmount;                    // 49
        public uint[] Totem = new uint[2];          // 50-51
        public int[] Reagent = new int[8];          // 52-59
        public uint[] ReagentCount = new uint[8];   // 60-67    
        public int EquippedItemClass;               // 68       
        public int EquippedItemSubClassMask;        // 69       
        public int EquippedItemInventoryTypeMask;   // 70
        public uint[] Effect = new uint[3];         // 71-73 
        public int[] EffectDieSides = new int[3];   // 74-76  
        public float[] EffectRealPointsPerLevel = new float[3];// 77-79
        public int[] EffectBasePoints1 = new int[1]; // 80
        public int SkillRank;                       // 81
        public int[] EffectBasePoints2 = new int[1]; // 82 
        public uint[] EffectMechanic = new uint[3]; // 83-85  
        public uint[] EffectImplicitTargetA = new uint[3];// 86-88   
        public uint[] EffectImplicitTargetB = new uint[3];// 89-91    
        public uint[] EffectRadiusIndex = new uint[3];// 92-94
        public uint[] EffectApplyAuraName = new uint[3];// 95-97 
        public uint[] EffectAmplitude = new uint[3];// 98-100  
        public float[] EffectMultipleValue = new float[3];// 101-103 
        public uint[] EffectChainTarget = new uint[3];// 104-106  
        public uint[] EffectItemType = new uint[3]; // 107-109
        public uint[] EffectMiscValue1 = new uint[1]; // 110
        public uint SkillId;                         // 111
        public int[] EffectMiscValue2 = new int[1];  // 112
        public int[] EffectMiscValueB = new int[3]; // 113-115
        public uint SpellSkillId;                   // 116
        public uint[] EffectTriggerSpell1 = new uint[1];// 117
        public uint[] EffectTriggerSpell2 = new uint[1];// 118
        public float[] EffectPointsPerComboPoint = new float[3];// 119-121
        public float[] EffectSpellClassMask = new float[9];// 122-130 
        public uint[] SpellVisual = new uint[2];    // 131-132
        public uint SpellIconID;                    // 133      
        public uint ActiveIconID;                   // 134      
        public uint SpellPriority;                  // 135
        public LocalizedString SpellName;           // 136-152  
        public LocalizedString Rank;                // 153-169    
        public LocalizedString Description;         // 170-186
        public LocalizedString ToolTip;             // 187-203 
        public uint ManaCostPercentage;             // 204      
        public uint StartRecoveryCategory;          // 205      
        public uint StartRecoveryTime;              // 206      
        public uint MaxTargetLevel;                 // 207      
        public uint SpellFamilyName;                // 208      
        public uint SpellFamilyFlags;               // 209
        public uint SpellFamilyFlags1;              // 210
        public uint SpellFamilyFlags2;              // 211
        public uint MaxAffectedTargets;             // 212      
        public uint DmgClass;                       // 213      
        public uint PreventionType;                 // 214      
        public uint StanceBarOrder;                 // 215 
        public float[] DmgMultiplier = new float[3];// 216-218  
        public uint MinFactionId;                   // 219      
        public uint MinReputation;                  // 220      
        public uint RequiredAuraVision;             // 221   
        public uint[] TotemCategory = new uint[2];  // 222-223  
        public int AreaGroupId;                     // 224      
        public uint SchoolMask;                     // 225      
        public uint runeCostID;                     // 226      
        public uint spellMissileID;                 // 227      
        public uint PowerDisplayId;                 // 228   
        public float[] EffectBonusCoefficient = new float[3];// 229-231  
        public uint SpellDescriptionVariableID;     // 232      
        public uint SpellDifficultyId;              // 233    

        public override string ToString()
        {
            return Id + ", " + SpellName + " " + Rank;
        }

        public void InitJobRanks()
        {
            Attributes = 0x1010010;
            AttributesEx6 = 0x1000;
            Unk3201 = 0x0;
            StancesNot = 0x0;
            CastingTimeIndex = 1;
            ProcChance = 101;
            RangeIndex = 1;
            EquippedItemClass = -1;
            EquippedItemSubClassMask = 0;
            Effect[0] = 47;
            Effect[1] = 118;
            EffectDieSides[1] = 1;
            DmgMultiplier[0] = 1.0f;
            DmgMultiplier[1] = 1.0f;
            DmgMultiplier[2] = 1.0f;
            SchoolMask = 1;
        }
        public void InitJobSpellRanks(uint attr = 0x100, uint targ = 0x100)
        {
            Attributes = attr;
            AttributesEx6 = 0x0;
            Targets = targ;
            CastingTimeIndex = 1;
            ProcChance = 101;
            RangeIndex = 6;
            EquippedItemClass = -1;
            EquippedItemSubClassMask = 0;
            Effect[0] = 36;
            Effect[1] = 44;
            EffectDieSides[1] = 1;
            SpellVisual[0] = 107;
            DmgMultiplier[0] = 1.0f;
            DmgMultiplier[1] = 1.0f;
            DmgMultiplier[2] = 1.0f;
            SchoolMask = 1;
        }

        public void InitRecipe()
        {
            Attributes = 65584;
            AttributesEx = 1024;
            CastingTimeIndex = 14;
            InterruptFlags = 15;
            ProcChance = 101;
            RangeIndex = 1;
            EquippedItemClass = -1;
            Effect[0] = 24;
            EffectDieSides[0] = 1;
            EffectImplicitTargetA[0] = 1;
            SpellVisual[0] = 92;
            SpellIconID = 1;
            DmgMultiplier[0] = 1.0f;
            SchoolMask = 1;
        }
    }

    public class SpellCastTimesEntry
    {

    }

    public class SpellCategoryEntry
    {

    }

    public class SpellChainEffectsEntry
    {

    }

    public class SpellDescriptionVariablesEntry
    {

    }

    public class SpellDifficultyEntry
    {

    }

    public class SpellDispelTypeEntry
    {

    }

    public class SpellDurationEntry
    {

    }

    public class SpellEffectCameraShakesEntry
    {

    }

    public class SpellFocusObjectEntry
    {
        public uint Id;                             // 0
        public LocalizedString Name;               // 1-17

        public override string ToString()
        {
            return Name;
        }
    }

    public class SpellIconEntry
    {
        public uint Id;                             // 0
        public string IconPath;                     // 1

        public override string ToString()
        {
            return IconPath;
        }

        public string ToStringIdNameFile
        {
            get { return Id.ToString() + " - " + System.IO.Path.GetFileNameWithoutExtension(IconPath); }
        }
    }

    public class SpellItemEnchantmentEntry
    {
        public uint Id;
        public uint Charges;                // Mostly unused. Added 3.x?
        public uint SpellDispelType1;       // Enchantment Type of effect 1
        public uint SpellDispelType2;       // Enchantment Type of effect 2
        public uint SpellDispelType3;       // Enchantment Type of effect 3
        public uint MinAmount1;             // Amount of damage/armor/apply/spell for effect 1
        public uint MinAmount2;             // Amount of damage/armor/apply/spell for effect 2
        public uint MinAmount3;             // Amount of damage/armor/apply/spell for effect 3
        public uint MaxAmount1;             // Mostly dupe
        public uint MaxAmount2;             // Mostly dupe
        public uint MaxAmount3;             // Mostly dupe
        public uint ObjectId1;              // if type1 == 5, then Stat Types, else Spell.dbc 
        public uint ObjectId2;              // if type2 == 5, then Stat Types, else Spell.dbc 
        public uint ObjectId3;              // if type3 == 5, then Stat Types, else Spell.dbc 
        public LocalizedString SRefName;    // 15-31 The name of the enchantment
        public uint ItemVisuals;            // The glow to add to the items that has this enchant
        public uint Flags;                  
        public uint ItemCache;              // Reference to the Gem that has this ability (Added in 2.0.0.5610) 
        public uint SpellItemEnchantmentCondition; // Conditions for the effect to take place (Added in 2.0.0.5610)
        public uint SkillLine;              // A required profession.
        public uint Skilllevel;             // And the level for that profession.
        public uint requiredLevel;          // Required level to use the enchant

        public override string ToString()
        {
            return SRefName;
        }
    }

    public class SpellItemEnchantmentConditionEntry
    {

    }

    public class SpellMechanicEntry
    {

    }

    public class SpellMissileEntry
    {

    }

    public class SpellMissileMotionEntry
    {

    }

    public class SpellRadiusEntry
    {

    }

    public class SpellRangeEntry
    {

    }

    public class SpellRuneCostEntry
    {

    }

    public class SpellShapeshiftFormEntry
    {

    }

    public class SpellVisualEntry
    {

    }

    public class SpellVisualEffectNameEntry
    {

    }

    public class SpellVisualKitEntry
    {

    }

    public class SpellVisualKitAreaModelEntry
    {

    }

    public class SpellVisualKitModelAttachEntry
    {

    }

    public class SpellVisualPrecastTransitionsEntry
    {

    }

    public class StableSlotPricesEntry
    {

    }

    public class StartupStringsEntry
    {

    }

    public class StationeryEntry
    {

    }

    public class StringLookupsEntry
    {

    }

    public class SummonPropertiesEntry
    {

    }

    public class TalentEntry
    {
        public uint Id;                             // 0
        public uint TabId;                          // 1
        public uint Row;                            // 2
        public uint Col;                            // 3
        public uint[] RankId = new uint[9];         // 4-12
        public uint[] ReqTalent = new uint[3];      // 13-15
        public uint[] ReqRank = new uint[3];        // 16-18
        public uint Flags;                          // 19
        public uint RequiredSpellId;                // 20
        public uint[] AllowForPetFlags = new uint[2];// 21-22

        public override string ToString()
        {
            return Id.ToString() + " (" + Row.ToString() + "," + Col.ToString() + ")";
        }
    }

    public class TalentTabEntry
    {
        public uint Id;                             // 0
        public LocalizedString Name;                // 1-17
        public uint SpellIcon;                      // 18
        public uint RaceMask;                       // 19
        public uint ClassMask;                      // 20
        public uint PetTalentMask;                  // 21
        public uint TabPage;                        // 22
        public string InternalName;                 // 23

        public override string ToString()
        {
            return InternalName;
        }
    }

    public class TaxiNodesEntry
    {

    }

    public class TaxiPathEntry
    {

    }

    public class TaxiPathNodeEntry
    {

    }

    public class TeamContributionPointsEntry
    {

    }

    public class TerrainTypeEntry
    {

    }

    public class TerrainTypeSoundsEntry
    {

    }

    public class TotemCategoryEntry
    {

    }

    public class TransportAnimationEntry
    {

    }

    public class TransportPhysicsEntry
    {

    }

    public class TransportRotationEntry
    {

    }

    public class UISoundLookupsEntry
    {

    }

    public class UnitBloodEntry
    {

    }

    public class UnitBloodLevelsEntry
    {

    }

    public class VehicleEntry
    {

    }

    public class VehicleSeatEntry
    {

    }

    public class VehicleUIIndicatorEntry
    {

    }

    public class VehicleUIIndSeatEntry
    {

    }

    public class VideoHardwareEntry
    {

    }

    public class VocalUISoundsEntry
    {

    }

    public class WeaponImpactSoundsEntry
    {

    }

    public class WeaponSwingSounds2Entry
    {

    }

    public class WeatherEntry
    {

    }

    public class WMOAreaTableEntry
    {

    }

    public class WorldChunkSoundsEntry
    {

    }

    public class WorldMapAreaEntry
    {
        public uint Id;                             // 0
        public uint MapId;                          // 1
        public uint AreaId;                         // 2 continent 0 areas ignored
        public string InternalName;                 // 3
        public float locLeft;                       // 4
        public float locRight;                      // 5
        public float locTop;                        // 6
        public float locBottom;                     // 7
        public int VirtualMapId;                    // 8 -1 (map_id have correct map) other: virtual map where zone show (map_id - where zone in fact internally)
        public int DungeonMapId;                    // 9 DungeonMap.dbc
        public uint ParentWorldMapId;               // 10

        public override string ToString()
        {
            return InternalName;
        }
    }

    public class WorldMapContinentEntry
    {

    }

    public class WorldMapOverlayEntry
    {
        public uint Id;                             // 0 Internal overlay id, probably not used anywhere
        public uint WorldMapAreaId;                 // 1 WorldMapArea.dbc
        public uint[] AreaTableId = new uint[4];    // 2-5
        public uint MapPointX;                      // 6 always 0
        public uint MapPointY;                      // 7 always 0
        public string TextureName;                  // 8
        public uint TextureWidth;                   // 9
        public uint TextureHeight;                  // 10
        public uint OffsetX;                        // 11
        public uint OffsetY;                        // 12
        public uint HitRectTop;                     // 13
        public uint HitRectLeft;                    // 14
        public uint HitRectBottom;                  // 15
        public uint HitRectRight;                   // 16

        public override string ToString()
        {
            return TextureName;
        }
    }

    public class WorldMapTransformsEntry
    {

    }

    public class WorldSafeLocsEntry
    {

    }

    public class WorldStateUIEntry
    {

    }

    public class WorldStateZoneSoundsEntry
    {

    }

    public class WowErrorStringsEntry
    {

    }

    public class ZoneIntroMusicTableEntry
    {

    }

    public class ZoneMusicEntry
    {

    }
}
