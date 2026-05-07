using Avalonia.Controls;

namespace Scheri.PETPanel.Features;

public partial class SettingsView : UserControl {
    private readonly SettingsViewModel _viewModel = new();
    public SettingsView() {
        InitializeComponent();
        DataContext = _viewModel;
    }

    private void OnExitClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
        App.ShutdownApplication();
    }
}