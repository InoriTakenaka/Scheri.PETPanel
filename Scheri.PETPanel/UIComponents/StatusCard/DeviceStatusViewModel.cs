using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Scheri.PETPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheri.PETPanel.UIComponents;

public partial class DeviceStatusViewModel:ViewModelBase
{
    [ObservableProperty]
    private string _header;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    [NotifyPropertyChangedFor(nameof(StatusColor))]
    private bool _isConnected;
    private readonly DispatcherTimer _statusUpdateTimer;
    private readonly Func<bool> _statusIndicatorFunc;
    public string StatusText => IsConnected ? "Online" : "Offline";
    public Color StatusColor => IsConnected ? Color.Parse("#50fa7b") : Colors.Red;

    public void UpdateDeviceStatus()
    {
        IsConnected = _statusIndicatorFunc.Invoke();
    }

    public DeviceStatusViewModel(string header, Func<bool> statusIndicatorFunc, bool initialStatus=false)
    {
        Header = header;
        IsConnected = initialStatus;
        _statusIndicatorFunc = statusIndicatorFunc;
        _statusUpdateTimer = new DispatcherTimer {
            Interval = TimeSpan.FromSeconds(2)
        };
        _statusUpdateTimer.Tick += (s, e) => UpdateDeviceStatus();
        _statusUpdateTimer.Start();
    }
}
