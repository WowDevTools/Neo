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
    }
}
