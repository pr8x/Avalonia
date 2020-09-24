using Avalonia;
using Avalonia.Controls;

namespace Sandbox
{
    public interface IItem : IControl
    {
    }

    public class ItemBase<T> : Control, IItem
    {
        public static readonly StyledProperty<bool> XProperty =
            AvaloniaProperty.Register<FancyControl, bool>(nameof(Item));

        public bool X
        {
            get => GetValue(XProperty);
            set => SetValue(XProperty, value);
        }
    };

    public class Item : ItemBase<bool>
    {

    }

    public class FancyControl : Control
    {
        public static readonly StyledProperty<IItem> ItemProperty =
            AvaloniaProperty.Register<FancyControl, IItem>(nameof(Item));

        public IItem Item
        {
            get => GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }
    }
}
