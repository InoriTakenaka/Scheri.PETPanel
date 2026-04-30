using Scheri.PETPanel.Interfaces;
using Scheri.PETPanel.Network;
using Scheri.PETPanel.Network.Contract;
using Scheri.PETPanel.Services;
using Splat;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Scheri.PETPanel.Utils;

public class DeviceCommand
{
    public const string CMD_LOGIN = "CM00";
    public const string RESP_LOGIN = "SM00";
    public const string CMD_GET_STATUS = "CM01";
    public const string RESP_GET_STATUS = "SM01";
}

public class DeviceManager
{
    private static readonly Lazy<DeviceManager> _instance = new(() => new DeviceManager());
    private DeviceService _deviceService;
    public static DeviceManager Instance => _instance.Value;
    public IEnumerable<StatusInfo> Cache => Instance._deviceService.StatusCache;
    public bool IsConnected => Instance._deviceService.IsConnected;
    private DeviceManager() { }

    public async Task InitializeAsync()
    {
        //workstation IP address
        string ip = "192.168.1.80";
        int port = 8066;
        var config = Locator.Current.GetService<IConfigurationService>();
        if (config!=null)
        {
            ip = config.AppSettings.Workstation.Address;
            port = config.AppSettings.Workstation.Port;
        }
        _deviceService = new DeviceService(IPAddress.Parse(ip), port);
        _deviceService.InitializeDeviceService();
    }
}

