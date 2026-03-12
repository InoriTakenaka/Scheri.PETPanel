using NModbus;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Scheri.PETPanel.Utils;

namespace CommonLib.Protocol.ScanTable;

public class ScanTable : IDisposable
{
    private TcpClient? _client; // 替换为实际 IP 和端口
    private readonly ModbusFactory _factory = new();
    private IModbusMaster? _master;

    public bool BForward { get; set; }
    public bool BCancel { get; set; }
    public bool BBackward { get; set; }
    public bool BAutoForward { get; set; }
    public bool BFloat { get; set; } = true;


    public double SFaultId { get; set; }

    public float SActualSpeed { get; set; }
    public float SActualPos { get; set; }
    public float SDisplayPos { get; set; }
    public float SAutoForwardPos { get; set; }


    public event Func<bool, bool> OnSStopChanged;

    private bool _sstop;

    /// <summary>
    /// 急停
    /// </summary>
    public bool SStop {
        get => _sstop;
        set {
            if (_sstop != value && value)
            {
                if (!OnSStopChanged.Invoke(value))
                    return;
            }
            _sstop = value;
        }
    }


    private bool _isListening = true;

    private string _ip = "";
    private int _port = 502;

    public bool IsConnected = false;

    public ScanTable()
    {
        StartListen();
    }

    public void SetIp(string ip, int port = 502)
    {
        _ip = ip;
        _port = port;
    }

    private void Connect(string ip, int port = 502)
    {
        _ip = ip;
        _port = port;
        lock (this)
        {
            _client?.Dispose();
            _client = new TcpClient(ip, port);
            _master?.Dispose();
            _master = _factory.CreateMaster(_client);
        }
    }

    private void StartListen()
    {
        Task.Run(() => {
            while (_isListening)
            {
                Thread.Sleep(100);
                try
                {
                    if (_master != null)
                    {
                        var coils = _master.ReadCoils(1, 0, 32);
                        BForward = coils[12];
                        BCancel = coils[13];
                        BBackward = coils[14];
                        BAutoForward = coils[15];
                        BFloat = coils[27];

                        SStop = coils[9];

                        var registers = _master.ReadHoldingRegisters(1, 0, 16);
                        SFaultId = registers[2];
                        SActualSpeed = ConvertRegistersToFloat(registers[4], registers[5]);
                        SActualPos = ConvertRegistersToFloat(registers[6], registers[7]);
                        SDisplayPos = ConvertRegistersToFloat(registers[8], registers[9]);
                        SAutoForwardPos = ConvertRegistersToFloat(registers[10], registers[11]);
                        IsConnected = true;
                    }
                    else
                    {
                        Connect(_ip, _port);
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        Connect(_ip, _port);
                    }
                    catch (Exception)
                    {
                        IsConnected = false;
                    }
                }
            }
        });
    }

    private static float ConvertRegistersToFloat(ushort highRegister, ushort lowRegister)
    {
        var bytes = new byte[4];

        // 将高位寄存器和低位寄存器分别拆分为字节，并按正确顺序填充到数组中
        bytes[0] = (byte)(highRegister & 0xFF); // D
        bytes[1] = (byte)(highRegister >> 8); // C
        bytes[2] = (byte)(lowRegister & 0xFF); // B
        bytes[3] = (byte)(lowRegister >> 8); // A

        return (float)Math.Round(BitConverter.ToSingle(bytes, 0), 4);
    }

    public bool CheckFloat()
    {
        return BFloat;
    }


    public async Task<bool> ToBForward(bool val)
    {
        try
        {
            await _master!.WriteSingleCoilAsync(1, 12, val);
            Debug.Write($"ToBForward:{val}");
            return true;
        }
        catch (Exception ex)
        {
            //MsLogger.Error(ex, "ToBForward");
            return false;
        }
    }

    public async Task<bool> ToBBackward(bool val)
    {
        try
        {
            await _master!.WriteSingleCoilAsync(1, 14, val);
            Debug.Write($"ToBBackward:{val}");
            return true;
        }
        catch (Exception ex)
        {
            //MsLogger.Error(ex, "ToBBackward");
            return false;
        }
    }

    public async Task<bool> ToBCancel()
    {
        try
        {
            await _master!.WriteSingleCoilAsync(1, 13, false);
            Debug.Write($"ToBCancel:false");
            await Task.Delay(100);

            await _master!.WriteSingleCoilAsync(1, 13, true);
            Debug.Write($"ToBCancel:true");
            await Task.Delay(100);

            await _master!.WriteSingleCoilAsync(1, 13, false);
            Debug.Write($"ToBCancel:false");
            return true;
        }
        catch (Exception ex)
        {
            //MsLogger.Error(ex, "ToBCancel");
            return false;
        }
    }

    public async Task<bool> ToBAutoForward(float pos)
    {
        try
        {
            Debug.Write($"ToBAutoForward:{pos}");
            byte[] floatBytes = BitConverter.GetBytes(pos);
            // 将 float 拆成两个 16 位寄存器
            ushort[] registers = new ushort[2];
            registers[0] = BitConverter.ToUInt16(floatBytes, 0); // 高位（地址 10）
            registers[1] = BitConverter.ToUInt16(floatBytes, 2); // 低位（地址 11）

            await _master!.WriteMultipleRegistersAsync(1, 10, registers);
            Debug.Write($"WriteMultipleRegistersAsync:{pos}");
            await Task.Delay(100);

            await _master!.WriteSingleCoilAsync(1, 15, false);
            Debug.Write($"ToBAutoForward:false");
            await Task.Delay(100);

            await _master!.WriteSingleCoilAsync(1, 15, true);
            Debug.Write($"ToBAutoForward:true");
            await Task.Delay(100);

            await _master!.WriteSingleCoilAsync(1, 15, false);
            Debug.Write($"ToBAutoForward:false");
            return true;
        }
        catch (Exception ex)
        {
            //MsLogger.Error(ex, "ToBAutoForward");
            return false;
        }
    }



    public void Dispose()
    {
        _isListening = false;
        lock (this)
        {
            _client?.Dispose();
            _master?.Dispose();
        }
    }
}
