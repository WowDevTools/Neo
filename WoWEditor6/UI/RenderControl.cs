using System;
using System.Windows.Forms;

namespace WoWEditor6.UI
{
    public partial class RenderControl : UserControl
    {
        public RenderControl()
        {
            InitializeComponent();
        }

        public void OnLoadFinished()
        {
            label1.Visible = false;
        }

        private void RenderControl_Click(object sender, EventArgs e)
        {
            Focus();
        }
    }
}
