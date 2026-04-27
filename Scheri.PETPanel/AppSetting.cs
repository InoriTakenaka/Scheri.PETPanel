namespace Scheri.PETPanel;

public class AppSettings
{
    public WorkStationConfiguration Workstation { get; set; } = new();
    public PlcConfiguration Plc { get; set; } = new();
    public AppInfoConfiguration AppInfo { get; set; } = new();
}
public class WorkStationConfiguration
{
    public string Address { get; set; } = string.Empty;
    public int Port { get; set; }
}
public class PlcConfiguration
{
    public string Address { get; set; } = string.Empty;
    public int Port { get; set; }
}
public class AppInfoConfiguration
{
    public string Organization { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int IdleTime { get; set; }
}


