using System;
using System.Windows.Forms;

namespace WoWEditor6.UI
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            // WM_SYSCOMMAND
            if (m.Msg == 0x0112)
            {
                // SC_KEYMENU -> menu invoked by pressing the alt key
                if (m.WParam.ToInt32() == 0xF100)
                {
                    m.Result = IntPtr.Zero;
                    return;
                }
            }

            base.WndProc(ref m);
        }
    }
}
