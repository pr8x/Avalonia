using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Diagnostics.Controls;
using Avalonia.Diagnostics.ViewModels;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;

#nullable enable

namespace Avalonia.Diagnostics.Views
{
    public class PerformancePageView : UserControl
    {
        public PerformancePageView()
        {
            InitializeComponent();

            var flameGraph = this.FindControl<FlameGraph>("FlameGraph");

            flameGraph.TimeExtractor = node =>
            {
                var bb = ReadTime(node);

                var xx = node.Children.SelectMany(x => x.Children).Select(ReadTime).Sum(c => c.Ticks);

                return bb + TimeSpan.FromTicks(xx);
            };
        }

        private TimeSpan ReadTime(TreeNode node)
        {
            return ViewModel!.Timings![node.Visual as ILayoutable];
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private PerformancePageViewModel? ViewModel => (PerformancePageViewModel?)DataContext;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            ViewModel?.Refresh();
        }
    }
}
