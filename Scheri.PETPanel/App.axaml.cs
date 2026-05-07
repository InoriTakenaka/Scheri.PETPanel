using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Scheri.PETPanel.UIComponents;
using Scheri.PETPanel.Utils;
using Scheri.PETPanel.ViewModels;
using Scheri.PETPanel.Views;
using System;
using System.Threading.Tasks;

namespace Scheri.PETPanel;

public partial class App : Application
{
    public static WindowNotificationManager? NotificationManager;
    public static IServiceProvider? ServiceProvider { get; private set; }
    public static Action? OnExitApp { get; set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ConfigureServices();
        InitNetworkConnections();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static void ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMenuConfig, PETPanelMenuConfig>();
        services.AddTransient<MenuControlViewModel>();
        services.AddLogging(loggingBuilder => {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddNLog(new NLogProviderOptions {
                IncludeScopes = true
            });
        });
        ServiceProvider = services.BuildServiceProvider();
    }

    private void InitNetworkConnections()
    {

        string plc = "192.168.1.88";

        Task.Run(async () => {
            try
            {
                ScanTableManager.Init(plc);
                await DeviceManager.Instance.InitializeAsync();
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex.Message, nameof(App));
            }
        });
    }

    public static void ShutdownApplication() { 
        if(Current?.ApplicationLifetime is IControlledApplicationLifetime desktop) {
            desktop.Shutdown();
        } else if(Current?.ApplicationLifetime is ISingleViewApplicationLifetime singleView) {
            OnExitApp?.Invoke();
        }
    }
}