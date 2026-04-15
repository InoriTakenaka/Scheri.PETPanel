using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Scheri.PETPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
    private readonly Func<Task<bool>> _statusIndicatorFunc;
    public string StatusText => IsConnected ? "Online" : "Offline";
    public Color StatusColor => IsConnected ? Color.Parse("#50fa7b") : Colors.Red;

    public async void UpdateDeviceStatus()
    {
        try
        {
            bool result = await _statusIndicatorFunc.Invoke();
            IsConnected = result;
        }catch { 
            IsConnected = false;
        }
    }

    public DeviceStatusViewModel(string header, Func<Task<bool>> statusIndicatorFunc, bool initialStatus=false)
    {
        Header = header;
        IsConnected = initialStatus;
        _statusIndicatorFunc = statusIndicatorFunc;
        _statusUpdateTimer = new DispatcherTimer {
            Interval = TimeSpan.FromSeconds(1)
        };
        _statusUpdateTimer.Tick += (s, e) => UpdateDeviceStatus();
        _statusUpdateTimer.Start();
    }
}
