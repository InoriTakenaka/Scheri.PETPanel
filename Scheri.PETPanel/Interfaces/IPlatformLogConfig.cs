using NLog.Config;

namespace Scheri.PETPanel.Interfaces
{
    public interface IPlatformLogConfig
    {
        LoggingConfiguration GetLoggingConfiguration();
    }
}
