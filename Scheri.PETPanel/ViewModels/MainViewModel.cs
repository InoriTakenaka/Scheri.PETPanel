using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Scheri.PETPanel.Features;
using Scheri.PETPanel.Interfaces;
using Scheri.PETPanel.UIComponents;
using Scheri.PETPanel.Utils;
using Splat;
using System;
using System.Threading.Tasks;

namespace Scheri.PETPanel.ViewModels;

public partial class MainViewModel : ViewModelBase, IRecipient<NavigateTypeMessage> {
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsReturnButtonVisible))]
    private UserControl _currentView = new HomeView();
    [ObservableProperty]
    private string _appTitle = "";
    [ObservableProperty]
    private bool _isUnlock = false;
    [ObservableProperty]
    private bool _isShowLockOverlay = false;
    [ObservableProperty]
    private string _inputPassword = string.Empty;
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    [ObservableProperty]
    private int _lockTimeoutSeconds = 300;
    [ObservableProperty]
    private string _organizeName = string.Empty;
    [ObservableProperty]
    private string _customerName = string.Empty;
    private int _currentIdleSeconds = 0;
    private readonly DispatcherTimer _autolockTimer;
    public bool IsReturnButtonVisible => CurrentView is not HomeView;
    private static string _unlockPassword = string.Empty;
    private const string DEFAULT_PASSWORD = "000000";
    private const string INTERNAL_PASSWORD = "scheri.admin";

    public MainViewModel() {

        WeakReferenceMessenger.Default.Register(this);
        _autolockTimer = new DispatcherTimer {
            Interval = TimeSpan.FromSeconds(1)
        };
        _autolockTimer.Tick += AutolockTimer_Tick;
        _autolockTimer.Start();
        AppTitle = "HOME";

        IConfigurationService? configService = Locator.Current.GetService<IConfigurationService>();
        if (configService != null) {
            var appInfo = configService.AppSettings.AppInfo;
            OrganizeName = appInfo.Organization.HasValue("YUAN WU");
            CustomerName = appInfo.Customer.HasValue("核医学科");
            _lockTimeoutSeconds = appInfo.IdleTime > 0 ? appInfo.IdleTime : 300;
            _unlockPassword = appInfo.Password.HasValue(DEFAULT_PASSWORD);
        } 
    }

    [RelayCommand]
    private void NavigateBack() {
        if (CurrentView is HomeView) return;
        CurrentView = new HomeView();
        AppTitle = "HOME";
    }

    public void Receive(NavigateTypeMessage message) {
        if (message == null) return;
        var view = message.GetViewFactory?.Invoke();
        if (view == null) return;
        CurrentView = view;
        AppTitle = $"{ViewProps.GetTitle(view)}";
    }

    public void LockScreen() {
        StopAndResetIdleTimer();
        AppLogger.Info("Screen locked manually.");
        IsShowLockOverlay = true;
    }

    public void StopAndResetIdleTimer() {
        _autolockTimer.Stop();
        _currentIdleSeconds = 0;
    }

    public async Task UnlockScreen() {
        if (InputPassword.In([_unlockPassword, INTERNAL_PASSWORD])) {
            IsUnlock = false;
            IsShowLockOverlay = false;
            await Task.Delay(200);
            InputPassword = string.Empty;
            ResetTimer();
        } else {
            AppLogger.Warn($"Incorrect password attempt to unlock the screen: {InputPassword}.");
            ErrorMessage = "Incorrect Password";
            await Task.Delay(200);
            InputPassword = string.Empty;
        }
    }

    partial void OnInputPasswordChanged(string? oldValue, string newValue) {
        if (!string.IsNullOrEmpty(newValue) && newValue.Length == 6) {
            UnlockScreen();
        }
    }


    private void AutolockTimer_Tick(object? sender, EventArgs e) {
        if (IsShowLockOverlay || IsUnlock) return;
        _currentIdleSeconds++;
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"[Timer] Current Idle: {_currentIdleSeconds}s / {LockTimeoutSeconds}s");
#endif 
        if (_currentIdleSeconds >= LockTimeoutSeconds) {
            if (CurrentView is HomeView homeview) IsShowLockOverlay = true;
            _currentIdleSeconds = 0;
            _autolockTimer.Stop();
            AppLogger.Info("Screen auto-locked due to inactivity.");
        }
    }

    public void ResetTimer() {
        _currentIdleSeconds = 0;
        if (!_autolockTimer.IsEnabled) {
            _autolockTimer.Start();
        }
    }
}
