using Microsoft.Extensions.Configuration;
using Scheri.PETPanel.Interfaces;
using Scheri.PETPanel.Utils;
using System.IO;
using System.Text.Json;
using static System.Environment;

namespace Scheri.PETPanel.Android;

public class AppConfigurationService : IConfigurationService
{
    private readonly string _configFilePath;
    private readonly string _configFileName = "AppSettings.json";
    public AppSettings AppSettings { get; private set; } = new();

    public AppConfigurationService()
    {
        _configFilePath = Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData), _configFileName);
        LoadAppSettings();
    }
    public void LoadAppSettings()
    {
        if (!File.Exists(_configFilePath))
        {
            AppSettings = new AppSettings { };
            return;
        }
        var json = File.ReadAllText(_configFilePath);
        AppSettings = JsonSerializer.Deserialize(json, AppJsonContext.Default.AppSettings) ?? new AppSettings { };
    }

    public void SaveAppSettings()
    {
        var json = JsonSerializer.Serialize(AppSettings, AppJsonContext.Default.AppSettings);
        File.WriteAllText(_configFilePath, json);
    }
}
