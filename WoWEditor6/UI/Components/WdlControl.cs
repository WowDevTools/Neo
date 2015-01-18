using System;
using SharpDX;
using SharpDX.Direct2D1;

namespace WoWEditor6.UI.Components
{
    class WdlControl : IComponent
    {
        private const int Width = 64 * 17;
        private const int Height = 64 * 17;

        private IO.Files.Terrain.WdlFile mWdlFile;
        private Bitmap mImage;
        private Vector2 mPosition;
        private Vector2 mSize;
        private RectangleF mTargetRectangle;
        private RectangleF mSourceRectangle = new RectangleF(0, 0, Width, Height);
        private float mZoomFactor = 0.5f;
        private Vector2 mCenterPos = new Vector2(0.5f, 0.5f);
        private bool mIsClicked;
        private Vector2 mLastMousePos;
        private readonly StaticText mIndexDraw;
        private int mIndexLocation = 0;

        public Vector2 Position { get { return mPosition; } set { ClientAreaChanged(value, mSize); } }
        public Vector2 Size { get { return mSize; } set { ClientAreaChanged(mPosition, value); } }

        public WdlControl()
        {
            mIndexDraw = new StaticText
            {
                FontFamily = "Segoe UI",
                FontSize = 18.0f,
                Weight = SharpDX.DirectWrite.FontWeight.Bold,
                Text = "Selected ADT: 0/0"
            };
        }

        public void UpdateMap(string mapName)
        {
            mWdlFile = new IO.Files.Terrain.WdlFile();
            mWdlFile.Load(mapName);
            mImage?.Dispose();

            var textureData = new uint[Width * Height];
            for(var i = 0; i < 64; ++i)
            {
                for(var j = 0; j < 64; ++j)
                {
                    if (mWdlFile.HasEntry(j, i) == false)
                        continue;

                    var entry = mWdlFile.GetEntry(j, i);
                    LoadEntry(entry, textureData, ref i, ref j);
                }
            }

            var bmpProps =
                new BitmapProperties(new PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Ignore));

            using (var dataStream = new DataStream(Width * Height * 4, true, true))
            {
                dataStream.WriteRange(textureData);
                dataStream.Position = 0;

                mImage = new Bitmap(InterfaceManager.Instance.Surface.RenderTarget, new Size2(Width, Height),
                    new DataPointer(dataStream.DataPointer, Width * Height * 4), Width * 4, bmpProps);
            }
        }

        public void OnRender(RenderTarget target)
        {
            if (mImage == null)
                return;

            target.DrawBitmap(mImage, mTargetRectangle, 1.0f, BitmapInterpolationMode.Linear, mSourceRectangle);
            if (mIndexLocation == 0)
            {
                target.FillRoundedRectangle(new RoundedRectangle
                {
                    RadiusX = 5,
                    RadiusY = 5,
                    Rect = new RectangleF(Position.X + 5, Position.Y + 2, mIndexDraw.GetLayout().Metrics.Width + 10, mIndexDraw.GetLayout().Metrics.Height + 4)
                }, Brushes.Solid[0xDD555555]);
            }
            else
            {
                target.FillRoundedRectangle(new RoundedRectangle
                {
                    RadiusX = 5,
                    RadiusY = 5,
                    Rect = new RectangleF(Position.X + mTargetRectangle.Width - mIndexDraw.GetLayout().Metrics.Width - 15, Position.Y + 2, mIndexDraw.GetLayout().Metrics.Width + 10, mIndexDraw.GetLayout().Metrics.Height + 4)
                }, Brushes.Solid[0xDD555555]);
            }

            target.DrawTextLayout(new Vector2(mPosition.X + 10, mPosition.Y + 5), mIndexDraw, Brushes.White);
        }

        public void OnMessage(Message message)
        {
            if (message.IsHandled)
                return;

            var msg = message as MouseMessage;
            if (msg == null)
                return;

            switch (msg.Type)
            {
                case MessageType.MouseWheel:
                    HandleMouseWheel(msg);
                    break;

                case MessageType.MouseDown:
                    HandleMouseDown(msg);
                    break;

                case MessageType.MouseUp:
                    HandleMouseUp(msg);
                    break;

                case MessageType.MouseMove:
                    HandleMouseMove(msg);
                    break;
            }
        }

        private void HandleMouseDown(MouseMessage msg)
        {
            if (msg.Buttons != MouseButton.Right)
                return;

            mIsClicked = mTargetRectangle.Contains(msg.Position);
            mLastMousePos = msg.Position;
        }

        private void HandleMouseUp(MouseMessage msg)
        {
            if (msg.Buttons != MouseButton.Right)
                return;

            mIsClicked = false;
        }

        private void HandleMouseMove(MouseMessage msg)
        {
            if(mIsClicked)
            {
                var diff = msg.Position - mLastMousePos;
                mCenterPos.X -= diff.X / 300.0f * mZoomFactor;
                mCenterPos.Y -= diff.Y / 300.0f * mZoomFactor;
                mCenterPos.X = Math.Max(mZoomFactor, Math.Min(1.0f - mZoomFactor, mCenterPos.X));
                mCenterPos.Y = Math.Max(mZoomFactor, Math.Min(1.0f - mZoomFactor, mCenterPos.Y));
                var leftPos = mCenterPos.X - mZoomFactor;
                var topPos = mCenterPos.Y - mZoomFactor;

                mSourceRectangle = new RectangleF(leftPos * Width, topPos * Height, Width * 2 * mZoomFactor, Height * 2 * mZoomFactor);
            }

            if (mTargetRectangle.Contains(msg.Position))
            {
                var facx = (msg.Position.X - mTargetRectangle.X) / mTargetRectangle.Width;
                var facy = (msg.Position.Y - mTargetRectangle.Y) / mTargetRectangle.Height;

                var sourcex = mSourceRectangle.Left + facx * mSourceRectangle.Width;
                var sourcey = mSourceRectangle.Top + facy * mSourceRectangle.Height;

                sourcex /= Width;
                sourcey /= Height;

                sourcex *= 64.0f;
                sourcey *= 64.0f;
                sourcex = Math.Max(0, Math.Min(63, sourcex));
                sourcey = Math.Max(0, Math.Min(63, sourcey));

                mIndexDraw.Text = "Selected ADT: " + (int)Math.Floor(sourcex) + "/" + (int)Math.Floor(sourcey);

                CheckIndexLocation(msg.Position);
            }

            mLastMousePos = msg.Position;
        }

        private void HandleMouseWheel(MouseMessage msg)
        {
            if (msg.Delta > 0)
                mZoomFactor *= 0.8f;
            else
                mZoomFactor /= 0.8f;

            mZoomFactor = Math.Min(0.5f, mZoomFactor);
            mCenterPos.X = Math.Max(mZoomFactor, Math.Min(1.0f - mZoomFactor, mCenterPos.X));
            mCenterPos.Y = Math.Max(mZoomFactor, Math.Min(1.0f - mZoomFactor, mCenterPos.Y));
            var leftPos = mCenterPos.X - mZoomFactor;
            var topPos = mCenterPos.Y - mZoomFactor;

            mSourceRectangle = new RectangleF(leftPos * Width, topPos * Height, Width * 2 * mZoomFactor, Height * 2 * mZoomFactor);
        }

        private void ClientAreaChanged(Vector2 position, Vector2 size)
        {
            mPosition = position;
            mSize = size;

            var facx = mSize.X / Width;
            var facy = mSize.Y / Height;
            var fac = Math.Min(facx, facy);

            mTargetRectangle = new RectangleF(mPosition.X, mPosition.Y, Width * fac, Height * fac);

            mIndexDraw.Size = new Size2F(mTargetRectangle.Width - 20, mTargetRectangle.Height - 10);
        }

        private void CheckIndexLocation(Vector2 position)
        {
            var ofsx = position.X - mPosition.X;
            var ofsy = position.Y - mPosition.Y;

            var metrics = mIndexDraw.GetLayout().Metrics;

            switch(mIndexLocation)
            {
                case 0:
                    {
                        if (ofsx > 5 && ofsx < 10 + metrics.Width && ofsy > 5 && ofsy < 5 + metrics.Height)
                        {
                            mIndexLocation = 1;
                            mIndexDraw.HorizontalAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
                        }
                    }
                    break;

                case 1:
                    {
                        if (ofsx < mTargetRectangle.Width - 5 && ofsx > mTargetRectangle.Width - 10 - metrics.Width && ofsy > 5 && ofsy < 5 + metrics.Height)
                        {
                            mIndexLocation = 0;
                            mIndexDraw.HorizontalAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
                        }
                    }
                    break;
            }
        }

        private static unsafe void LoadEntry(IO.Files.Terrain.MareEntry entry, uint[] textureData, ref int i, ref int j)
        {
            for (var k = 0; k < 17; ++k)
            {
                for (var l = 0; l < 17; ++l)
                {
                    uint r;
                    uint g;
                    uint b;
                    var h = entry.outer[k * 17 + l];
                    if (h > 2000)
                    {
                        r = g = b = 255;
                    }
                    else if (h > 1000)
                    {
                        var am = (h - 1000) / 1000.0f;
                        r = (uint)(0.75f + am * 0.25f * 255);
                        g = (uint)(0.5f * am * 255);
                        b = (uint)(0.75f + am * 0.5f * 255);
                    }
                    else if (h > 600)
                    {
                        var am = (h - 600) / 400.0f;
                        r = (uint)(0.75 + am * 0.25f * 255);
                        g = (uint)(0.5f * am * 255);
                        b = (uint)(am * 255);
                    }
                    else if (h > 300)
                    {
                        var am = (h - 300) / 300.0f;
                        r = (uint)(255 - am * 255);
                        g = 1;
                        b = 0;
                    }
                    else if (h > 0)
                    {
                        var am = h / 300.0f;
                        r = (uint)(0.75 * am * 255);
                        g = (uint)(255 - (0.5f * am * 255));
                        b = 0;
                    }
                    else if (h > -100)
                    {
                        var am = (h + 100.0f) / 100.0f;
                        r = (uint)(0.0f);
                        g = (uint)(am * 127);
                        b = 200;
                    }
                    else
                    {
                        r = g = 0;
                        b = 0x2F;
                    }

                    if (k == 0 || l == 0)
                        r = g = b = 0;

                    textureData[(i * 17 + k) * (64 * 17) + j * 17 + l] = 0xFF000000 | (r << 16) | (g << 8) | (b << 0);
                }
            }
        }
    }
}
