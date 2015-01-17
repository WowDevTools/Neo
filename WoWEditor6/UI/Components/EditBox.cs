using System;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System.Linq;

namespace WoWEditor6.UI.Components
{
    class EditBox : IComponent
    {
        private readonly StaticText mTextDraw;
        private readonly StaticText mFullTextDraw;
        private string mText;
        private int mCaretPosition;
        private float mCaretOffset;
        private float mStartPosition;
        private Vector2 mSize = new Vector2(200.0f, 20.0f);
        private DateTime mLastBlinkTime = new DateTime();
        private bool mCaretVisible;

        public string Text { get { return mFullTextDraw.Text; } set { SetNewText(value); } }

        public EditBox()
        {
            mTextDraw = new StaticText
            {
                Size = new Size2F(float.MaxValue, 20.0f),
                VerticalAlignment = ParagraphAlignment.Center,
                FontSize = 13.0f
            };

            mFullTextDraw = new StaticText
            {
                Size = new Size2F(float.MaxValue, 20.0f),
                VerticalAlignment = ParagraphAlignment.Center,
                FontSize = 13.0f
            };

            SetNewText("Das ist ein langer Test, dieser Test ist noch länger!");
        }

        public void OnRender(RenderTarget target)
        {
            UpdateCaret();

            target.FillRectangle(new RectangleF(5, 5, mSize.X, mSize.Y), Brushes.Solid[0xCC333333]);
            target.PushAxisAlignedClip(new RectangleF(7, 5, mSize.X - 4, mSize.Y), AntialiasMode.Aliased);
            target.DrawTextLayout(new Vector2(5 + mStartPosition, 5), mTextDraw, Brushes.Solid[0xFFFFFFFF]);
            target.PopAxisAlignedClip();
            if (mCaretVisible)
            {
                target.DrawLine(new Vector2(5 + mCaretOffset, 7), new Vector2(5 + mCaretOffset, 23),
                    Brushes.Solid[0xFFFFFFFF], 2.0f);
            }
        }

        public void OnMessage(Message message)
        {
            if (message.IsHandled)
                return;

            var keyMsg = message as KeyboardMessage;
            if (keyMsg?.Type == MessageType.KeyDown)
                OnKeyDown(keyMsg);
        }

        private void OnKeyDown(KeyboardMessage msg)
        {
            if(msg.Character == '\b')
            {
                EraseCharacter();
                return;
            }

            switch (msg.KeyCode)
            {
                case System.Windows.Forms.Keys.Left:
                    MoveCaretLeft();
                    return;
                case System.Windows.Forms.Keys.Right:
                    MoveCaretRight();
                    return;

                case System.Windows.Forms.Keys.Home:
                    MoveCaretToStart();
                    return;

                case System.Windows.Forms.Keys.End:
                    MoveCaretToEnd();
                    return;
            }

            if ((char.IsWhiteSpace(msg.Character) && msg.Character != ' ') || char.IsControl(msg.Character))
                return;

            AppendCharacter(msg.Character);
        }

        private void UpdateCaret()
        {
            var diff = DateTime.Now - mLastBlinkTime;
            if(mCaretVisible)
            {
                if (!(diff.TotalMilliseconds > 650)) return;
                mCaretVisible = false;
                mLastBlinkTime = DateTime.Now;
            }
            else
            {
                if (!(diff.TotalMilliseconds > 500)) return;
                mCaretVisible = true;
                mLastBlinkTime = DateTime.Now;
            }
        }

        private void EraseCharacter()
        {
            if (mCaretPosition == 0 || mText.Length == 0)
                return;

            mCaretVisible = true;
            mLastBlinkTime = DateTime.Now;
            var layout = (TextLayout)mFullTextDraw;
            var metrics = layout.GetClusterMetrics();
            mCaretPosition -= 1;
            mText = mText.Remove(mCaretPosition, 1);
            mFullTextDraw.Text = mText;
            layout = mFullTextDraw;

            if ((mCaretPosition == mText.Length && layout.Metrics.WidthIncludingTrailingWhitespace >= mSize.X - 4))
            {
                OnCaretAtEnd();
                return;
            }

            mCaretOffset -= metrics[mCaretPosition].Width;

            if (layout.Metrics.WidthIncludingTrailingWhitespace < mSize.X - 4)
            {
                BuildTextFit();
                return;
            }

            if(mCaretOffset <= 2 || mCaretPosition == 0)
            {
                OnCaretAtStart();
                return;
            }

            var curOffset = mCaretOffset;
            var i = mCaretPosition;
            var fullText = "";
            while(curOffset < mSize.X - 2 && i < mText.Length)
            {
                fullText += mText[i];
                curOffset += metrics[i].Width;
                ++i;
            }

            if(i == mText.Length)
            {
                BuildTextFromEnd();
                return;
            }

            i = mCaretPosition - 1;
            curOffset = mCaretOffset;
            while(curOffset > -20 && i >= 0)
            {
                fullText = mText[i] + fullText;
                curOffset -= metrics[i].Width;
                --i;
            }

            mStartPosition = curOffset;
            mTextDraw.Text = fullText;
        }

        private void SetNewText(string text)
        {
            mText = text;
            mCaretPosition = text.Length;
            mFullTextDraw.Text = text;

            OnCaretAtEnd();
        }

        private void AppendCharacter(char c)
        {
            mCaretVisible = true;
            mLastBlinkTime = DateTime.Now;

            mText = mText.Insert(mCaretPosition, c.ToString());
            mCaretPosition += 1;
            mFullTextDraw.Text = mText;
            var layout = (TextLayout)mFullTextDraw;
            var metrics = layout.GetClusterMetrics();
            mCaretOffset += metrics[mCaretPosition - 1].Width;

            if((mCaretPosition == mText.Length && layout.Metrics.WidthIncludingTrailingWhitespace >= mSize.X - 4) || mCaretOffset > mSize.X - 2)
            {
                OnCaretAtEnd();
                return;
            }

            if (layout.Metrics.WidthIncludingTrailingWhitespace < mSize.X - 4)
            {
                BuildTextFit();
                return;
            }

            var curOffset = mCaretOffset;
            var i = mCaretPosition;
            var fullText = "";
            while (curOffset < mSize.X - 2 && i < mText.Length)
            {
                fullText += mText[i];
                curOffset += metrics[i].Width;
                ++i;
            }

            i = mCaretPosition - 1;
            curOffset = mCaretOffset;
            while (curOffset > -20 && i >= 0)
            {
                fullText = mText[i] + fullText;
                curOffset -= metrics[i].Width;
                --i;
            }

            mStartPosition = curOffset;
            mTextDraw.Text = fullText;
        }

        private void MoveCaretLeft()
        {
            if (mCaretPosition == 0)
                return;

            mCaretVisible = true;
            mLastBlinkTime = DateTime.Now;

            var layout = (TextLayout)mFullTextDraw;
            var metrics = layout.GetClusterMetrics();
            mCaretPosition -= 1;
            mCaretOffset -= metrics[mCaretPosition].Width;
            if ((mCaretOffset >= 2)) return;

            OnCaretAtStart();
        }

        private void MoveCaretRight()
        {
            if (mCaretPosition >= mText.Length)
                return;

            mCaretVisible = true;
            mLastBlinkTime = DateTime.Now;

            mCaretPosition += 1;

            var layout = (TextLayout)mFullTextDraw;
            var metrics = layout.GetClusterMetrics();
            mCaretOffset += metrics[mCaretPosition - 1].Width;
            if (mCaretOffset <= mSize.X - 2)
                return;

            OnCaretAtEnd();
        }

        private void OnCaretAtEnd()
        {
            var layout = (TextLayout)mFullTextDraw;
            var metrics = layout.GetClusterMetrics();
            var curOffset = mSize.X - 2;
            var finalText = "";
            var i = mCaretPosition - 1;
            while (curOffset >= -20 && i >= 0)
            {
                finalText = mText[i] + finalText;
                curOffset -= metrics[i].Width;
                --i;
            }

            mTextDraw.Text = finalText;
            mStartPosition = curOffset;
            mCaretOffset = mSize.X - 2;
        }

        private void OnCaretAtStart()
        {
            var layout = (TextLayout)mFullTextDraw;
            var metrics = layout.GetClusterMetrics();
            mCaretOffset = 2;
            mStartPosition = 2;
            var curEnd = mStartPosition;
            var i = mCaretPosition;
            var fullText = "";
            while (curEnd < mSize.X && i < mText.Length)
            {
                fullText += mText[i];
                curEnd += metrics[i].Width;
                ++i;
            }

            mTextDraw.Text = fullText;
        }

        private void MoveCaretToEnd()
        {
            mCaretVisible = true;
            mLastBlinkTime = DateTime.Now;
            mCaretPosition = mText.Length;
            OnCaretAtEnd();
        }

        private void MoveCaretToStart()
        {
            mCaretVisible = true;
            mLastBlinkTime = DateTime.Now;
            mCaretPosition = 0;
            OnCaretAtStart();
        }

        private void BuildTextFit()
        {
            mStartPosition = 2;
            var metrics = ((TextLayout) mFullTextDraw).GetClusterMetrics();
            mCaretOffset = metrics.Take(mCaretPosition).Sum(m => m.Width) + 2;
            mTextDraw.Text = mFullTextDraw.Text;
        }

        private void BuildTextFromEnd()
        {
            var metrics = ((TextLayout)mFullTextDraw).GetClusterMetrics();
            var i = mText.Length - 1;
            var fullText = "";
            var curOffset = mSize.X - 2;
            while(curOffset > -30 && i >= 0)
            {
                fullText = mText[i] + fullText;
                curOffset -= metrics[i].Width;
                --i;
            }

            mStartPosition = curOffset;
            mCaretOffset = metrics.Skip(i + 1).Take(mCaretPosition - i - 1).Sum(w => w.Width) + curOffset;
            mTextDraw.Text = fullText;
        }
    }
}
