using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Scheri.PETPanel.UIComponents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheri.PETPanel.Features;

public partial class HomeViewModel:ObservableObject
{
    [RelayCommand]
    private void Navigate(Type? viewType)
    {
        if (viewType == null) return;
        WeakReferenceMessenger.Default.Send(new NavigateTypeMessage(viewType));
    }
}
