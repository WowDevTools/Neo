using System.Windows.Forms;

namespace WoWEditor6.Dbc
{
    public static partial class DbcStores
    {
        public static void LoadTitlesEditorFiles()
        {
            try
            {
                DbcStores.CharTitles.LoadData();
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
                DbcStores.NamesProfanity.LoadData();
                DbcStores.NamesReserved.LoadData();
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
                DbcStores.Spell.LoadData();
                DbcStores.SkillLine.LoadData();
                DbcStores.SkillLineAbility.LoadData();
                DbcStores.SkillRaceClassInfo.LoadData();
                DbcStores.SpellFocusObject.LoadData();
                DbcStores.ChrRaces.LoadData();
                DbcStores.ChrClasses.LoadData();
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
                DbcStores.ChrClasses.LoadData();
                DbcStores.ChrRaces.LoadData();
                DbcStores.Faction.LoadData();
                DbcStores.FactionGroup.LoadData();
                DbcStores.FactionTemplate.LoadData();
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
                DbcStores.ChrClasses.LoadData();
                DbcStores.ChrRaces.LoadData();
                DbcStores.Spell.LoadData();
                DbcStores.SpellIcon.LoadData();
                DbcStores.Talent.LoadData();
                DbcStores.TalentTab.LoadData();
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
                DbcStores.Achievement.LoadData();
                DbcStores.AchievementCategory.LoadData();
                DbcStores.AchievementCriteria.LoadData();
                DbcStores.Map.LoadData();
                DbcStores.SpellIcon.LoadData();
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
                DbcStores.ChrRaces.LoadData();
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
                DbcStores.ChrClasses.LoadData();
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
                DbcStores.AreaPoi.LoadData();
                DbcStores.AreaTable.LoadData();
                DbcStores.DungeonMap.LoadData();
                DbcStores.Map.LoadData();
                DbcStores.WorldMapArea.LoadData();
                DbcStores.WorldMapOverlay.LoadData();
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
                DbcStores.WorldMapArea.LoadData();
                DbcStores.WorldMapOverlay.LoadData();
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
                DbcStores.Item.LoadData();
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
                DbcStores.GameTips.LoadData();
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
                DbcStores.Item.LoadData();
                DbcStores.GemProperties.LoadData();
                DbcStores.SpellItemEnchantment.LoadData();
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
                DbcStores.CharBaseInfo.LoadData();
                DbcStores.ChrClasses.LoadData();
                DbcStores.ChrRaces.LoadData();
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
                DbcStores.ItemSet.LoadData();
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
                DbcStores.CharStartOutfit.LoadData();
                DbcStores.ChrRaces.LoadData();
                DbcStores.ChrClasses.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
