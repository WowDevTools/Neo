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
            Entry.Text = gameObject.EntryId.ToString();
            Type.Text = gameObject.Type.ToString();
            Type_SelectedIndexChanged(this, EventArgs.Empty);
            ModelId.Text = gameObject.DisplayId.ToString();
            GameObjectName.Text = gameObject.Name;
            IconName.Text = gameObject.IconName;
            CastBarCaption.Text = gameObject.CastBarCaption;
            Faction.Text = gameObject.Faction.ToString();
            checkFlagOrBitmask(Flags, typeof(Storage.Database.WotLk.TrinityCore.Flags), gameObject.Flags);
            GameObjectSize.Text = gameObject.Size.ToString();
            if(tbData.Controls.ContainsKey("Data0"))
                tbData.Controls.Find("Data0", false)[0].Text = gameObject.Data0.ToString();
            if (tbData.Controls.ContainsKey("Data1"))
                tbData.Controls.Find("Data1", false)[0].Text = gameObject.Data1.ToString();
            if (tbData.Controls.ContainsKey("Data2"))
                tbData.Controls.Find("Data2", false)[0].Text = gameObject.Data2.ToString();
            if (tbData.Controls.ContainsKey("Data3"))
                tbData.Controls.Find("Data3", false)[0].Text = gameObject.Data3.ToString();
            if (tbData.Controls.ContainsKey("Data4"))
                tbData.Controls.Find("Data4", false)[0].Text = gameObject.Data4.ToString();
            if (tbData.Controls.ContainsKey("Data5"))
                tbData.Controls.Find("Data5", false)[0].Text = gameObject.Data5.ToString();
            if (tbData.Controls.ContainsKey("Data6"))
                tbData.Controls.Find("Data6", false)[0].Text = gameObject.Data6.ToString();
            if (tbData.Controls.ContainsKey("Data7"))
                tbData.Controls.Find("Data7", false)[0].Text = gameObject.Data7.ToString();
            if (tbData.Controls.ContainsKey("Data8"))
                tbData.Controls.Find("Data8", false)[0].Text = gameObject.Data8.ToString();
            if (tbData.Controls.ContainsKey("Data9"))
                tbData.Controls.Find("Data9", false)[0].Text = gameObject.Data9.ToString();
            if (tbData.Controls.ContainsKey("Data10"))
                tbData.Controls.Find("Data10", false)[0].Text = gameObject.Data10.ToString();
            if (tbData.Controls.ContainsKey("Data11"))
                tbData.Controls.Find("Data11", false)[0].Text = gameObject.Data11.ToString();
            if (tbData2.Controls.ContainsKey("Data12"))
                tbData2.Controls.Find("Data12", false)[0].Text = gameObject.Data12.ToString();
            if (tbData2.Controls.ContainsKey("Data13"))
                tbData2.Controls.Find("Data13", false)[0].Text = gameObject.Data13.ToString();
            if (tbData2.Controls.ContainsKey("Data14"))
                tbData2.Controls.Find("Data14", false)[0].Text = gameObject.Data14.ToString();
            if (tbData2.Controls.ContainsKey("Data15"))
                tbData2.Controls.Find("Data15", false)[0].Text = gameObject.Data15.ToString();
            if (tbData2.Controls.ContainsKey("Data16"))
                tbData2.Controls.Find("Data16", false)[0].Text = gameObject.Data16.ToString();
            if (tbData2.Controls.ContainsKey("Data17"))
                tbData2.Controls.Find("Data17", false)[0].Text = gameObject.Data17.ToString();
            if (tbData2.Controls.ContainsKey("Data18"))
                tbData2.Controls.Find("Data18", false)[0].Text = gameObject.Data18.ToString();
            if (tbData2.Controls.ContainsKey("Data19"))
                tbData2.Controls.Find("Data19", false)[0].Text = gameObject.Data19.ToString();
            if (tbData2.Controls.ContainsKey("Data20"))
                tbData2.Controls.Find("Data20", false)[0].Text = gameObject.Data20.ToString();
            if (tbData2.Controls.ContainsKey("Data21"))
                tbData2.Controls.Find("Data21", false)[0].Text = gameObject.Data21.ToString();
            if (tbData2.Controls.ContainsKey("Data22"))
                tbData2.Controls.Find("Data22", false)[0].Text = gameObject.Data22.ToString();
            if (tbData2.Controls.ContainsKey("Data23"))
                tbData2.Controls.Find("Data23", false)[0].Text = gameObject.Data23.ToString();
            AiName.Text = gameObject.AiName;
            ScriptName.Text = gameObject.ScriptName;
            VerifiedBuild.Text = gameObject.VerifiedBuild.ToString();
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
            Storage.Database.WotLk.TrinityCore.GameObject gameObject = new Storage.Database.WotLk.TrinityCore.GameObject();
            gameObject.EntryId = int.Parse(Entry.Text);
            gameObject.Type = (Storage.Database.WotLk.TrinityCore.EnumType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.EnumType), Type.Text);
            gameObject.DisplayId = int.Parse(ModelId.Text);
            gameObject.Name = GameObjectName.Text;
            gameObject.IconName = IconName.Text;
            gameObject.CastBarCaption = CastBarCaption.Text;
            gameObject.Faction = int.Parse(Faction.Text);
            gameObject.Flags = makeFlagOrBitmask(Flags, typeof(Storage.Database.WotLk.TrinityCore.Flags));
            gameObject.Size = float.Parse(GameObjectSize.Text);
            gameObject.Data0 = tbData.Controls.ContainsKey("Data0") ? int.Parse(tbData.Controls.Find("Data0", false)[0].Text) : 0;
            gameObject.Data1 = tbData.Controls.ContainsKey("Data1") ? int.Parse(tbData.Controls.Find("Data1", false)[0].Text) : 0;
            gameObject.Data2 = tbData.Controls.ContainsKey("Data2") ? int.Parse(tbData.Controls.Find("Data2", false)[0].Text) : 0;
            gameObject.Data3 = tbData.Controls.ContainsKey("Data3") ? int.Parse(tbData.Controls.Find("Data3", false)[0].Text) : 0;
            gameObject.Data4 = tbData.Controls.ContainsKey("Data4") ? int.Parse(tbData.Controls.Find("Data4", false)[0].Text) : 0;
            gameObject.Data5 = tbData.Controls.ContainsKey("Data5") ? int.Parse(tbData.Controls.Find("Data5", false)[0].Text) : 0;
            gameObject.Data6 = tbData.Controls.ContainsKey("Data6") ? int.Parse(tbData.Controls.Find("Data6", false)[0].Text) : 0;
            gameObject.Data7 = tbData.Controls.ContainsKey("Data7") ? int.Parse(tbData.Controls.Find("Data7", false)[0].Text) : 0;
            gameObject.Data8 = tbData.Controls.ContainsKey("Data8") ? int.Parse(tbData.Controls.Find("Data8", false)[0].Text) : 0;
            gameObject.Data9 = tbData.Controls.ContainsKey("Data9") ? int.Parse(tbData.Controls.Find("Data9", false)[0].Text) : 0;
            gameObject.Data10 = tbData.Controls.ContainsKey("Data10") ? int.Parse(tbData.Controls.Find("Data10", false)[0].Text) : 0;
            gameObject.Data11 = tbData.Controls.ContainsKey("Data11") ? int.Parse(tbData.Controls.Find("Data11", false)[0].Text) : 0;
            gameObject.Data12 = tbData.Controls.ContainsKey("Data12") ? int.Parse(tbData.Controls.Find("Data12", false)[0].Text) : 0;
            gameObject.Data13 = tbData.Controls.ContainsKey("Data13") ? int.Parse(tbData.Controls.Find("Data13", false)[0].Text) : 0;
            gameObject.Data14 = tbData.Controls.ContainsKey("Data14") ? int.Parse(tbData.Controls.Find("Data14", false)[0].Text) : 0;
            gameObject.Data15 = tbData.Controls.ContainsKey("Data15") ? int.Parse(tbData.Controls.Find("Data15", false)[0].Text) : 0;
            gameObject.Data16 = tbData.Controls.ContainsKey("Data16") ? int.Parse(tbData.Controls.Find("Data16", false)[0].Text) : 0;
            gameObject.Data17 = tbData.Controls.ContainsKey("Data17") ? int.Parse(tbData.Controls.Find("Data17", false)[0].Text) : 0;
            gameObject.Data18 = tbData.Controls.ContainsKey("Data18") ? int.Parse(tbData.Controls.Find("Data18", false)[0].Text) : 0;
            gameObject.Data19 = tbData.Controls.ContainsKey("Data19") ? int.Parse(tbData.Controls.Find("Data19", false)[0].Text) : 0;
            gameObject.Data20 = tbData.Controls.ContainsKey("Data20") ? int.Parse(tbData.Controls.Find("Data20", false)[0].Text) : 0;
            gameObject.Data21 = tbData.Controls.ContainsKey("Data21") ? int.Parse(tbData.Controls.Find("Data21", false)[0].Text) : 0;
            gameObject.Data22 = tbData.Controls.ContainsKey("Data22") ? int.Parse(tbData.Controls.Find("Data22", false)[0].Text) : 0;
            gameObject.Data23 = tbData.Controls.ContainsKey("Data23") ? int.Parse(tbData.Controls.Find("Data23", false)[0].Text) : 0;
            gameObject.AiName = AiName.Text;
            gameObject.ScriptName = ScriptName.Text;
            gameObject.VerifiedBuild = int.Parse(VerifiedBuild.Text);

            if (Storage.Database.WotLk.TrinityCore.GameObjectManager.Instance.GetGameObjectByEntry(gameObject.EntryId) == null)
            {
                Storage.Database.MySqlConnector.Instance.Query(gameObject.GetInsertSqlQuery());
                Storage.Database.WotLk.TrinityCore.GameObjectManager.Instance.addGameObject(gameObject);
                MessageBox.Show("Inserted");
            }
            else
            {
                Storage.Database.MySqlConnector.Instance.Query(gameObject.GetUpdateSqlQuery());
                MessageBox.Show("Updated");
            }
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
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 4, desc = "customAnim (unknown value from 1 to 4)" });
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
                    list.Add(new data() { type = type.Text, desc = "chairslots (number of players that can sit down on it)" });
                    list.Add(new data() { type = type.Text, desc = "chairorientation? (number of usable side?)" });
                    break;

                case "SpellFocus":
                    nbData = 7;
                    list.Add(new data() { type = type.Text, desc = "spellFocusType (from SpellFocusObject.dbc; value also appears as RequiresSpellFocus in Spell.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "diameter (so radius*2)" });
                    list.Add(new data() { type = type.Text, desc = "linkedTrap (gameobject_template.entry (Spawned GO type 6))" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "serverOnly? (Always 0)" });
                    list.Add(new data() { type = type.Text, desc = "questID (Required active quest_template.id to work)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "large? (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "floatingTooltip (Boolean flag)" });
                    break;

                case "Text":
                    nbData = 3;
                    list.Add(new data() { type = type.Text, desc = "pageID (page_text.entry)" });
                    list.Add(new data() { type = type.Text, desc = "language (from  Languages.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "pageMaterial (PageTextMaterial.dbc)" });
                    break;

                case "Goober":
                    nbData = 20;
                    list.Add(new data() { type = type.Text, desc = "open (LockId from Lock.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "questID (Required active quest_template.id to work)" });
                    list.Add(new data() { type = type.Text, desc = "eventID (event_script id)" });
                    list.Add(new data() { type = type.Text, desc = "? (unknown flag)" });
                    list.Add(new data() { type = type.Text, desc = "customAnim (unknown)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "consumable (Boolean flag controling if gameobject will despawn or not)" });
                    list.Add(new data() { type = type.Text, desc = "cooldown (time is seconds)" });
                    list.Add(new data() { type = type.Text, desc = "pageID (page_text.entry)" });
                    list.Add(new data() { type = type.Text, desc = "language (from Languages.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "pageMaterial (PageTextMaterial.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "spell (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "noDamageImmune (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "linkedTrap (gameobject_template.entry (Spawned GO type 6))" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "large? (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "openTextID (Unknown ID)" });
                    list.Add(new data() { type = type.Text, desc = "closeTextID (Unknown ID)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "losOK (Boolean flag) (somewhat related to battlegrounds)" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    list.Add(new data() { type = type.Text, desc = "gossipID - casts the spell when used" });
                    break;

                case "Transport":
                    nbData = 0;
                    break;

                case "AreaDamage":
                    nbData = 0;
                    break;

                case "Camera":
                    nbData = 2;
                    list.Add(new data() { type = type.Text, desc = "open (LockId from Lock.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "camera (Cinematic entry from CinematicCamera.dbc)" });
                    break;

                case "MapObject":
                    nbData = 0;
                    break;

                case "MoTransport":
                    nbData = 9;
                    list.Add(new data() { type = type.Text, desc = "taxiPathID (Id from TaxiPath.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "moveSpeed" });
                    list.Add(new data() { type = type.Text, desc = "accelRate" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    list.Add(new data() { type = type.Text, desc = "?" });
                    list.Add(new data() { type = type.Text, desc = "?" });
                    list.Add(new data() { type = type.Text, desc = "?" });
                    list.Add(new data() { type = type.Text, desc = "?" });
                    break;

                case "DuelArbiter":
                    nbData = 0;
                    break;

                case "FishingNode":
                    nbData = 0;
                    break;

                case "Ritual":
                    nbData = 7;
                    list.Add(new data() { type = type.Text, desc = "casters?" });
                    list.Add(new data() { type = type.Text, desc = "spell (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "animSpell (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "ritualPersistent (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "casterTargetSpell (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "casterTargetSpellTargets (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "castersGrouped (Boolean flag)" });
                    break;

                case "MailBox":
                    nbData = 0;
                    break;

                case "AuctionHouse":
                    nbData = 1;
                    list.Add(new data() { type = type.Text, desc = "actionHouseID (From AuctionHouse.dbc ?)" });
                    break;

                case "GuardPost":
                    nbData = 2;
                    list.Add(new data() { type = type.Text, desc = "CreatureID" });
                    list.Add(new data() { type = type.Text, desc = "unk" });
                    break;

                case "SpellCaster":
                    nbData = 3;
                    list.Add(new data() { type = type.Text, desc = "spell (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "charges" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "partyOnly (Boolean flag, need to be in group to use it)" });
                    break;

                case "MeetingStone":
                    nbData = 3;
                    list.Add(new data() { type = type.Text, desc = "minLevel" });
                    list.Add(new data() { type = type.Text, desc = "maxLevel" });
                    list.Add(new data() { type = type.Text, desc = "areaID (From AreaTable.dbc)" });
                    break;

                case "FlagStand":
                    nbData = 8;
                    list.Add(new data() { type = type.Text, desc = "open (LockId from Lock.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "pickupSpell (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "radius (distance)" });
                    list.Add(new data() { type = type.Text, desc = "returnAura (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "returnSpell (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "noDamageImmune (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "?" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "losOK (Boolean flag)" });
                    break;

                case "FishingPole":
                    nbData = 4;
                    list.Add(new data() { type = type.Text, desc = "radius (distance)" });
                    list.Add(new data() { type = type.Text, desc = "chestLoot (gameobject_loot_template.entry)" });
                    list.Add(new data() { type = type.Text, desc = "minRestock" });
                    list.Add(new data() { type = type.Text, desc = "maxRestock" });
                    break;

                case "FlagDrop":
                    nbData = 4;
                    list.Add(new data() { type = type.Text, desc = "open (LockId from Lock.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "eventID (Unknown Event ID)" });
                    list.Add(new data() { type = type.Text, desc = "pickupSpell (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "noDamageImmune (Boolean flag)" });
                    break;

                case "MiniGame":
                    nbData = -1;
                    break;

                case "LotteryKiosk":
                    nbData = -1;
                    break;

                case "CapturePoint":
                    nbData = 19;
                    list.Add(new data() { type = type.Text, desc = "radius (Distance)" });
                    list.Add(new data() { type = type.Text, desc = "spell (Unknown ID, not a spell id in dbc file, maybe server only side spell)" });
                    list.Add(new data() { type = type.Text, desc = "worldState1" });
                    list.Add(new data() { type = type.Text, desc = "worldState2" });
                    list.Add(new data() { type = type.Text, desc = "winEventID1 (Unknown Event ID)" });
                    list.Add(new data() { type = type.Text, desc = "winEventID1 (Unknown Event ID)" });
                    list.Add(new data() { type = type.Text, desc = "contestedEventID1 (Unknown Event ID)" });
                    list.Add(new data() { type = type.Text, desc = "contestedEventID2 (Unknown Event ID)" });
                    list.Add(new data() { type = type.Text, desc = "progressEventID1 (Unknown Event ID)" });
                    list.Add(new data() { type = type.Text, desc = "progressEventID2 (Unknown Event ID)" });
                    list.Add(new data() { type = type.Text, desc = "neutralEventID1 (Unknown Event ID)" });
                    list.Add(new data() { type = type.Text, desc = "neutralEventID2 (Unknown Event ID)" });
                    list.Add(new data() { type = type.Text, desc = "neutralPercent" });
                    list.Add(new data() { type = type.Text, desc = "worldState3" });
                    list.Add(new data() { type = type.Text, desc = "minSuperiority" });
                    list.Add(new data() { type = type.Text, desc = "maxSuperiority" });
                    list.Add(new data() { type = type.Text, desc = "minTime (in seconds)" });
                    list.Add(new data() { type = type.Text, desc = "maxTime (in seconds)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "large? (Boolean flag)" });
                    break;

                case "AuraGenerator":
                    nbData = 4;
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "startOpen (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "radius (Distance)" });
                    list.Add(new data() { type = type.Text, desc = "auraID1 (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "conditionID1 (Unknown ID)" });
                    break;

                case "DungeonDifficulty":
                    nbData = 2;
                    list.Add(new data() { type = type.Text, desc = "mapID (From Map.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "difficulty" });
                    break;

                case "BarberChair":
                    nbData = 0;
                    break;

                case "DestructibleBuilding":
                    nbData = 24;
                    list.Add(new data() { type = type.Text, desc = "intactNumHits" });
                    list.Add(new data() { type = type.Text, desc = "creditProxyCreature" });
                    list.Add(new data() { type = type.Text, desc = "state1Name" });
                    list.Add(new data() { type = type.Text, desc = "intactEvent" });
                    list.Add(new data() { type = type.Text, desc = "damagedDisplayId" });
                    list.Add(new data() { type = type.Text, desc = "damagedNumHits" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    list.Add(new data() { type = type.Text, desc = "damagedEvent" });
                    list.Add(new data() { type = type.Text, desc = "destroyedDisplayId" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    list.Add(new data() { type = type.Text, desc = "destroyedEvent" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    list.Add(new data() { type = type.Text, desc = "debuildingTimeSecs" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    list.Add(new data() { type = type.Text, desc = "destructibleData" });
                    list.Add(new data() { type = type.Text, desc = "rebuildingEvent" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    list.Add(new data() { type = type.Text, desc = "damageEvent" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "unused" });
                    break;

                case "GuildBank":
                    nbData = 0;
                    break;

                case "TrapDoor":
                    nbData = 3;
                    list.Add(new data() { type = type.Text, desc = "whenToPause" });
                    list.Add(new data() { type = type.Text, desc = "startOpen" });
                    list.Add(new data() { type = type.Text, desc = "autoClose" });
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
                            textBox.Text = "0";

                            if(d < 12)
                            {
                                tbData.Controls.Add(textBox);
                                textBox.Location = new System.Drawing.Point(73, 17 + d * 30);
                            }
                            else
                            {
                                tbData2.Controls.Add(textBox);
                                textBox.Location = new System.Drawing.Point(73, 17 + (d-12) * 30);
                            }
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

                            comboBox.SelectedIndex = 0;

                            if (d < 12)
                            {
                                tbData.Controls.Add(comboBox);
                                comboBox.Location = new System.Drawing.Point(73, 17 + d * 30);
                            }
                            else
                            {
                                tbData2.Controls.Add(comboBox);
                                comboBox.Location = new System.Drawing.Point(73, 17 + (d - 12) * 30);
                            }
                            break;

                        case type.Always:
                            TextBox alwaysTextBox = new TextBox();
                            alwaysTextBox.Name = "Data" + d;
                            alwaysTextBox.Location = new System.Drawing.Point(73, 17 + d * 30);
                            alwaysTextBox.Size = new System.Drawing.Size(456, 20);
                            alwaysTextBox.Text = Convert.ToString(item.startNbr);
                            alwaysTextBox.Enabled = false;
                            if (d < 12)
                            {
                                tbData.Controls.Add(alwaysTextBox);
                                alwaysTextBox.Location = new System.Drawing.Point(73, 17 + d * 30);
                            }
                            else
                            {
                                tbData2.Controls.Add(alwaysTextBox);
                                alwaysTextBox.Location = new System.Drawing.Point(73, 17 + (d - 12) * 30);
                            }
                            break;
                    }

                    d++;
                }
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (LoadEntry.Text != "")
            {
                if (Storage.Database.WotLk.TrinityCore.GameObjectManager.Instance.GetGameObjectByEntry(Convert.ToInt32(LoadEntry.Text)) == null)
                {
                    MessageBox.Show("There is no gameObject with this id.");
                }

                Storage.Database.WotLk.TrinityCore.GameObject gameObjectLoaded = new Storage.Database.WotLk.TrinityCore.GameObject();

                gameObjectLoaded = Storage.Database.WotLk.TrinityCore.GameObjectManager.Instance.GetGameObjectByEntry(int.Parse(LoadEntry.Text));

                if (gameObjectLoaded != null)
                {
                    loadGameObject(gameObjectLoaded);
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

