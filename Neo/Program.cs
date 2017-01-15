using System;
using System.IO;
using System.Reflection;
using Neo.Graphics;
using Neo.Scene;
using Neo.UI;
using Neo.UI.Services;

namespace Neo
{
	internal class Program
    {
        [STAThread]
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (args, e) => Log.Debug(e.ExceptionObject.ToString());

            string baseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? Directory.GetCurrentDirectory();

	        /*
			var profilesDir = Path.Combine(baseDir, "JitProfiles");
	        if (!Directory.Exists(profilesDir))
	        {
		        Directory.CreateDirectory(profilesDir);
	        }
			*/

            string dbcdir = Path.Combine(baseDir, "DBC");
	        if (!Directory.Exists(dbcdir))
	        {
		        Directory.CreateDirectory(dbcdir);
	        }

            FontCollection.Initialize();
            Settings.KeyBindings.Initialize();
            XmlService.Initialize();

            //var window = new EditorWindow();
            //var context = new GxContext(window.DrawTarget);
            //context.InitContext();

            // TODO: move all shader initializations somehwere?
            //context.InitShaders();

            //WorldFrame.Instance.Initialize(window.DrawTarget, context);
            //WorldFrame.Instance.OnResize((int) window.RenderSize.Width, (int) window.RenderSize.Height);

	        /*
            var wnd = new MainWindow {elementHost1 = {Child = window}};
            wnd.Show();
            var isClosed = false;
            wnd.FormClosing += (sender, args) => isClosed = true;

            while (isClosed == false)
            {
                context.BeginFrame();
                WorldFrame.Instance.OnFrame();
                context.EndFrame();
                System.Windows.Forms.Application.DoEvents();
            }
			*/

            //WorldFrame.Instance.Shutdown();
        }
    }
}
