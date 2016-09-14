using System.Collections.Generic;
using System.Windows.Forms;
using DBCLib.Structures;

namespace WoWEditor6.Dbc
{
    public static partial class DbcStores
    {
        public static void SaveTitlesEditorFiles()
        {
            try
            {
                DbcStores.CharTitles.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void SaveFactionsEditorFiles()
        {
            try
            {
                DbcStores.Faction.SaveDBC();
                DbcStores.FactionGroup.SaveDBC();
                DbcStores.FactionTemplate.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void SaveProfessionEditorFiles()
        {
            try
            {
                DbcStores.Spell.SaveDBC();
                DbcStores.SkillLine.SaveDBC();
                DbcStores.SkillLineAbility.SaveDBC();
                DbcStores.SkillRaceClassInfo.SaveDBC();
                DbcStores.SpellFocusObject.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void SaveTalentsEditorFiles(IComparer<TalentEntry> comparator)
        {
            try
            {
                DbcStores.Talent.SaveDBC(comparator);
                DbcStores.TalentTab.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void SaveAchievementsEditor()
        {
            try
            {
                DbcStores.Achievement.SaveDBC();
                DbcStores.AchievementCategory.SaveDBC();
                DbcStores.AchievementCriteria.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void SaveRacesEditorFiles()
        {
            try
            {
                DbcStores.ChrRaces.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void SavePoIsEditorFiles()
        {
            try
            {
                DbcStores.AreaPoi.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void SaveClassesEditorFiles()
        {
            try
            {
                DbcStores.ChrClasses.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void SaveMapsEditorFiles()
        {
            try
            {
                DbcStores.WorldMapArea.SaveDBC();
                DbcStores.WorldMapOverlay.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void SaveItemDbcGeneratorFiles()
        {
            try
            {
                DbcStores.Item.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void SaveGameTipsEditorFiles()
        {
            try
            {
                DbcStores.GameTips.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void SaveNamesReservedEditorFiles()
        {
            try
            {
                DbcStores.NamesReserved.SaveDBC();
                DbcStores.NamesProfanity.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void SaveGemsEditorFiles()
        {
            try
            {
                DbcStores.Item.SaveDBC();
                DbcStores.GemProperties.SaveDBC();
                DbcStores.SpellItemEnchantment.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void SaveRacesClassCombosEditorFiles()
        {
            try
            {
                DbcStores.ChrClasses.SaveDBC();
                DbcStores.ChrRaces.SaveDBC();
                DbcStores.CharBaseInfo.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void SaveItemSetEditorFiles()
        {
            try
            {
                DbcStores.ItemSet.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
