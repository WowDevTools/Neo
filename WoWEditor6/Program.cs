using System;
using System.Windows.Forms;
using WoWEditor6.Graphics;
using WoWEditor6.Scene;
using WoWEditor6.UI;

namespace WoWEditor6
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();

            Settings.KeyBindings.Initialize();

            var window = new MainWindow();
            var context = new GxContext(window);
            context.InitContext();
            var isClosed = false;
            window.FormClosed += (s, e) => isClosed = true;
            window.Show();

	        Application.DoEvents();

            InterfaceManager.Instance.Initialize(window, context);
            WorldFrame.Instance.Initialize(window, context);
            WorldFrame.Instance.OnResize(window.ClientSize.Width, window.ClientSize.Height);

	        InterfaceManager.Instance.Window.OnLoadFinished();

            while (isClosed == false)
            {
                context.BeginFrame();
                WorldFrame.Instance.OnFrame();
                InterfaceManager.Instance.OnFrame();
                context.EndFrame();
                Application.DoEvents();
            }

            WorldFrame.Instance.Shutdown();
        }
    }
}
