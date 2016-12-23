using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Neo.UI.Dialog
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
                foreach (TabPage page in ctl.TabPages)
                {
	                EnableDesignMode(page, page.Name);
                }
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
            this.SoundOverrideSubclass.Items.Clear();
            this.SoundOverrideSubclass.Items.Add("None");
            switch (this.ItemClass.SelectedIndex)
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
                    this.SoundOverrideSubclass.Items.Add("Consumable");
                    this.SoundOverrideSubclass.Items.Add("Potion");
                    this.SoundOverrideSubclass.Items.Add("Elixir");
                    this.SoundOverrideSubclass.Items.Add("Flask");
                    this.SoundOverrideSubclass.Items.Add("Scroll");
                    this.SoundOverrideSubclass.Items.Add("Food & Drink");
                    this.SoundOverrideSubclass.Items.Add("Item Enhancement");
                    this.SoundOverrideSubclass.Items.Add("Bandage");
                    this.SoundOverrideSubclass.Items.Add("Other");
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
                    this.SoundOverrideSubclass.Items.Add("Bag");
                    this.SoundOverrideSubclass.Items.Add("Soul Bag");
                    this.SoundOverrideSubclass.Items.Add("Herb Bag");
                    this.SoundOverrideSubclass.Items.Add("Enchanting Bag");
                    this.SoundOverrideSubclass.Items.Add("Engineering Bag");
                    this.SoundOverrideSubclass.Items.Add("Gem Bag");
                    this.SoundOverrideSubclass.Items.Add("Mining Bag");
                    this.SoundOverrideSubclass.Items.Add("Leatherworking Bag");
                    this.SoundOverrideSubclass.Items.Add("Inscription Bag");
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
                    this.SoundOverrideSubclass.Items.Add("One handed axe");
                    this.SoundOverrideSubclass.Items.Add("Tne handed axe");
                    this.SoundOverrideSubclass.Items.Add("Bow");
                    this.SoundOverrideSubclass.Items.Add("Gun");
                    this.SoundOverrideSubclass.Items.Add("One handed mace");
                    this.SoundOverrideSubclass.Items.Add("Two handed mace");
                    this.SoundOverrideSubclass.Items.Add("Polearm");
                    this.SoundOverrideSubclass.Items.Add("One handed sword");
                    this.SoundOverrideSubclass.Items.Add("Two handed sword");
                    this.SoundOverrideSubclass.Items.Add("Obsolete");
                    this.SoundOverrideSubclass.Items.Add("Staff");
                    this.SoundOverrideSubclass.Items.Add("Exotic");
                    this.SoundOverrideSubclass.Items.Add("Exotic");
                    this.SoundOverrideSubclass.Items.Add("Fist Weapon");
                    this.SoundOverrideSubclass.Items.Add("Miscellaneous");
                    this.SoundOverrideSubclass.Items.Add("Dagger");
                    this.SoundOverrideSubclass.Items.Add("Thrown");
                    this.SoundOverrideSubclass.Items.Add("Spear");
                    this.SoundOverrideSubclass.Items.Add("Crossbow");
                    this.SoundOverrideSubclass.Items.Add("Wand");
                    this.SoundOverrideSubclass.Items.Add("Fishing Pole");
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
                    this.SoundOverrideSubclass.Items.Add("Red");
                    this.SoundOverrideSubclass.Items.Add("Blue");
                    this.SoundOverrideSubclass.Items.Add("Yellow");
                    this.SoundOverrideSubclass.Items.Add("Purple");
                    this.SoundOverrideSubclass.Items.Add("Green");
                    this.SoundOverrideSubclass.Items.Add("Orange");
                    this.SoundOverrideSubclass.Items.Add("Meta");
                    this.SoundOverrideSubclass.Items.Add("Simple");
                    this.SoundOverrideSubclass.Items.Add("Prismatic");
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
                    this.SoundOverrideSubclass.Items.Add("Miscellaneous");
                    this.SoundOverrideSubclass.Items.Add("Cloth");
                    this.SoundOverrideSubclass.Items.Add("Leather");
                    this.SoundOverrideSubclass.Items.Add("Mail");
                    this.SoundOverrideSubclass.Items.Add("Plate");
                    this.SoundOverrideSubclass.Items.Add("Buckler(OBSOLETE)");
                    this.SoundOverrideSubclass.Items.Add("Shield");
                    this.SoundOverrideSubclass.Items.Add("Libram");
                    this.SoundOverrideSubclass.Items.Add("Idol");
                    this.SoundOverrideSubclass.Items.Add("Totem");
                    this.SoundOverrideSubclass.Items.Add("Sigil");
                    break;

                case 5:
                    this.SubClass.Items.Add("Reagent");
                    this.SoundOverrideSubclass.Items.Add("Reagent");
                    break;

                case 6:
                    this.SubClass.Items.Add("Wand(OBSOLETE)");
                    this.SubClass.Items.Add("Bolt(OBSOLETE)");
                    this.SubClass.Items.Add("Arrow");
                    this.SubClass.Items.Add("Bullet");
                    this.SubClass.Items.Add("Thrown(OBSOLETE)");
                    this.SoundOverrideSubclass.Items.Add("Wand(OBSOLETE)");
                    this.SoundOverrideSubclass.Items.Add("Bolt(OBSOLETE)");
                    this.SoundOverrideSubclass.Items.Add("Arrow");
                    this.SoundOverrideSubclass.Items.Add("Bullet");
                    this.SoundOverrideSubclass.Items.Add("Thrown(OBSOLETE)");
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
                    this.SoundOverrideSubclass.Items.Add("Trade Goods");
                    this.SoundOverrideSubclass.Items.Add("Parts");
                    this.SoundOverrideSubclass.Items.Add("Explosives");
                    this.SoundOverrideSubclass.Items.Add("Devices");
                    this.SoundOverrideSubclass.Items.Add("Jewelcrafting");
                    this.SoundOverrideSubclass.Items.Add("Cloth");
                    this.SoundOverrideSubclass.Items.Add("Leather");
                    this.SoundOverrideSubclass.Items.Add("Metal & Stone");
                    this.SoundOverrideSubclass.Items.Add("Meat");
                    this.SoundOverrideSubclass.Items.Add("Herb");
                    this.SoundOverrideSubclass.Items.Add("Elemental");
                    this.SoundOverrideSubclass.Items.Add("Other");
                    this.SoundOverrideSubclass.Items.Add("Enchanting");
                    this.SoundOverrideSubclass.Items.Add("Materials");
                    this.SoundOverrideSubclass.Items.Add("Armor Enchantment");
                    this.SoundOverrideSubclass.Items.Add("Weapon Enchantment");
                    break;

                case 8:
                    this.SubClass.Items.Add("Generic(OBSOLETE)");
                    this.SoundOverrideSubclass.Items.Add("Generic(OBSOLETE)");
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
                    this.SoundOverrideSubclass.Items.Add("Book");
                    this.SoundOverrideSubclass.Items.Add("Leatherworking");
                    this.SoundOverrideSubclass.Items.Add("Tailoring");
                    this.SoundOverrideSubclass.Items.Add("Engineering");
                    this.SoundOverrideSubclass.Items.Add("Blacksmithing");
                    this.SoundOverrideSubclass.Items.Add("Cooking");
                    this.SoundOverrideSubclass.Items.Add("Alchemy");
                    this.SoundOverrideSubclass.Items.Add("First Aid");
                    this.SoundOverrideSubclass.Items.Add("Enchanting");
                    this.SoundOverrideSubclass.Items.Add("Fishing");
                    this.SoundOverrideSubclass.Items.Add("Jewelcrafting");
                    break;

                case 10:
                    this.SubClass.Items.Add("Money(OBSOLETE)");
                    this.SoundOverrideSubclass.Items.Add("Money(OBSOLETE)");
                    break;

                case 11:
                    this.SubClass.Items.Add("Quiver(OBSOLETE)");
                    this.SubClass.Items.Add("Quiver(OBSOLETE)");
                    this.SubClass.Items.Add("Quiver");
                    this.SubClass.Items.Add("Ammo Pouch");
                    this.SoundOverrideSubclass.Items.Add("Quiver(OBSOLETE)");
                    this.SoundOverrideSubclass.Items.Add("Quiver(OBSOLETE)");
                    this.SoundOverrideSubclass.Items.Add("Quiver");
                    this.SoundOverrideSubclass.Items.Add("Ammo Pouch");
                    break;

                case 12:
                    this.SubClass.Items.Add("Quest");
                    this.SoundOverrideSubclass.Items.Add("Quest");
                    break;

                case 13:
                    this.SubClass.Items.Add("Key");
                    this.SubClass.Items.Add("Lockpick");
                    this.SoundOverrideSubclass.Items.Add("Key");
                    this.SoundOverrideSubclass.Items.Add("Lockpick");
                    break;

                case 14:
                    this.SubClass.Items.Add("Permanent");
                    this.SoundOverrideSubclass.Items.Add("Permanent");
                    break;

                case 15:
                    this.SubClass.Items.Add("Junk");
                    this.SubClass.Items.Add("Reagent");
                    this.SubClass.Items.Add("Pet");
                    this.SubClass.Items.Add("Holiday");
                    this.SubClass.Items.Add("Other");
                    this.SubClass.Items.Add("Mount");
                    this.SoundOverrideSubclass.Items.Add("Junk");
                    this.SoundOverrideSubclass.Items.Add("Reagent");
                    this.SoundOverrideSubclass.Items.Add("Pet");
                    this.SoundOverrideSubclass.Items.Add("Holiday");
                    this.SoundOverrideSubclass.Items.Add("Other");
                    this.SoundOverrideSubclass.Items.Add("Mount");
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
                    this.SoundOverrideSubclass.Items.Add("Warrior");
                    this.SoundOverrideSubclass.Items.Add("Paladin");
                    this.SoundOverrideSubclass.Items.Add("Hunter");
                    this.SoundOverrideSubclass.Items.Add("Rogue");
                    this.SoundOverrideSubclass.Items.Add("Priest");
                    this.SoundOverrideSubclass.Items.Add("Death Knight");
                    this.SoundOverrideSubclass.Items.Add("Shaman");
                    this.SoundOverrideSubclass.Items.Add("Mage");
                    this.SoundOverrideSubclass.Items.Add("Warlock");
                    this.SoundOverrideSubclass.Items.Add("None");
                    this.SoundOverrideSubclass.Items.Add("Druid");
                    break;
            }

            this.SubClass.SelectedIndex = 0;
        }

        //Use of long because i need more than a int32 and less than an uint32 (-1)
        private long makeFlagOrBitmask(CheckedListBox list, Type e)
        {
            if (!e.IsEnum)
            {
	            return 0;
            }

	        long myFlags = 0x0;

            foreach (Object item in list.CheckedItems)
            {
                myFlags += Convert.ToInt64(Enum.Parse(e, item.ToString()));
            }

            if(list == this.AllowableClass || list == this.AllowableRace)
            {
                if (myFlags == 0)
                {
	                myFlags = -1;
                }
            }

            return myFlags;
        }

        private void checkFlagOrBitmask(CheckedListBox list, Type e, long value)
        {
            if (!e.IsEnum)
            {
	            return;
            }

	        foreach (int i in list.CheckedIndices)
            {
                list.SetItemCheckState(i, CheckState.Unchecked);
            }

            if (value == 0 && list == this.AllowableClass || list == this.AllowableRace)
            {
                list.SetItemCheckState(0, CheckState.Checked);
            }
            else
            {
                var values = Enum.GetValues(e);
                for (int i = values.Length - 1; i >= 0; i--)
                {
                    long val = Convert.ToInt64(values.GetValue(i));
                    if (val <= value)
                    {
                        if(val != 0)
                        {
                            list.SetItemCheckState(i, CheckState.Checked);
                            value -= val;
                        }
                    }
                }
            }
        }

        public void loadItem(Storage.Database.WotLk.TrinityCore.Item item)
        {
            this.Entry.Text = item.EntryId.ToString();
            this.ItemClass.Text = item.Class.ToString();
            this.SubClass.Text = item.SubClass.ToString();
            this.SoundOverrideSubclass.SelectedIndex = (item.SoundOverrideSubclass == -1 ? 0 : item.SoundOverrideSubclass);
            this.ItemName.Text = item.name;
            this.ModelId.Text = item.displayid.ToString();
            this.Quality.Text = item.Quality.ToString();
            checkFlagOrBitmask(this.Flags, typeof(Storage.Database.WotLk.TrinityCore.itemFlags), item.ItemFlags);
            checkFlagOrBitmask(this.FlagsExtra, typeof(Storage.Database.WotLk.TrinityCore.itemFlagsExtra), item.ItemFlagsExtra);
            this.BuyCount.Text = item.BuyCount.ToString();
            this.BuyPrice.Text = item.BuyPrice.ToString();
            this.SellPrice.Text = item.SellPrice.ToString();
            this.InventoryType.Text = item.InventoryType.ToString();
            checkFlagOrBitmask(this.AllowableClass, typeof(Storage.Database.WotLk.TrinityCore.itemAllowableClass), item.AllowableClass == -1 ? 0: item.AllowableClass);
            checkFlagOrBitmask(this.AllowableRace, typeof(Storage.Database.WotLk.TrinityCore.itemAllowableRace), item.AllowableRace == -1 ? 0 : item.AllowableRace);
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
            this.ContainerSlots.Text = item.ContainerSlots.ToString();
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
            Storage.Database.WotLk.TrinityCore.Item item = new Storage.Database.WotLk.TrinityCore.Item();

            item.EntryId = int.Parse(this.Entry.Text);
            item.Class = (Storage.Database.WotLk.TrinityCore.itemClass)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemClass), this.ItemClass.Text);
            item.SubClass = int.Parse(this.SubClass.Text);
            item.SoundOverrideSubclass = this.SoundOverrideSubclass.SelectedIndex == 0 ? -1 : this.SoundOverrideSubclass.SelectedIndex;
            item.name = this.ItemName.Text;
            item.displayid = int.Parse(this.ModelId.Text);
            item.Quality = (Storage.Database.WotLk.TrinityCore.itemQuality)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemQuality), this.Quality.Text);
            item.ItemFlags = (uint)makeFlagOrBitmask(this.Flags, typeof(Storage.Database.WotLk.TrinityCore.itemFlags));
            item.ItemFlagsExtra = (uint)makeFlagOrBitmask(this.FlagsExtra, typeof(Storage.Database.WotLk.TrinityCore.itemFlagsExtra));
            item.BuyCount = int.Parse(this.BuyCount.Text);
            item.BuyPrice = uint.Parse(this.BuyPrice.Text);
            item.SellPrice = uint.Parse(this.SellPrice.Text);
            item.InventoryType = (Storage.Database.WotLk.TrinityCore.itemInventoryType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemInventoryType), this.InventoryType.Text);
            item.AllowableClass = (int)makeFlagOrBitmask(this.AllowableClass, typeof(Storage.Database.WotLk.TrinityCore.itemAllowableClass));
            item.AllowableRace = (int)makeFlagOrBitmask(this.AllowableRace, typeof(Storage.Database.WotLk.TrinityCore.itemAllowableRace));
            item.ItemLevel = int.Parse(this.ItemLevel.Text);
            item.RequiredLevel = int.Parse(this.RequiredLevel.Text);
            item.RequiredSkill = int.Parse(this.RequiredSkill.Text);
            item.RequiredSkillRank = int.Parse(this.RequiredSkillRank.Text);
            item.requiredspell = int.Parse(this.RequiredSpell.Text);
            item.requiredhonorrank = int.Parse(this.RequiredHonorRank.Text);
            item.RequiredCityRank = int.Parse(this.RequiredCityRank.Text);
            item.RequiredReputationFaction = int.Parse(this.RequiredReputationFaction.Text);
            item.RequiredReputationRank = (Storage.Database.WotLk.TrinityCore.itemReputationRank)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemReputationRank), this.RequiredReputationRank.Text);
            item.maxcount = int.Parse(this.MaxCount.Text);
            item.stackable = int.Parse(this.Stackable.Text);
            item.ContainerSlots = int.Parse(this.ContainerSlots.Text);
            item.StatsCount = int.Parse(this.StatsCount.Text);
            item.stat_type1 = (Storage.Database.WotLk.TrinityCore.itemStatType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemStatType), this.StatType1.Text);
            item.stat_value1 = int.Parse(this.StatValue1.Text);
            item.stat_type2 = (Storage.Database.WotLk.TrinityCore.itemStatType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemStatType), this.StatType2.Text);
            item.stat_value2 = int.Parse(this.StatValue2.Text);
            item.stat_type3 = (Storage.Database.WotLk.TrinityCore.itemStatType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemStatType), this.StatType3.Text);
            item.stat_value3 = int.Parse(this.StatValue3.Text);
            item.stat_type4 = (Storage.Database.WotLk.TrinityCore.itemStatType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemStatType), this.StatType4.Text);
            item.stat_value4 = int.Parse(this.StatValue4.Text);
            item.stat_type5 = (Storage.Database.WotLk.TrinityCore.itemStatType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemStatType), this.StatType5.Text);
            item.stat_value5 = int.Parse(this.StatValue5.Text);
            item.stat_type6 = (Storage.Database.WotLk.TrinityCore.itemStatType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemStatType), this.StatType6.Text);
            item.stat_value6 = int.Parse(this.StatValue6.Text);
            item.stat_type7 = (Storage.Database.WotLk.TrinityCore.itemStatType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemStatType), this.StatType7.Text);
            item.stat_value7 = int.Parse(this.StatValue7.Text);
            item.stat_type8 = (Storage.Database.WotLk.TrinityCore.itemStatType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemStatType), this.StatType8.Text);
            item.stat_value8 = int.Parse(this.StatValue8.Text);
            item.stat_type9 = (Storage.Database.WotLk.TrinityCore.itemStatType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemStatType), this.StatType9.Text);
            item.stat_value9 = int.Parse(this.StatValue9.Text);
            item.stat_type10 = (Storage.Database.WotLk.TrinityCore.itemStatType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemStatType), this.StatType10.Text);
            item.stat_value10 = int.Parse(this.StatValue10.Text);
            item.ScalingStatDistribution = int.Parse(this.ScalingStatDistribution.Text);
            item.ScalingStatValue = int.Parse(this.ScalingStatValue.Text);
            item. dmg_min1 = float.Parse(this.DmgMin1.Text);
            item. dmg_max1 = float.Parse(this.DmgMax1.Text);
            item.dmg_type1 = (Storage.Database.WotLk.TrinityCore.itemDmgType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemDmgType), this.dmgType1.Text);
            item. dmg_min2 = float.Parse(this.DmgMin2.Text);
            item. dmg_max2 = float.Parse(this.DmgMax2.Text);
            item.dmg_type2 = (Storage.Database.WotLk.TrinityCore.itemDmgType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemDmgType), this.dmgType2.Text);
            item.armor = int.Parse(this.Armor.Text);
            item.holy_res = int.Parse(this.ResHoly.Text);
            item.fire_res = int.Parse(this.ResFire.Text);
            item.nature_res = int.Parse(this.ResNature.Text);
            item.frost_res = int.Parse(this.ResFrost.Text);
            item.shadow_res = int.Parse(this.ResShadow.Text);
            item.arcane_res = int.Parse(this.ResArcane.Text);
            item.delay = int.Parse(this.Delay.Text);
            item.ammo_type = (Storage.Database.WotLk.TrinityCore.itemAmmoType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemAmmoType), this.AmmoType.Text);
            item.RangedModRange = float.Parse(this.RangedModRange.Text);
            item.spellid_1 = int.Parse(this.SpellID1.Text);
            item.spelltrigger_1 = (Storage.Database.WotLk.TrinityCore.itemSpellTrigger)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemSpellTrigger), this.SpellTrigger1.Text);
            item.spellcharges_1 = int.Parse(this.SpellCharges1.Text);
            item. spellppmRate_1 = int.Parse(this.SpellPpmRate1.Text);
            item.spellcooldown_1 = int.Parse(this.SpellCooldown1.Text);
            item.spellcategory_1 = int.Parse(this.SpellCategory1.Text);
            item.spellcategorycooldown_1 = int.Parse(this.SpellCategoryCooldown1.Text);
            item.spellid_2 = int.Parse(this.SpellID2.Text);
            item.spelltrigger_2 = (Storage.Database.WotLk.TrinityCore.itemSpellTrigger)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemSpellTrigger), this.SpellTrigger2.Text);
            item.spellcharges_2 = int.Parse(this.SpellCharges2.Text);
            item.spellppmRate_2 = int.Parse(this.SpellPpmRate2.Text);
            item.spellcooldown_2 = int.Parse(this.SpellCooldown2.Text);
            item.spellcategory_2 = int.Parse(this.SpellCategory2.Text);
            item.spellcategorycooldown_2 = int.Parse(this.SpellCategoryCooldown2.Text);
            item.spellid_3 = int.Parse(this.SpellID3.Text);
            item.spelltrigger_3 = (Storage.Database.WotLk.TrinityCore.itemSpellTrigger)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemSpellTrigger), this.SpellTrigger3.Text);
            item.spellcharges_3 = int.Parse(this.SpellCharges3.Text);
            item.spellppmRate_3 = int.Parse(this.SpellPpmRate3.Text);
            item.spellcooldown_3 = int.Parse(this.SpellCooldown3.Text);
            item.spellcategory_3 = int.Parse(this.SpellCategory3.Text);
            item.spellcategorycooldown_3 = int.Parse(this.SpellCategoryCooldown3.Text);
            item.spellid_4 = int.Parse(this.SpellID4.Text);
            item.spelltrigger_4 = (Storage.Database.WotLk.TrinityCore.itemSpellTrigger)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemSpellTrigger), this.SpellTrigger4.Text);
            item.spellcharges_4 = int.Parse(this.SpellCharges4.Text);
            item.spellppmRate_4 = int.Parse(this.SpellPpmRate4.Text);
            item.spellcooldown_4 = int.Parse(this.SpellCooldown4.Text);
            item.spellcategory_4 = int.Parse(this.SpellCategory4.Text);
            item.spellcategorycooldown_4 = int.Parse(this.SpellCategoryCooldown4.Text);
            item.spellid_5 = int.Parse(this.SpellID5.Text);
            item.spelltrigger_5 = (Storage.Database.WotLk.TrinityCore.itemSpellTrigger)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemSpellTrigger), this.SpellTrigger5.Text);
            item.spellcharges_5 = int.Parse(this.SpellCharges5.Text);
            item.spellppmRate_5 = int.Parse(this.SpellPpmRate5.Text);
            item.spellcooldown_5 = int.Parse(this.SpellCooldown5.Text);
            item.spellcategory_5 = int.Parse(this.SpellCategory5.Text);
            item.spellcategorycooldown_5 = int.Parse(this.SpellCategoryCooldown5.Text);
            item.bonding = (Storage.Database.WotLk.TrinityCore.itemBonding)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemBonding), this.Bonding.Text);
            item.description = this.Description.Text;
            item.PageText = int.Parse(this.PageText.Text);
            item.LanguageID = int.Parse(this.LanguageId.Text);
            item.PageMaterial = int.Parse(this.PageMaterial.Text);
            item.startquest = int.Parse(this.StartQuest.Text);
            item.lockid = int.Parse(this.LockId.Text);
            item.Material = (Storage.Database.WotLk.TrinityCore.itemMaterial)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemMaterial), this.Material.Text);
            item.sheath = (Storage.Database.WotLk.TrinityCore.itemSheath)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemSheath), this.Sheath.Text);
            item.RandomProperty = int.Parse(this.RandomProperty.Text);
            item.RandomSuffix = int.Parse(this.RandomSuffix.Text);
            item.block = int.Parse(this.Block.Text);
            item.itemset = int.Parse(this.ItemSet.Text);
            item.MaxDurability = int.Parse(this.MaxDurability.Text);
            item.area = int.Parse(this.Area.Text);
            item.Map = int.Parse(this.Map.Text);
            item.BagFamily = (int)makeFlagOrBitmask(this.BagFamily, typeof(Storage.Database.WotLk.TrinityCore.itemBagFamily));
            item.TotemCategory = (Storage.Database.WotLk.TrinityCore.itemTotemCategory)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemTotemCategory), this.TotemCategory.Text);
            item.socketColor_1 = (Storage.Database.WotLk.TrinityCore.itemSocketColor)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemSocketColor), this.SocketColor1.Text);
            item.socketContent_1 = int.Parse(this.SocketContent1.Text);
            item.socketColor_2 = (Storage.Database.WotLk.TrinityCore.itemSocketColor)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemSocketColor), this.SocketColor2.Text);
            item.socketContent_2 = int.Parse(this.SocketContent2.Text);
            item.socketColor_3 = (Storage.Database.WotLk.TrinityCore.itemSocketColor)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemSocketColor), this.SocketColor3.Text);
            item.socketContent_3 = int.Parse(this.SocketContent3.Text);
            item.socketBonus = int.Parse(this.SocketBonus.Text);
            item.GemProperties = int.Parse(this.GemProperties.Text);
            item.RequiredDisenchantSkill = int.Parse(this.ReqDisenchantSkill.Text);
            item.ArmorDamageModifier = float.Parse(this.ArmorDmgMod.Text);
            item.duration = int.Parse(this.Duration.Text);
            item.ItemLimitCategory = int.Parse(this.ItemLimitCategory.Text);
            item.HolidayId = int.Parse(this.HolidayId.Text);
            item.ScriptName = this.ScriptName.Text;
            item.DisenchantID = int.Parse(this.DisenchantId.Text);
            item.FoodType = (Storage.Database.WotLk.TrinityCore.itemFoodType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.itemFoodType), this.FoodType.Text);
            item.VerifiedBuild = int.Parse(this.VerifiedBuild.Text);
            item.minMoneyLoot = int.Parse(this.MinMoneyLoot.Text);
            item.maxMoneyLoot = int.Parse(this.MaxMoneyLoot.Text);
            item.flagsCustom = (int)makeFlagOrBitmask(this.FlagsCustom, typeof(Storage.Database.WotLk.TrinityCore.itemFlagsCustom));

            if (Storage.Database.WotLk.TrinityCore.ItemManager.Instance.GetItemByEntry(item.EntryId) == null)
            {
                Storage.Database.MySqlConnector.Instance.Query(item.GetInsertSqlQuery());
                Storage.Database.WotLk.TrinityCore.ItemManager.Instance.addCreatedItem(item);
                MessageBox.Show("Inserted");
            }
            else
            {
                Storage.Database.MySqlConnector.Instance.Query(item.GetUpdateSqlQuery());
                MessageBox.Show("Updated");
            }
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
