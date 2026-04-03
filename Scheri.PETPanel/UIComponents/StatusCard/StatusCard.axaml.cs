using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentIcons.Common;

namespace Scheri.PETPanel.UIComponents;

public partial class StatusCard : UserControl
{
    public static readonly StyledProperty<Symbol> SymbolProperty
        = AvaloniaProperty.Register<StatusCard, Symbol>(nameof(Symbol));

    public static readonly StyledProperty<string> HeaderProperty
        = AvaloniaProperty.Register<StatusCard, string>(nameof(Header));

    public static readonly StyledProperty<string> StatusTextProperty
        = AvaloniaProperty.Register<StatusCard, string>(nameof(StatusText));

    public static readonly StyledProperty<Color> StatusColorProperty
        = AvaloniaProperty.Register<StatusCard, Color>(nameof(StatusColor), Color.Parse("#50fa7b"));

    public static readonly StyledProperty<bool> IsAlarmingProperty
        = AvaloniaProperty.Register<StatusCard, bool>(nameof(IsAlarming), false);

    public Symbol Symbol {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    public string Header {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public string StatusText {
        get => GetValue(StatusTextProperty);
        set => SetValue(StatusTextProperty, value);
    }

    public Color StatusColor {
        get => GetValue(StatusColorProperty);
        set => SetValue(StatusColorProperty, value);
    }

    public bool IsAlarming {
        get => GetValue(IsAlarmingProperty);
        set => SetValue(IsAlarmingProperty, value);
    }

    public StatusCard()
    {
        InitializeComponent();
    }
}