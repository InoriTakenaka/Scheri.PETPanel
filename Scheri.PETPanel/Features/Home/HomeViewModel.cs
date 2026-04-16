using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Scheri.PETPanel.UIComponents;
using Scheri.PETPanel.Utils;
using Scheri.PETPanel.Views;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Scheri.PETPanel.Features;

public partial class HomeViewModel : ObservableObject
{
    public DeviceStatusViewModel PlcStatus { get; } = new DeviceStatusViewModel("PLC", CheckPlcStatus);
    public DeviceStatusViewModel CameraStatus { get; } = new DeviceStatusViewModel("Camera", CheckCameraStatus);

    [RelayCommand]
    private static void Navigate(NavigateType type)
    {
        if (!ViewRoute.ViewRoutes.TryGetValue(type, out var viewFactory)) return;
        WeakReferenceMessenger.Default.Send(new NavigateTypeMessage(viewFactory));
    }

    [RelayCommand]
    private static void LockScreen()
    {
        MainView.Instance?.ViewModel?.LockScreen();
    }

    private static async Task<bool> CheckPlcStatus()
    {
#if DEBUG
        return true;
#endif 
        return await ScanTableManager.IsConnected();
    }

    private static async Task<bool> CheckCameraStatus()
    {
#if DEBUG
        return true;
#endif 
        //string testingUri = "rtsp://127.0.0.1:8554/test";
        string rtsp = "rtsp://192.168.1.244:554";
        try
        {
            var uri = new Uri(rtsp);
            string host = uri.Host;
            var port = uri.Port;

            using var client = new TcpClient();
            using var cts = new CancellationTokenSource();
            await client.ConnectAsync(host, port, cts.Token);
            return client.Connected;
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex.Message, nameof(CheckCameraStatus));
            return false;
        }
    }
}
