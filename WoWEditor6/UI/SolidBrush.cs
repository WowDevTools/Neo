using SharpDX.Direct2D1;

namespace WoWEditor6.UI
{
    class SolidBrush
    {
        private SolidColorBrush mBrush;

        public void OnUpdateBrush(uint color, RenderTarget target)
        {
            mBrush?.Dispose();
            mBrush = new SolidColorBrush(target, new SharpDX.Color4(color));
        }

        public static implicit operator SolidColorBrush(SolidBrush brush)
        {
            return brush.mBrush;
        }
    }
}
