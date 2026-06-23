using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Scheri.PETPanel.Interfaces;
using Scheri.PETPanel.Utils;
using Splat;

namespace Scheri.PETPanel.Features {
    public partial class SettingsViewModel : ObservableObject {
        readonly IConfigurationService? _configurationService;
        readonly IPlatformSettingService? _platformSettingsService;
        [ObservableProperty]
        private AppSettings _settings = new() { };
        public SettingsViewModel() {
            _configurationService = Locator.Current.GetService<IConfigurationService>();
            _platformSettingsService = Locator.Current.GetService<IPlatformSettingService>();
            if (_configurationService != null) {
                Settings = _configurationService.AppSettings;
                AppLogger.Info("loading configurations...", nameof(SettingsViewModel));
            } else {
                AppLogger.Error("configuration load failed", nameof(SettingsViewModel));
            }
        }

        [RelayCommand]
        public void UpdateAppSettings() {
            _configurationService?.SaveAppSettings();           
        }

        [RelayCommand]
        public void ReplaceHomeApp() {
            _platformSettingsService?.RedirectToHomeSettings();
        }
    }
}
