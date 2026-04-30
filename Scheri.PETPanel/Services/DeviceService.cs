using Scheri.PETPanel.Interfaces;
using Scheri.PETPanel.Network;
using Scheri.PETPanel.Network.Contract;
using Scheri.PETPanel.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        private readonly SemaphoreSlim _loginLock = new SemaphoreSlim(1, 1);
        public bool IsConnected { get => _isLogin; }
        public ConcurrentQueue<StatusInfo> StatusCache { get; private set; } = [];
        public const int MAX_CACHE_COUNT = 10;

        public DeviceService(IPAddress address, int port)
        {
            _connection = new TcpConnection(address, port);
            _connection.OnConnected += async () => {

                AppLogger.Info("Device connected, Attempting login...");
                await TryLoginAsync();
            };
            _connection.OnDisconnected += async () => {
                _isLogin = false;
                AppLogger.Info("Device disconnected!");
            };
        }

        public async void InitializeDeviceService()
        {
            await _connection.StartAsync();

            StartPollingDeviceStatus();
        }

        public async Task<bool> Login(int timeout)
        {
            await _loginLock.WaitAsync();
            try
            {
                if (!_connection.IsConnected) throw new InvalidOperationException("Device is not connected.");

                using var cst = new CancellationTokenSource(timeout);
                var tcs = new TaskCompletionSource<bool>();
                using var registration = cst.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);

                var encodeData = BuildCommand(DeviceCommand.CMD_LOGIN, [0x30]);

                bool anyResponse = await _connection.SendAndReceiveAsync(
                    data: encodeData,
                    onResponse: response => {
                        if (response.Command.SequenceEqual(Encoding.ASCII.GetBytes(DeviceCommand.RESP_LOGIN)))
                        {
                            tcs.TrySetResult(true);
                            AppLogger.Info("Login successful.");
                        }
                    });

                if (!anyResponse)
                {
                    AppLogger.Warn("Failed to send login command.");
                    return false;
                }

                try
                {
                    return await tcs.Task;
                }
                catch (TaskCanceledException)
                {
                    AppLogger.Warn("Login response timed out.");
                    return false;
                }
            }
            finally
            {
                _loginLock.Release();
            }

        }
        public async Task TryLoginAsync()
        {
            try
            {
                if (_isLogin) return;
                int retryCount = 0;
                while (!_isLogin && _connection.IsConnected && retryCount < 3)
                {
                    _isLogin = await Login(5000);
                    if (_isLogin && _connection.IsConnected) break;
                    retryCount++;
                    await Task.Delay(2000);
                }
                if (!_isLogin)
                    AppLogger.Warn("Login failed after multiple attempts."); return;

            }
            catch (Exception ex)
            {
                AppLogger.Error($"Login attempt failed: {ex.Message}");
            }
        }
        public void StartPollingDeviceStatus(int intervalMs = 200)
        {
            Task.Run(async () => {
                while (true)
                {
                    try
                    {
                        if (!_connection.IsConnected)
                        {
                            _isLogin= false;
                            await Task.Delay(1000);
                            continue;
                        }
                        if (!_isLogin)
                        {
                            AppLogger.Warn("Not logged in, attempting to login...");
                            _isLogin = await Login(5000);
                            if(!_isLogin)
                            {
                                AppLogger.Warn("Login failed, will retry in 3s.");
                                await Task.Delay(3000);
                                continue;
                            }
                        }

                        //after login, get status every 200ms
                        await GetStatusAsync(3000);
                        await Task.Delay(intervalMs);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.Error($"Polling critical error, restarting in 5s: {ex.Message}");
                        await Task.Delay(5000);
                    }
                }
            });
        }

        public async Task GetStatusAsync(int timeoutMs = 5000)
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
                    StatusCache.Enqueue(statusInfo);
                    while (StatusCache.Count>MAX_CACHE_COUNT)
                    {
                        StatusCache.TryDequeue(out _);
                    }
                    AppLogger.Info($"Status info cached. Total: {StatusCache.Count}");
                }
            }
            catch (JsonException ex)
            {
                AppLogger.Error($"JSON deserialization error: {ex.Message}");
            }
        }

        private static byte[] BuildCommand(string command, byte[]? payload = null)
        {
            ReadOnlySpan<byte> cmd = Encoding.ASCII.GetBytes(command);
            var packet = new DevicePacket(cmd, payload ?? Array.Empty<byte>());
            return packet.ToBytes();
        }

        public Task GetDeviceConfig()
        {
            throw new NotImplementedException();
        }

        public Task SetDeviceConfig()
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
