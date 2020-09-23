using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Diagnostics.ViewModels;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Avalonia.Diagnostics.Controls
{
    internal class FlameGraph : Control
    {
        public static readonly DirectProperty<FlameGraph, TreeNode> RootProperty =
            AvaloniaProperty.RegisterDirect<FlameGraph, TreeNode>(
                nameof(Root),
                o => o.Root,
                (o, v) => o.Root = v);

        public TreeNode Root
        {
            get;
            set;
        }

        public Func<TreeNode, TimeSpan> TimeExtractor { get; set; }

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

            var k = (index % 10) / 10.0;

            return ColorFromHSV(k * maxHue, 1, 1);
        }

        private TimeSpan GetNodeTime(TreeNode node)
        {
            return TimeExtractor(node);
        }

        private string GetNodeName(TreeNode node)
        {
            var name = node.Type;

            if (node.Visual is Control control &&
                !string.IsNullOrEmpty(control.Name))
            {
                name += $" '{control.Name}'";
            }

            name += $" {GetNodeTime(node).TotalMilliseconds}ms";

            return name;
        }

        private readonly struct GraphNode
        {
            public GraphNode(TreeNode treeNode, int level)
            {
                TreeNode = treeNode;
                Level = level;
            }

            public TreeNode TreeNode { get; }

            public int Level { get; }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            const double barHeight = 20;

            var baseline = GetNodeTime(Root.Children[0]);

            var nodes = new Queue<GraphNode>();
            var index = 0;
            var levelOffsetX = 0.0;

            nodes.Enqueue(new GraphNode(Root, 0));

            while (nodes.Count > 0)
            {
                var current = nodes.Dequeue();

                foreach (var child in current.TreeNode.Children)
                {
                    nodes.Enqueue(new GraphNode(child, current.Level + 1));
                }

                var nodeTime = GetNodeTime(current.TreeNode);

                var nodeColor = GetNodeColor(index++);

                var rect = new Rect(
                    levelOffsetX,
                    Bounds.Height - barHeight * current.Level,
                    nodeTime.Ticks * Bounds.Width / baseline.Ticks,
                    barHeight);

                levelOffsetX += rect.Width;

                context.FillRectangle(new ImmutableSolidColorBrush(nodeColor), rect);

                var text = new FormattedText
                {
                    Text = GetNodeName(current.TreeNode), Typeface = Typeface.Default, FontSize = 10.0
                };

                using (context.PushClip(rect))
                {
                    context.DrawText(
                        Brushes.White,
                        new Point(0, rect.Center.Y - text.Bounds.Height / 2),
                        text);
                }

                if (nodes.Count != 0 &&
                    current.Level != nodes.Peek().Level)
                {
                    levelOffsetX = 0;
                }
            }

            //Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
        }
    }
}
