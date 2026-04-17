using Scheri.PETPanel.Network.Contract;
using System.Threading.Tasks;

namespace Scheri.PETPanel.Interfaces
{
    public interface IDeviceInterface
    {
        Task<bool> Login();
        Task<StatusInfo> GetStates();
        Task GetConfig();
        Task SetConfig();
        Task RealControl();
        Task ModuleControl();
    }
}
