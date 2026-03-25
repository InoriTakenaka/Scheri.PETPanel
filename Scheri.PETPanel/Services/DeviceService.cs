using Scheri.PETPanel.Interfaces;
using Scheri.PETPanel.Network.Contract;
using Scheri.PETPanel.Utils;
using System.Threading;
using System.Threading.Tasks;


namespace Scheri.PETPanel.Services
{
    public class DeviceService
       // :IDeviceInterface
    {
        public bool IsConnected => DeviceManager.Instance.IsConnected;
        public async Task<bool> ConnectAsync(CancellationToken ct)
        {
            await DeviceManager.Instance.InitializeAsync(ct);
            return IsConnected;
        }
    }
}
