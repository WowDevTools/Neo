using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace WoWEditor6.UI.Dialog
{
    [Designer(typeof(ItemEditorControlDesigner))]
    public partial class ItemEditorControl : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabControl TabControl
        {
            get { return this.tbcEditor; }
        }

        public ItemEditorControl()
        {
            InitializeComponent();
            tbcEditor.Appearance = TabAppearance.FlatButtons;
            tbcEditor.ItemSize = new Size(0, 1);
            tbcEditor.SizeMode = TabSizeMode.Fixed;
        }

        internal class ItemEditorControlDesigner : ControlDesigner
        {
            public override void Initialize(System.ComponentModel.IComponent component)
            {
                base.Initialize(component);

                var ctl = (this.Control as ItemEditorControl).TabControl as TabControl;
                EnableDesignMode(ctl, "TabControl");
                foreach (TabPage page in ctl.TabPages) EnableDesignMode(page, page.Name);
            }
        }


        private void btnShowModelId1_Click(object sender, EventArgs e)
        {

        }

        private void lbMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbcEditor.SelectedIndex = lbMenu.SelectedIndex;
        }

        private void AllowableClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(this.AllowableClass.GetSelected(0))
            {
                for(int i=1;i< this.AllowableClass.Items.Count;i++)
                {
                    this.AllowableClass.SetItemChecked(i, false);
                }
            }
            else
            {
                this.AllowableClass.SetItemChecked(0, false);
            }
        }

        private void AllowableRace_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.AllowableRace.GetSelected(0))
            {
                for (int i = 1; i < this.AllowableRace.Items.Count; i++)
                {
                    this.AllowableRace.SetItemChecked(i, false);
                }
            }
            else
            {
                this.AllowableRace.SetItemChecked(0, false);
            }
        }

        private void ItemClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SubClass.Items.Clear();

            switch(this.ItemClass.SelectedIndex)
            {
                case 0:
                    this.SubClass.Items.Add("Consumable");
                    this.SubClass.Items.Add("Potion");
                    this.SubClass.Items.Add("Elixir");
                    this.SubClass.Items.Add("Flask");
                    this.SubClass.Items.Add("Scroll");
                    this.SubClass.Items.Add("Food & Drink");
                    this.SubClass.Items.Add("Item Enhancement");
                    this.SubClass.Items.Add("Bandage");
                    this.SubClass.Items.Add("Other");
                    break;

                case 1:
                    this.SubClass.Items.Add("Bag");
                    this.SubClass.Items.Add("Soul Bag");
                    this.SubClass.Items.Add("Herb Bag");
                    this.SubClass.Items.Add("Enchanting Bag");
                    this.SubClass.Items.Add("Engineering Bag");
                    this.SubClass.Items.Add("Gem Bag");
                    this.SubClass.Items.Add("Mining Bag");
                    this.SubClass.Items.Add("Leatherworking Bag");
                    this.SubClass.Items.Add("Inscription Bag");
                    break;

                case 2:
                    this.SubClass.Items.Add("One handed axe");
                    this.SubClass.Items.Add("Tne handed axe");
                    this.SubClass.Items.Add("Bow");
                    this.SubClass.Items.Add("Gun");
                    this.SubClass.Items.Add("One handed mace");
                    this.SubClass.Items.Add("Two handed mace");
                    this.SubClass.Items.Add("Polearm");
                    this.SubClass.Items.Add("One handed sword");
                    this.SubClass.Items.Add("Two handed sword");
                    this.SubClass.Items.Add("Obsolete");
                    this.SubClass.Items.Add("Staff");
                    this.SubClass.Items.Add("Exotic");
                    this.SubClass.Items.Add("Exotic");
                    this.SubClass.Items.Add("Fist Weapon");
                    this.SubClass.Items.Add("Miscellaneous");
                    this.SubClass.Items.Add("Dagger");
                    this.SubClass.Items.Add("Thrown");
                    this.SubClass.Items.Add("Spear");
                    this.SubClass.Items.Add("Crossbow");
                    this.SubClass.Items.Add("Wand");
                    this.SubClass.Items.Add("Fishing Pole");
                    break;

                case 3:
                    this.SubClass.Items.Add("Red");
                    this.SubClass.Items.Add("Blue");
                    this.SubClass.Items.Add("Yellow");
                    this.SubClass.Items.Add("Purple");
                    this.SubClass.Items.Add("Green");
                    this.SubClass.Items.Add("Orange");
                    this.SubClass.Items.Add("Meta");
                    this.SubClass.Items.Add("Simple");
                    this.SubClass.Items.Add("Prismatic");
                    break;

                case 4:
                    this.SubClass.Items.Add("Miscellaneous");
                    this.SubClass.Items.Add("Cloth");
                    this.SubClass.Items.Add("Leather");
                    this.SubClass.Items.Add("Mail");
                    this.SubClass.Items.Add("Plate");
                    this.SubClass.Items.Add("Buckler(OBSOLETE)");
                    this.SubClass.Items.Add("Shield");
                    this.SubClass.Items.Add("Libram");
                    this.SubClass.Items.Add("Idol");
                    this.SubClass.Items.Add("Totem");
                    this.SubClass.Items.Add("Sigil");
                    break;

                case 5:
                    this.SubClass.Items.Add("Reagent");
                    break;

                case 6:
                    this.SubClass.Items.Add("Wand(OBSOLETE)");
                    this.SubClass.Items.Add("Bolt(OBSOLETE)");
                    this.SubClass.Items.Add("Arrow");
                    this.SubClass.Items.Add("Bullet");
                    this.SubClass.Items.Add("Thrown(OBSOLETE)");
                    break;

                case 7:
                    this.SubClass.Items.Add("Trade Goods");
                    this.SubClass.Items.Add("Parts");
                    this.SubClass.Items.Add("Explosives");
                    this.SubClass.Items.Add("Devices");
                    this.SubClass.Items.Add("Jewelcrafting");
                    this.SubClass.Items.Add("Cloth");
                    this.SubClass.Items.Add("Leather");
                    this.SubClass.Items.Add("Metal & Stone");
                    this.SubClass.Items.Add("Meat");
                    this.SubClass.Items.Add("Herb");
                    this.SubClass.Items.Add("Elemental");
                    this.SubClass.Items.Add("Other");
                    this.SubClass.Items.Add("Enchanting");
                    this.SubClass.Items.Add("Materials");
                    this.SubClass.Items.Add("Armor Enchantment");
                    this.SubClass.Items.Add("Weapon Enchantment");
                    break;

                case 8:
                    this.SubClass.Items.Add("Generic(OBSOLETE)");
                    break;

                case 9:
                    this.SubClass.Items.Add("Book");
                    this.SubClass.Items.Add("Leatherworking");
                    this.SubClass.Items.Add("Tailoring");
                    this.SubClass.Items.Add("Engineering");
                    this.SubClass.Items.Add("Blacksmithing");
                    this.SubClass.Items.Add("Cooking");
                    this.SubClass.Items.Add("Alchemy");
                    this.SubClass.Items.Add("First Aid");
                    this.SubClass.Items.Add("Enchanting");
                    this.SubClass.Items.Add("Fishing");
                    this.SubClass.Items.Add("Jewelcrafting");
                    break;

                case 10:
                    this.SubClass.Items.Add("Money(OBSOLETE)");
                    break;

                case 11:
                    this.SubClass.Items.Add("Quiver(OBSOLETE)");
                    this.SubClass.Items.Add("Quiver(OBSOLETE)");
                    this.SubClass.Items.Add("Quiver");
                    this.SubClass.Items.Add("Ammo Pouch");
                    break;

                case 12:
                    this.SubClass.Items.Add("Quest");
                    break;

                case 13:
                    this.SubClass.Items.Add("Key");
                    this.SubClass.Items.Add("Lockpick");
                    break;

                case 14:
                    this.SubClass.Items.Add("Permanent");
                    break;

                case 15:
                    this.SubClass.Items.Add("Junk");
                    this.SubClass.Items.Add("Reagent");
                    this.SubClass.Items.Add("Pet");
                    this.SubClass.Items.Add("Holiday");
                    this.SubClass.Items.Add("Other");
                    this.SubClass.Items.Add("Mount");
                    break;

                    //Had to place none items since i'll use index as value
                case 16:
                    this.SubClass.Items.Add("None");
                    this.SubClass.Items.Add("Warrior");
                    this.SubClass.Items.Add("Paladin");
                    this.SubClass.Items.Add("Hunter");
                    this.SubClass.Items.Add("Rogue");
                    this.SubClass.Items.Add("Priest");
                    this.SubClass.Items.Add("Death Knight");
                    this.SubClass.Items.Add("Shaman");
                    this.SubClass.Items.Add("Mage");
                    this.SubClass.Items.Add("Warlock");
                    this.SubClass.Items.Add("None");
                    this.SubClass.Items.Add("Druid");
                    break;
            }

            this.SubClass.SelectedIndex = 0;
        }

        //Use of long because i need more than a int32 and less than an uint32 (-1)
        private long makeFlagOrBitmask(CheckedListBox list, Type e)
        {
            if (!e.IsEnum)
                return 0;

            long myFlags = 0x0;

            foreach (Object item in list.CheckedItems)
            {
                myFlags += Convert.ToInt64(Enum.Parse(e, item.ToString()));
            }

            return myFlags;
        }

        private void checkFlagOrBitmask(CheckedListBox list, Type e, long value)
        {
            if (!e.IsEnum)
                return;

            list.ClearSelected();

            if(value == -1)
            {
                list.SetItemCheckState(0, CheckState.Checked);
            }
            else
            {
                var values = Enum.GetValues(e);
                for (int i = values.Length - 1; i > 0; i--)
                {
                    long val = Convert.ToInt64(values.GetValue(i));
                    if (val <= value)
                    {
                        list.SetItemCheckState(i, CheckState.Checked);
                        value -= val;
                    }
                }
            }
        }

        public void loadItem(Storage.Database.WotLk.TrinityCore.Item item)
        {
            this.Entry.Text = item.EntryId.ToString();
            this.ItemClass.Text = item.Class.ToString();
            this.SubClass.Text = item.SubClass.ToString();
            this.SoundOverrideSubclass.Text = item.SoundOverrideSubclass.ToString();
            this.ItemName.Text = item.name;
            this.ModelId.Text = item.displayid.ToString();
            this.Quality.Text = item.Quality.ToString();
            checkFlagOrBitmask(this.Flags, typeof(Storage.Database.WotLk.TrinityCore.itemFlags), item.ItemFlags);
            checkFlagOrBitmask(this.FlagsExtra, typeof(Storage.Database.WotLk.TrinityCore.itemFlagsExtra), item.ItemFlagsExtra);
            this.BuyCount.Text = item.BuyCount.ToString();
            this.BuyPrice.Text = item.BuyPrice.ToString();
            this.SellPrice.Text = item.SellPrice.ToString();
            this.InventoryType.Text = item.InventoryType.ToString();
            checkFlagOrBitmask(this.AllowableClass, typeof(Storage.Database.WotLk.TrinityCore.itemAllowableClass), item.AllowableClass);
            checkFlagOrBitmask(this.AllowableRace, typeof(Storage.Database.WotLk.TrinityCore.itemAllowableRace), item.AllowableRace);
            this.ItemLevel.Text = item.ItemLevel.ToString();
            this.RequiredLevel.Text = item.RequiredLevel.ToString();
            this.RequiredSkill.Text = item.RequiredSkill.ToString();
            this.RequiredSkillRank.Text = item.RequiredSkillRank.ToString();
            this.RequiredSpell.Text = item.requiredspell.ToString();
            this.RequiredHonorRank.Text = item.requiredhonorrank.ToString();
            this.RequiredCityRank.Text = item.RequiredCityRank.ToString();
            this.RequiredReputationFaction.Text = item.RequiredReputationFaction.ToString();
            this.RequiredReputationRank.Text = item.RequiredReputationRank.ToString();
            this.MaxCount.Text = item.maxcount.ToString();
            this.Stackable.Text = item.stackable.ToString();
            this.ContainerSlot.Text = item.ContainerSlots.ToString();
            this.StatsCount.Text = item.StatsCount.ToString();
            this.StatType1.Text = item.stat_type1.ToString();
            this.StatValue1.Text = item.stat_value1.ToString();
            this.StatType2.Text = item.stat_type2.ToString();
            this.StatValue2.Text = item.stat_value2.ToString();
            this.StatType3.Text = item.stat_type3.ToString();
            this.StatValue3.Text = item.stat_value3.ToString();
            this.StatType4.Text = item.stat_type4.ToString();
            this.StatValue4.Text = item.stat_value4.ToString();
            this.StatType5.Text = item.stat_type5.ToString();
            this.StatValue5.Text = item.stat_value5.ToString();
            this.StatType6.Text = item.stat_type6.ToString();
            this.StatValue6.Text = item.stat_value6.ToString();
            this.StatType7.Text = item.stat_type7.ToString();
            this.StatValue7.Text = item.stat_value7.ToString();
            this.StatType8.Text = item.stat_type8.ToString();
            this.StatValue8.Text = item.stat_value8.ToString();
            this.StatType9.Text = item.stat_type9.ToString();
            this.StatValue9.Text = item.stat_value9.ToString();
            this.StatType10.Text = item.stat_type10.ToString();
            this.StatValue10.Text = item.stat_value10.ToString();
            this.ScalingStatDistribution.Text = item.ScalingStatDistribution.ToString();
            this.ScalingStatValue.Text = item.ScalingStatValue.ToString();
            this.DmgMin1.Text = item.dmg_min1.ToString();
            this.DmgMax1.Text = item.dmg_max1.ToString();
            this.dmgType1.Text = item.dmg_type1.ToString();
            this.DmgMin2.Text = item.dmg_min2.ToString();
            this.DmgMax2.Text = item.dmg_max2.ToString();
            this.dmgType2.Text = item.dmg_type2.ToString();
            this.Armor.Text = item.armor.ToString();
            this.ResHoly.Text = item.holy_res.ToString();
            this.ResFire.Text = item.fire_res.ToString();
            this.ResNature.Text = item.nature_res.ToString();
            this.ResFrost.Text = item.frost_res.ToString();
            this.ResShadow.Text = item.shadow_res.ToString();
            this.ResArcane.Text = item.arcane_res.ToString();
            this.Delay.Text = item.delay.ToString();
            this.AmmoType.Text = item.ammo_type.ToString();
            this.RangedModRange.Text = item.RangedModRange.ToString();
            this.SpellID1.Text = item.spellid_1.ToString();
            this.SpellTrigger1.Text = item.spelltrigger_1.ToString();
            this.SpellCharges1.Text = item.spellcharges_1.ToString();
            this.SpellPpmRate1.Text = item.spellppmRate_1.ToString();
            this.SpellCooldown1.Text = item.spellcooldown_1.ToString();
            this.SpellCategory1.Text = item.spellcategory_1.ToString();
            this.SpellCategoryCooldown1.Text = item.spellcategorycooldown_1.ToString();
            this.SpellID2.Text = item.spellid_2.ToString();
            this.SpellTrigger2.Text = item.spelltrigger_2.ToString();
            this.SpellCharges2.Text = item.spellcharges_2.ToString();
            this.SpellPpmRate2.Text = item.spellppmRate_2.ToString();
            this.SpellCooldown2.Text = item.spellcooldown_2.ToString();
            this.SpellCategory2.Text = item.spellcategory_2.ToString();
            this.SpellCategoryCooldown2.Text = item.spellcategorycooldown_2.ToString();
            this.SpellID3.Text = item.spellid_3.ToString();
            this.SpellTrigger3.Text = item.spelltrigger_3.ToString();
            this.SpellCharges3.Text = item.spellcharges_3.ToString();
            this.SpellPpmRate3.Text = item.spellppmRate_3.ToString();
            this.SpellCooldown3.Text = item.spellcooldown_3.ToString();
            this.SpellCategory3.Text = item.spellcategory_3.ToString();
            this.SpellCategoryCooldown3.Text = item.spellcategorycooldown_3.ToString();
            this.SpellID4.Text = item.spellid_4.ToString();
            this.SpellTrigger4.Text = item.spelltrigger_4.ToString();
            this.SpellCharges4.Text = item.spellcharges_4.ToString();
            this.SpellPpmRate4.Text = item.spellppmRate_4.ToString();
            this.SpellCooldown4.Text = item.spellcooldown_4.ToString();
            this.SpellCategory4.Text = item.spellcategory_4.ToString();
            this.SpellCategoryCooldown4.Text = item.spellcategorycooldown_4.ToString();
            this.SpellID5.Text = item.spellid_5.ToString();
            this.SpellTrigger5.Text = item.spelltrigger_5.ToString();
            this.SpellCharges5.Text = item.spellcharges_5.ToString();
            this.SpellPpmRate5.Text = item.spellppmRate_5.ToString();
            this.SpellCooldown5.Text = item.spellcooldown_5.ToString();
            this.SpellCategory5.Text = item.spellcategory_5.ToString();
            this.SpellCategoryCooldown5.Text = item.spellcategorycooldown_5.ToString();
            this.Bonding.Text = item.bonding.ToString();
            this.Description.Text = item.description.ToString();
            this.PageText.Text = item.PageText.ToString();
            this.LanguageId.Text = item.LanguageID.ToString();
            this.PageMaterial.Text = item.PageMaterial.ToString();
            this.StartQuest.Text = item.startquest.ToString();
            this.LockId.Text = item.lockid.ToString();
            this.Material.Text = item.Material.ToString();
            this.Sheath.Text = item.sheath.ToString();
            this.RandomProperty.Text = item.RandomProperty.ToString();
            this.RandomSuffix.Text = item.RandomSuffix.ToString();
            this.Block.Text = item.block.ToString();
            this.ItemSet.Text = item.itemset.ToString();
            this.MaxDurability.Text= item.MaxDurability.ToString();
            this.Area.Text = item.area.ToString();
            this.Map.Text = item.Map.ToString();
            checkFlagOrBitmask(this.BagFamily, typeof(Storage.Database.WotLk.TrinityCore.itemBagFamily), item.BagFamily);
            this.TotemCategory.Text = item.TotemCategory.ToString();
            this.SocketColor1.Text = item.socketColor_1.ToString();
            this.SocketContent1.Text = item.socketContent_1.ToString();
            this.SocketColor2.Text = item.socketColor_2.ToString();
            this.SocketContent2.Text = item.socketContent_2.ToString();
            this.SocketColor3.Text = item.socketColor_3.ToString();
            this.SocketContent3.Text = item.socketContent_3.ToString();
            this.SocketBonus.Text = item.socketBonus.ToString();
            this.GemProperties.Text = item.GemProperties.ToString();
            this.ReqDisenchantSkill.Text = item.RequiredDisenchantSkill.ToString();
            this.ArmorDmgMod.Text = item.ArmorDamageModifier.ToString();
            this.Duration.Text = item.duration.ToString();
            this.ItemLimitCategory.Text = item.ItemLimitCategory.ToString();
            this.HolidayId.Text = item.HolidayId.ToString();
            this.ScriptName.Text = item.ScriptName;
            this.DisenchantId.Text = item.DisenchantID.ToString();
            this.FoodType.Text = item.FoodType.ToString();
            this.VerifiedBuild.Text = item.VerifiedBuild.ToString();
            this.MinMoneyLoot.Text = item.minMoneyLoot.ToString();
            this.MaxMoneyLoot.Text = item.maxMoneyLoot.ToString();
            checkFlagOrBitmask(this.FlagsCustom, typeof(Storage.Database.WotLk.TrinityCore.itemFlagsCustom), item.flagsCustom);
    }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (LoadEntry.Text != "")
            {
                if (Storage.Database.WotLk.TrinityCore.ItemManager.Instance.GetItemByEntry(Convert.ToInt32(LoadEntry.Text)) == null)
                {
                    MessageBox.Show("There is no item with this id.");
                }

                Storage.Database.WotLk.TrinityCore.Item itemLoaded = new Storage.Database.WotLk.TrinityCore.Item();

                itemLoaded = Storage.Database.WotLk.TrinityCore.ItemManager.Instance.GetItemByEntry(int.Parse(LoadEntry.Text));

                if (itemLoaded != null)
                {
                    loadItem(itemLoaded);
                }
            }
        }
    }
}
