using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;


namespace Scheri.PETPanel.Android;

[Activity(
    Label = "Scheri.PETPanel.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        try
        {
            Java.Lang.JavaSystem.LoadLibrary("vlc");
        }
        catch(Java.Lang.UnsatisfiedLinkError e) {
            System.Diagnostics.Debug.WriteLine($"Native library load failed: {e.Message}");
        }
        LibVLCSharp.Shared.Core.Initialize();
        base.OnCreate(savedInstanceState);
    }
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}
