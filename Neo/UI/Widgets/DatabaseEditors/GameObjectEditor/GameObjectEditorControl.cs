using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections.Generic;

namespace Neo.UI.Dialog
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
	        this.tbcEditor.Appearance = TabAppearance.FlatButtons;
	        this.tbcEditor.ItemSize = new Size(0, 1);
	        this.tbcEditor.SizeMode = TabSizeMode.Fixed;
        }



        private void showModelInRenderControl(string pModelId)
        {
            if (!string.IsNullOrEmpty(pModelId))
            {
                int displayId;
                if (int.TryParse(pModelId, out displayId))
                {
                    MessageBox.Show("" + displayId);
                }

            }
        }

        public void loadGameObject(Storage.Database.WotLk.TrinityCore.GameObject gameObject)
        {
	        this.Entry.Text = gameObject.EntryId.ToString();
	        this.Type.Text = gameObject.Type.ToString();
            Type_SelectedIndexChanged(this, EventArgs.Empty);
	        this.ModelId.Text = gameObject.DisplayId.ToString();
	        this.GameObjectName.Text = gameObject.Name;
	        this.IconName.Text = gameObject.IconName;
	        this.CastBarCaption.Text = gameObject.CastBarCaption;
	        this.Faction.Text = gameObject.Faction.ToString();
            checkFlagOrBitmask(this.Flags, typeof(Storage.Database.WotLk.TrinityCore.Flags), gameObject.Flags);
	        this.GameObjectSize.Text = gameObject.Size.ToString();
            if(this.tbData.Controls.ContainsKey("Data0"))
            {
	            this.tbData.Controls.Find("Data0", false)[0].Text = gameObject.Data0.ToString();
            }
	        if (this.tbData.Controls.ContainsKey("Data1"))
	        {
		        this.tbData.Controls.Find("Data1", false)[0].Text = gameObject.Data1.ToString();
	        }
	        if (this.tbData.Controls.ContainsKey("Data2"))
	        {
		        this.tbData.Controls.Find("Data2", false)[0].Text = gameObject.Data2.ToString();
	        }
	        if (this.tbData.Controls.ContainsKey("Data3"))
	        {
		        this.tbData.Controls.Find("Data3", false)[0].Text = gameObject.Data3.ToString();
	        }
	        if (this.tbData.Controls.ContainsKey("Data4"))
	        {
		        this.tbData.Controls.Find("Data4", false)[0].Text = gameObject.Data4.ToString();
	        }
	        if (this.tbData.Controls.ContainsKey("Data5"))
	        {
		        this.tbData.Controls.Find("Data5", false)[0].Text = gameObject.Data5.ToString();
	        }
	        if (this.tbData.Controls.ContainsKey("Data6"))
	        {
		        this.tbData.Controls.Find("Data6", false)[0].Text = gameObject.Data6.ToString();
	        }
	        if (this.tbData.Controls.ContainsKey("Data7"))
	        {
		        this.tbData.Controls.Find("Data7", false)[0].Text = gameObject.Data7.ToString();
	        }
	        if (this.tbData.Controls.ContainsKey("Data8"))
	        {
		        this.tbData.Controls.Find("Data8", false)[0].Text = gameObject.Data8.ToString();
	        }
	        if (this.tbData.Controls.ContainsKey("Data9"))
	        {
		        this.tbData.Controls.Find("Data9", false)[0].Text = gameObject.Data9.ToString();
	        }
	        if (this.tbData.Controls.ContainsKey("Data10"))
	        {
		        this.tbData.Controls.Find("Data10", false)[0].Text = gameObject.Data10.ToString();
	        }
	        if (this.tbData.Controls.ContainsKey("Data11"))
	        {
		        this.tbData.Controls.Find("Data11", false)[0].Text = gameObject.Data11.ToString();
	        }
	        if (this.tbData2.Controls.ContainsKey("Data12"))
	        {
		        this.tbData2.Controls.Find("Data12", false)[0].Text = gameObject.Data12.ToString();
	        }
	        if (this.tbData2.Controls.ContainsKey("Data13"))
	        {
		        this.tbData2.Controls.Find("Data13", false)[0].Text = gameObject.Data13.ToString();
	        }
	        if (this.tbData2.Controls.ContainsKey("Data14"))
	        {
		        this.tbData2.Controls.Find("Data14", false)[0].Text = gameObject.Data14.ToString();
	        }
	        if (this.tbData2.Controls.ContainsKey("Data15"))
	        {
		        this.tbData2.Controls.Find("Data15", false)[0].Text = gameObject.Data15.ToString();
	        }
	        if (this.tbData2.Controls.ContainsKey("Data16"))
	        {
		        this.tbData2.Controls.Find("Data16", false)[0].Text = gameObject.Data16.ToString();
	        }
	        if (this.tbData2.Controls.ContainsKey("Data17"))
	        {
		        this.tbData2.Controls.Find("Data17", false)[0].Text = gameObject.Data17.ToString();
	        }
	        if (this.tbData2.Controls.ContainsKey("Data18"))
	        {
		        this.tbData2.Controls.Find("Data18", false)[0].Text = gameObject.Data18.ToString();
	        }
	        if (this.tbData2.Controls.ContainsKey("Data19"))
	        {
		        this.tbData2.Controls.Find("Data19", false)[0].Text = gameObject.Data19.ToString();
	        }
	        if (this.tbData2.Controls.ContainsKey("Data20"))
	        {
		        this.tbData2.Controls.Find("Data20", false)[0].Text = gameObject.Data20.ToString();
	        }
	        if (this.tbData2.Controls.ContainsKey("Data21"))
	        {
		        this.tbData2.Controls.Find("Data21", false)[0].Text = gameObject.Data21.ToString();
	        }
	        if (this.tbData2.Controls.ContainsKey("Data22"))
	        {
		        this.tbData2.Controls.Find("Data22", false)[0].Text = gameObject.Data22.ToString();
	        }
	        if (this.tbData2.Controls.ContainsKey("Data23"))
	        {
		        this.tbData2.Controls.Find("Data23", false)[0].Text = gameObject.Data23.ToString();
	        }
	        this.AiName.Text = gameObject.AiName;
	        this.ScriptName.Text = gameObject.ScriptName;
	        this.VerifiedBuild.Text = gameObject.VerifiedBuild.ToString();
        }


        private uint makeFlagOrBitmask(CheckedListBox list, Type e)
        {
            if (!e.IsEnum)
            {
	            return 0;
            }

	        uint myFlags = 0x0;

            foreach (object item in list.CheckedItems)
            {
                myFlags += Convert.ToUInt32(Enum.Parse(e, item.ToString()));
            }

            return myFlags;
        }

        private void checkFlagOrBitmask(CheckedListBox list, Type e, uint value)
        {
            if (!e.IsEnum)
            {
	            return;
            }

	        foreach (int i in list.CheckedIndices)
            {
                list.SetItemCheckState(i, CheckState.Unchecked);
            }

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
	        this.tbcEditor.SelectedIndex = this.lbMenu.SelectedIndex;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Storage.Database.WotLk.TrinityCore.GameObject gameObject = new Storage.Database.WotLk.TrinityCore.GameObject();
            gameObject.EntryId = int.Parse(this.Entry.Text);
            gameObject.Type = (Storage.Database.WotLk.TrinityCore.EnumType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.EnumType), this.Type.Text);
            gameObject.DisplayId = int.Parse(this.ModelId.Text);
            gameObject.Name = this.GameObjectName.Text;
            gameObject.IconName = this.IconName.Text;
            gameObject.CastBarCaption = this.CastBarCaption.Text;
            gameObject.Faction = int.Parse(this.Faction.Text);
            gameObject.Flags = makeFlagOrBitmask(this.Flags, typeof(Storage.Database.WotLk.TrinityCore.Flags));
            gameObject.Size = float.Parse(this.GameObjectSize.Text);
            gameObject.Data0 = this.tbData.Controls.ContainsKey("Data0") ? int.Parse(this.tbData.Controls.Find("Data0", false)[0].Text) : 0;
            gameObject.Data1 = this.tbData.Controls.ContainsKey("Data1") ? int.Parse(this.tbData.Controls.Find("Data1", false)[0].Text) : 0;
            gameObject.Data2 = this.tbData.Controls.ContainsKey("Data2") ? int.Parse(this.tbData.Controls.Find("Data2", false)[0].Text) : 0;
            gameObject.Data3 = this.tbData.Controls.ContainsKey("Data3") ? int.Parse(this.tbData.Controls.Find("Data3", false)[0].Text) : 0;
            gameObject.Data4 = this.tbData.Controls.ContainsKey("Data4") ? int.Parse(this.tbData.Controls.Find("Data4", false)[0].Text) : 0;
            gameObject.Data5 = this.tbData.Controls.ContainsKey("Data5") ? int.Parse(this.tbData.Controls.Find("Data5", false)[0].Text) : 0;
            gameObject.Data6 = this.tbData.Controls.ContainsKey("Data6") ? int.Parse(this.tbData.Controls.Find("Data6", false)[0].Text) : 0;
            gameObject.Data7 = this.tbData.Controls.ContainsKey("Data7") ? int.Parse(this.tbData.Controls.Find("Data7", false)[0].Text) : 0;
            gameObject.Data8 = this.tbData.Controls.ContainsKey("Data8") ? int.Parse(this.tbData.Controls.Find("Data8", false)[0].Text) : 0;
            gameObject.Data9 = this.tbData.Controls.ContainsKey("Data9") ? int.Parse(this.tbData.Controls.Find("Data9", false)[0].Text) : 0;
            gameObject.Data10 = this.tbData.Controls.ContainsKey("Data10") ? int.Parse(this.tbData.Controls.Find("Data10", false)[0].Text) : 0;
            gameObject.Data11 = this.tbData.Controls.ContainsKey("Data11") ? int.Parse(this.tbData.Controls.Find("Data11", false)[0].Text) : 0;
            gameObject.Data12 = this.tbData.Controls.ContainsKey("Data12") ? int.Parse(this.tbData.Controls.Find("Data12", false)[0].Text) : 0;
            gameObject.Data13 = this.tbData.Controls.ContainsKey("Data13") ? int.Parse(this.tbData.Controls.Find("Data13", false)[0].Text) : 0;
            gameObject.Data14 = this.tbData.Controls.ContainsKey("Data14") ? int.Parse(this.tbData.Controls.Find("Data14", false)[0].Text) : 0;
            gameObject.Data15 = this.tbData.Controls.ContainsKey("Data15") ? int.Parse(this.tbData.Controls.Find("Data15", false)[0].Text) : 0;
            gameObject.Data16 = this.tbData.Controls.ContainsKey("Data16") ? int.Parse(this.tbData.Controls.Find("Data16", false)[0].Text) : 0;
            gameObject.Data17 = this.tbData.Controls.ContainsKey("Data17") ? int.Parse(this.tbData.Controls.Find("Data17", false)[0].Text) : 0;
            gameObject.Data18 = this.tbData.Controls.ContainsKey("Data18") ? int.Parse(this.tbData.Controls.Find("Data18", false)[0].Text) : 0;
            gameObject.Data19 = this.tbData.Controls.ContainsKey("Data19") ? int.Parse(this.tbData.Controls.Find("Data19", false)[0].Text) : 0;
            gameObject.Data20 = this.tbData.Controls.ContainsKey("Data20") ? int.Parse(this.tbData.Controls.Find("Data20", false)[0].Text) : 0;
            gameObject.Data21 = this.tbData.Controls.ContainsKey("Data21") ? int.Parse(this.tbData.Controls.Find("Data21", false)[0].Text) : 0;
            gameObject.Data22 = this.tbData.Controls.ContainsKey("Data22") ? int.Parse(this.tbData.Controls.Find("Data22", false)[0].Text) : 0;
            gameObject.Data23 = this.tbData.Controls.ContainsKey("Data23") ? int.Parse(this.tbData.Controls.Find("Data23", false)[0].Text) : 0;
            gameObject.AiName = this.AiName.Text;
            gameObject.ScriptName = this.ScriptName.Text;
            gameObject.VerifiedBuild = int.Parse(this.VerifiedBuild.Text);

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
            showModelInRenderControl(this.ModelId.Text);
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
	    private enum type
        {
            Text = 0,
            List = 1,
            Always = 3
        }

        //Data
	    private struct data
        {
            public type type;
            public int startNbr;
            public int endNbr;
            public string desc; //will appear as a tooltip on the data label
        };

        private void Type_SelectedIndexChanged(object sender, EventArgs e)
        {
	        this.WarningLabel1.Hide();
	        this.WarningLabel2.Hide();

            for(int i = 0; i< this.nbData;i++)
            {
                if (i < 12)
                {
	                this.tbData.Controls.RemoveByKey("Data" + i);
                }
                else
                {
	                this.tbData2.Controls.RemoveByKey("Data" + i);
                }
            }

	        this.nbData = 0;

            List<data> list = new List<data>();
            Label[] dataArray = {this.label6, this.label7, this.label8, this.label9, this.label10, this.label11, this.label12, this.label13, this.label14, this.label15, this.label16, this.label17, this.label18, this.label19, this.label20, this.label21, this.label22, this.label23, this.label24, this.label25, this.label26, this.label27, this.label28, this.label29 };

            foreach(Label item in dataArray)
            {
	            this.toolTip1.SetToolTip(item, "");
                item.Hide();
            }

            switch(this.Type.Text)
            {
                case "Door":
	                this.nbData = 6;
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc="startOpen (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "open (LockId from Lock.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "autoClose (Time in milliseconds)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "noDamageImmune (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "openTextID (Unknown Text ID)" });
                    list.Add(new data() { type = type.Text, desc = "closeTextID (Unknown Text ID)" });
                    break;

                case "Button":
	                this.nbData = 9;
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
	                this.nbData = 10;
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
	                this.nbData = 16;
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
	                this.nbData = -1;
                    break;

                case "Generic":
	                this.nbData = 6;
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "floatingTooltip (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "highlight (Boolean flag)" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "serverOnly? (Always 0)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "large? (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "floatOnWater (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "questID (Required active quest_template.id to work)" });
                    break;

                case "Trap":
	                this.nbData = 13;
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
	                this.nbData = 2;
                    list.Add(new data() { type = type.Text, desc = "chairslots (number of players that can sit down on it)" });
                    list.Add(new data() { type = type.Text, desc = "chairorientation? (number of usable side?)" });
                    break;

                case "SpellFocus":
	                this.nbData = 7;
                    list.Add(new data() { type = type.Text, desc = "spellFocusType (from SpellFocusObject.dbc; value also appears as RequiresSpellFocus in Spell.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "diameter (so radius*2)" });
                    list.Add(new data() { type = type.Text, desc = "linkedTrap (gameobject_template.entry (Spawned GO type 6))" });
                    list.Add(new data() { type = type.Always, startNbr = 0, desc = "serverOnly? (Always 0)" });
                    list.Add(new data() { type = type.Text, desc = "questID (Required active quest_template.id to work)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "large? (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "floatingTooltip (Boolean flag)" });
                    break;

                case "Text":
	                this.nbData = 3;
                    list.Add(new data() { type = type.Text, desc = "pageID (page_text.entry)" });
                    list.Add(new data() { type = type.Text, desc = "language (from  Languages.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "pageMaterial (PageTextMaterial.dbc)" });
                    break;

                case "Goober":
	                this.nbData = 20;
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
	                this.nbData = 0;
                    break;

                case "AreaDamage":
	                this.nbData = 0;
                    break;

                case "Camera":
	                this.nbData = 2;
                    list.Add(new data() { type = type.Text, desc = "open (LockId from Lock.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "camera (Cinematic entry from CinematicCamera.dbc)" });
                    break;

                case "MapObject":
	                this.nbData = 0;
                    break;

                case "MoTransport":
	                this.nbData = 9;
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
	                this.nbData = 0;
                    break;

                case "FishingNode":
	                this.nbData = 0;
                    break;

                case "Ritual":
	                this.nbData = 7;
                    list.Add(new data() { type = type.Text, desc = "casters?" });
                    list.Add(new data() { type = type.Text, desc = "spell (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "animSpell (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "ritualPersistent (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "casterTargetSpell (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "casterTargetSpellTargets (Boolean flag)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "castersGrouped (Boolean flag)" });
                    break;

                case "MailBox":
	                this.nbData = 0;
                    break;

                case "AuctionHouse":
	                this.nbData = 1;
                    list.Add(new data() { type = type.Text, desc = "actionHouseID (From AuctionHouse.dbc ?)" });
                    break;

                case "GuardPost":
	                this.nbData = 2;
                    list.Add(new data() { type = type.Text, desc = "CreatureID" });
                    list.Add(new data() { type = type.Text, desc = "unk" });
                    break;

                case "SpellCaster":
	                this.nbData = 3;
                    list.Add(new data() { type = type.Text, desc = "spell (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "charges" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "partyOnly (Boolean flag, need to be in group to use it)" });
                    break;

                case "MeetingStone":
	                this.nbData = 3;
                    list.Add(new data() { type = type.Text, desc = "minLevel" });
                    list.Add(new data() { type = type.Text, desc = "maxLevel" });
                    list.Add(new data() { type = type.Text, desc = "areaID (From AreaTable.dbc)" });
                    break;

                case "FlagStand":
	                this.nbData = 8;
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
	                this.nbData = 4;
                    list.Add(new data() { type = type.Text, desc = "radius (distance)" });
                    list.Add(new data() { type = type.Text, desc = "chestLoot (gameobject_loot_template.entry)" });
                    list.Add(new data() { type = type.Text, desc = "minRestock" });
                    list.Add(new data() { type = type.Text, desc = "maxRestock" });
                    break;

                case "FlagDrop":
	                this.nbData = 4;
                    list.Add(new data() { type = type.Text, desc = "open (LockId from Lock.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "eventID (Unknown Event ID)" });
                    list.Add(new data() { type = type.Text, desc = "pickupSpell (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "noDamageImmune (Boolean flag)" });
                    break;

                case "MiniGame":
	                this.nbData = -1;
                    break;

                case "LotteryKiosk":
	                this.nbData = -1;
                    break;

                case "CapturePoint":
	                this.nbData = 19;
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
	                this.nbData = 4;
                    list.Add(new data() { type = type.List, startNbr = 0, endNbr = 1, desc = "startOpen (Boolean flag)" });
                    list.Add(new data() { type = type.Text, desc = "radius (Distance)" });
                    list.Add(new data() { type = type.Text, desc = "auraID1 (Spell Id from Spell.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "conditionID1 (Unknown ID)" });
                    break;

                case "DungeonDifficulty":
	                this.nbData = 2;
                    list.Add(new data() { type = type.Text, desc = "mapID (From Map.dbc)" });
                    list.Add(new data() { type = type.Text, desc = "difficulty" });
                    break;

                case "BarberChair":
	                this.nbData = 0;
                    break;

                case "DestructibleBuilding":
	                this.nbData = 24;
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
	                this.nbData = 0;
                    break;

                case "TrapDoor":
	                this.nbData = 3;
                    list.Add(new data() { type = type.Text, desc = "whenToPause" });
                    list.Add(new data() { type = type.Text, desc = "startOpen" });
                    list.Add(new data() { type = type.Text, desc = "autoClose" });
                    break;

            }

            if (this.nbData == -1)
            {
	            this.WarningLabel1.Text = "Object type not used";
	            this.WarningLabel2.Text = "Object type not used";
	            this.WarningLabel1.Show();
	            this.WarningLabel2.Show();
            }
            else if(this.nbData == 0)
            {
	            this.WarningLabel1.Text = "All data are always 0";
	            this.WarningLabel2.Text = "All data are always 0";
	            this.WarningLabel1.Show();
	            this.WarningLabel2.Show();
            }
            else
            {
                for (int i = 0; i < this.nbData; i++)
                {
                    dataArray[i].Show();
	                this.toolTip1.SetToolTip(dataArray[i], list[i].desc);
                }

                int d = 0;

                foreach(data item in list)
                {
                    switch(item.type)
                    {
                        case type.Text:
                            TextBox textBox = new TextBox();
                            textBox.Name = "Data"+d;
                            textBox.Location = new Point(73, 17 + d*30);
                            textBox.Size = new Size(456, 20);
                            textBox.Text = "0";

                            if(d < 12)
                            {
	                            this.tbData.Controls.Add(textBox);
                                textBox.Location = new Point(73, 17 + d * 30);
                            }
                            else
                            {
	                            this.tbData2.Controls.Add(textBox);
                                textBox.Location = new Point(73, 17 + (d-12) * 30);
                            }
                            break;

                        case type.List:
                            ComboBox comboBox = new ComboBox();
                            comboBox.Name = "Data" + d;
                            comboBox.Location = new Point(73, 17 + d * 30);
                            comboBox.Size = new Size(456, 20);

                            for(int a = item.startNbr; a <= item.endNbr; a++)
                            {
                                comboBox.Items.Add(a);
                            }

                            comboBox.SelectedIndex = 0;

                            if (d < 12)
                            {
	                            this.tbData.Controls.Add(comboBox);
                                comboBox.Location = new Point(73, 17 + d * 30);
                            }
                            else
                            {
	                            this.tbData2.Controls.Add(comboBox);
                                comboBox.Location = new Point(73, 17 + (d - 12) * 30);
                            }
                            break;

                        case type.Always:
                            TextBox alwaysTextBox = new TextBox();
                            alwaysTextBox.Name = "Data" + d;
                            alwaysTextBox.Location = new Point(73, 17 + d * 30);
                            alwaysTextBox.Size = new Size(456, 20);
                            alwaysTextBox.Text = Convert.ToString(item.startNbr);
                            alwaysTextBox.Enabled = false;
                            if (d < 12)
                            {
	                            this.tbData.Controls.Add(alwaysTextBox);
                                alwaysTextBox.Location = new Point(73, 17 + d * 30);
                            }
                            else
                            {
	                            this.tbData2.Controls.Add(alwaysTextBox);
                                alwaysTextBox.Location = new Point(73, 17 + (d - 12) * 30);
                            }
                            break;
                    }

                    d++;
                }
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (this.LoadEntry.Text != "")
            {
                if (Storage.Database.WotLk.TrinityCore.GameObjectManager.Instance.GetGameObjectByEntry(Convert.ToInt32(this.LoadEntry.Text)) == null)
                {
                    MessageBox.Show("There is no gameObject with this id.");
                }

                Storage.Database.WotLk.TrinityCore.GameObject gameObjectLoaded = new Storage.Database.WotLk.TrinityCore.GameObject();

                gameObjectLoaded = Storage.Database.WotLk.TrinityCore.GameObjectManager.Instance.GetGameObjectByEntry(int.Parse(this.LoadEntry.Text));

                if (gameObjectLoaded != null)
                {
                    loadGameObject(gameObjectLoaded);
                }
            }
        }
    }

    internal class GameObjectEditorControlDesigner : ControlDesigner
    {
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            var ctl = (this.Control as GameObjectEditorControl).TabControl as TabControl;
            EnableDesignMode(ctl, "TabControl");
            foreach (TabPage page in ctl.TabPages)
            {
	            EnableDesignMode(page, page.Name);
            }
        }
    }
}

