using System;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Avalonia.Diagnostics.Controls
{
    internal class FlameGraph : Control
    {
        public static readonly DirectProperty<FlameGraph, object> RootProperty =
            AvaloniaProperty.RegisterDirect<FlameGraph, object>(nameof(Root), o => o.Root, (o,v) => o.Root = v);

        public object Root
        {
            get;
            set;
        }

        private static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = (int)Math.Floor(hue / 60) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;
            byte v = (byte)value;
            byte p = (byte)(value * (1 - saturation));
            byte q = (byte)(value * (1 - f * saturation));
            byte t = (byte)(value * (1 - (1 - f) * saturation));

            return hi switch
            {
                0 => Color.FromArgb(255, v, t, p),
                1 => Color.FromArgb(255, q, v, p),
                2 => Color.FromArgb(255, p, v, t),
                3 => Color.FromArgb(255, p, q, v),
                4 => Color.FromArgb(255, t, p, v),
                _ => Color.FromArgb(255, v, p, q)
            };
        }

        private static Color GetNodeColor(int index)
        {
            if (index == 0)
            {
                return ColorFromHSV(0, 1, 1);
            }

            const double maxHue = 55;

            var k = index / 10;

            return ColorFromHSV(k * maxHue, 1, 1);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            const double barHeight = 10;

            var color = GetNodeColor(0);
            var rect = new Rect(0, Bounds.Height - barHeight, Bounds.Width, barHeight);
           
            context.FillRectangle(new ImmutableSolidColorBrush(color), rect);

        }
    }
}
