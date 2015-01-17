using System.Linq;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct2D1;

namespace WoWEditor6.UI.Components
{
    class Frame : IComponent
    {
        private static SolidBrush gBackground;
        private static SolidBrush gBorder;
        private static SolidBrush gCaptionText;

        private Vector2 mPosition;
        private Vector2 mSize;
        private RectangleF mClientRectangle;
        private Matrix3x2 mTransform;
        private RectangleF mClipRect;
        private RectangleF mCaptionRect;
        private readonly StaticText mCaptionText;

        public bool HasCaption { get; set; } = false;

        public string Text { get { return mCaptionText.Text; } set { mCaptionText.Text = value; } }

        public List<IComponent> Children { get; } = new List<IComponent>();

        public Vector2 Position
        {
            get { return mPosition; }
            set
            {
                mPosition = value;
                ClientAreaChanged(mPosition, mSize);
            }
        }

        public Vector2 Size
        {
            get { return mSize; }
            set
            {
                mSize = value;
                ClientAreaChanged(mPosition, mSize);
            }
        }

        public Frame()
        {
            mCaptionText = new StaticText
            {
                FontFamily = "Segoe UI",
                FontSize = 12.0f,
                Weight = SharpDX.DirectWrite.FontWeight.Bold,
                VerticalAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center
            };

            Position = new Vector2(0, 0);
            Size = new Vector2(2, 2);
        }

        public void OnRender(RenderTarget target)
        {
            target.AntialiasMode = AntialiasMode.Aliased;
            target.FillRectangle(mClientRectangle, gBackground);
            target.DrawRectangle(mClientRectangle, gBorder);

            if (HasCaption)
            {
                target.FillRectangle(mCaptionRect, gBorder);
                target.DrawTextLayout(new Vector2(Position.X + 5, Position.Y - 18.0f), mCaptionText, gCaptionText);
            }

            var oldTransform = target.Transform;
            target.Transform *= mTransform;

            target.PushAxisAlignedClip(mClipRect, AntialiasMode.Aliased);

            lock(Children)
                foreach (var child in Children) child.OnRender(target);

            target.PopAxisAlignedClip();

            target.Transform = oldTransform;
        }

        public void OnMessage(Message message)
        {
            var msg = message.Forward(Position);
            lock(Children)
            {
                var copy = Children.ToList();
                for (var i = copy.Count - 1; i >= 0; --i)
                    copy[i].OnMessage(msg);
            }

            if (msg.IsHandled)
                message.IsHandled = true;
            else
            {
                var mouseMsg = message as MouseMessage;
                if (mouseMsg != null)
                    message.IsHandled = mClientRectangle.Contains(mouseMsg.Position);
            }
        }

        private void ClientAreaChanged(Vector2 position, Vector2 size)
        {
            mClientRectangle = new RectangleF(position.X, position.Y, size.X, size.Y);
            mCaptionRect = new RectangleF(position.X - 1, position.Y - 18, size.X + 1, 18.0f);
            mClipRect = new RectangleF(1, 1, size.X - 2, size.Y - 2);
            mTransform = Matrix3x2.Translation(position);

            mCaptionText.Size = new Size2F(size.X - 6, 18.0f);
        }

        public static void Initialize()
        {
            gBackground = Brushes.Solid[0xCC444444];
            gBorder = Brushes.Solid[0xFFFF7F00];
            gCaptionText = Brushes.Solid[System.Drawing.Color.White];
        }
    }
}
