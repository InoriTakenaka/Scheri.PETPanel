using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Scheri.PETPanel.ViewModels;

namespace Scheri.PETPanel;

public partial class SystemOverview : UserControl
{
    private readonly SystemOverviewViewModel _viewModel = new();
    public SystemOverview()
    {
        InitializeComponent();
        DataContext = _viewModel;
    }

    private async void Backward_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {

    }

    private async void Backward_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {

    }

    private async void Forward_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {

    }

    private async void Forward_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {

    }

    private void SystemOverviewLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        VlcPlayer.Play("rtsp://admin:admin@78.110.219.124:554/live/ch0");
    }
}