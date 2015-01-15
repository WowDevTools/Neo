using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WoWEditor6
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new UI.MainWindow();
            var context = new Graphics.GxContext(window);
            context.InitContext();
            var isClosed = false;
            window.FormClosed += (s, e) => isClosed = true;
            window.Show();

            while(isClosed == false)
            {
                context.BeginFrame();
                context.EndFrame();
                Application.DoEvents();
            }
        }
    }
}
