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
    public partial class ColorPickerControl
    {
        private bool mIsMouseDown;
        private bool mIsDetailMouseDown;
        private float mCurHue;

        public event Action<Color> ColorChanged;

        public ColorPickerControl()
        {
            InitializeComponent();
        }

        public void SetColor(Color color)
        {
            var h = System.Drawing.Color.FromArgb(255, color.R, color.G, color.B).GetHue();
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            var s = (max == 0) ? 0 : 1.0f - ((float) min / max);
            var v = max / 255.0f;

            Canvas.SetLeft(ColorEllipse, s * DetailGradient.RenderSize.Width - 5);
            Canvas.SetTop(ColorEllipse, (1 - v) * DetailGradient.RenderSize.Height - 5);

            var pos = new Point(0, h * SpectrumGradient.RenderSize.Height);
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

            mCurHue = h;

            var gradient = DetailGradient.Fill as LinearGradientBrush;
            if (gradient == null)
                return;

            gradient.GradientStops = new GradientStopCollection
            {
                new GradientStop(Color.FromRgb(255, 255, 255), 0.0),
                new GradientStop(color, 1.0)
            };

            UpdateColor();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var elem = sender as UIElement;
            if (elem != null)
                elem.CaptureMouse();
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
            var elem = sender as UIElement;
            if (elem != null)
                elem.ReleaseMouseCapture();
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
            var elem = sender as UIElement;
            if (elem != null)
                elem.CaptureMouse();

            mIsDetailMouseDown = true;
        }

        private void DetailGradient_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var elem = sender as UIElement;
            if (elem != null)
                elem.ReleaseMouseCapture();
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

            Canvas.SetLeft(ColorEllipse, pos.X - 5);
            Canvas.SetTop(ColorEllipse, pos.Y - 5);

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
            var clr = Color.FromRgb((byte) r, (byte) g, (byte) b);
            if (ColorChanged != null)
                ColorChanged(clr);

            ColorEllipse.Stroke = new SolidColorBrush(Color.FromRgb((byte) (255 - r), (byte) (255 - g), (byte) (255 - b)));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(ColorEllipse, DetailGradient.RenderSize.Width - 5);
            Canvas.SetTop(ColorEllipse, -5.0);
            UpdateColor();
        }
    }
}
