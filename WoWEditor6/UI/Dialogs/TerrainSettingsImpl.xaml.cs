using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WoWEditor6.Utils;

namespace WoWEditor6.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for TerrainSettingsImpl.xaml
    /// </summary>
    public partial class TerrainSettingsImpl
    {
        private bool mIsMouseDown;
        private bool mIsDetailMouseDown;
        private float mCurHue;

        public event Action<Color> ColorChanged;

        public TerrainSettingsImpl()
        {
            InitializeComponent();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as UIElement)?.CaptureMouse();
            mIsMouseDown = true;
            var rc = sender as Rectangle;
            if (rc == null)
                return;

            var pos = e.GetPosition(rc);
            RightSlider.Points = new PointCollection
            {
                new Point(25, pos.Y),
                new Point(30, pos.Y - 5),
                new Point(30, pos.Y + 5)
            };

            LeftSlider.Points = new PointCollection
            {
                new Point(0, pos.Y - 5),
                new Point(5, pos.Y),
                new Point(0, pos.Y + 5)
            };

            SliderLine.Points = new PointCollection
            {
                new Point(5, pos.Y),
                new Point(25, pos.Y)
            };

            mCurHue = (float)(pos.Y / rc.RenderSize.Height) * 360.0f;

            var gradient = DetailGradient.Fill as LinearGradientBrush;
            if (gradient == null)
                return;

            int r, g, b;
            ColorUtils.HsvToRgb((pos.Y / rc.RenderSize.Height) * 360.0, 1, 1, out r, out g, out b);
            gradient.GradientStops = new GradientStopCollection
            {
                new GradientStop(Color.FromRgb(255, 255, 255), 0.0),
                new GradientStop(Color.FromRgb((byte)r, (byte)g, (byte)b), 1.0)
            };

            UpdateColor();
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            (sender as UIElement)?.ReleaseMouseCapture();
            mIsMouseDown = false;
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (mIsMouseDown == false)
                return;

            var rc = sender as Rectangle;
            if (rc == null)
                return;

            var pos = e.GetPosition(rc);
            if (pos.Y < 0 || pos.Y > rc.RenderSize.Height)
                return;

            RightSlider.Points = new PointCollection
            {
                new Point(25, pos.Y),
                new Point(30, pos.Y - 5),
                new Point(30, pos.Y + 5)
            };

            LeftSlider.Points = new PointCollection
            {
                new Point(0, pos.Y - 5),
                new Point(5, pos.Y),
                new Point(0, pos.Y + 5)
            };

            SliderLine.Points = new PointCollection
            {
                new Point(5, pos.Y),
                new Point(25, pos.Y)
            };

            mCurHue = (float)(pos.Y / rc.RenderSize.Height) * 360.0f;

            var gradient = DetailGradient.Fill as LinearGradientBrush;
            if (gradient == null)
                return;

            int r, g, b;
            ColorUtils.HsvToRgb((pos.Y / rc.RenderSize.Height) * 360.0, 1, 1, out r, out g, out b);
            gradient.GradientStops = new GradientStopCollection
            {
                new GradientStop(Color.FromRgb(255, 255, 255), 0.0),
                new GradientStop(Color.FromRgb((byte)r, (byte)g, (byte)b), 1.0)
            };

            UpdateColor();
        }

        private void DetailGradient_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as UIElement)?.CaptureMouse();
            mIsDetailMouseDown = true;
        }

        private void DetailGradient_MouseUp(object sender, MouseButtonEventArgs e)
        {
            (sender as UIElement)?.ReleaseMouseCapture();
            mIsDetailMouseDown = false;
        }

        private void DetailGradient_MouseMove(object sender, MouseEventArgs e)
        {
            if (mIsDetailMouseDown == false)
                return;

            var pos = e.GetPosition(DetailGradient);
            if (pos.X < 0 || pos.Y < 0 || pos.X >= DetailGradient.RenderSize.Width ||
                pos.Y >= DetailGradient.RenderSize.Height)
                return;

            Canvas.SetLeft(ColorEllipse, pos.X);
            Canvas.SetTop(ColorEllipse, pos.Y);

            UpdateColor();
        }

        private void UpdateColor()
        {
            var left = Canvas.GetLeft(ColorEllipse);
            var top = Canvas.GetTop(ColorEllipse);

            var h = mCurHue;
            var s = left / DetailGradient.RenderSize.Width;
            var v = top / DetailGradient.RenderSize.Height;

            int r, g, b;
            ColorUtils.HsvToRgb(h, s, 1.0 - v, out r, out g, out b);
            ColorChanged?.Invoke(Color.FromRgb((byte)r, (byte)g, (byte)b));
        }
    }
}
