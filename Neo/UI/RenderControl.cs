using System;
using System.Windows.Forms;

namespace Neo.UI
{
    public partial class RenderControl : UserControl
    {
        public RenderControl()
        {
            InitializeComponent();
        }

        private void RenderControl_Click(object sender, EventArgs e)
        {
            Focus();
        }
    }
}
