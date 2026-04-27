using Scheri.PETPanel.Network;
using Scheri.PETPanel.Network.Contract;
using System;
using System.Collections.Generic;
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
    private static List<StatusInfo> _cache = [];
    public static DeviceManager Instance=> _instance.Value;
    public bool IsConnected => _connection.IsConnected;
    private TcpConnection _connection;
    private bool _isInitialezed = false;

    private DeviceManager()
    {
        _connection = new TcpConnection(System.Net.IPAddress.Parse("127.0.0.1"), 8066);
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        if (_isInitialezed) return;

        _connection.OnConnected += () => AppLogger.Info("Device connected!");
        _connection.OnDisconnected += () => AppLogger.Info("Device disconnected!");

        await _connection.StartAsync(ct);
        _isInitialezed = true;
    }

    public async Task<bool> ExcuteCommandAsync(byte[] command, byte[] payload, Action<DevicePacket> onResponse, int timeoutMs = 5000)
    {
        if(!_connection.IsConnected) return false;

        var request = new DevicePacket(command,payload);
        byte[] encodeData = request.ToBytes();

        return await _connection.SendAndReceiveAsync(encodeData, onResponse, timeoutMs); 
    }  
}

