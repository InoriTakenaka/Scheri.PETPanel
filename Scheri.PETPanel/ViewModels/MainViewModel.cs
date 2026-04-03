using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Scheri.PETPanel.Features;
using Scheri.PETPanel.UIComponents;
using Scheri.PETPanel.Utils;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Scheri.PETPanel.ViewModels;

public partial class MainViewModel : ViewModelBase, IRecipient<NavigateTypeMessage>
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsReturnButtonVisible))]
    private UserControl _currentView = new HomeView();
    [ObservableProperty]
    private string _appTitle = "PET PANEL";
    [ObservableProperty]
    private bool _isUnlock = false;
    [ObservableProperty]
    private string _inputPassword = string.Empty;
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    [ObservableProperty]
    private int _lockTimeoutSeconds = 30;
    
    public bool IsReturnButtonVisible => CurrentView is not HomeView;

    private int _currentIdleSeconds = 0;
    private readonly DispatcherTimer _autolockTimer;

    public MainViewModel()
    {
        WeakReferenceMessenger.Default.Register(this);
    }

    [RelayCommand]
    private void NavigateBack()
    {
        if (CurrentView is HomeView) return;
        CurrentView = new HomeView();
        AppTitle = "PET PANEL";
    }

    public void Receive(NavigateTypeMessage message)
    {
        if (message == null) return;
        var view = Activator.CreateInstance(message.ViewType) as UserControl; 
        if (view != null)
        {
            CurrentView = view;
            AppTitle = $"PET PANEL - {ViewProps.GetTitle(view)}";
        }
    }

//    public void StopAndResetIdleTimer()
//    {
//        _autolockTimer.Stop();
//        _currentIdleSeconds = 0;
//    }

//    public void ShowUnlockDialog() => IsUnlock = true;

//    public async void UnlockScreen()
//    {
//        if (InputPassword == "000000")
//        {
//            IsUnlock = false;
//            await Task.Delay(200);
//            InputPassword = string.Empty;
//            if (CurrentView is SystemOverview overview) overview.SetLockState(false);
//            ResetTimer();
//        }
//        else
//        {
//            AppLogger.Warn($"Incorrect password attempt to unlock the screen: {InputPassword}.");
//            ErrorMessage = "Incorrect Password";
//            await Task.Delay(200);
//            InputPassword = string.Empty;         
//        }
//    }

//    partial void OnInputPasswordChanged(string? oldValue, string newValue)
//    {
//        if (!string.IsNullOrEmpty(newValue) && newValue.Length == 6)
//        {
//            UnlockScreen();
//        }
//    }


//    private void AutolockTimer_Tick(object? sender, EventArgs e)
//    {
//        if (IsUnlock) return;
//        _currentIdleSeconds++;
//#if DEBUG
//        System.Diagnostics.Debug.WriteLine($"[Timer] Current Idle: {_currentIdleSeconds}s / {LockTimeoutSeconds}s");
//#endif 
//        if (_currentIdleSeconds >= LockTimeoutSeconds)
//        {
//            if(CurrentView is SystemOverview overview) overview.SetLockState(true);
//            _currentIdleSeconds = 0;
//            _autolockTimer.Stop();
//            AppLogger.Info("Screen auto-locked due to inactivity.");
//        }
//    }

//    public void ResetTimer()
//    {
//        _currentIdleSeconds = 0;
//        if (!_autolockTimer.IsEnabled)
//        {
//            _autolockTimer.Start();
//        }
//    }
}
