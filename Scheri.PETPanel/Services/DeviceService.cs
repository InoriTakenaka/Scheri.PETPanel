using Scheri.PETPanel.Interfaces;
using Scheri.PETPanel.Network;
using Scheri.PETPanel.Network.Contract;
using Scheri.PETPanel.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace Scheri.PETPanel.Services
{
    public class DeviceService
       : IDeviceInterface
    {
        private TcpConnection _connection;
        private bool _isLogin;

        public List<StatusInfo> Cache { get; private set; } = [];

        public DeviceService(IPAddress address, int port)
        {
            _connection = new TcpConnection(address, port);
            _connection.OnConnected += () => AppLogger.Info("Device connected!");
            _connection.OnDisconnected += () => AppLogger.Info("Device disconnected!");
        }

        public async void InitializeDeviceService()
        {
            await _connection.StartAsync();
            try
            {
                _isLogin = await Login(5000);
                if (_isLogin)
                {
                    StartPollingDeviceStatus();
                }
            }
            catch (Exception)
            {
                AppLogger.Error("Failed to login to device after connecting.", nameof(DeviceService));
            }
        }

        public async Task<bool> Login(int timeout)
        {
            if (!_connection.IsConnected) throw new InvalidOperationException("Device is not connected.");

            using var cst = new CancellationTokenSource(timeout);
            var tcs = new TaskCompletionSource<bool>();
            using var registration = cst.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

            var encodeData = BuildCommand(DeviceCommand.CMD_LOGIN, [0x30]);

            bool success = await _connection.SendAndReceiveAsync(
                data: encodeData,
                onResponse: response => {
                    bool loginResult = false;
                    if (response.Command.SequenceEqual(Encoding.ASCII.GetBytes(DeviceCommand.RESP_LOGIN)))
                    {
                        loginResult = true;
                        tcs.TrySetResult(loginResult);
                        AppLogger.Info("Login successful.");
                    }
                });

            if (!success)
            {
                AppLogger.Warn("Failed to send login command.");
                return false;
            }
            return await tcs.Task;
        }

        public void StartPollingDeviceStatus(int intervalMs = 200)
        {
            Task.Run(async () => {
                while (true)
                {
                    try
                    {
                        if (_connection.IsConnected)
                        {
                            await GetStatusAsync();
                            await Task.Delay(intervalMs);
                        }
                        else
                        {
                            await Task.Delay(1000);
                        }
                    }
                    catch (Exception ex)
                    {
                        AppLogger.Error($"Polling critical error, restarting in 5s: {ex.Message}");
                        await Task.Delay(5000);
                    }
                }
            });
        }

        private async Task GetStatusAsync(int timeoutMs = 5000)
        {
            if (!_connection.IsConnected) throw new InvalidOperationException("Device is not connected.");

            var tcs = new TaskCompletionSource<string?>();
            using var cts = new CancellationTokenSource(timeoutMs);
            using var registration = cts.Token.Register(() => tcs.TrySetResult(null));


            byte[] encodeData = BuildCommand(DeviceCommand.CMD_GET_STATUS);
            bool success = await _connection.SendAndReceiveAsync(
                data: encodeData,
                onResponse: response => {
                    if (response.Command.SequenceEqual(Encoding.ASCII.GetBytes(DeviceCommand.RESP_GET_STATUS)))
                    {
                        string jsonData = Encoding.UTF8.GetString(response.Payload);
                        tcs.TrySetResult(jsonData);
                        // Process status response
                        AppLogger.Info($"Received status response with payload:\r\n{jsonData}");
                    }
                }, timeoutMs);

            string? jsonResult = await tcs.Task;
            if (string.IsNullOrEmpty(jsonResult))
            {
                AppLogger.Warn("Failed to get status response within timeout.");
                return;
            }

            try
            {
                var statusInfo = JsonSerializer.Deserialize(jsonResult, AppJsonSerializerContext.Default.StatusInfo);
                if (statusInfo == null)
                {
                    AppLogger.Warn("Failed to deserialize status info from JSON.");
                }
                else
                {
                    Cache.Add(statusInfo);
                    AppLogger.Info($"Status info cached. Total: {Cache.Count}");
                }
            }
            catch (JsonException ex)
            {
                AppLogger.Error($"JSON deserialization error: {ex.Message}");
            }
        }

        public static byte[] BuildCommand(string command, byte[]? payload = null)
        {
            ReadOnlySpan<byte> cmd = Encoding.ASCII.GetBytes(command);
            var packet = new DevicePacket(cmd, payload ?? Array.Empty<byte>());
            return packet.ToBytes();
        }

        public Task<bool> Login()
        {
            throw new NotImplementedException();
        }

        public Task<StatusInfo> GetStates()
        {
            throw new NotImplementedException();
        }

        public Task GetConfig()
        {
            throw new NotImplementedException();
        }

        public Task SetConfig()
        {
            throw new NotImplementedException();
        }

        public Task RealControl()
        {
            throw new NotImplementedException();
        }

        public Task ModuleControl()
        {
            throw new NotImplementedException();
        }
    }
}
