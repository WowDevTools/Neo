using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DBCLib.Structures;
using Neo.Dbc;
using System.Collections;
using System.Collections.Generic;

namespace Neo.UI.DbcEditors
{
	internal struct labelStruct : IEnumerable
    {
        public string Text;
        //public string Tooltip;
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
	    private int raceNbr = 0;
	    private int startRaceNbr = 0;

	    private labelStruct[] labelArray = { new labelStruct() { Text = "Activated (12 or 15 are Activated)", x = 7, y = 7 },
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
                MessageBox.Show(ex.ToString());
            }
            //We're creating every tab => one races
            foreach (var entry in DbcStores.ChrRaces.Records)
            {
                ++raceNbr;
                if (entry.RaceNameNeutral.String != "0")
                {
	                this.lbMenu.Items.Add(entry.RaceNameNeutral.String);
                }
                else
                {
	                this.lbMenu.Items.Add("" + this.raceNbr);
                }

	            TabPage page = new TabPage();
                if (entry.RaceNameNeutral.String != "0")
                {
	                page.Name = "" + entry.RaceId;
                }
                else
                {
	                page.Name = "" + this.raceNbr;
                }
	            tbcEditor.TabPages.Add(page);
                FillTab(page,(int)entry.RaceId);

                if (entry.RaceId > startRaceNbr)
                {
	                this.startRaceNbr = (int)entry.RaceId;
                }
            }

            lbMenu.Size = new Size(150, (raceNbr*14 > 342) ? 342 : raceNbr * 14);
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
                    {
	                    box.Text = raceInformation[i];
                    }
                    else
                    {
	                    box.Text = "0";
                    }
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
                        {
	                        box.Text = raceInformation[i];
                        }
                        else
                        {
	                        box.Text = "0";
                        }
	                    box.Size = new Size(121, 21);
                        box.Location = new Point(7, 23+(i*44));
                        page.Controls.Add(box);
                    }
                    else
                    {
                        TextBox box = new TextBox();
                        if (raceId > 0)
                        {
	                        box.Text = raceInformation[i];
                        }
                        else
                        {
	                        box.Text = "0";
                        }
	                    if (i == 13)
	                    {
		                    box.KeyDown += new KeyEventHandler(tb_KeyDown);
	                    }
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
            lbMenu.Items.Add("New race "+ raceNbr);
            TabPage page = new TabPage();
            page.Name = "" + raceNbr;
            tbcEditor.TabPages.Add(page);
            FillTab(page, -1);
            ChrRacesEntry race = new ChrRacesEntry();
            race.RaceId = (uint)raceNbr;
            race.RaceNameNeutral = "New race " + raceNbr;
            //Add an empty race for the up/down button
            DbcStores.ChrRaces.AddEntry((uint)raceNbr, race);
            lbMenu.Size = new Size(150, (raceNbr*14 > 342) ? 342 : raceNbr* 14);
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (lbMenu.SelectedIndex < 21)
            {
                MessageBox.Show("Please do not remove blizzard races to avoid problems");
                return;
            }
            --raceNbr;
            tbcEditor.TabPages.RemoveAt(lbMenu.SelectedIndex);
            lbMenu.Items.Remove(lbMenu.SelectedItem);

            lbMenu.Size = new Size(150, (raceNbr * 14 > 342) ? 342 : raceNbr * 14);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DbcStores.ChrRaces.ClearDbc();
            //Loop trough each tabs
            foreach (TabPage tab in tbcEditor.TabPages)
            {
                var race = new ChrRacesEntry();
                int inputFieldscount = 0;
                //Loop trough each controls
                foreach(Control control in tab.Controls)
                {
                    //If it's an input field, get the value
                    if (control is TextBox || control is ComboBox)
                    {
                        switch(inputFieldscount)
                        {
                            case 0:
                                    race.Flags = uint.Parse(control.Text);
                                break;
                            case 1:
                                    race.FactionId = uint.Parse(control.Text);
                                break;
                            case 2:
                                    race.ExplorationSoundId = uint.Parse(control.Text);
                                break;
                            case 3:
                                    race.ModelM = uint.Parse(control.Text);
                                break;
                            case 4:
                                    race.ModelF = uint.Parse(control.Text);
                                break;
                            case 5:
                                    race.ClientPrefix = control.Text;
                                break;
                            case 6:
                                    race.RequiredExpansion = uint.Parse(control.Text);
                                break;
                            case 7:
                                    race.HairCustomization = control.Text;
                                break;
                            case 8:
                                    race.FacialHairCustomization = control.Text;
                                break;
                            case 9:
                                    race.FacialHairCustomization2 = control.Text;
                                break;
                            case 10:
                                    race.BaseLanguage = uint.Parse(control.Text);
                                break;
                            case 11:
                                    race.ClientFileString = control.Text;
                                break;
                            case 12:
                                    race.CinematicSequenceId = uint.Parse(control.Text);
                                break;
                            case 13:
                                    race.RaceNameNeutral = control.Text;
                                break;
                            case 14:
                                    race.RaceNameFemale = control.Text;
                                break;
                            case 15:
                                    race.RaceNameMale = control.Text;
                                break;
                            case 16:
                                    race.Alliance = uint.Parse(control.Text);
                                break;
                            case 17:
                                    race.ResSicknessSpellId = uint.Parse(control.Text);
                                break;
                            case 18:
                                    race.CreatureType = uint.Parse(control.Text);
                                break;
                            case 19:
                                    race.SplashSoundId = uint.Parse(control.Text);
                                break;
                        }

                        ++inputFieldscount;
                    }
                }

                race.RaceId = (uint)tbcEditor.TabPages.IndexOf(tab)+1;
                DbcStores.ChrRaces.AddEntry(race.RaceId, race);
            }
            //Everything is ok, save
            DbcStores.ChrRaces.SaveDBC();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(lbMenu.SelectedIndex <= 21)
            {
                MessageBox.Show("Please do not move blizzard races to avoid problems");
                return;
            }
            int count = 0;
            int index = tbcEditor.SelectedIndex;
            TabPage[] pages = new TabPage[tbcEditor.TabPages.Count];
            foreach(TabPage page in tbcEditor.TabPages)
            {
                pages[count] = page;
                ++count;
            }

            TabPage temp = pages[index];
            pages[index] = pages[index - 1];
            pages[index - 1] = temp;

            tbcEditor.TabPages.Clear();
            lbMenu.Items.Clear();

            foreach (TabPage page in pages)
            {
                tbcEditor.TabPages.Add(page);

                foreach (var entry in DbcStores.ChrRaces.Records)
                {
                    if (entry.RaceId == uint.Parse(page.Name.ToString()))
                    {
	                    this.lbMenu.Items.Add("" + entry.RaceNameNeutral);
                    }
                }
            }
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            if (lbMenu.SelectedIndex < 21)
            {
                MessageBox.Show("Please do not move blizzard races to avoid problems");
                return;
            }
            int count = 0;
            int index = tbcEditor.SelectedIndex;
            TabPage[] pages = new TabPage[tbcEditor.TabPages.Count];
            foreach (TabPage page in tbcEditor.TabPages)
            {
                pages[count] = page;
                ++count;
            }

            if(index+1 >= pages.Length)
            {
                MessageBox.Show("This race is already at the bottom");
                return;
            }
            TabPage temp = pages[index];
            pages[index] = pages[index + 1];
            pages[index + 1] = temp;

            tbcEditor.TabPages.Clear();
            lbMenu.Items.Clear();

            foreach (TabPage page in pages)
            {
                tbcEditor.TabPages.Add(page);

                foreach (var entry in DbcStores.ChrRaces.Records)
                {
                    if (entry.RaceId == uint.Parse(page.Name.ToString()))
                    {
	                    this.lbMenu.Items.Add("" + entry.RaceNameNeutral);
                    }
                }
            }
        }
    }

    internal class ChrRacesEditorControlDesigner : ControlDesigner
    {
        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);

            var ctl = (this.Control as ChrRacesEditorControl).TabControl as TabControl;
            EnableDesignMode(ctl, "TabControl");
            foreach (TabPage page in ctl.TabPages)
            {
	            EnableDesignMode(page, page.Name);
            }
        }
    }
}
