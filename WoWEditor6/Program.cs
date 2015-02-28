using System;
using System.IO;
using System.Reflection;
using System.Runtime;
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
            System.Windows.Forms.Application.EnableVisualStyles();
            AppDomain.CurrentDomain.UnhandledException += (args, e) => Log.Debug(e.ExceptionObject.ToString());

            var baseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? Directory.GetCurrentDirectory();
            var profilesDir = Path.Combine(baseDir, "JitProfiles");
            if (!Directory.Exists(profilesDir))
                Directory.CreateDirectory(profilesDir);

            ProfileOptimization.SetProfileRoot(profilesDir);
            ProfileOptimization.StartProfile("JitProfile.jpf");

            FontCollection.Initialize();
            Settings.KeyBindings.Initialize();

            var window = new EditorWindow();
            var context = new GxContext(window.DrawTarget);
            context.InitContext();

            // TODO: move all shader initializations somehwere?
            context.InitShaders();
    
            WorldFrame.Instance.Initialize(window.DrawTarget, context);
            WorldFrame.Instance.OnResize((int) window.RenderSize.Width, (int) window.RenderSize.Height);

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

            WorldFrame.Instance.Shutdown();
        }
    }
}
