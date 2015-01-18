using System;
using SharpDX;
using SharpDX.Direct2D1;
using WoWEditor6.UI.Components;

namespace WoWEditor6.UI.Views
{
    class FileSystemInitView : IView
    {
        private readonly Label mLoadIndicator;
        private Vector2 mSize;
        private DateTime mAnimTime;

        public FileSystemInitView()
        {
            mLoadIndicator = new Label
            {
                Color = Brushes.White,
                FontSize = 35.0f,
                Multiline = true,
                Position = new Vector2(0, 0),
                Text = "Loading files...",
                HorizontalAlignment = SharpDX.DirectWrite.TextAlignment.Center,
                VerticalAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center
            };
        }

        public void OnShow()
        {
            if(IO.FileManager.Instance.Initialized)
            {
                InterfaceManager.Instance.UpdateState(Scene.AppState.MapSelect);
                return;
            }

            mAnimTime = DateTime.Now;
            IO.FileManager.Instance.LoadComplete += () =>
                InterfaceManager.Instance.UpdateState(Scene.AppState.MapSelect);
            IO.FileManager.Instance.InitFromPath();
        }

        public void OnRender(RenderTarget target)
        {
            var msDiff = (DateTime.Now - mAnimTime).Milliseconds;
            var numDots = msDiff > 750 ? 3 : (msDiff > 500 ? 2 : (msDiff > 250 ? 1 : 0));
            var text = "Loading files";
            for (var i = 0; i < numDots; ++i)
            {
                text = '-' + text;
                text += '-';
            }

            if (mLoadIndicator.Text.Equals(text) == false)
                mLoadIndicator.Text = text;

            target.FillRectangle(new RectangleF(0, 0, mSize.X, mSize.Y), Brushes.Solid[0xFF333333]);
            mLoadIndicator.OnRender(target);
        }

        public void OnMessage(Message message)
        {

        }

        public void OnResize(Vector2 newSize)
        {
            mSize = newSize;
            mLoadIndicator.Size = newSize;
        }
    }
}
