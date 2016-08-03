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

