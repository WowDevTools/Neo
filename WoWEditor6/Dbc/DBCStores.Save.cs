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
                WowEditor6.Dbc.DbcStores.CharTitles.SaveDBC();
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
                WowEditor6.Dbc.DbcStores.Faction.SaveDBC();
                WowEditor6.Dbc.DbcStores.FactionGroup.SaveDBC();
                WowEditor6.Dbc.DbcStores.FactionTemplate.SaveDBC();
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
                WowEditor6.Dbc.DbcStores.Spell.SaveDBC();
                WowEditor6.Dbc.DbcStores.SkillLine.SaveDBC();
                WowEditor6.Dbc.DbcStores.SkillLineAbility.SaveDBC();
                WowEditor6.Dbc.DbcStores.SkillRaceClassInfo.SaveDBC();
                WowEditor6.Dbc.DbcStores.SpellFocusObject.SaveDBC();
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
                WowEditor6.Dbc.DbcStores.Talent.SaveDBC(comparator);
                WowEditor6.Dbc.DbcStores.TalentTab.SaveDBC();
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
                WowEditor6.Dbc.DbcStores.Achievement.SaveDBC();
                WowEditor6.Dbc.DbcStores.AchievementCategory.SaveDBC();
                WowEditor6.Dbc.DbcStores.AchievementCriteria.SaveDBC();
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
                WowEditor6.Dbc.DbcStores.ChrRaces.SaveDBC();
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
                WowEditor6.Dbc.DbcStores.AreaPoi.SaveDBC();
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
                WowEditor6.Dbc.DbcStores.ChrClasses.SaveDBC();
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
                WowEditor6.Dbc.DbcStores.WorldMapArea.SaveDBC();
                WowEditor6.Dbc.DbcStores.WorldMapOverlay.SaveDBC();
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
                WowEditor6.Dbc.DbcStores.Item.SaveDBC();
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
                WowEditor6.Dbc.DbcStores.GameTips.SaveDBC();
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
                WowEditor6.Dbc.DbcStores.NamesReserved.SaveDBC();
                WowEditor6.Dbc.DbcStores.NamesProfanity.SaveDBC();
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
                WowEditor6.Dbc.DbcStores.Item.SaveDBC();
                WowEditor6.Dbc.DbcStores.GemProperties.SaveDBC();
                WowEditor6.Dbc.DbcStores.SpellItemEnchantment.SaveDBC();
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
                WowEditor6.Dbc.DbcStores.ChrClasses.SaveDBC();
                WowEditor6.Dbc.DbcStores.ChrRaces.SaveDBC();
                WowEditor6.Dbc.DbcStores.CharBaseInfo.SaveDBC();
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
                WowEditor6.Dbc.DbcStores.ItemSet.SaveDBC();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
