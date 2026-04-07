using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Scheri.PETPanel.Features;

public partial class BedMovementView : UserControl
{
    public BedMovementView()
    {
        InitializeComponent();
        Forward.AddHandler(PointerPressedEvent, Forward_PointerPressed, handledEventsToo: true);
        Forward.AddHandler(PointerReleasedEvent, Forward_PointerReleased, handledEventsToo: true);
        Backward.AddHandler(PointerPressedEvent, Backward_PointerPressed, handledEventsToo: true);
        Backward.AddHandler(PointerReleasedEvent, Backward_PointerReleased, handledEventsToo: true);
    }

    private void Backward_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Backward_PointerPressed");
    }

    private void Backward_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Backward_PointerReleased");
    }

    private void Forward_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Backward_PointerPressed");
    }

    private void Forward_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Forward_PointerReleased");
    }
}