using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Scheri.PETPanel.UIComponents;
using System;
using System.Reflection;

namespace Scheri.PETPanel.ViewModels;

public partial class MainViewModel : ViewModelBase,IRecipient<NavigateTypeMessage>
{
    public MainViewModel()
    {
        WeakReferenceMessenger.Default.Register(this);
    }
    public void Receive(NavigateTypeMessage message)
    {
        if (message == null) return;
        var view = Activator.CreateInstance(message.ViewType) as UserControl;
        if (view != null)
        {
            CurrentView = view;
        }
    }

    [ObservableProperty]
    private UserControl _currentView = new SystemOverview();
}
