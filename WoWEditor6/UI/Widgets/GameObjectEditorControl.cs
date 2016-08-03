using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections.Generic;

namespace WoWEditor6.UI.Dialog
{
    [Designer(typeof(GameObjectEditorControlDesigner))]
    public partial class GameObjectEditorControl : UserControl
    {
        private int nbData;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabControl TabControl
        {
            get { return this.tbcEditor; }
        }

        public GameObjectEditorControl()
        {
            InitializeComponent();
            tbcEditor.Appearance = TabAppearance.FlatButtons;
            tbcEditor.ItemSize = new Size(0, 1);
            tbcEditor.SizeMode = TabSizeMode.Fixed;
        }



        private void showModelInRenderControl(string pModelId)
        {
            if (!string.IsNullOrEmpty(pModelId))
            {
                int displayId;
                if (Int32.TryParse(pModelId, out displayId))
                {
                    MessageBox.Show("" + displayId);
                }

            }
        }

        public void loadGameObject(Storage.Database.WotLk.TrinityCore.GameObject gameObject)
        {
           
        }


        private uint makeFlagOrBitmask(CheckedListBox list, Type e)
        {
            if (!e.IsEnum)
                return 0;

            uint myFlags = 0x0;

            foreach (Object item in list.CheckedItems)
            {
                myFlags += Convert.ToUInt32(Enum.Parse(e, item.ToString()));
            }

            return myFlags;
        }

        private void checkFlagOrBitmask(CheckedListBox list, Type e, uint value)
        {
            if (!e.IsEnum)
                return;

            list.ClearSelected();

            var values = Enum.GetValues(e);

            for (int i = values.Length - 1; i > 0; i--)
            {
                uint val = Convert.ToUInt32(values.GetValue(i));
                if (val <= value)
                {
                    list.SetItemCheckState(i, CheckState.Checked);
                    value -= val;
                }
            }
        }

        private void lbMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbcEditor.SelectedIndex = lbMenu.SelectedIndex;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void btnShowModelId1_Click(object sender, EventArgs e)
        {
            showModelInRenderControl(ModelId.Text);
        }


        /*
        *  That's really tricky, but couldn't find another way to don't have a thousand of lines or ugly code, let me know if you have another one.
        */

        /* 
        *  Type :
        *  Text = textBox
        *  List = comboBox with data.startnbr to data.endnbr
        *  Always = Always data.startnbr
        */
        enum type
        {
            Text = 0,
            List = 1,
            Always = 3
        }

        //Data
        struct data
        {
            public type type;
            public int startNbr;
            public int endNbr;
            public string desc; //will appear as a tooltip on the data label
        };

        private void Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            WarningLabel1.Hide();
            WarningLabel2.Hide();

            for(int i = 0; i< nbData;i++)
            {
                if (i < 12)
                    tbData.Controls.RemoveByKey("Data" + i);
                else
                    tbData2.Controls.RemoveByKey("Data" + i);
            }

            nbData = 0;

            List<data> list = new List<data>();
            Label[] dataArray = { label6, label7, label8, label9, label10, label11, label12, label13, label14, label15, label16, label17, label18, label19, label20, label21, label22, label23, label24, label25, label26, label27, label28, label29 };

            foreach(Label item in dataArray)
            {
                toolTip1.SetToolTip(item, "");
                item.Hide();
            }

            switch(Type.Text)
            {
                case "Door":
                    nbData = 6;
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc="startOpen (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "open (LockId from Lock.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "autoClose (Time in milliseconds)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "noDamageImmune (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "openTextID (Unknown Text ID)" });
                    list.Add(new data() { type = type.Text, desc = "closeTextID (Unknown Text ID)" });
                    break;

                case "Button":
                    nbData = 9;
                    list.Add(new data() { type = type.Text, desc = "startOpen (State)" }); //Maybe make it a list if the possible state are known.
                    list.Add(new data() { type = type.Text, desc = "open (LockId from Lock.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "autoClose (long unknown flag)" });
                    list.Add(new data() { type = type.Text, desc = "linkedTrap (gameobject_template.entry (Spawned GO type 6))" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "noDamageImmune (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "large? (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "openTextID (Unknown Text ID)" });
                    list.Add(new data() { type = type.Text, desc = "closeTextID (Unknown Text ID)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "losOK (Boolean flag)" });
                    break;

                case "QuestGiver":
                    nbData = 10;
                    list.Add(new data() { type = type.Text, desc = "open (LockId from Lock.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "questList (unknown ID)" });
                    list.Add(new data() { type = type.Text, desc = "pageMaterial (PageTextMaterial.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "gossipID (gossip_menu id)" });
                    list.Add(new data() { type = type.List, startNbr = 1, endNbr = 4, desc = "customAnim (unknown value from 1 to 4)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "noDamageImmune (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "openTextID (broadcast_text ID)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "losOK (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "allowMounted (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "large? (Boolean flag)" });
                    break;

                case "Chest":
                    nbData = 16;
                    list.Add(new data() { type = type.Text, desc = "open (LockId from Lock.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "chestLoot (gameobject_loot_template.entry)" });
                    list.Add(new data() { type = type.Text, desc = "chestRestockTime (time in seconds)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "consumable (State: Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "minRestock (Min successful loot attempts for Mining, Herbalism etc)" });
                    list.Add(new data() { type = type.Text, desc = "maxRestock (Max successful loot attempts for Mining, Herbalism etc)" });
                    list.Add(new data() { type = type.Text, desc = "lootedEvent (unknown ID)" });
                    list.Add(new data() { type = type.Text, desc = "linkedTrap (gameobject_template.entry (Spawned GO type 6))" });
                    list.Add(new data() { type = type.Text, desc = "questID (quest_template.id of completed quest)" });
                    list.Add(new data() { type = type.Text, desc = "level (minimal level required to open this gameobject)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "losOK (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "leaveLoot (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "notInCombat (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "log loot (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "openTextID (Unknown ID)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "use group loot rules (Boolean flag)" });
                    break;

                case "Binder":
                    nbData = -1;
                    break;

                case "Generic":
                    nbData = 6;
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "floatingTooltip (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "highlight (Boolean flag)" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "serverOnly? (Always 0)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "large? (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "floatOnWater (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "questID (Required active quest_template.id to work)" });
                    break;

                case "Trap":
                    nbData = 13;
                    list.Add(new data() { type = type.Text, desc = "open (LockId from Lock.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "chestLoot (gameobject_loot_template.entry)" });
                    list.Add(new data() { type = type.Text, desc = "chestRestockTime (time in seconds)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "consumable (State: Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "minRestock (Min successful loot attempts for Mining, Herbalism etc)" });
                    list.Add(new data() { type = type.Text, desc = "maxRestock (Max successful loot attempts for Mining, Herbalism etc)" });
                    list.Add(new data() { type = type.Text, desc = "lootedEvent (unknown ID)" });
                    list.Add(new data() { type = type.Text, desc = "linkedTrap (gameobject_template.entry (Spawned GO type 6))" });
                    list.Add(new data() { type = type.Text, desc = "questID (quest_template.id of completed quest)" });
                    list.Add(new data() { type = type.Text, desc = "level (minimal level required to open this gameobject)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "losOK (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "leaveLoot (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "notInCombat (Boolean flag)" });
                    break;

                case "Chair":
                    nbData = 2;
                    break;

                case "SpellFocus":
                    nbData = 7;
                    break;

                case "Text":
                    nbData = 3;
                    break;

                case "Goober":
                    nbData = 20;
                    break;

                case "Transport":
                    nbData = 0;
                    break;

                case "AreaDamage":
                    nbData = 0;
                    break;

                case "Camera":
                    nbData = 2;
                    break;

                case "MapObject":
                    nbData = 0;
                    break;

                case "MoTransport":
                    nbData = 9;
                    break;

                case "DuelArbiter":
                    nbData = 0;
                    break;

                case "FishingNode":
                    nbData = 0;
                    break;

                case "Ritual":
                    nbData = 7;
                    break;

                case "MailBox":
                    nbData = 0;
                    break;

                case "AuctionHouse":
                    nbData = 1;
                    break;

                case "GuardPost":
                    nbData = 2;
                    break;

                case "SpellCaster":
                    nbData = 3;
                    break;

                case "MeetingStone":
                    nbData = 3;
                    break;

                case "FlagStand":
                    nbData = 8;
                    break;

                case "FishingPole":
                    nbData = 4;
                    break;

                case "FlagDrop":
                    nbData = 4;
                    break;

                case "MiniGame":
                    nbData = -1;
                    break;

                case "LotteryKiosk":
                    nbData = -1;
                    break;

                case "CapturePoint":
                    nbData = 19;
                    break;

                case "AuraGenerator":
                    nbData = 4;
                    break;

                case "DungeonDifficulty":
                    nbData = 2;
                    break;

                case "BarberChair":
                    nbData = 0;
                    break;

                case "DestructibleBuilding":
                    nbData = 24;
                    break;

                case "GuildBank":
                    nbData = 0;
                    break;

                case "TrapDoor":
                    nbData = 3;
                    break;

            }

            if (nbData == -1)
            {
                WarningLabel1.Text = "Object type not used";
                WarningLabel2.Text = "Object type not used";
                WarningLabel1.Show();
                WarningLabel2.Show();
            }
            else if(nbData == 0)
            {
                WarningLabel1.Text = "All data are always 0";
                WarningLabel2.Text = "All data are always 0";
                WarningLabel1.Show();
                WarningLabel2.Show();
            }
            else
            {             
                for (int i = 0; i < nbData; i++)
                {
                    dataArray[i].Show();
                    toolTip1.SetToolTip(dataArray[i], list[i].desc);
                }

                int d = 0;

                foreach(data item in list)
                {
                    switch(item.type)
                    {
                        case type.Text:
                            TextBox textBox = new TextBox();
                            textBox.Name = "Data"+d;
                            textBox.Location = new System.Drawing.Point(73, 17 + d*30);
                            textBox.Size = new System.Drawing.Size(456, 20);
                            if(d < 12)
                                tbData.Controls.Add(textBox);
                            else
                                tbData2.Controls.Add(textBox);
                            break;

                        case type.List:
                            ComboBox comboBox = new ComboBox();
                            comboBox.Name = "Data" + d;
                            comboBox.Location = new System.Drawing.Point(73, 17 + d * 30);
                            comboBox.Size = new System.Drawing.Size(456, 20);

                            for(int a = item.startNbr; a <= item.endNbr; a++)
                            {
                                comboBox.Items.Add(a);
                            }

                            if (d < 12)
                                tbData.Controls.Add(comboBox);
                            else
                                tbData2.Controls.Add(comboBox);
                            break;

                        case type.Always:
                            TextBox alwaysTextBox = new TextBox();
                            alwaysTextBox.Name = "Data" + d;
                            alwaysTextBox.Location = new System.Drawing.Point(73, 17 + d * 30);
                            alwaysTextBox.Size = new System.Drawing.Size(456, 20);
                            alwaysTextBox.Text = Convert.ToString(item.startNbr);
                            alwaysTextBox.Enabled = false;
                            if (d < 12)
                                tbData.Controls.Add(alwaysTextBox);
                            else
                                tbData2.Controls.Add(alwaysTextBox);
                            break;
                    }

                    d++;
                }
            }
        }
    }

    internal class GameObjectEditorControlDesigner : ControlDesigner
    {
        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);

            var ctl = (this.Control as GameObjectEditorControl).TabControl as TabControl;
            EnableDesignMode(ctl, "TabControl");
            foreach (TabPage page in ctl.TabPages) EnableDesignMode(page, page.Name);
        }
    }
}

