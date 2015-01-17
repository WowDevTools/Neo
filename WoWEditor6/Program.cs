using System.Windows.Forms;
using WoWEditor6.Graphics;
using WoWEditor6.UI;

namespace WoWEditor6
{
    class Program
    {
        static void Main()
        {
            var window = new MainWindow();
            var context = new GxContext(window);
            context.InitContext();
            var isClosed = false;
            window.FormClosed += (s, e) => isClosed = true;
            window.Show();

            InterfaceManager.Instance.Initialize(window, context);

            while (isClosed == false)
            {
                context.BeginFrame();
                InterfaceManager.Instance.OnFrame();
                context.EndFrame();
                Application.DoEvents();
            }
        }
    }
}
