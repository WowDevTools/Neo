using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace WoWEditor6.UI.Dialog
{
    [Designer(typeof(GameObjectEditorControlDesigner))]
    public partial class GameObjectEditorControl : UserControl
    {
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
            showModelInRenderControl(ModelId1.Text);
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

