using System;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace Avalonia.Diagnostics.ViewModels
{
    internal class LogicalTreeNode : TreeNode
    {
        public LogicalTreeNode(ILogical logical, TreeNode parent)
            : base((Control)logical, parent)
        {
            Children = new LogicalTreeNodeCollection(this, logical);
            
            SubtreeSize = logical.GetLogicalDescendants().LongCount();
            UpdateTooltip();
        }

        public static LogicalTreeNode[] Create(object control)
        {
            return control is ILogical logical ? new[] { new LogicalTreeNode(logical, null) } : null;
        }

        internal class LogicalTreeNodeCollection : TreeNodeCollection
        {
            private readonly ILogical _control;
            private IDisposable _subscription;

            public LogicalTreeNodeCollection(TreeNode owner, ILogical control)
                : base(owner)
            {
                _control = control;
            }

            public override void Dispose()
            {
                base.Dispose();
                _subscription?.Dispose();
            }

            protected override void Initialize(AvaloniaList<TreeNode> nodes)
            {
                _subscription = _control.LogicalChildren.ForEachItem(
                    (i, item) => nodes.Insert(i, new LogicalTreeNode(item, Owner)),
                    (i, item) => nodes.RemoveAt(i),
                    () => nodes.Clear());
            }
        }

    }
}
