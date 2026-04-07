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
}
