using SharpDX;
using SharpDX.Direct2D1;
using WoWEditor6.UI.Components;

namespace WoWEditor6.UI.Views
{
    class WorldView : IView
    {
        private readonly PerformanceControl mPerfControl = new PerformanceControl();

        public void OnRender(RenderTarget target)
        {
#if !DEBUG
            mPerfControl.OnRender(target);
#endif
        }

        public void OnMessage(Message message)
        {

        }

        public void OnResize(Vector2 newSize)
        {
            mPerfControl.Position = new Vector2(newSize.X - 350, 30.0f);
            mPerfControl.Size = new Vector2(300, 280);
        }

        public void OnShow()
        {
            mPerfControl.Reset();
        }
    }
}
