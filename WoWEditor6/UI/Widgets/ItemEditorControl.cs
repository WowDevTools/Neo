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
    }
}
