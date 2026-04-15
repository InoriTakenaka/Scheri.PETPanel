using CommonLib.Protocol.ScanTable;
using System.Threading.Tasks;

namespace Scheri.PETPanel.Utils;

public static class ScanTableManager
{
    public static ScanTable Instance { get; } = new();
    public static async Task<bool> IsConnected() => Instance.IsConnected;
    public static void Init(string ip, int port = 502)
    {
        Instance.SetIp(ip, port);
    }
}
