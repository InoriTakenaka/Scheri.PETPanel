using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Avalonia;
using Avalonia.Android;
using Scheri.PETPanel.Interfaces;
using Scheri.PETPanel.Services;
using Scheri.PETPanel.Utils;
using Splat;
using System.Runtime.Versioning;
using CoreApp = Scheri.PETPanel.App;

namespace Scheri.PETPanel.Android;

[SupportedOSPlatform("Android")]
[Activity(
    Label = "PETPanel",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        LibVLCSharp.Shared.Core.Initialize();
        AppLogger.Initialize(new NLogAndroidConfig());
        
        CoreApp.OnExitApp = () => {
            // exit the app 
            FinishAffinity();
            // call java runtime to ensure the app is fully closed
            Java.Lang.Runtime.GetRuntime()?.Exit(0);
        };

        base.OnCreate(savedInstanceState);
    }
    public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent? e)
    {
        if (keyCode == Keycode.Back)
        {
            return true;
        }
        return base.OnKeyDown(keyCode, e);
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        Locator.CurrentMutable.RegisterLazySingleton<INotificationService>(() => new AppNotificationService());
        Locator.CurrentMutable.RegisterLazySingleton<IConfigurationService>(() => new AppConfigurationService());
        return base.CustomizeAppBuilder(builder)
            .With(new AndroidPlatformOptions {
                RenderingMode = [AndroidRenderingMode.Egl],
            })
            .With(new SkiaOptions { 
                MaxGpuResourceSizeBytes = 64 * 1024 * 1024, // 64MB
            })
            .WithInterFont();
    }
}
