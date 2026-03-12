using Scheri.PETPanel.Network.Contract;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Scheri.PETPanel.Network;

public partial class PetProtocol : IDisposable
{
    private const string CmdLogin = "CM00";
    private const string CmdGetStatus = "CM01";
    private const string CmdGetConfig = "CM02";
    private const string CmdSetConfig = "CM03";
    private const string CmdHistoryControl = "CM04";
    private const string CmdRealControl = "CM05";
    private const string CmdModuleControl = "CM06";


    private const string RevLogin = "SM00";
    private const string RevGetStatus = "SM01";
    private const string RevGetConfig = "SM02";
    private const string RevSetConfig = "SM03";
    private const string RevHistoryControl = "SM04";
    private const string RevRealControl = "SM05";
    private const string RevModuleControl = "SM06";

    private const byte ConfigSystem = 0x30;
    private const byte ConfigDevice = 0x31;
    private const byte ConfigMatrix = 0x32;
    private const byte ConfigNormalization = 0x33;

    private const byte ControlNew = 0x30;
    private const byte ControlPause = 0x31;
    private const byte ControlStop = 0x32;
    private const byte ControlContinue = 0x33;
    private const byte ControlClear = 0x34;

    private const byte ControlDstSystemMatrix = 0x30;
    private const byte ControlDstNormalization = 0x31;
    private const byte ControlDstPatientScan = 0x32;
    private const byte ControlDstDataCapture = 0x33;
    private const byte ControlDstDataReconstruction = 0x34;


    private const byte CmdModuleControl_Active = 0x30;

    private const byte CmdModuleControl_DailyQC = 0x30;
    private const byte CmdModuleControl_WeeklyQC = 0x31;
    private const byte CmdModuleControl_SpatialRes = 0x32;
    private const byte CmdModuleControl_Sensitivity = 0x33;
    private const byte CmdModuleControl_Counting = 0x34;
    private const byte CmdModuleControl_ImageQuality = 0x35;

    private const byte Head = 0x0f;
    private const byte Foot = 0xff;

    private const byte ResultSuccess = 0x31;

    private string _destIp = "";
    private int _destPort;
    private int _destUdpPort;
    private List<byte> _buffer = [];

    public event Action? OnConnect;
    public event Action? OnDisconnect;
    public event Action<string>? OnRev;
    public event Action<string>? OnSend;

    private bool _isConnected;

    public bool IsConnected => _isConnected;

    private TcpClient? _tcpClient;

    private SemaphoreSlim _socketSlim = new(1, 1);

    private bool _autoConnect = true;

    public StatusInfo? StatusInfo { get; set; } = new();

    public object StatusInfoLocker { get; set; } = new();

    public void Dispose()
    {
        _autoConnect = false;
    }


    public enum PetConfigType
    {
        System,
        Device,
        Matrix,
        Normalization
    }

    public enum PetControlType
    {
        New,
        Pause,
        Stop,
        Continue,
        Clear
    }

    public enum PetControlDst
    {
        SystemMatrix,
        Normalization,
        PatientScan,
        DataCapture,
        DataReconstruction,
    }


    public enum ModuleControlType
    {
        Active,
    }

    public enum ModuleControlDst
    {
        DailyQC,
        WeeklyQC,
        SpatialRes,
        Sensitivity,
        Counting,
        ImageQuality,
    }
}

