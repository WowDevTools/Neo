using System;
using System.Windows.Threading;
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

            FontCollection.Initialize();
            Settings.KeyBindings.Initialize();

            var window = new EditorWindow();
            var context = new GxContext(window.DrawTarget);
            context.InitContext();

            // TODO: move all shader initializations somehwere?
            context.InitShaders();
    
            WorldFrame.Instance.Initialize(window.DrawTarget, context);
            WorldFrame.Instance.OnResize((int) window.RenderSize.Width, (int) window.RenderSize.Height);

            var app = new Application();
            var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(10), DispatcherPriority.Send,
                (sender, args) =>
                {
                    context.BeginFrame();
                    WorldFrame.Instance.OnFrame();
                    context.EndFrame();
                }, app.Dispatcher);
            
            app.Run(window);

            WorldFrame.Instance.Shutdown();
        }
    }
}
