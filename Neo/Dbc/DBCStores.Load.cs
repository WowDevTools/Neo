using System.Windows.Forms;

namespace Neo.Dbc
{
    public static partial class DbcStores
    {
        public static void LoadTitlesEditorFiles()
        {
            try
            {
                DbcStores.InitFiles();
                CharTitles.LoadData();
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
                DbcStores.InitFiles();
                NamesProfanity.LoadData();
                NamesReserved.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LoadProfessionEditorFiles()
        {
            try
            {
                DbcStores.InitFiles();
                Spell.LoadData();
                SkillLine.LoadData();
                SkillLineAbility.LoadData();
                SkillRaceClassInfo.LoadData();
                SpellFocusObject.LoadData();
                ChrRaces.LoadData();
                ChrClasses.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LoadFactionsEditorFiles()
        {
            try
            {
                DbcStores.InitFiles();
                ChrClasses.LoadData();
                ChrRaces.LoadData();
                Faction.LoadData();
                FactionGroup.LoadData();
                FactionTemplate.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LoadTalentsEditorFiles()
        {
            try
            {
                DbcStores.InitFiles();
                ChrClasses.LoadData();
                ChrRaces.LoadData();
                Spell.LoadData();
                SpellIcon.LoadData();
                Talent.LoadData();
                TalentTab.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LoadAchievementsEditor()
        {
            try
            {
                DbcStores.InitFiles();
                Achievement.LoadData();
                AchievementCategory.LoadData();
                AchievementCriteria.LoadData();
                Map.LoadData();
                SpellIcon.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LoadRacesEditorFiles()
        {
            try
            {
                DbcStores.InitFiles();
                ChrRaces.LoadData();
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
                DbcStores.InitFiles();
                ChrClasses.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LoadPoIsEditorFiles()
        {
            try
            {
                DbcStores.InitFiles();
                AreaPoi.LoadData();
                AreaTable.LoadData();
                DungeonMap.LoadData();
                Map.LoadData();
                WorldMapArea.LoadData();
                WorldMapOverlay.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LoadMapsEditorFiles()
        {
            try
            {
                DbcStores.InitFiles();
                WorldMapArea.LoadData();
                WorldMapOverlay.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LoadItemDbcGeneratorFiles()
        {
            try
            {
                DbcStores.InitFiles();
                Item.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LoadGameTipsEditorFiles()
        {
            try
            {
                DbcStores.InitFiles();
                GameTips.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LoadGemsEditorFiles()
        {
            try
            {
                DbcStores.InitFiles();
                Item.LoadData();
                GemProperties.LoadData();
                SpellItemEnchantment.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LoadRacesClassCombosEditorFiles()
        {
            try
            {
                DbcStores.InitFiles();
                CharBaseInfo.LoadData();
                ChrClasses.LoadData();
                ChrRaces.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LoadItemSetEditorFiles()
        {
            try
            {
                DbcStores.InitFiles();
                ItemSet.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void LoadCharStartOutfit()
        {
            try
            {
                DbcStores.InitFiles();
                CharStartOutfit.LoadData();
                ChrRaces.LoadData();
                ChrClasses.LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
