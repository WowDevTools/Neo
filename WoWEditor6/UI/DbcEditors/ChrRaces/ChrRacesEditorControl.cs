using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DBCLib.Structures;
using WoWEditor6.Dbc;

namespace WoWEditor6.UI.DbcEditors
{
    [Designer(typeof(ChrRacesEditorControlDesigner))]
    public partial class ChrRacesEditorControl : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabControl TabControl
        {
            get { return this.tbcEditor; }
        }

        public ChrRacesEditorControl()
        {
            InitializeComponent();
            tbcEditor.Appearance = TabAppearance.FlatButtons;
            tbcEditor.ItemSize = new Size(0, 1);
            tbcEditor.SizeMode = TabSizeMode.Fixed;
        }

        private void ChrRacesEditorControl_Load(object sender, EventArgs e)
        {
            //Load the dbc
            try
            {
                DbcStores.LoadRacesEditorFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            foreach (var entry in DbcStores.ChrRaces.Records)
            {
                lbMenu.Items.Add(entry.RaceNameNeutral.String);
                tbcEditor.TabPages.Add(entry.RaceNameNeutral.String);
            }

            lbMenu.SelectedIndex = 0;
        }
    }

    internal class ChrRacesEditorControlDesigner : ControlDesigner
    {
        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);

            var ctl = (this.Control as ChrRacesEditorControl).TabControl as TabControl;
            EnableDesignMode(ctl, "TabControl");
            foreach (TabPage page in ctl.TabPages) EnableDesignMode(page, page.Name);
        }
    }
}
