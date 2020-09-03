using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace Avalonia.Diagnostics.ViewModels
{
    internal abstract class TreeNode : ViewModelBase, IDisposable
    {
        private IDisposable _classesSubscription;
        private string _classes;
        private bool _isExpanded;
        private string _toolTip;

        protected TreeNode(IVisual visual, TreeNode parent)
        {
            Parent = parent;
            Type = visual.GetType().Name;
            Visual = visual;

            if (visual is IControl control)
            {
                var removed = Observable.FromEventPattern<LogicalTreeAttachmentEventArgs>(
                    x => control.DetachedFromLogicalTree += x,
                    x => control.DetachedFromLogicalTree -= x);
                var classesChanged = Observable.FromEventPattern<
                        NotifyCollectionChangedEventHandler,
                        NotifyCollectionChangedEventArgs>(
                        x => control.Classes.CollectionChanged += x,
                        x => control.Classes.CollectionChanged -= x)
                    .TakeUntil(removed);

                _classesSubscription = classesChanged.Select(_ => Unit.Default)
                    .StartWith(Unit.Default)
                    .Subscribe(_ =>
                    {
                        if (control.Classes.Count > 0)
                        {
                            Classes = "(" + string.Join(" ", control.Classes) + ")";
                        }
                        else
                        {
                            Classes = string.Empty;
                        }
                    });
            }
        }

        protected void UpdateTooltip()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{Type}");

            if (Visual is Control control &&
                !string.IsNullOrEmpty(control.Name))
            {
                sb.AppendLine($"Name: {control.Name}");
            }

            sb.Append($"Subtree Size: {SubtreeSize}");

            ToolTip = sb.ToString();
        }

        protected long SubtreeSize
        {
            get; 
            set;
        }

        public TreeNodeCollection Children
        {
            get;
            protected set;
        }

        public string Classes
        {
            get { return _classes; }
            private set { RaiseAndSetIfChanged(ref _classes, value); }
        }

        public IVisual Visual
        {
            get;
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { RaiseAndSetIfChanged(ref _isExpanded, value); }
        }

        public string ToolTip
        {
            get { return _toolTip; }
            set { RaiseAndSetIfChanged(ref _toolTip, value); }
        }

        public TreeNode Parent
        {
            get;
        }

        public string Type
        {
            get;
        }

        public void Dispose()
        {
            _classesSubscription.Dispose();
            Children.Dispose();
        }
    }
}
