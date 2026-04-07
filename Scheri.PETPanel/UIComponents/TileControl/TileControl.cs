using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Scheri.PETPanel.UIComponents;
//public enum TileSize
//{
   
//    Small,
//    Wide,
//    Large
//}
public class TileControl : ContentControl
{ 
    public const double BaseSize = 450;
    public const double Spacing = 10;

    public static readonly StyledProperty<Color> BackgroundStartProperty =
        AvaloniaProperty.Register<TileControl, Color>(nameof(BackgroundStart), Color.Parse("#6BAEEA"));

    public Color BackgroundStart {
        get => GetValue(BackgroundStartProperty);
        set => SetValue(BackgroundStartProperty, value);
    }

    public static readonly StyledProperty<Color> BackgroundEndProperty =
        AvaloniaProperty.Register<TileControl, Color>(nameof(BackgroundEnd), Color.Parse("#1A4A8E"));

    public Color BackgroundEnd {
        get => GetValue(BackgroundEndProperty);
        set => SetValue(BackgroundEndProperty, value);
    }

    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<TileControl, string>(nameof(Label), "Tile");

    public string Label {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<TileControl, ICommand?>(nameof(Command));

    public ICommand? Command {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<TileControl, object?>(nameof(CommandParameter));

    public object? CommandParameter {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var button = e.NameScope.Find<Button>("PART_TileButton");
        if (button != null)
        {
            button.AddHandler(PointerPressedEvent, (s, e) => {
                var currentPoint = e.GetCurrentPoint(this);

                var newArgs = new PointerPressedEventArgs(
                  this,                 // Source
                  e.Pointer,          // Pointer
                  this,                 // Visual
                  currentPoint.Position, // Point
                  e.Timestamp,        // Timestamp
                  currentPoint.Properties, // Properties 
                  e.KeyModifiers,     // KeyModifiers
                  e.ClickCount        // ClickCount
              );
            },RoutingStrategies.Bubble,true);

                button.AddHandler(PointerReleasedEvent, (s, e) => {
                    RaiseEvent(e);
            },RoutingStrategies.Bubble,true);
        }
    }
}
