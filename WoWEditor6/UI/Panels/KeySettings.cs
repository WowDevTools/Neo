using SharpDX;
using SharpDX.Direct2D1;
using WoWEditor6.Scene;
using WoWEditor6.UI.Components;

namespace WoWEditor6.UI.Panels
{
    class KeySettings
    {
        private readonly Frame mFrame;

        public bool Visible { get; set; }

        public KeySettings()
        {
            var invertMouseBox = new Checkbox
            {
                Position = new Vector2(5, 5),
                Checked = true,
                Size = 16.0f,
                Text = "Invert mouse"
            };

            invertMouseBox.CheckChanged += (box, check) => WorldFrame.Instance.CamControl.Invert = check;

            mFrame = new Frame
            {
                HasCaption = false,
                Size = new Vector2(300, 250),
                Position = new Vector2(80.0f, 100.0f),
                Children =
                {
                    invertMouseBox
                }
            };
        }

        public void OnResize(Vector2 newSize)
        {

        }

        public void OnRender(RenderTarget target)
        {
            if (Visible == false)
                return;

            mFrame.OnRender(target);
        }

        public void OnMessage(Message message)
        {
            mFrame.OnMessage(message);
        }
    }
}
