using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WoWEditor6.UI.Dialogs
{
    public partial class TerrainSettings : Form
    {
        public TerrainSettings()
        {
            InitializeComponent();
        }

        private void TerrainSettings_Load(object sender, EventArgs e)
        {
            ClientSize = new Size(527, 320);
            var terrainSettings = new TerrainSettingsImpl();
            elementHost1.Child = terrainSettings;
            //terrainSettings.ColorChanged += (clr) =>
            //    colorPreviewPanel.BackColor = Color.FromArgb(0xFF, clr.R, clr.G, clr.B);
        }
    }
}
