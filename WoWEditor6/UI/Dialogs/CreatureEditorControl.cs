using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Design;
using System.Windows.Forms.Design;

namespace WoWEditor6.UI.Dialogs
{
    [Designer(typeof(CreatureEditorControlDesigner))]
    public partial class CreatureEditorControl : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabControl TabControl
        {
            get { return this.tabControl1;  }
        }
        public CreatureEditorControl()
        {
            InitializeComponent();
        }

        private void btnShowModelId1_Click(object sender, EventArgs e)
        {
            showModelInRenderControl(int.Parse(ModelId1.Text));
        }

        private void btnShowModelId2_Click(object sender, EventArgs e)
        {
            showModelInRenderControl(int.Parse(ModelId2.Text));
        }

        private void btnShowModelId3_Click(object sender, EventArgs e)
        {
            showModelInRenderControl(int.Parse(ModelId3.Text));
        }

        private void btnShowModelId4_Click(object sender, EventArgs e)
        {
            showModelInRenderControl(int.Parse(ModelId4.Text));
        }

        private void showModelInRenderControl(int pModelId)
        {
            if(!(pModelId == 0))
            {
                //modelRenderControl1.SetModel("");
            }
        }
    }

    internal class CreatureEditorControlDesigner : ControlDesigner
    {
        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);

            var ctl = (this.Control as CreatureEditorControl).TabControl as TabControl;
            EnableDesignMode(ctl, "TabControl");
            foreach (TabPage page in ctl.TabPages) EnableDesignMode(page, page.Name);
            
        }
    }
}
