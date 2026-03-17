using Scheri.PETPanel.Network;
using Scheri.PETPanel.Network.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scheri.PETPanel.Utils;

public static class PetToolsManager
{
    private static PetProtocol? _petTools;
    private static readonly int RealControlTimeoutCount = 10;
    private static bool CanScan => _petTools?.StatusInfo?.collect_state <= 0;
    private static bool CanRecon => _petTools?.StatusInfo?.recon_state <= 0;

    public static StatusInfo? StatusInfo => _petTools?.StatusInfo;
    public static bool IsConnected => _petTools is { IsConnected: true };

    public static StudyJson? StudyInfo = null;

    public static void Init(string ip, int port = 8066)
    {
        _petTools = new PetProtocol();
        _petTools.Connect(ip, port);
    }

    private static void CheckInit()
    {
        if (_petTools == null)
            throw new Exception("重建服务器连接尚未初始化");
        if (_petTools.IsConnected == false)
            throw new Exception("重建服务器连接已断开");
    }

    /// <summary>
    /// 等待某个状态10s
    /// </summary>
    private static async Task<bool> WaitState(Func<bool> waitAction, CancellationToken cancelToken)
    {
        for (var i = 0; i < RealControlTimeoutCount; i++)
        {
            await Task.Delay(1000, cancelToken);
            if (waitAction.Invoke())
                return true;
        }

        return false;
    }
}
