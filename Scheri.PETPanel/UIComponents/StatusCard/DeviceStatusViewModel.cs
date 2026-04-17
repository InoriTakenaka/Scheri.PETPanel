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
    public const string CONNECTED_COLOR_HEX = "#50fa7b";
    public const string DISCONNECTED_COLOR_HEX = "#ffff0000";
    public const string CONNECTED_STATUS_TEXT = "Online";
    public const string DISCONNECTED_STATUS_TEXT = "Offline";
    public readonly static Color CONNECTED = Color.Parse(CONNECTED_COLOR_HEX);
    public readonly static Color DISCONNECTED = Color.Parse(DISCONNECTED_COLOR_HEX);   
    [ObservableProperty]
    private string _header;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    [NotifyPropertyChangedFor(nameof(StatusColor))]
    private bool _isConnected;
    public string StatusText => IsConnected ? CONNECTED_STATUS_TEXT: DISCONNECTED_STATUS_TEXT;  
    public Color StatusColor => IsConnected ? CONNECTED : DISCONNECTED;

    private readonly DispatcherTimer _statusUpdateTimer;
    private readonly Func<Task<bool>> _statusIndicatorFunc;

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
