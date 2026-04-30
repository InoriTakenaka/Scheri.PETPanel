using Scheri.PETPanel.Network.Contract;
using System.Threading.Tasks;

namespace Scheri.PETPanel.Interfaces
{
    public interface IDeviceInterface
    {
        Task<bool> Login(int timeout);
        Task GetStatusAsync(int timeoutMs = 5000);
        void StartPollingDeviceStatus(int intervalMs = 200);
        void InitializeDeviceService();
        Task GetDeviceConfig();
        Task SetDeviceConfig();
        Task RealControl();
        Task ModuleControl();
    }
}
