using SharpDX;
using SharpDX.Direct2D1;
using WoWEditor6.Resources;
using WoWEditor6.UI.Components;

namespace WoWEditor6.UI.Views
{
    class SplashView : IView
    {
        private Vector2 mSize;

        private readonly Label mAppTitle;
        private readonly Label mDescription;

        public SplashView()
        {
            mAppTitle = new Label
            {
                Color = Brushes.White,
                Position = new Vector2(30, 30),
                Text = Strings.SplashView_AppTitle,
                Size = new Vector2(float.MaxValue, 30.0f),
                FontSize = 25.0f
            };

            mDescription = new Label
            {
                Color = Brushes.White,
                Position = new Vector2(30, 70),
                Text = Strings.SplashView_AppDescription,
                FontSize = 20.0f,
                Multiline = true
            };
        }

        public void OnRender(RenderTarget target)
        {
            target.FillRectangle(new RectangleF(0, 0, mSize.X, mSize.Y), Brushes.Solid[0xFF333333]);
            mAppTitle.OnRender(target);
            mDescription.OnRender(target);
        }

        public void OnMessage(Message message)
        {

        }

        public void OnResize(Vector2 newSize)
        {
            mSize = newSize;
            mDescription.Size = new Vector2(newSize.X - 60, float.MaxValue);
        }
    }
}
