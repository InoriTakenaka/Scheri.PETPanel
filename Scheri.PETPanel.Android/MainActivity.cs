using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Avalonia;
using Avalonia.Android;
using Scheri.PETPanel.Interfaces;
using Scheri.PETPanel.Utils;
using Splat;


namespace Scheri.PETPanel.Android;

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
        Locator.CurrentMutable.RegisterLazySingleton<IConfigurationService>(() => new AppConfigurationService());
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}
