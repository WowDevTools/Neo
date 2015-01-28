using SharpDX;
using SharpDX.Direct2D1;
using WoWEditor6.UI.Components;

namespace WoWEditor6.UI.Views
{
    class WorldView : IView
    {
        private readonly PerformanceControl mPerfControl = new PerformanceControl();

        private readonly Toolbar mTopToolbar = new Toolbar();
        private readonly Toolbar mLeftToolbar = new Toolbar();
        private bool mToolbarInitialized;

        public void OnRender(RenderTarget target)
        {
#if DEBUG
            mPerfControl.OnRender(target);
#endif
            mTopToolbar.OnRender(target);
            mLeftToolbar.OnRender(target);
        }

        public void OnMessage(Message message)
        {
            mTopToolbar.OnMessage(message);
            mLeftToolbar.OnMessage(message);
        }

        public void OnResize(Vector2 newSize)
        {
            mPerfControl.Position = new Vector2(newSize.X - 350, 30.0f);
            mPerfControl.Size = new Vector2(300, 280);
            mTopToolbar.Position = new Vector2(0, 0);
            mTopToolbar.Size = new Vector2(newSize.X, 56);
            mLeftToolbar.Position = new Vector2(0, 56.0f);
            mLeftToolbar.Size = new Vector2(66.0f, newSize.Y - 56.0f);
        }

        public void OnShow()
        {
            if (mToolbarInitialized == false)
                InitToolbars();

            mPerfControl.Reset();
        }

        private void InitToolbars()
        {
            mToolbarInitialized = false;
            mTopToolbar.Buttons.AddRange(new[]
                {
                    new ImageButton {Image = @"Interface\Icons\Ability_Earthen_Pillar.blp"}
                }
           );

            mTopToolbar.BorderOffsets = new Vector2(66.0f, 0.0f);
            mLeftToolbar.Orientation = ToolbarOrientation.Vertical;
        }
    }
}
