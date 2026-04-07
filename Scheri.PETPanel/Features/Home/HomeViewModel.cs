using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Scheri.PETPanel.UIComponents;
using Scheri.PETPanel.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheri.PETPanel.Features;

public partial class HomeViewModel:ObservableObject
{
    public DeviceStatusViewModel PlcStatus { get;} =  new DeviceStatusViewModel("PLC", ()=>true);
    public DeviceStatusViewModel CameraStatus { get; } = new DeviceStatusViewModel("Camera", () => false);

    [RelayCommand]
    private void Navigate(Type? viewType)
    {
        if (viewType == null) return;
        WeakReferenceMessenger.Default.Send(new NavigateTypeMessage(viewType));
 
    }  
}
