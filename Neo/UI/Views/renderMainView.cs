using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WoWEditor6.UI.Views
{
    public partial class renderMainView : UserControl
    {
        private readonly MainWindow mWindow;
        public renderMainView(MainWindow window)
        {
            this.mWindow = window;
            InitializeComponent();
            Dock = DockStyle.Fill;
        }
    }
}
