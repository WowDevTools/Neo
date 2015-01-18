using System;
using SharpDX;
using SharpDX.DirectWrite;
using Factory = SharpDX.DirectWrite.Factory;

namespace WoWEditor6.UI
{
    class StaticText : IDisposable
    {
        private TextLayout mLayout;
        private readonly Factory mFactory;
        private bool mChanged = true;
        private string mText = string.Empty;
        private Size2F mSize = new Size2F(0, 0);
        private string mFontFamily = "Segoe UI";
        private float mFontSize = 15.0f;
        private TextAlignment mHorizontalAlignment = TextAlignment.Leading;
        private ParagraphAlignment mVerticalAlignment = ParagraphAlignment.Near;
        private FontWeight mWeight = FontWeight.Normal;
        private bool mAlignmentChanged;

        public string Text { get { return mText; } set { mText = value; mChanged = true; } }
        public Size2F Size { get { return mSize; } set { mSize = value; mChanged = true; } }
        public string FontFamily { get { return mFontFamily; } set { mFontFamily = value; mChanged = true; } }
        public float FontSize { get { return mFontSize; } set { mFontSize = value; mChanged = true; } }
        public FontWeight Weight { get { return mWeight; } set { mWeight = value; mChanged = true; } }
        public TextAlignment HorizontalAlignment
        {
            get { return mHorizontalAlignment; }
            set
            {
                mHorizontalAlignment = value;
                mAlignmentChanged = true;
            }
        }
        public ParagraphAlignment VerticalAlignment
        {
            get { return mVerticalAlignment; }
            set
            {
                mVerticalAlignment = value;
                mAlignmentChanged = true;
            }
        }

        public StaticText()
        {
            mFactory = InterfaceManager.Instance.Surface.DirectWriteFactory;
        }

        public TextLayout GetLayout()
        {
            if(mAlignmentChanged)
            {
                if (mLayout != null)
                {
                    mLayout.TextAlignment = mHorizontalAlignment;
                    mLayout.ParagraphAlignment = mVerticalAlignment;
                }
                mAlignmentChanged = false;
            }

            if (mChanged == false)
                return mLayout;

            var font = Fonts.Cache[mFontFamily, mFontSize, mWeight];

            mLayout?.Dispose();
            mLayout = new TextLayout(mFactory, mText, font, mSize.Width,
                mSize.Height)
            {
                TextAlignment = mHorizontalAlignment,
                ParagraphAlignment = mVerticalAlignment,
            };

            mChanged = false;
            mAlignmentChanged = false;
            return mLayout;
        }

        public void Dispose()
        {
            mLayout?.Dispose();
        }

        public static implicit operator TextLayout(StaticText text)
        {
            return text.GetLayout();
        }
    }
}
