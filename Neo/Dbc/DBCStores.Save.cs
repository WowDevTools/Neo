using System;
using System.Collections.Generic;
using DBCLib.Structures;

namespace Neo.Dbc
{
    public static partial class DbcStores
    {
        public static void SaveTitlesEditorFiles()
        {
            try
            {
                CharTitles.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveFactionsEditorFiles()
        {
            try
            {
                Faction.SaveDBC();
                FactionGroup.SaveDBC();
                FactionTemplate.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveProfessionEditorFiles()
        {
            try
            {
                Spell.SaveDBC();
                SkillLine.SaveDBC();
                SkillLineAbility.SaveDBC();
                SkillRaceClassInfo.SaveDBC();
                SpellFocusObject.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveTalentsEditorFiles(IComparer<TalentEntry> comparator)
        {
            try
            {
                Talent.SaveDBC(comparator);
                TalentTab.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveAchievementsEditor()
        {
            try
            {
                Achievement.SaveDBC();
                AchievementCategory.SaveDBC();
                AchievementCriteria.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveRacesEditorFiles()
        {
            try
            {
                ChrRaces.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SavePoIsEditorFiles()
        {
            try
            {
                AreaPoi.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveClassesEditorFiles()
        {
            try
            {
                ChrClasses.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveMapsEditorFiles()
        {
            try
            {
                WorldMapArea.SaveDBC();
                WorldMapOverlay.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveItemDbcGeneratorFiles()
        {
            try
            {
                Item.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveGameTipsEditorFiles()
        {
            try
            {
                GameTips.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveNamesReservedEditorFiles()
        {
            try
            {
                NamesReserved.SaveDBC();
                NamesProfanity.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveGemsEditorFiles()
        {
            try
            {
                Item.SaveDBC();
                GemProperties.SaveDBC();
                SpellItemEnchantment.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveRacesClassCombosEditorFiles()
        {
            try
            {
                ChrClasses.SaveDBC();
                ChrRaces.SaveDBC();
                CharBaseInfo.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveItemSetEditorFiles()
        {
            try
            {
                ItemSet.SaveDBC();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
