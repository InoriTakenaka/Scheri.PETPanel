namespace Scheri.PETPanel.Interfaces;

public interface IConfigurationService
{
    AppSettings AppSettings { get; }
    void LoadAppSettings();
    void SaveAppSettings();
}

