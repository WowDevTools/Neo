using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBCLib.Structures;

namespace WoWEditor6.Dbc
{
    public static partial class DbcStores
    {
        public static void LoadTitlesEditorFiles()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.CharTitles.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadNamesReservedFiles()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.NamesProfanity.LoadData();
                WowEditor6.Dbc.DbcStores.NamesReserved.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadProfessionEditorFiles()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.Spell.LoadData();
                WowEditor6.Dbc.DbcStores.SkillLine.LoadData();
                WowEditor6.Dbc.DbcStores.SkillLineAbility.LoadData();
                WowEditor6.Dbc.DbcStores.SkillRaceClassInfo.LoadData();
                WowEditor6.Dbc.DbcStores.SpellFocusObject.LoadData();
                WowEditor6.Dbc.DbcStores.ChrRaces.LoadData();
                WowEditor6.Dbc.DbcStores.ChrClasses.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadFactionsEditorFiles()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.ChrClasses.LoadData();
                WowEditor6.Dbc.DbcStores.ChrRaces.LoadData();
                WowEditor6.Dbc.DbcStores.Faction.LoadData();
                WowEditor6.Dbc.DbcStores.FactionGroup.LoadData();
                WowEditor6.Dbc.DbcStores.FactionTemplate.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadTalentsEditorFiles()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.ChrClasses.LoadData();
                WowEditor6.Dbc.DbcStores.ChrRaces.LoadData();
                WowEditor6.Dbc.DbcStores.Spell.LoadData();
                WowEditor6.Dbc.DbcStores.SpellIcon.LoadData();
                WowEditor6.Dbc.DbcStores.Talent.LoadData();
                WowEditor6.Dbc.DbcStores.TalentTab.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadAchievementsEditor()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.Achievement.LoadData();
                WowEditor6.Dbc.DbcStores.AchievementCategory.LoadData();
                WowEditor6.Dbc.DbcStores.AchievementCriteria.LoadData();
                WowEditor6.Dbc.DbcStores.Map.LoadData();
                WowEditor6.Dbc.DbcStores.SpellIcon.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadRacesEditorFiles()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.ChrRaces.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadClassesEditorFiles()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.ChrClasses.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadPoIsEditorFiles()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.AreaPoi.LoadData();
                WowEditor6.Dbc.DbcStores.AreaTable.LoadData();
                WowEditor6.Dbc.DbcStores.DungeonMap.LoadData();
                WowEditor6.Dbc.DbcStores.Map.LoadData();
                WowEditor6.Dbc.DbcStores.WorldMapArea.LoadData();
                WowEditor6.Dbc.DbcStores.WorldMapOverlay.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadMapsEditorFiles()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.WorldMapArea.LoadData();
                WowEditor6.Dbc.DbcStores.WorldMapOverlay.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadItemDbcGeneratorFiles()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.Item.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadGameTipsEditorFiles()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.GameTips.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadGemsEditorFiles()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.Item.LoadData();
                WowEditor6.Dbc.DbcStores.GemProperties.LoadData();
                WowEditor6.Dbc.DbcStores.SpellItemEnchantment.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadRacesClassCombosEditorFiles()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.CharBaseInfo.LoadData();
                WowEditor6.Dbc.DbcStores.ChrClasses.LoadData();
                WowEditor6.Dbc.DbcStores.ChrRaces.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadItemSetEditorFiles()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.ItemSet.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void LoadCharStartOutfit()
        {
            try
            {
                WowEditor6.Dbc.DbcStores.CharStartOutfit.LoadData();
                WowEditor6.Dbc.DbcStores.ChrRaces.LoadData();
                WowEditor6.Dbc.DbcStores.ChrClasses.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
