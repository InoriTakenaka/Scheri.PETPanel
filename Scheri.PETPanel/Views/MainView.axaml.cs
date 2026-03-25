using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Scheri.PETPanel.Utils;
using Scheri.PETPanel.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Scheri.PETPanel.Views;

public partial class MainView : UserControl
{
    public static MainView? Instance { get; private set; }
    public MainViewModel? ViewModel => DataContext as MainViewModel;
    public MainView()
    {
        InitializeComponent();
        Instance = this;
        AddHandler(PointerPressedEvent, (s, e) => {
            ViewModel?.ResetTimer();
        }, RoutingStrategies.Tunnel);
    }

    private void MainView_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)=> ViewModel?.ResetTimer();

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataContext is MainViewModel viewModel) viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (DataContext is MainViewModel viewModel) viewModel.PropertyChanged -= ViewModel_PropertyChanged;
    }

    private async void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not MainViewModel viewModel) return;
        if (e.PropertyName == nameof(MainViewModel.IsUnlock) && viewModel.IsUnlock)
        {
            Dispatcher.UIThread.Post(async () => {
                await Task.Delay(200); // Ensure the UI has updated before showing the dialog
                var PasswordBox = this.FindControl<TextBox>("PasswordBox");
                PasswordBox?.Focus();
                PasswordBox?.SelectAll();
            }, DispatcherPriority.Input);

        }
    }

    private void Overlay_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.IsUnlock = false;
            viewModel.InputPassword = string.Empty;
            viewModel.ErrorMessage = string.Empty;
            AppLogger.Info("User canceled the unlock process.", nameof(MainView));
        }
    }

    private void StackPanel_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        e.Handled = true;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Instance = null;
    }
}