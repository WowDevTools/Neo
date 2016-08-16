using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DBCLib.Structures;
using WoWEditor6.Dbc;
using System.Collections;
using System.Collections.Generic;

namespace WoWEditor6.UI.DbcEditors
{   
    struct labelStruct : IEnumerable
    {
        public string Text;
        public string Tooltip;
        public int x;
        public int y;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }
    }

    [Designer(typeof(ChrRacesEditorControlDesigner))]
    public partial class ChrRacesEditorControl : UserControl
    {
        int raceNbr = 0;
        labelStruct[] labelArray = { new labelStruct() { Text = "Actiaved (12 or 15 are Activated)", x = 7, y = 7 },
                                     new labelStruct() { Text = "Faction", x = 7, y = 51 },
                                     new labelStruct() { Text = "Exploration", x = 7, y = 95 },
                                     new labelStruct() { Text = "Male Model", x = 7, y = 139 },
                                     new labelStruct() { Text = "Female Model", x = 7, y = 183 },
                                     new labelStruct() { Text = "Helm Type", x = 7, y = 226 },
                                     new labelStruct() { Text = "Expansion", x = 7, y = 270 },
                                     new labelStruct() { Text = "Hair Customization", x = 7, y = 313 },
                                     new labelStruct() { Text = "Facial Customization 1", x = 7, y = 357 },
                                     new labelStruct() { Text = "Facial Customization 2", x = 7, y = 401 },
                                     new labelStruct() { Text = "Language", x = 270, y = 7 },
                                     new labelStruct() { Text = "ClientFile String", x = 270, y = 51 },
                                     new labelStruct() { Text = "Cinematic ID", x = 270, y = 95 },
                                     new labelStruct() { Text = "RaceName Neutral", x = 270, y = 139 },
                                     new labelStruct() { Text = "Female Race Name", x = 270, y = 183 },
                                     new labelStruct() { Text = "Male Race Name", x = 270, y = 226 },
                                     new labelStruct() { Text = "Alliance", x = 270, y = 270 },
                                     new labelStruct() { Text = "Sikness ID Spell", x = 270, y = 313 },
                                     new labelStruct() { Text = "Creature Type", x = 270, y = 357 },
                                     new labelStruct() { Text = "Splash Sound ID", x = 270, y = 401 }};

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabControl TabControl
        {
            get { return this.tbcEditor; }
        }

        public ChrRacesEditorControl()
        {
            InitializeComponent();
            tbcEditor.Appearance = TabAppearance.FlatButtons;
            tbcEditor.ItemSize = new Size(0, 1);
            tbcEditor.SizeMode = TabSizeMode.Fixed;
        }

        private void ChrRacesEditorControl_Load(object sender, EventArgs e)
        {
            //Load the dbc
            try
            {
                DbcStores.LoadRacesEditorFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //We're creating every tab => one races
            foreach (var entry in DbcStores.ChrRaces.Records)
            {
                ++raceNbr;
                lbMenu.Items.Add(entry.RaceNameNeutral.String);
                TabPage page = new TabPage();
                page.Name = entry.RaceNameNeutral.String;
                tbcEditor.TabPages.Add(page);
                FillTab(page,(int)entry.RaceId);
            }

            lbMenu.Size = new Size(150, (raceNbr*14 > 446) ? 446 : raceNbr * 14);
            lbMenu.SelectedIndex = 0;

        }

        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextBox textBox = sender as TextBox;
                if (textBox != null)
                {
                    lbMenu.Items[lbMenu.SelectedIndex] = textBox.Text;
                }
            }
        }

        private void FillTab(TabPage page, int raceId)
        {
            List<string> raceInformation = new List<string>();
            if(raceId > 0)
            {
                var c = DbcStores.ChrRaces[(uint)raceId];
                raceInformation.Add(c.Flags.ToString());
                raceInformation.Add(c.FactionId.ToString());
                raceInformation.Add(c.ExplorationSoundId.ToString());
                raceInformation.Add(c.ModelM.ToString());
                raceInformation.Add(c.ModelF.ToString());
                raceInformation.Add(c.ClientPrefix.ToString());
                raceInformation.Add(c.RequiredExpansion.ToString());
                raceInformation.Add(c.HairCustomization.ToString());
                raceInformation.Add(c.FacialHairCustomization.ToString());
                raceInformation.Add(c.FacialHairCustomization2.ToString());
                raceInformation.Add(c.BaseLanguage.ToString());
                raceInformation.Add(c.ClientFileString.ToString());
                raceInformation.Add(c.CinematicSequenceId.ToString());
                raceInformation.Add(c.RaceNameNeutral.String);
                raceInformation.Add(c.RaceNameFemale.String);
                raceInformation.Add(c.RaceNameMale.String);
                raceInformation.Add(c.Alliance.ToString());
                raceInformation.Add(c.ResSicknessSpellId.ToString());
                raceInformation.Add(c.CreatureType.ToString());
                raceInformation.Add(c.SplashSoundId.ToString());
            }

            //Create and place every label
            foreach (labelStruct item in labelArray)
            {
                Label label = new Label();
                label.Text = item.Text;
                label.Location = new Point(item.x, item.y);
                label.Size = new Size(163, 13);
                page.Controls.Add(label);
            }

            //Create and place every box
            for (int i = 0;i<20;++i)
            {
                if(i==0)
                {
                    ComboBox box = new ComboBox();
                    if (raceId > 0)
                        box.Text = raceInformation[i];
                    box.Size = new Size(121,21);
                    box.Location = new Point(7, 23);
                    page.Controls.Add(box);
                }
                else
                {
                    if(i<10)
                    {
                        TextBox box = new TextBox();
                        if (raceId > 0)
                            box.Text = raceInformation[i];
                        box.Size = new Size(121, 21);
                        box.Location = new Point(7, 23+(i*44));
                        page.Controls.Add(box);
                    }
                    else
                    {
                        TextBox box = new TextBox();
                        if (raceId > 0)
                            box.Text = raceInformation[i];
                        if(i == 13)
                            box.KeyDown += new KeyEventHandler(tb_KeyDown);
                        box.Size = new Size(121, 21);
                        box.Location = new Point(270, 23 + ((i-10) * 44));
                        page.Controls.Add(box);
                    }
                }
            }

        }

        private void lbMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbcEditor.SelectedIndex = lbMenu.SelectedIndex;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            ++raceNbr;
            lbMenu.Items.Add("New race");
            TabPage page = new TabPage();
            page.Name = "NewRace";
            tbcEditor.TabPages.Add(page);
            FillTab(page, -1);

            lbMenu.Size = new Size(150, (raceNbr*14 > 446) ? 446 : raceNbr* 14);
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            --raceNbr;
            tbcEditor.TabPages.RemoveByKey(lbMenu.SelectedItem.ToString());
            lbMenu.Items.Remove(lbMenu.SelectedItem);

            lbMenu.Size = new Size(150, (raceNbr * 14 > 446) ? 446 : raceNbr * 14);
        }
    }

    internal class ChrRacesEditorControlDesigner : ControlDesigner
    {
        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);

            var ctl = (this.Control as ChrRacesEditorControl).TabControl as TabControl;
            EnableDesignMode(ctl, "TabControl");
            foreach (TabPage page in ctl.TabPages) EnableDesignMode(page, page.Name);
        }
    }
}
