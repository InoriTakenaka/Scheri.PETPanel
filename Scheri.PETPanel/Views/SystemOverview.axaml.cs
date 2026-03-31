using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Scheri.PETPanel.Utils;
using Scheri.PETPanel.ViewModels;
using Scheri.PETPanel.Views;

namespace Scheri.PETPanel;

public partial class SystemOverview : UserControl
{
    private readonly SystemOverviewViewModel _viewModel = new();
    private bool _isLocked = false;

    public SystemOverview()
    {
        InitializeComponent();
        DataContext = _viewModel;
        Forward.AddHandler(PointerPressedEvent, Forward_PointerPressed, handledEventsToo: true);
        Backward.AddHandler(PointerPressedEvent, Backward_PointerPressed, handledEventsToo: true);
        Forward.AddHandler(PointerReleasedEvent, Forward_PointerReleased, handledEventsToo: true);
        Backward.AddHandler(PointerReleasedEvent, Backward_PointerReleased, handledEventsToo: true);
    }

    #region Move Device Handlers
    private async void Backward_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
       ScanTableManager.Instance.CheckFloat();
        await ScanTableManager.Instance.ToBBackward(true);
    }

    private async void Backward_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        ScanTableManager.Instance.CheckFloat();
        await ScanTableManager.Instance.ToBBackward(false);
    }

    private async void Forward_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        ScanTableManager.Instance.CheckFloat();
        await ScanTableManager.Instance.ToBForward(true);
    }

    private async void Forward_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        ScanTableManager.Instance.CheckFloat();
        await ScanTableManager.Instance.ToBForward(false);
    }
    #endregion

    private void SystemOverviewLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //VlcPlayer.Play("rtsp://admin:admin@78.110.219.124:554/live/ch0");
    }

    private void LockButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!_isLocked)
        {
            MainView.Instance?.ViewModel?.StopAndResetIdleTimer();
            SetLockState(true);
        }
        else
        {
            MainView.Instance?.ViewModel?.ShowUnlockDialog();
        }
    }

    /// <summary>
    /// Sets the lock state of the user interface, enabling or disabling device movement controls and updating lock
    /// indicators accordingly.
    /// </summary>
    /// <remarks>When the interface is locked, device movement controls are disabled and the lock icon and text are
    /// updated to reflect the locked state. Use this method to control user interaction based on application
    /// state.</remarks>
    /// <param name="Locked">A value indicating whether the interface should be locked. Set to <see langword="true"/> to lock the interface;
    /// otherwise, <see langword="false"/>.</param>
    public void SetLockState(bool Locked)
    {
        _isLocked = Locked;

        Backward.IsEnabled = !_isLocked;
        Forward.IsEnabled = !_isLocked;
        LockIcon.Symbol = _isLocked ? FluentIcons.Common.Symbol.LockClosed : FluentIcons.Common.Symbol.LockOpen;
        LockText.Text = _isLocked ? "Unlock Screen" : "Lock Screen";
    }
}