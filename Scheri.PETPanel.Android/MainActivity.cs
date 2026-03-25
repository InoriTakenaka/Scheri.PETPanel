using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Avalonia;
using Avalonia.Android;
using Scheri.PETPanel.Utils;
using System;
using System.IO;


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
        AppLogger.Initialize(new NLogAndroidConfig());
        /*
        try
        {
            // Java.Lang.JavaSystem.LoadLibrary("vlc");
        }
        catch (Java.Lang.UnsatisfiedLinkError e)
        {
            System.Diagnostics.Debug.WriteLine($"Native library load failed: {e.Message}");
        }
        // LibVLCSharp.Shared.Core.Initialize();
        */

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
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}
