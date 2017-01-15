using System;

namespace Neo.Dbc
{
    public static partial class DbcStores
    {
        public static void LoadTitlesEditorFiles()
        {
            try
            {
                InitFiles();
                CharTitles.LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void LoadNamesReservedFiles()
        {
            try
            {
                InitFiles();
                NamesProfanity.LoadData();
                NamesReserved.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }

        public static void LoadProfessionEditorFiles()
        {
            try
            {
                InitFiles();
                Spell.LoadData();
                SkillLine.LoadData();
                SkillLineAbility.LoadData();
                SkillRaceClassInfo.LoadData();
                SpellFocusObject.LoadData();
                ChrRaces.LoadData();
                ChrClasses.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }

        public static void LoadFactionsEditorFiles()
        {
            try
            {
                InitFiles();
                ChrClasses.LoadData();
                ChrRaces.LoadData();
                Faction.LoadData();
                FactionGroup.LoadData();
                FactionTemplate.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }

        public static void LoadTalentsEditorFiles()
        {
            try
            {
                InitFiles();
                ChrClasses.LoadData();
                ChrRaces.LoadData();
                Spell.LoadData();
                SpellIcon.LoadData();
                Talent.LoadData();
                TalentTab.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }

        public static void LoadAchievementsEditor()
        {
            try
            {
                InitFiles();
                Achievement.LoadData();
                AchievementCategory.LoadData();
                AchievementCriteria.LoadData();
                Map.LoadData();
                SpellIcon.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }

        public static void LoadRacesEditorFiles()
        {
            try
            {
                InitFiles();
                ChrRaces.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }

        public static void LoadClassesEditorFiles()
        {
            try
            {
                InitFiles();
                ChrClasses.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }

        public static void LoadPoIsEditorFiles()
        {
            try
            {
                InitFiles();
                AreaPoi.LoadData();
                AreaTable.LoadData();
                DungeonMap.LoadData();
                Map.LoadData();
                WorldMapArea.LoadData();
                WorldMapOverlay.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }

        public static void LoadMapsEditorFiles()
        {
            try
            {
                InitFiles();
                WorldMapArea.LoadData();
                WorldMapOverlay.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }

        public static void LoadItemDbcGeneratorFiles()
        {
            try
            {
                InitFiles();
                Item.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }

        public static void LoadGameTipsEditorFiles()
        {
            try
            {
                InitFiles();
                GameTips.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }

        public static void LoadGemsEditorFiles()
        {
            try
            {
                InitFiles();
                Item.LoadData();
                GemProperties.LoadData();
                SpellItemEnchantment.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }

        public static void LoadRacesClassCombosEditorFiles()
        {
            try
            {
                InitFiles();
                CharBaseInfo.LoadData();
                ChrClasses.LoadData();
                ChrRaces.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }

        public static void LoadItemSetEditorFiles()
        {
            try
            {
                InitFiles();
                ItemSet.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }

        public static void LoadCharStartOutfit()
        {
            try
            {
                InitFiles();
                CharStartOutfit.LoadData();
                ChrRaces.LoadData();
                ChrClasses.LoadData();
            }
            catch (Exception ex)
            {
	            Console.WriteLine(ex.Message);
            }
        }
    }
}
