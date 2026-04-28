using Scheri.PETPanel.Interfaces;
using Scheri.PETPanel.Utils;
using Splat;
using System;
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
        AppLogger.Info($"config_path:{_configFilePath}",nameof(AppConfigurationService));
        LoadAppSettings();
    }
    public void LoadAppSettings()
    {
        if (!File.Exists(_configFilePath))
        {
            AppSettings = new AppSettings { };
            SaveAppSettings();
            return;
        }
        try
        {
            var json = File.ReadAllText(_configFilePath);
            AppSettings = JsonSerializer.Deserialize(json, AppJsonContext.Default.AppSettings) ?? new AppSettings { };
        }
        catch (Exception)
        {
            AppSettings = new AppSettings { };

        }
    }

    public void SaveAppSettings()
    {
        try
        {
            var json = JsonSerializer.Serialize(AppSettings, AppJsonContext.Default.AppSettings);
            File.WriteAllText(_configFilePath, json);
            AppLogger.Info($"Configuration Updated: {json}", nameof(SaveAppSettings));
            var notify = Locator.Current.GetService<INotificationService>();
            notify?.Show(
                "App Settings",
                "App configuration update success!",
                TimeSpan.FromMilliseconds(300)
            );
        }
        catch (Exception ex)
        {   
            System.Diagnostics.Debug.WriteLine(ex.Message);
            AppLogger.Error(ex.Message, nameof(SaveAppSettings));
        }
    }
}
