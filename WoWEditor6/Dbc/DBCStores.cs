using DBCLib;
using DBCLib.Structures;

namespace WoWEditor6.Dbc
{
    public static partial class DbcStores
    {
        #region Déclarations des DBCs
        public static DBCFile<AchievementEntry> Achievement { get; private set; }
        public static DBCFile<AchievementCategoryEntry> AchievementCategory { get; private set; }
        public static DBCFile<AchievementCriteriaEntry> AchievementCriteria { get; private set; }
        //public static DBCFile<AnimationDataEntry> AnimationData { get; private set; }
        //public static DBCFile<AreaGroupEntry> AreaGroup { get; private set; }
        public static DBCFile<AreaPOIEntry> AreaPoi { get; private set; }
        public static DBCFile<AreaTableEntry> AreaTable { get; private set; }
        //public static DBCFile<AreaTriggerEntry> AreaTrigger { get; private set; }
        //public static DBCFile<AttackAnimKitsEntry> AttackAnimKits { get; private set; }
        //public static DBCFile<AttackAnimTypesEntry> AttackAnimTypes { get; private set; }
        //public static DBCFile<AuctionHouseEntry> AuctionHouse { get; private set; }
        //public static DBCFile<BankBagSlotPricesEntry> BankBagSlotPrices { get; private set; }
        //public static DBCFile<BannedAddOnsEntry> BannedAddOns { get; private set; }
        //public static DBCFile<BarberShopStyleEntry> BarberShopStyle { get; private set; }
        //public static DBCFile<BattlemasterListEntry> BattlemasterList { get; private set; }
        //public static DBCFile<CameraShakesEntry> CameraShakes { get; private set; }
        //public static DBCFile<CfgCategoriesEntry> CfgCategories { get; private set; }
        //public static DBCFile<CfgConfigsEntry> CfgConfigs { get; private set; }
        //public static DBCFile<CharacterFacialHairStylesEntry> CharacterFacialHairStyles { get; private set; }
        public static DBCFile<CharBaseInfoEntry> CharBaseInfo { get; private set; }
        //public static DBCFile<CharHairGeosetsEntry> CharHairGeosets { get; private set; }
        //public static DBCFile<CharHairTexturesEntry> CharHairTextures { get; private set; }
        //public static DBCFile<CharSectionsEntry> CharSections { get; private set; }
        public static DBCFile<CharStartOutfitEntry> CharStartOutfit { get; private set; }
        public static DBCFile<CharTitlesEntry> CharTitles { get; private set; }
        //public static DBCFile<CharVariationsEntry> CharVariations { get; private set; }
        //public static DBCFile<ChatChannelsEntry> ChatChannels { get; private set; }
        //public static DBCFile<ChatProfanityEntry> ChatProfanity { get; private set; }
        public static DBCFile<ChrClassesEntry> ChrClasses { get; private set; }
        public static DBCFile<ChrRacesEntry> ChrRaces { get; private set; }
        //public static DBCFile<CinematicCameraEntry> CinematicCamera { get; private set; }
        //public static DBCFile<CinematicSequencesEntry> CinematicSequences { get; private set; }
        //public static DBCFile<CreatureDisplayInfoEntry> CreatureDisplayInfo { get; private set; }
        //public static DBCFile<CreatureDisplayInfoExtraEntry> CreatureDisplayInfoExtra { get; private set; }
        //public static DBCFile<CreatureFamilyEntry> CreatureFamily { get; private set; }
        //public static DBCFile<CreatureModelDataEntry> CreatureModelData { get; private set; }
        //public static DBCFile<CreatureMovementInfoEntry> CreatureMovementInfo { get; private set; }
        //public static DBCFile<CreatureSoundDataEntry> CreatureSoundData { get; private set; }
        //public static DBCFile<CreatureSpellDataEntry> CreatureSpellData { get; private set; }
        //public static DBCFile<CreatureTypeEntry> CreatureType { get; private set; }
        //public static DBCFile<CurrencyCategoryEntry> CurrencyCategory { get; private set; }
        //public static DBCFile<CurrencyTypesEntry> CurrencyTypes { get; private set; }
        //public static DBCFile<DanceMovesEntry> DanceMoves { get; private set; }
        //public static DBCFile<DeathThudLookupsEntry> DeathThudLookups { get; private set; }
        //public static DBCFile<DeclinedWordEntry> DeclinedWord { get; private set; }
        //public static DBCFile<DeclinedWordCasesEntry> DeclinedWordCases { get; private set; }
        //public static DBCFile<DestructibleModelDataEntry> DestructibleModelData { get; private set; }
        //public static DBCFile<DungeonEncounterEntry> DungeonEncounter { get; private set; }
        public static DBCFile<DungeonMapEntry> DungeonMap { get; private set; }
        //public static DBCFile<DungeonMapChunkEntry> DungeonMapChunk { get; private set; }
        //public static DBCFile<DurabilityCostsEntry> DurabilityCosts { get; private set; }
        //public static DBCFile<DurabilityQualityEntry> DurabilityQuality { get; private set; }
        //public static DBCFile<EmotesEntry> Emotes { get; private set; }
        //public static DBCFile<EmotesTextEntry> EmotesText { get; private set; }
        //public static DBCFile<EmotesTextDataEntry> EmotesTextData { get; private set; }
        //public static DBCFile<EmotesTextSoundEntry> EmotesTextSound { get; private set; }
        //public static DBCFile<EnvironmentalDamageEntry> EnvironmentalDamage { get; private set; }
        //public static DBCFile<ExhaustionEntry> Exhaustion { get; private set; }
        public static DBCFile<FactionEntry> Faction { get; private set; }
        public static DBCFile<FactionGroupEntry> FactionGroup { get; private set; }
        public static DBCFile<FactionTemplateEntry> FactionTemplate { get; private set; }
        //public static DBCFile<FileDataEntry> FileData { get; private set; }
        //public static DBCFile<FootprintTexturesEntry> FootprintTextures { get; private set; }
        //public static DBCFile<FootstepTerrainLookupEntry> FootstepTerrainLookup { get; private set; }
        //public static DBCFile<GameObjectArtKitEntry> GameObjectArtKit { get; private set; }
        //public static DBCFile<GameObjectDisplayInfoEntry> GameObjectDisplayInfo { get; private set; }
        //public static DBCFile<GameTablesEntry> GameTables { get; private set; }
        public static DBCFile<GameTipsEntry> GameTips { get; private set; }
        public static DBCFile<GemPropertiesEntry> GemProperties { get; private set; }
        //public static DBCFile<GlyphPropertiesEntry> GlyphProperties { get; private set; }
        //public static DBCFile<GlyphSlotEntry> GlyphSlot { get; private set; }
        //public static DBCFile<GMSurveyAnswersEntry> GMSurveyAnswers { get; private set; }
        //public static DBCFile<GMSurveyCurrentSurveyEntry> GMSurveyCurrentSurvey { get; private set; }
        //public static DBCFile<GMSurveyQuestionsEntry> GMSurveyQuestions { get; private set; }
        //public static DBCFile<GMSurveySurveysEntry> GMSurveySurveys { get; private set; }
        //public static DBCFile<GMTicketCategoryEntry> GMTicketCategory { get; private set; }
        //public static DBCFile<GroundEffectDoodadEntry> GroundEffectDoodad { get; private set; }
        //public static DBCFile<GroundEffectTextureEntry> GroundEffectTexture { get; private set; }
        //public static DBCFile<gtBarberShopCostBaseEntry> gtBarberShopCostBase { get; private set; }
        //public static DBCFile<gtChanceToMeleeCritEntry> gtChanceToMeleeCrit { get; private set; }
        //public static DBCFile<gtChanceToMeleeCritBaseEntry> gtChanceToMeleeCritBase { get; private set; }
        //public static DBCFile<gtChanceToSpellCritEntry> gtChanceToSpellCrit { get; private set; }
        //public static DBCFile<gtChanceToSpellCritBaseEntry> gtChanceToSpellCritBase { get; private set; }
        //public static DBCFile<gtCombatRatingsEntry> gtCombatRatings { get; private set; }
        //public static DBCFile<gtNPCManaCostScalerEntry> gtNPCManaCostScaler { get; private set; }
        //public static DBCFile<gtOCTClassCombatRatingScalarEntry> gtOCTClassCombatRatingScalar { get; private set; }
        //public static DBCFile<gtOCTRegenHPEntry> gtOCTRegenHP { get; private set; }
        //public static DBCFile<gtOCTRegenMPEntry> gtOCTRegenMP { get; private set; }
        //public static DBCFile<gtRegenHPPerSptEntry> gtRegenHPPerSpt { get; private set; }
        //public static DBCFile<gtRegenMPPerSptEntry> gtRegenMPPerSpt { get; private set; }
        //public static DBCFile<HelmetGeosetVisDataEntry> HelmetGeosetVisData { get; private set; }
        //public static DBCFile<HolidayDescriptionsEntry> HolidayDescriptions { get; private set; }
        //public static DBCFile<HolidayNamesEntry> HolidayNames { get; private set; }
        //public static DBCFile<HolidaysEntry> Holidays { get; private set; }
        public static DBCFile<ItemEntry> Item { get; private set; }
        //public static DBCFile<ItemBagFamilyEntry> ItemBagFamily { get; private set; }
        //public static DBCFile<ItemClassEntry> ItemClass { get; private set; }
        //public static DBCFile<ItemCondExtCostsEntry> ItemCondExtCosts { get; private set; }
        //public static DBCFile<ItemDisplayInfoEntry> ItemDisplayInfo { get; private set; }
        //public static DBCFile<ItemExtendedCostEntry> ItemExtendedCost { get; private set; }
        //public static DBCFile<ItemGroupSoundsEntry> ItemGroupSounds { get; private set; }
        //public static DBCFile<ItemLimitCategoryEntry> ItemLimitCategory { get; private set; }
        //public static DBCFile<ItemPetFoodEntry> ItemPetFood { get; private set; }
        //public static DBCFile<ItemPurchaseGroupEntry> ItemPurchaseGroup { get; private set; }
        //public static DBCFile<ItemRandomPropertiesEntry> ItemRandomProperties { get; private set; }
        //public static DBCFile<ItemRandomSuffixEntry> ItemRandomSuffix { get; private set; }
        public static DBCFile<ItemSetEntry> ItemSet { get; private set; }
        //public static DBCFile<ItemSubClassEntry> ItemSubClass { get; private set; }
        //public static DBCFile<ItemSubClassMaskEntry> ItemSubClassMask { get; private set; }
        //public static DBCFile<ItemVisualEffectsEntry> ItemVisualEffects { get; private set; }
        //public static DBCFile<ItemVisualsEntry> ItemVisuals { get; private set; }
        //public static DBCFile<LanguagesEntry> Languages { get; private set; }
        //public static DBCFile<LanguageWordsEntry> LanguageWords { get; private set; }
        //public static DBCFile<LFGDungeonExpansionEntry> LFGDungeonExpansion { get; private set; }
        //public static DBCFile<LFGDungeonGroupEntry> LFGDungeonGroup { get; private set; }
        //public static DBCFile<LFGDungeonsEntry> LFGDungeons { get; private set; }
        //public static DBCFile<LightEntry> Light { get; private set; }
        //public static DBCFile<LightFloatBandEntry> LightFloatBand { get; private set; }
        //public static DBCFile<LightIntBandEntry> LightIntBand { get; private set; }
        //public static DBCFile<LightParamsEntry> LightParams { get; private set; }
        //public static DBCFile<LightSkyboxEntry> LightSkybox { get; private set; }
        //public static DBCFile<LiquidMaterialEntry> LiquidMaterial { get; private set; }
        //public static DBCFile<LiquidTypeEntry> LiquidType { get; private set; }
        //public static DBCFile<LoadingScreensEntry> LoadingScreens { get; private set; }
        //public static DBCFile<LoadingScreenTaxiSplinesEntry> LoadingScreenTaxiSplines { get; private set; }
        //public static DBCFile<LockEntry> Lock { get; private set; }
        //public static DBCFile<LockTypeEntry> LockType { get; private set; }
        //public static DBCFile<MailTemplateEntry> MailTemplate { get; private set; }
        public static DBCFile<MapEntry> Map { get; private set; }
        //public static DBCFile<MapDifficultyEntry> MapDifficulty { get; private set; }
        //public static DBCFile<MaterialEntry> Material { get; private set; }
        //public static DBCFile<MovieEntry> Movie { get; private set; }
        //public static DBCFile<MovieFileDataEntry> MovieFileData { get; private set; }
        //public static DBCFile<MovieVariationEntry> MovieVariation { get; private set; }
        //public static DBCFile<NameGenEntry> NameGen { get; private set; }
        public static DBCFile<NamesProfanityEntry> NamesProfanity { get; private set; }
        public static DBCFile<NamesReservedEntry> NamesReserved { get; private set; }
        //public static DBCFile<NPCSoundsEntry> NPCSounds { get; private set; }
        //public static DBCFile<ObjectEffectEntry> ObjectEffect { get; private set; }
        //public static DBCFile<ObjectEffectGroupEntry> ObjectEffectGroup { get; private set; }
        //public static DBCFile<ObjectEffectModifierEntry> ObjectEffectModifier { get; private set; }
        //public static DBCFile<ObjectEffectPackageEntry> ObjectEffectPackage { get; private set; }
        //public static DBCFile<ObjectEffectPackageElemEntry> ObjectEffectPackageElem { get; private set; }
        //public static DBCFile<OverrideSpellDataEntry> OverrideSpellData { get; private set; }
        //public static DBCFile<PackageEntry> Package { get; private set; }
        //public static DBCFile<PageTextMaterialEntry> PageTextMaterial { get; private set; }
        //public static DBCFile<PaperDollItemFrameEntry> PaperDollItemFrame { get; private set; }
        //public static DBCFile<ParticleColorEntry> ParticleColor { get; private set; }
        //public static DBCFile<PetitionTypeEntry> PetitionType { get; private set; }
        //public static DBCFile<PetPersonalityEntry> PetPersonality { get; private set; }
        //public static DBCFile<PowerDisplayEntry> PowerDisplay { get; private set; }
        //public static DBCFile<PvpDifficultyEntry> PvpDifficulty { get; private set; }
        //public static DBCFile<QuestFactionRewardEntry> QuestFactionReward { get; private set; }
        //public static DBCFile<QuestInfoEntry> QuestInfo { get; private set; }
        //public static DBCFile<QuestSortEntry> QuestSort { get; private set; }
        //public static DBCFile<QuestXPEntry> QuestXP { get; private set; }
        //public static DBCFile<RandPropPointsEntry> RandPropPoints { get; private set; }
        //public static DBCFile<ResistancesEntry> Resistances { get; private set; }
        //public static DBCFile<ScalingStatDistributionEntry> ScalingStatDistribution { get; private set; }
        //public static DBCFile<ScalingStatValuesEntry> ScalingStatValues { get; private set; }
        //public static DBCFile<ScreenEffectEntry> ScreenEffect { get; private set; }
        //public static DBCFile<ServerMessagesEntry> ServerMessages { get; private set; }
        //public static DBCFile<SheatheSoundLookupsEntry> SheatheSoundLookups { get; private set; }
        //public static DBCFile<SkillCostsDataEntry> SkillCostsData { get; private set; }
        public static DBCFile<SkillLineEntry> SkillLine { get; private set; }
        public static DBCFile<SkillLineAbilityEntry> SkillLineAbility { get; private set; }
        //public static DBCFile<SkillLineCategoryEntry> SkillLineCategory { get; private set; }
        public static DBCFile<SkillRaceClassInfoEntry> SkillRaceClassInfo { get; private set; }
        //public static DBCFile<SkillTiersEntry> SkillTiers { get; private set; }
        //public static DBCFile<SoundAmbienceEntry> SoundAmbience { get; private set; }
        //public static DBCFile<SoundEmittersEntry> SoundEmitters { get; private set; }
        //public static DBCFile<SoundEntriesEntry> SoundEntries { get; private set; }
        //public static DBCFile<SoundEntriesAdvancedEntry> SoundEntriesAdvanced { get; private set; }
        //public static DBCFile<SoundFilterEntry> SoundFilter { get; private set; }
        //public static DBCFile<SoundFilterElemEntry> SoundFilterElem { get; private set; }
        //public static DBCFile<SoundProviderPreferencesEntry> SoundProviderPreferences { get; private set; }
        //public static DBCFile<SoundSamplePreferencesEntry> SoundSamplePreferences { get; private set; }
        //public static DBCFile<SoundWaterTypeEntry> SoundWaterType { get; private set; }
        //public static DBCFile<SpamMessagesEntry> SpamMessages { get; private set; }
        public static DBCFile<SpellEntry> Spell { get; private set; }
        //public static DBCFile<SpellCastTimesEntry> SpellCastTimes { get; private set; }
        //public static DBCFile<SpellCategoryEntry> SpellCategory { get; private set; }
        //public static DBCFile<SpellChainEffectsEntry> SpellChainEffects { get; private set; }
        //public static DBCFile<SpellDescriptionVariablesEntry> SpellDescriptionVariables { get; private set; }
        //public static DBCFile<SpellDifficultyEntry> SpellDifficulty { get; private set; }
        //public static DBCFile<SpellDispelTypeEntry> SpellDispelType { get; private set; }
        //public static DBCFile<SpellDurationEntry> SpellDuration { get; private set; }
        //public static DBCFile<SpellEffectCameraShakesEntry> SpellEffectCameraShakes { get; private set; }
        public static DBCFile<SpellFocusObjectEntry> SpellFocusObject { get; private set; }
        public static DBCFile<SpellIconEntry> SpellIcon { get; private set; }
        public static DBCFile<SpellItemEnchantmentEntry> SpellItemEnchantment { get; private set; }
        //public static DBCFile<SpellItemEnchantmentConditionEntry> SpellItemEnchantmentCondition { get; private set; }
        //public static DBCFile<SpellMechanicEntry> SpellMechanic { get; private set; }
        //public static DBCFile<SpellMissileEntry> SpellMissile { get; private set; }
        //public static DBCFile<SpellMissileMotionEntry> SpellMissileMotion { get; private set; }
        //public static DBCFile<SpellRadiusEntry> SpellRadius { get; private set; }
        //public static DBCFile<SpellRangeEntry> SpellRange { get; private set; }
        //public static DBCFile<SpellRuneCostEntry> SpellRuneCost { get; private set; }
        //public static DBCFile<SpellShapeshiftFormEntry> SpellShapeshiftForm { get; private set; }
        //public static DBCFile<SpellVisualEntry> SpellVisual { get; private set; }
        //public static DBCFile<SpellVisualEffectNameEntry> SpellVisualEffectName { get; private set; }
        //public static DBCFile<SpellVisualKitEntry> SpellVisualKit { get; private set; }
        //public static DBCFile<SpellVisualKitAreaModelEntry> SpellVisualKitAreaModel { get; private set; }
        //public static DBCFile<SpellVisualKitModelAttachEntry> SpellVisualKitModelAttach { get; private set; }
        //public static DBCFile<SpellVisualPrecastTransitionsEntry> SpellVisualPrecastTransitions { get; private set; }
        //public static DBCFile<StableSlotPricesEntry> StableSlotPrices { get; private set; }
        //public static DBCFile<StartupStringsEntry> StartupStrings { get; private set; }
        //public static DBCFile<StationeryEntry> Stationery { get; private set; }
        //public static DBCFile<StringLookupsEntry> StringLookups { get; private set; }
        //public static DBCFile<SummonPropertiesEntry> SummonProperties { get; private set; }
        public static DBCFile<TalentEntry> Talent { get; private set; }
        public static DBCFile<TalentTabEntry> TalentTab { get; private set; }
        //public static DBCFile<TaxiNodesEntry> TaxiNodes { get; private set; }
        //public static DBCFile<TaxiPathEntry> TaxiPath { get; private set; }
        //public static DBCFile<TaxiPathNodeEntry> TaxiPathNode { get; private set; }
        //public static DBCFile<TeamContributionPointsEntry> TeamContributionPoints { get; private set; }
        //public static DBCFile<TerrainTypeEntry> TerrainType { get; private set; }
        //public static DBCFile<TerrainTypeSoundsEntry> TerrainTypeSounds { get; private set; }
        //public static DBCFile<TotemCategoryEntry> TotemCategory { get; private set; }
        //public static DBCFile<TransportAnimationEntry> TransportAnimation { get; private set; }
        //public static DBCFile<TransportPhysicsEntry> TransportPhysics { get; private set; }
        //public static DBCFile<TransportRotationEntry> TransportRotation { get; private set; }
        //public static DBCFile<UISoundLookupsEntry> UISoundLookups { get; private set; }
        //public static DBCFile<UnitBloodEntry> UnitBlood { get; private set; }
        //public static DBCFile<UnitBloodLevelsEntry> UnitBloodLevels { get; private set; }
        //public static DBCFile<VehicleEntry> Vehicle { get; private set; }
        //public static DBCFile<VehicleSeatEntry> VehicleSeat { get; private set; }
        //public static DBCFile<VehicleUIIndicatorEntry> VehicleUIIndicator { get; private set; }
        //public static DBCFile<VehicleUIIndSeatEntry> VehicleUIIndSeat { get; private set; }
        //public static DBCFile<VideoHardwareEntry> VideoHardware { get; private set; }
        //public static DBCFile<VocalUISoundsEntry> VocalUISounds { get; private set; }
        //public static DBCFile<WeaponImpactSoundsEntry> WeaponImpactSounds { get; private set; }
        //public static DBCFile<WeaponSwingSounds2Entry> WeaponSwingSounds2 { get; private set; }
        //public static DBCFile<WeatherEntry> Weather { get; private set; }
        //public static DBCFile<WMOAreaTableEntry> WMOAreaTable { get; private set; }
        //public static DBCFile<WorldChunkSoundsEntry> WorldChunkSounds { get; private set; }
        public static DBCFile<WorldMapAreaEntry> WorldMapArea { get; private set; }
        //public static DBCFile<WorldMapContinentEntry> WorldMapContinent { get; private set; }
        public static DBCFile<WorldMapOverlayEntry> WorldMapOverlay { get; private set; }
        //public static DBCFile<WorldMapTransformsEntry> WorldMapTransforms { get; private set; }
        //public static DBCFile<WorldSafeLocsEntry> WorldSafeLocs { get; private set; }
        //public static DBCFile<WorldStateUIEntry> WorldStateUI { get; private set; }
        //public static DBCFile<WorldStateZoneSoundsEntry> WorldStateZoneSounds { get; private set; }
        //public static DBCFile<WowErrorStringsEntry> WowErrorStrings { get; private set; }
        //public static DBCFile<ZoneIntroMusicTableEntry> ZoneIntroMusicTable { get; private set; }
        //public static DBCFile<ZoneMusicEntry> ZoneMusic { get; private set; }
        #endregion

        public static void InitFiles()
        {
            string path = System.Environment.CurrentDirectory;
            string dbcFolder = path + "\\DBC\\";

            #region Initialisation des DBCs
            Achievement = new DBCFile<AchievementEntry>(dbcFolder + "Achievement.dbc");
            AchievementCategory = new DBCFile<AchievementCategoryEntry>(dbcFolder + "Achievement_Category.dbc");
            AchievementCriteria = new DBCFile<AchievementCriteriaEntry>(dbcFolder + "Achievement_Criteria.dbc");
            //AnimationData = new DBCFile<AnimationDataEntry>(dbcFolder + "AnimationData.dbc");
            //AreaGroup = new DBCFile<AreaGroupEntry>(dbcFolder + "AreaGroup.dbc");
            AreaPoi = new DBCFile<AreaPOIEntry>(dbcFolder + "AreaPOI.dbc");
            AreaTable = new DBCFile<AreaTableEntry>(dbcFolder + "AreaTable.dbc");
            //AreaTrigger = new DBCFile<AreaTriggerEntry>(dbcFolder + "AreaTrigger.dbc");
            //AttackAnimKits = new DBCFile<AttackAnimKitsEntry>(dbcFolder + "AttackAnimKits.dbc");
            //AttackAnimTypes = new DBCFile<AttackAnimTypesEntry>(dbcFolder + "AttackAnimTypes.dbc");
            //AuctionHouse = new DBCFile<AuctionHouseEntry>(dbcFolder + "AuctionHouse.dbc");
            //BankBagSlotPrices = new DBCFile<BankBagSlotPricesEntry>(dbcFolder + "BankBagSlotPrices.dbc");
            //BannedAddOns = new DBCFile<BannedAddOnsEntry>(dbcFolder + "BannedAddOns.dbc");
            //BarberShopStyle = new DBCFile<BarberShopStyleEntry>(dbcFolder + "BarberShopStyle.dbc");
            //BattlemasterList = new DBCFile<BattlemasterListEntry>(dbcFolder + "BattlemasterList.dbc");
            //CameraShakes = new DBCFile<CameraShakesEntry>(dbcFolder + "CameraShakes.dbc");
            //CfgCategories = new DBCFile<CfgCategoriesEntry>(dbcFolder + "Cfg_Categories.dbc");
            //CfgConfigs = new DBCFile<CfgConfigsEntry>(dbcFolder + "Cfg_Configs.dbc");
            //CharacterFacialHairStyles = new DBCFile<CharacterFacialHairStylesEntry>(dbcFolder + "CharacterFacialHairStyles.dbc");
            CharBaseInfo = new DBCFile<CharBaseInfoEntry>(dbcFolder + "CharBaseInfo.dbc");
            //CharHairGeosets = new DBCFile<CharHairGeosetsEntry>(dbcFolder + "CharHairGeosets.dbc");
            //CharHairTextures = new DBCFile<CharHairTexturesEntry>(dbcFolder + "CharHairTextures.dbc");
            //CharSections = new DBCFile<CharSectionsEntry>(dbcFolder + "CharSections.dbc");
            CharStartOutfit = new DBCFile<CharStartOutfitEntry>(dbcFolder + "CharStartOutfit.dbc");
            CharTitles = new DBCFile<CharTitlesEntry>(dbcFolder + "CharTitles.dbc");
            //CharVariations = new DBCFile<CharVariationsEntry>(dbcFolder + "CharVariations.dbc");
            //ChatChannels = new DBCFile<ChatChannelsEntry>(dbcFolder + "ChatChannels.dbc");
            //ChatProfanity = new DBCFile<ChatProfanityEntry>(dbcFolder + "ChatProfanity.dbc");
            ChrClasses = new DBCFile<ChrClassesEntry>(dbcFolder + "ChrClasses.dbc");
            ChrRaces = new DBCFile<ChrRacesEntry>(dbcFolder + "ChrRaces.dbc");
            //CinematicCamera = new DBCFile<CinematicCameraEntry>(dbcFolder + "CinematicCamera.dbc");
            //CinematicSequences = new DBCFile<CinematicSequencesEntry>(dbcFolder + "CinematicSequences.dbc");
            //CreatureDisplayInfo = new DBCFile<CreatureDisplayInfoEntry>(dbcFolder + "CreatureDisplayInfo.dbc");
            //CreatureDisplayInfoExtra = new DBCFile<CreatureDisplayInfoExtraEntry>(dbcFolder + "CreatureDisplayInfoExtra.dbc");
            //CreatureFamily = new DBCFile<CreatureFamilyEntry>(dbcFolder + "CreatureFamily.dbc");
            //CreatureModelData = new DBCFile<CreatureModelDataEntry>(dbcFolder + "CreatureModelData.dbc");
            //CreatureMovementInfo = new DBCFile<CreatureMovementInfoEntry>(dbcFolder + "CreatureMovementInfo.dbc");
            //CreatureSoundData = new DBCFile<CreatureSoundDataEntry>(dbcFolder + "CreatureSoundData.dbc");
            //CreatureSpellData = new DBCFile<CreatureSpellDataEntry>(dbcFolder + "CreatureSpellData.dbc");
            //CreatureType = new DBCFile<CreatureTypeEntry>(dbcFolder + "CreatureType.dbc");
            //CurrencyCategory = new DBCFile<CurrencyCategoryEntry>(dbcFolder + "CurrencyCategory.dbc");
            //CurrencyTypes = new DBCFile<CurrencyTypesEntry>(dbcFolder + "CurrencyTypes.dbc");
            //DanceMoves = new DBCFile<DanceMovesEntry>(dbcFolder + "DanceMoves.dbc");
            //DeathThudLookups = new DBCFile<DeathThudLookupsEntry>(dbcFolder + "DeathThudLookups.dbc");
            //DeclinedWord = new DBCFile<DeclinedWordEntry>(dbcFolder + "DeclinedWord.dbc");
            //DeclinedWordCases = new DBCFile<DeclinedWordCasesEntry>(dbcFolder + "DeclinedWordCases.dbc");
            //DestructibleModelData = new DBCFile<DestructibleModelDataEntry>(dbcFolder + "DestructibleModelData.dbc");
            //DungeonEncounter = new DBCFile<DungeonEncounterEntry>(dbcFolder + "DungeonEncounter.dbc");
            DungeonMap = new DBCFile<DungeonMapEntry>(dbcFolder + "DungeonMap.dbc");
            //DungeonMapChunk = new DBCFile<DungeonMapChunkEntry>(dbcFolder + "DungeonMapChunk.dbc");
            //DurabilityCosts = new DBCFile<DurabilityCostsEntry>(dbcFolder + "DurabilityCosts.dbc");
            //DurabilityQuality = new DBCFile<DurabilityQualityEntry>(dbcFolder + "DurabilityQuality.dbc");
            //Emotes = new DBCFile<EmotesEntry>(dbcFolder + "Emotes.dbc");
            //EmotesText = new DBCFile<EmotesTextEntry>(dbcFolder + "EmotesText.dbc");
            //EmotesTextData = new DBCFile<EmotesTextDataEntry>(dbcFolder + "EmotesTextData.dbc");
            //EmotesTextSound = new DBCFile<EmotesTextSoundEntry>(dbcFolder + "EmotesTextSound.dbc");
            //EnvironmentalDamage = new DBCFile<EnvironmentalDamageEntry>(dbcFolder + "EnvironmentalDamage.dbc");
            //Exhaustion = new DBCFile<ExhaustionEntry>(dbcFolder + "Exhaustion.dbc");
            Faction = new DBCFile<FactionEntry>(dbcFolder + "Faction.dbc");
            FactionGroup = new DBCFile<FactionGroupEntry>(dbcFolder + "FactionGroup.dbc");
            FactionTemplate = new DBCFile<FactionTemplateEntry>(dbcFolder + "FactionTemplate.dbc");
            //FileData = new DBCFile<FileDataEntry>(dbcFolder + "FileData.dbc");
            //FootprintTextures = new DBCFile<FootprintTexturesEntry>(dbcFolder + "FootprintTextures.dbc");
            //FootstepTerrainLookup = new DBCFile<FootstepTerrainLookupEntry>(dbcFolder + "FootstepTerrainLookup.dbc");
            //GameObjectArtKit = new DBCFile<GameObjectArtKitEntry>(dbcFolder + "GameObjectArtKit.dbc");
            //GameObjectDisplayInfo = new DBCFile<GameObjectDisplayInfoEntry>(dbcFolder + "GameObjectDisplayInfo.dbc");
            //GameTables = new DBCFile<GameTablesEntry>(dbcFolder + "GameTables.dbc");
            GameTips = new DBCFile<GameTipsEntry>(dbcFolder + "GameTips.dbc");
            GemProperties = new DBCFile<GemPropertiesEntry>(dbcFolder + "GemProperties.dbc");
            //GlyphProperties = new DBCFile<GlyphPropertiesEntry>(dbcFolder + "GlyphProperties.dbc");
            //GlyphSlot = new DBCFile<GlyphSlotEntry>(dbcFolder + "GlyphSlot.dbc");
            //GMSurveyAnswers = new DBCFile<GMSurveyAnswersEntry>(dbcFolder + "GMSurveyAnswers.dbc");
            //GMSurveyCurrentSurvey = new DBCFile<GMSurveyCurrentSurveyEntry>(dbcFolder + "GMSurveyCurrentSurvey.dbc");
            //GMSurveyQuestions = new DBCFile<GMSurveyQuestionsEntry>(dbcFolder + "GMSurveyQuestions.dbc");
            //GMSurveySurveys = new DBCFile<GMSurveySurveysEntry>(dbcFolder + "GMSurveySurveys.dbc");
            //GMTicketCategory = new DBCFile<GMTicketCategoryEntry>(dbcFolder + "GMTicketCategory.dbc");
            //GroundEffectDoodad = new DBCFile<GroundEffectDoodadEntry>(dbcFolder + "GroundEffectDoodad.dbc");
            //GroundEffectTexture = new DBCFile<GroundEffectTextureEntry>(dbcFolder + "GroundEffectTexture.dbc");
            //gtBarberShopCostBase = new DBCFile<gtBarberShopCostBaseEntry>(dbcFolder + "gtBarberShopCostBase.dbc");
            //gtChanceToMeleeCrit = new DBCFile<gtChanceToMeleeCritEntry>(dbcFolder + "gtChanceToMeleeCrit.dbc");
            //gtChanceToMeleeCritBase = new DBCFile<gtChanceToMeleeCritBaseEntry>(dbcFolder + "gtChanceToMeleeCritBase.dbc");
            //gtChanceToSpellCrit = new DBCFile<gtChanceToSpellCritEntry>(dbcFolder + "gtChanceToSpellCrit.dbc");
            //gtChanceToSpellCritBase = new DBCFile<gtChanceToSpellCritBaseEntry>(dbcFolder + "gtChanceToSpellCritBase.dbc");
            //gtCombatRatings = new DBCFile<gtCombatRatingsEntry>(dbcFolder + "gtCombatRatings.dbc");
            //gtNPCManaCostScaler = new DBCFile<gtNPCManaCostScalerEntry>(dbcFolder + "gtNPCManaCostScaler.dbc");
            //gtOCTClassCombatRatingScalar = new DBCFile<gtOCTClassCombatRatingScalarEntry>(dbcFolder + "gtOCTClassCombatRatingScalar.dbc");
            //gtOCTRegenHP = new DBCFile<gtOCTRegenHPEntry>(dbcFolder + "gtOCTRegenHP.dbc");
            //gtOCTRegenMP = new DBCFile<gtOCTRegenMPEntry>(dbcFolder + "gtOCTRegenMP.dbc");
            //gtRegenHPPerSpt = new DBCFile<gtRegenHPPerSptEntry>(dbcFolder + "gtRegenHPPerSpt.dbc");
            //gtRegenMPPerSpt = new DBCFile<gtRegenMPPerSptEntry>(dbcFolder + "gtRegenMPPerSpt.dbc");
            //HelmetGeosetVisData = new DBCFile<HelmetGeosetVisDataEntry>(dbcFolder + "HelmetGeosetVisData.dbc");
            //HolidayDescriptions = new DBCFile<HolidayDescriptionsEntry>(dbcFolder + "HolidayDescriptions.dbc");
            //HolidayNames = new DBCFile<HolidayNamesEntry>(dbcFolder + "HolidayNames.dbc");
            //Holidays = new DBCFile<HolidaysEntry>(dbcFolder + "Holidays.dbc");
            Item = new DBCFile<ItemEntry>(dbcFolder + "Item.dbc");
            //ItemBagFamily = new DBCFile<ItemBagFamilyEntry>(dbcFolder + "ItemBagFamily.dbc");
            //ItemClass = new DBCFile<ItemClassEntry>(dbcFolder + "ItemClass.dbc");
            //ItemCondExtCosts = new DBCFile<ItemCondExtCostsEntry>(dbcFolder + "ItemCondExtCosts.dbc");
            //ItemDisplayInfo = new DBCFile<ItemDisplayInfoEntry>(dbcFolder + "ItemDisplayInfo.dbc");
            //ItemExtendedCost = new DBCFile<ItemExtendedCostEntry>(dbcFolder + "ItemExtendedCost.dbc");
            //ItemGroupSounds = new DBCFile<ItemGroupSoundsEntry>(dbcFolder + "ItemGroupSounds.dbc");
            //ItemLimitCategory = new DBCFile<ItemLimitCategoryEntry>(dbcFolder + "ItemLimitCategory.dbc");
            //ItemPetFood = new DBCFile<ItemPetFoodEntry>(dbcFolder + "ItemPetFood.dbc");
            //ItemPurchaseGroup = new DBCFile<ItemPurchaseGroupEntry>(dbcFolder + "ItemPurchaseGroup.dbc");
            //ItemRandomProperties = new DBCFile<ItemRandomPropertiesEntry>(dbcFolder + "ItemRandomProperties.dbc");
            //ItemRandomSuffix = new DBCFile<ItemRandomSuffixEntry>(dbcFolder + "ItemRandomSuffix.dbc");
            ItemSet = new DBCFile<ItemSetEntry>(dbcFolder + "ItemSet.dbc");
            //ItemSubClass = new DBCFile<ItemSubClassEntry>(dbcFolder + "ItemSubClass.dbc");
            //ItemSubClassMask = new DBCFile<ItemSubClassMaskEntry>(dbcFolder + "ItemSubClassMask.dbc");
            //ItemVisualEffects = new DBCFile<ItemVisualEffectsEntry>(dbcFolder + "ItemVisualEffects.dbc");
            //ItemVisuals = new DBCFile<ItemVisualsEntry>(dbcFolder + "ItemVisuals.dbc");
            //Languages = new DBCFile<LanguagesEntry>(dbcFolder + "Languages.dbc");
            //LanguageWords = new DBCFile<LanguageWordsEntry>(dbcFolder + "LanguageWords.dbc");
            //LFGDungeonExpansion = new DBCFile<LFGDungeonExpansionEntry>(dbcFolder + "LFGDungeonExpansion.dbc");
            //LFGDungeonGroup = new DBCFile<LFGDungeonGroupEntry>(dbcFolder + "LFGDungeonGroup.dbc");
            //LFGDungeons = new DBCFile<LFGDungeonsEntry>(dbcFolder + "LFGDungeons.dbc");
            //Light = new DBCFile<LightEntry>(dbcFolder + "Light.dbc");
            //LightFloatBand = new DBCFile<LightFloatBandEntry>(dbcFolder + "LightFloatBand.dbc");
            //LightIntBand = new DBCFile<LightIntBandEntry>(dbcFolder + "LightIntBand.dbc");
            //LightParams = new DBCFile<LightParamsEntry>(dbcFolder + "LightParams.dbc");
            //LightSkybox = new DBCFile<LightSkyboxEntry>(dbcFolder + "LightSkybox.dbc");
            //LiquidMaterial = new DBCFile<LiquidMaterialEntry>(dbcFolder + "LiquidMaterial.dbc");
            //LiquidType = new DBCFile<LiquidTypeEntry>(dbcFolder + "LiquidType.dbc");
            //LoadingScreens = new DBCFile<LoadingScreensEntry>(dbcFolder + "LoadingScreens.dbc");
            //LoadingScreenTaxiSplines = new DBCFile<LoadingScreenTaxiSplinesEntry>(dbcFolder + "LoadingScreenTaxiSplines.dbc");
            //Lock = new DBCFile<LockEntry>(dbcFolder + "Lock.dbc");
            //LockType = new DBCFile<LockTypeEntry>(dbcFolder + "LockType.dbc");
            //MailTemplate = new DBCFile<MailTemplateEntry>(dbcFolder + "MailTemplate.dbc");
            Map = new DBCFile<MapEntry>(dbcFolder + "Map.dbc");
            //MapDifficulty = new DBCFile<MapDifficultyEntry>(dbcFolder + "MapDifficulty.dbc");
            //Material = new DBCFile<MaterialEntry>(dbcFolder + "Material.dbc");
            //Movie = new DBCFile<MovieEntry>(dbcFolder + "Movie.dbc");
            //MovieFileData = new DBCFile<MovieFileDataEntry>(dbcFolder + "MovieFileData.dbc");
            //MovieVariation = new DBCFile<MovieVariationEntry>(dbcFolder + "MovieVariation.dbc");
            //NameGen = new DBCFile<NameGenEntry>(dbcFolder + "NameGen.dbc");
            NamesProfanity = new DBCFile<NamesProfanityEntry>(dbcFolder + "NamesProfanity.dbc");
            NamesReserved = new DBCFile<NamesReservedEntry>(dbcFolder + "NamesReserved.dbc");
            //NPCSounds = new DBCFile<NPCSoundsEntry>(dbcFolder + "NPCSounds.dbc");
            //ObjectEffect = new DBCFile<ObjectEffectEntry>(dbcFolder + "ObjectEffect.dbc");
            //ObjectEffectGroup = new DBCFile<ObjectEffectGroupEntry>(dbcFolder + "ObjectEffectGroup.dbc");
            //ObjectEffectModifier = new DBCFile<ObjectEffectModifierEntry>(dbcFolder + "ObjectEffectModifier.dbc");
            //ObjectEffectPackage = new DBCFile<ObjectEffectPackageEntry>(dbcFolder + "ObjectEffectPackage.dbc");
            //ObjectEffectPackageElem = new DBCFile<ObjectEffectPackageElemEntry>(dbcFolder + "ObjectEffectPackageElem.dbc");
            //OverrideSpellData = new DBCFile<OverrideSpellDataEntry>(dbcFolder + "OverrideSpellData.dbc");
            //Package = new DBCFile<PackageEntry>(dbcFolder + "Package.dbc");
            //PageTextMaterial = new DBCFile<PageTextMaterialEntry>(dbcFolder + "PageTextMaterial.dbc");
            //PaperDollItemFrame = new DBCFile<PaperDollItemFrameEntry>(dbcFolder + "PaperDollItemFrame.dbc");
            //ParticleColor = new DBCFile<ParticleColorEntry>(dbcFolder + "ParticleColor.dbc");
            //PetitionType = new DBCFile<PetitionTypeEntry>(dbcFolder + "PetitionType.dbc");
            //PetPersonality = new DBCFile<PetPersonalityEntry>(dbcFolder + "PetPersonality.dbc");
            //PowerDisplay = new DBCFile<PowerDisplayEntry>(dbcFolder + "PowerDisplay.dbc");
            //PvpDifficulty = new DBCFile<PvpDifficultyEntry>(dbcFolder + "PvpDifficulty.dbc");
            //QuestFactionReward = new DBCFile<QuestFactionRewardEntry>(dbcFolder + "QuestFactionReward.dbc");
            //QuestInfo = new DBCFile<QuestInfoEntry>(dbcFolder + "QuestInfo.dbc");
            //QuestSort = new DBCFile<QuestSortEntry>(dbcFolder + "QuestSort.dbc");
            //QuestXP = new DBCFile<QuestXPEntry>(dbcFolder + "QuestXP.dbc");
            //RandPropPoints = new DBCFile<RandPropPointsEntry>(dbcFolder + "RandPropPoints.dbc");
            //Resistances = new DBCFile<ResistancesEntry>(dbcFolder + "Resistances.dbc");
            //ScalingStatDistribution = new DBCFile<ScalingStatDistributionEntry>(dbcFolder + "ScalingStatDistribution.dbc");
            //ScalingStatValues = new DBCFile<ScalingStatValuesEntry>(dbcFolder + "ScalingStatValues.dbc");
            //ScreenEffect = new DBCFile<ScreenEffectEntry>(dbcFolder + "ScreenEffect.dbc");
            //ServerMessages = new DBCFile<ServerMessagesEntry>(dbcFolder + "ServerMessages.dbc");
            //SheatheSoundLookups = new DBCFile<SheatheSoundLookupsEntry>(dbcFolder + "SheatheSoundLookups.dbc");
            //SkillCostsData = new DBCFile<SkillCostsDataEntry>(dbcFolder + "SkillCostsData.dbc");
            SkillLine = new DBCFile<SkillLineEntry>(dbcFolder + "SkillLine.dbc");
            SkillLineAbility = new DBCFile<SkillLineAbilityEntry>(dbcFolder + "SkillLineAbility.dbc");
            //SkillLineCategory = new DBCFile<SkillLineCategoryEntry>(dbcFolder + "SkillLineCategory.dbc");
            SkillRaceClassInfo = new DBCFile<SkillRaceClassInfoEntry>(dbcFolder + "SkillRaceClassInfo.dbc");
            //SkillTiers = new DBCFile<SkillTiersEntry>(dbcFolder + "SkillTiers.dbc");
            //SoundAmbience = new DBCFile<SoundAmbienceEntry>(dbcFolder + "SoundAmbience.dbc");
            //SoundEmitters = new DBCFile<SoundEmittersEntry>(dbcFolder + "SoundEmitters.dbc");
            //SoundEntries = new DBCFile<SoundEntriesEntry>(dbcFolder + "SoundEntries.dbc");
            //SoundEntriesAdvanced = new DBCFile<SoundEntriesAdvancedEntry>(dbcFolder + "SoundEntriesAdvanced.dbc");
            //SoundFilter = new DBCFile<SoundFilterEntry>(dbcFolder + "SoundFilter.dbc");
            //SoundFilterElem = new DBCFile<SoundFilterElemEntry>(dbcFolder + "SoundFilterElem.dbc");
            //SoundProviderPreferences = new DBCFile<SoundProviderPreferencesEntry>(dbcFolder + "SoundProviderPreferences.dbc");
            //SoundSamplePreferences = new DBCFile<SoundSamplePreferencesEntry>(dbcFolder + "SoundSamplePreferences.dbc");
            //SoundWaterType = new DBCFile<SoundWaterTypeEntry>(dbcFolder + "SoundWaterType.dbc");
            //SpamMessages = new DBCFile<SpamMessagesEntry>(dbcFolder + "SpamMessages.dbc");
            Spell = new DBCFile<SpellEntry>(dbcFolder + "Spell.dbc");
            //SpellCastTimes = new DBCFile<SpellCastTimesEntry>(dbcFolder + "SpellCastTimes.dbc");
            //SpellCategory = new DBCFile<SpellCategoryEntry>(dbcFolder + "SpellCategory.dbc");
            //SpellChainEffects = new DBCFile<SpellChainEffectsEntry>(dbcFolder + "SpellChainEffects.dbc");
            //SpellDescriptionVariables = new DBCFile<SpellDescriptionVariablesEntry>(dbcFolder + "SpellDescriptionVariables.dbc");
            //SpellDifficulty = new DBCFile<SpellDifficultyEntry>(dbcFolder + "SpellDifficulty.dbc");
            //SpellDispelType = new DBCFile<SpellDispelTypeEntry>(dbcFolder + "SpellDispelType.dbc");
            //SpellDuration = new DBCFile<SpellDurationEntry>(dbcFolder + "SpellDuration.dbc");
            //SpellEffectCameraShakes = new DBCFile<SpellEffectCameraShakesEntry>(dbcFolder + "SpellEffectCameraShakes.dbc");
            SpellFocusObject = new DBCFile<SpellFocusObjectEntry>(dbcFolder + "SpellFocusObject.dbc");
            SpellIcon = new DBCFile<SpellIconEntry>(dbcFolder + "SpellIcon.dbc");
            SpellItemEnchantment = new DBCFile<SpellItemEnchantmentEntry>(dbcFolder + "SpellItemEnchantment.dbc");
            //SpellItemEnchantmentCondition = new DBCFile<SpellItemEnchantmentConditionEntry>(dbcFolder + "SpellItemEnchantmentCondition.dbc");
            //SpellMechanic = new DBCFile<SpellMechanicEntry>(dbcFolder + "SpellMechanic.dbc");
            //SpellMissile = new DBCFile<SpellMissileEntry>(dbcFolder + "SpellMissile.dbc");
            //SpellMissileMotion = new DBCFile<SpellMissileMotionEntry>(dbcFolder + "SpellMissileMotion.dbc");
            //SpellRadius = new DBCFile<SpellRadiusEntry>(dbcFolder + "SpellRadius.dbc");
            //SpellRange = new DBCFile<SpellRangeEntry>(dbcFolder + "SpellRange.dbc");
            //SpellRuneCost = new DBCFile<SpellRuneCostEntry>(dbcFolder + "SpellRuneCost.dbc");
            //SpellShapeshiftForm = new DBCFile<SpellShapeshiftFormEntry>(dbcFolder + "SpellShapeshiftForm.dbc");
            //SpellVisual = new DBCFile<SpellVisualEntry>(dbcFolder + "SpellVisual.dbc");
            //SpellVisualEffectName = new DBCFile<SpellVisualEffectNameEntry>(dbcFolder + "SpellVisualEffectName.dbc");
            //SpellVisualKit = new DBCFile<SpellVisualKitEntry>(dbcFolder + "SpellVisualKit.dbc");
            //SpellVisualKitAreaModel = new DBCFile<SpellVisualKitAreaModelEntry>(dbcFolder + "SpellVisualKitAreaModel.dbc");
            //SpellVisualKitModelAttach = new DBCFile<SpellVisualKitModelAttachEntry>(dbcFolder + "SpellVisualKitModelAttach.dbc");
            //SpellVisualPrecastTransitions = new DBCFile<SpellVisualPrecastTransitionsEntry>(dbcFolder + "SpellVisualPrecastTransitions.dbc");
            //StableSlotPrices = new DBCFile<StableSlotPricesEntry>(dbcFolder + "StableSlotPrices.dbc");
            //StartupStrings = new DBCFile<StartupStringsEntry>(dbcFolder + "Startup_Strings.dbc");
            //Stationery = new DBCFile<StationeryEntry>(dbcFolder + "Stationery.dbc");
            //StringLookups = new DBCFile<StringLookupsEntry>(dbcFolder + "StringLookups.dbc");
            //SummonProperties = new DBCFile<SummonPropertiesEntry>(dbcFolder + "SummonProperties.dbc");
            Talent = new DBCFile<TalentEntry>(dbcFolder + "Talent.dbc");
            TalentTab = new DBCFile<TalentTabEntry>(dbcFolder + "TalentTab.dbc");
            //TaxiNodes = new DBCFile<TaxiNodesEntry>(dbcFolder + "TaxiNodes.dbc");
            //TaxiPath = new DBCFile<TaxiPathEntry>(dbcFolder + "TaxiPath.dbc");
            //TaxiPathNode = new DBCFile<TaxiPathNodeEntry>(dbcFolder + "TaxiPathNode.dbc");
            //TeamContributionPoints = new DBCFile<TeamContributionPointsEntry>(dbcFolder + "TeamContributionPoints.dbc");
            //TerrainType = new DBCFile<TerrainTypeEntry>(dbcFolder + "TerrainType.dbc");
            //TerrainTypeSounds = new DBCFile<TerrainTypeSoundsEntry>(dbcFolder + "TerrainTypeSounds.dbc");
            //TotemCategory = new DBCFile<TotemCategoryEntry>(dbcFolder + "TotemCategory.dbc");
            //TransportAnimation = new DBCFile<TransportAnimationEntry>(dbcFolder + "TransportAnimation.dbc");
            //TransportPhysics = new DBCFile<TransportPhysicsEntry>(dbcFolder + "TransportPhysics.dbc");
            //TransportRotation = new DBCFile<TransportRotationEntry>(dbcFolder + "TransportRotation.dbc");
            //UISoundLookups = new DBCFile<UISoundLookupsEntry>(dbcFolder + "UISoundLookups.dbc");
            //UnitBlood = new DBCFile<UnitBloodEntry>(dbcFolder + "UnitBlood.dbc");
            //UnitBloodLevels = new DBCFile<UnitBloodLevelsEntry>(dbcFolder + "UnitBloodLevels.dbc");
            //Vehicle = new DBCFile<VehicleEntry>(dbcFolder + "Vehicle.dbc");
            //VehicleSeat = new DBCFile<VehicleSeatEntry>(dbcFolder + "VehicleSeat.dbc");
            //VehicleUIIndicator = new DBCFile<VehicleUIIndicatorEntry>(dbcFolder + "VehicleUIIndicator.dbc");
            //VehicleUIIndSeat = new DBCFile<VehicleUIIndSeatEntry>(dbcFolder + "VehicleUIIndSeat.dbc");
            //VideoHardware = new DBCFile<VideoHardwareEntry>(dbcFolder + "VideoHardware.dbc");
            //VocalUISounds = new DBCFile<VocalUISoundsEntry>(dbcFolder + "VocalUISounds.dbc");
            //WeaponImpactSounds = new DBCFile<WeaponImpactSoundsEntry>(dbcFolder + "WeaponImpactSounds.dbc");
            //WeaponSwingSounds2 = new DBCFile<WeaponSwingSounds2Entry>(dbcFolder + "WeaponSwingSounds2.dbc");
            //Weather = new DBCFile<WeatherEntry>(dbcFolder + "Weather.dbc");
            //WMOAreaTable = new DBCFile<WMOAreaTableEntry>(dbcFolder + "WMOAreaTable.dbc");
            //WorldChunkSounds = new DBCFile<WorldChunkSoundsEntry>(dbcFolder + "WorldChunkSounds.dbc");
            WorldMapArea = new DBCFile<WorldMapAreaEntry>(dbcFolder + "WorldMapArea.dbc");
            //WorldMapContinent = new DBCFile<WorldMapContinentEntry>(dbcFolder + "WorldMapContinent.dbc");
            WorldMapOverlay = new DBCFile<WorldMapOverlayEntry>(dbcFolder + "WorldMapOverlay.dbc");
            //WorldMapTransforms = new DBCFile<WorldMapTransformsEntry>(dbcFolder + "WorldMapTransforms.dbc");
            //WorldSafeLocs = new DBCFile<WorldSafeLocsEntry>(dbcFolder + "WorldSafeLocs.dbc");
            //WorldStateUI = new DBCFile<WorldStateUIEntry>(dbcFolder + "WorldStateUI.dbc");
            //WorldStateZoneSounds = new DBCFile<WorldStateZoneSoundsEntry>(dbcFolder + "WorldStateZoneSounds.dbc");
            //WowErrorStrings = new DBCFile<WowErrorStringsEntry>(dbcFolder + "WowError_Strings.dbc");
            //ZoneIntroMusicTable = new DBCFile<ZoneIntroMusicTableEntry>(dbcFolder + "ZoneIntroMusicTable.dbc");
            //ZoneMusic = new DBCFile<ZoneMusicEntry>(dbcFolder + "ZoneMusic.dbc");
            #endregion
        }
    }
}
