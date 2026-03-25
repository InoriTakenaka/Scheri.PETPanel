using Scheri.PETPanel.Network.Contract;
using Scheri.PETPanel.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Scheri.PETPanel.Network
{
    public partial class PetProtocol
    {
        public PetProtocol()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            OnRev += json => AppLogger.Debug("rev:" + json);
            OnSend += json => AppLogger.Debug("send:" + json);
        }

        public void Connect(string destIp, int destPort = 8066, int destUdpPort = 8055)
        {
            _destIp = destIp;
            _destPort = destPort;
            _destUdpPort = destUdpPort;
            StartConnectTask();
        }

        private void StartConnectTask()
        {
            Task.Run(async () => {
                Thread.CurrentThread.Name = "握手线程";
                while (true)
                {
                    if (!_autoConnect)
                        break;
                    try
                    {
                        await GetStatus();
                        _isConnected = true;
                    }
                    catch (Exception)
                    {
                        _isConnected = false;
                        OnDisconnect?.Invoke();
                        try
                        {
                            await DisConnect();
                            await Connect();
                            OnConnect?.Invoke();
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        private async Task Connect()
        {
            await _socketSlim.WaitAsync();
            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.ReceiveTimeout = 10000;
                _tcpClient.SendTimeout = 10000;
                await _tcpClient.ConnectAsync(IPAddress.Parse(_destIp), _destPort);
            }
            finally
            {
                _socketSlim.Release();
            }
            await Login();
        }

        public async Task GetStatus()
        {
            List<byte> cmd = [Head, Foot];
            cmd.InsertRange(1, CmdGetStatus.ToAsciiBytes());
            var revCmd = await SendAndRev(cmd.ToArray(), RevGetStatus, false);
            var json = GetJsonFromRevBytes(revCmd, 6);
            var status = JsonSerializer.Deserialize<StatusInfo>(json);
            if (status != null)
            {
                if (status.collect_prompt_count < 0)
                    status.collect_prompt_count = 0;
                if (status.collect_random_count < 0)
                    status.collect_random_count = 0;
                if (StatusInfo != null)
                {
                    var tmp = StatusInfo.Cache.ToList();
                    tmp.Reverse();
                    tmp = tmp.Take(10).ToList();
                    tmp.Reverse();
                    status.Cache.AddRange(tmp);
                }
                status.Cache.Add(status);
            }

            StatusInfo = status;
            //OnRev?.Invoke(json);
        }

        public async Task Login()
        {
            List<byte> cmd = [Head, Foot];
            cmd.InsertRange(1, [0x30]);
            cmd.InsertRange(1, CmdLogin.ToAsciiBytes());
            var revCmd = await SendAndRev(cmd.ToArray(), RevLogin);
        }

        public async Task GetConfig(PetConfigType configType)
        {
            List<byte> cmd = [Head, Foot];
            cmd.InsertRange(1, CmdGetConfig.ToAsciiBytes());
            byte configTypeByte = 0x30;
            switch (configType)
            {
                case PetConfigType.System: configTypeByte = 0x30; break;
                case PetConfigType.Device: configTypeByte = 0x31; break;
                case PetConfigType.Matrix: configTypeByte = 0x32; break;
                case PetConfigType.Normalization: configTypeByte = 0x33; break;
            }

            cmd.Insert(5, configTypeByte);
            var revCmd = await SendAndRev(cmd.ToArray(), RevGetConfig);
            if (revCmd[5] == ResultSuccess)
            {
                switch (revCmd[6])
                {
                    case ConfigSystem: break;
                    case ConfigDevice: break;
                    case ConfigMatrix: break;
                    case ConfigNormalization: break;
                }
            }
        }

        public async Task SetConfig(PetConfigType configType, string config)
        {
            List<byte> cmd = [Head, Foot];
            cmd.InsertRange(1, CmdSetConfig.ToAsciiBytes());
            byte configTypeByte = 0x30;
            switch (configType)
            {
                case PetConfigType.System: configTypeByte = 0x30; break;
                case PetConfigType.Device: configTypeByte = 0x31; break;
                case PetConfigType.Matrix: configTypeByte = 0x32; break;
                case PetConfigType.Normalization: configTypeByte = 0x33; break;
            }

            cmd.Insert(5, configTypeByte);
            cmd.InsertRange(6, config.ToAsciiBytes());
            var revCmd = await SendAndRev(cmd.ToArray(), RevSetConfig);
        }

        public async Task<bool> RealControl(PetControlType controlType, PetControlDst dst, string id,
            BuildInfo buildInfo)
        {
            List<byte> cmd = [Head, Foot];
            cmd.InsertRange(1, CmdRealControl.ToAsciiBytes());

            switch (controlType)
            {
                case PetControlType.New: cmd.Insert(5, ControlNew); break;
                case PetControlType.Pause: cmd.Insert(5, ControlPause); break;
                case PetControlType.Stop: cmd.Insert(5, ControlStop); break;
                case PetControlType.Continue: cmd.Insert(5, ControlContinue); break;
                case PetControlType.Clear: cmd.Insert(5, ControlClear); break;
            }

            switch (dst)
            {
                case PetControlDst.SystemMatrix: cmd.Insert(6, ControlDstSystemMatrix); break;
                case PetControlDst.Normalization: cmd.Insert(6, ControlDstNormalization); break;
                case PetControlDst.PatientScan: cmd.Insert(6, ControlDstPatientScan); break;
                case PetControlDst.DataCapture: cmd.Insert(6, ControlDstDataCapture); break;
                case PetControlDst.DataReconstruction: cmd.Insert(6, ControlDstDataReconstruction); break;
            }

            cmd.InsertRange(7, PackIdData(id));
            OnSend?.Invoke($"id={id}");
            OnSend?.Invoke($"controlType:{controlType.ToString()}");
            cmd.InsertRange(25, PackJsonData(buildInfo.ToJson()));
            if (controlType == PetControlType.New)
            {
                var options = new JsonSerializerOptions {
                    WriteIndented = true
                };
                OnSend?.Invoke(buildInfo.ToJsonWithFormat(options));
            }
            var revCmd = await SendAndRev(cmd.ToArray(), RevRealControl);
            var returnId = GetIdFromRevBytes(revCmd, 8, 25);
            return revCmd[5] == ResultSuccess;
        }


        public async Task<bool> ModuleControl(ModuleControlType moduleControlType, ModuleControlDst dst)
        {
            List<byte> cmd = [Head, Foot];
            cmd.InsertRange(1, CmdModuleControl.ToAsciiBytes());

            switch (moduleControlType)
            {
                case ModuleControlType.Active: cmd.Insert(5, CmdModuleControl_Active); break;
            }

            switch (dst)
            {
                case ModuleControlDst.DailyQC: cmd.Insert(6, CmdModuleControl_DailyQC); break;
                case ModuleControlDst.WeeklyQC: cmd.Insert(6, CmdModuleControl_WeeklyQC); break;
                case ModuleControlDst.SpatialRes: cmd.Insert(6, CmdModuleControl_SpatialRes); break;
                case ModuleControlDst.Sensitivity: cmd.Insert(6, CmdModuleControl_Sensitivity); break;
                case ModuleControlDst.Counting: cmd.Insert(6, CmdModuleControl_Counting); break;
                case ModuleControlDst.ImageQuality: cmd.Insert(6, CmdModuleControl_ImageQuality); break;
            }

            var revCmd = await SendAndRev(cmd.ToArray(), RevModuleControl);
            return revCmd[5] == ResultSuccess;
        }

        private async Task<byte[]> SendAndRev(byte[] cmd, string revCmd, bool callEvent = true)
        {
            if (_tcpClient == null)
                throw new Exception("disconnect");
            try
            {
                await _socketSlim.WaitAsync();
                var networkStream = _tcpClient.GetStream();
                var revBuffer = new byte[1024 * 1024];
                while (_tcpClient.Available > 0)
                {
                    networkStream.Read(revBuffer);
                } //清空接收缓冲区

                networkStream.Write(cmd);
                if (callEvent)
                {
                    OnSend?.Invoke(GetCmdFromRevBytes(cmd.ToArray()));
                    //OnSend?.Invoke(cmd.Aggregate("", (m, n) => m + " " + n.ToString("X2")));
                }
                List<byte> allRevBuffer = [];
                while (true)
                {
                    var count = networkStream.Read(revBuffer);
                    if (count == 0)
                    {
                        if (callEvent)
                        {
                            OnRev?.Invoke(GetCmdFromRevBytes(allRevBuffer.ToArray()));
                            //OnRev?.Invoke(allRevBuffer.Aggregate("", (m, n) => m + " " + n.ToString("X2")));
                        }
                        return allRevBuffer.ToArray();
                    }

                    allRevBuffer.AddRange(revBuffer[..count].ToArray());
                    var revCmdBuffer = GetRevCmdByRevBuffer(allRevBuffer);
                    if (revCmdBuffer != null && revCmdBuffer[1..5].ToAsciiString().Contains(revCmd))
                    {
                        if (callEvent)
                        {
                            OnRev?.Invoke(GetCmdFromRevBytes(revCmdBuffer.ToArray()));
                            //OnRev?.Invoke(revCmdBuffer.Aggregate("", (m, n) => m + " " + n.ToString("X2")));
                        }
                        return revCmdBuffer;
                    }
                }
            }
            finally
            {
                _socketSlim.Release();
            }
        }

        public async Task DisConnect()
        {
            await _socketSlim.WaitAsync();
            try
            {
                _tcpClient?.Close();
                _tcpClient?.Dispose();
                _tcpClient = null;
            }
            finally
            {
                _socketSlim.Release();
            }
        }

        private static byte[]? GetRevCmdByRevBuffer(List<byte> revBuffer)
        {
            while (true)
            {
                var startIndex = -1;
                var endIndex = -1;
                for (var i = 0; i < revBuffer.Count; i++)
                {
                    if (revBuffer[i] == Head)
                    {
                        startIndex = i;
                    }
                    else if (revBuffer[i] == Foot)
                    {
                        if (startIndex == -1)
                            continue;
                        endIndex = i;
                        break;
                    }
                }

                if (startIndex == -1 || endIndex == -1) //未找到一组包头包尾
                    break;
                var cmd = revBuffer.Skip(startIndex).Take(endIndex - startIndex + 1).ToList();
                revBuffer.RemoveRange(0, endIndex + 1);
                return cmd.ToArray();
            }

            return null;
        }

        private static string GetIdFromRevBytes(byte[] buffer, int startIndex, int endIndex)
        {
            if (buffer.Length < endIndex || startIndex < 0 || startIndex >= buffer.Length)
                return "";

            var idEndIndex = startIndex;
            while (idEndIndex < buffer.Length && idEndIndex < endIndex && buffer[idEndIndex] != 0)
            {
                idEndIndex++;
            }

            return buffer[startIndex..idEndIndex].ToAsciiString();
        }

        private static string GetJsonFromRevBytes(byte[] buffer, int startIndex)
        {
            if (startIndex < 0 || startIndex >= buffer.Length || buffer[startIndex] == 0)
                return "";

            var currentIndex = startIndex;
            while (currentIndex < buffer.Length && buffer[currentIndex] != 0)
            {
                currentIndex++;
            }

            return buffer[startIndex..(currentIndex - 1)].ToAsciiString();
        }

        private static string GetCmdFromRevBytes(byte[] buffer)
        {
            return buffer.Length > 5 ? buffer.Skip(1).Take(4).ToArray().ToAsciiString() : "None";
        }

        private static byte[] PackIdData(string id)
        {
            // 确保输入字符串不超过 17 个字符（最后一个字节保留给 '\0'）
            if (id.Length > 17)
                throw new ArgumentException("ID 最多为 17 个字符");
            var buffer = new byte[18]; // BYTE7 - BYTE24 共 18 字节
            // 使用 ASCII 编码将字符串写入 buffer
            var idBytes = Encoding.ASCII.GetBytes(id);
            Array.Copy(idBytes, buffer, idBytes.Length);
            buffer[idBytes.Length] = 0; // 添加字符串结尾标识 \0
            return buffer;
        }

        private static byte[] PackJsonData(string? json)
        {
            if (string.IsNullOrEmpty(json))
            {
                // 没有 JSON 字符串时，返回单个 0 字节
                return [0];
            }

            // 使用 ASCII 编码将 JSON 转换为字节数组，并在末尾添加 \0
            var jsonBytes = Encoding.ASCII.GetBytes(json);
            var buffer = new byte[jsonBytes.Length + 1]; // 多出一个字节存放 \0
            Array.Copy(jsonBytes, buffer, jsonBytes.Length);
            buffer[jsonBytes.Length] = 0; // 添加字符串结尾标识
            return buffer;
        }
    }
}
