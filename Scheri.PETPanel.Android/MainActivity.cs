using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Avalonia;
using Avalonia.Android;
using Scheri.PETPanel.Interfaces;
using Scheri.PETPanel.Services;
using Scheri.PETPanel.Utils;
using Splat;
using System;
using System.Runtime.Versioning;
using CoreApp = Scheri.PETPanel.App;

namespace Scheri.PETPanel.Android;

[SupportedOSPlatform("Android")]
[Activity(
    Label = "PETPanel",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",             
    MainLauncher = true,                 
    Exported = true,                     
    LaunchMode = LaunchMode.SingleInstance, 
    ConfigurationChanges = ConfigChanges.Orientation |
    ConfigChanges.ScreenSize | ConfigChanges.UiMode |
    ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]

[IntentFilter([Intent.ActionMain], Categories = new[] { Intent.CategoryHome, Intent.CategoryDefault })]
public class MainActivity : AvaloniaMainActivity<App> {
    protected override void OnCreate(Bundle? savedInstanceState) {
        LibVLCSharp.Shared.Core.Initialize();
        AppLogger.Initialize(new NLogAndroidConfig());

        CoreApp.OnExitApp = () => {
            try { StopLockTask(); } catch { }
            // exit the app 
            FinishAffinity();
            // call java runtime to ensure the app is fully closed
            Java.Lang.Runtime.GetRuntime()?.Exit(0);
        };

        base.OnCreate(savedInstanceState);
        bool defaultLauncher = IsMyselfDefaultLauncher();
        if (!defaultLauncher) {
            SetDefualtLauncher();
        }
    }

    public override void OnWindowFocusChanged(bool hasFocus) {
        base.OnWindowFocusChanged(hasFocus);

        AppLogger.Info($"build.VERSION.SdkInt = {Build.VERSION.SdkInt}");

        if (hasFocus && Build.VERSION.SdkInt >= BuildVersionCodes.R && Window != null) {
            global::AndroidX.Core.View.WindowCompat.SetDecorFitsSystemWindows(Window, false);

            var windowInsetsController = global::AndroidX.Core.View.WindowCompat.GetInsetsController(Window, Window.DecorView);
            if (windowInsetsController != null) {
                windowInsetsController.Hide(global::AndroidX.Core.View.WindowInsetsCompat.Type.StatusBars() |
                                            global::AndroidX.Core.View.WindowInsetsCompat.Type.NavigationBars());
                windowInsetsController.SystemBarsBehavior = global::AndroidX.Core.View.WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
            }
        } else if (!hasFocus) {
            try { SendBroadcast(new Intent(Intent.ActionCloseSystemDialogs)); } catch { }
        }
    }

    public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent? e) {
        if (keyCode == Keycode.Back) {
            return true;
        }
        return base.OnKeyDown(keyCode, e);
    }

    protected override void OnStart() {
        base.OnStart();
        try { StartLockTask(); } catch { }
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) {
        Locator.CurrentMutable.RegisterLazySingleton<INotificationService>(() => new AppNotificationService());
        Locator.CurrentMutable.RegisterLazySingleton<IConfigurationService>(() => new AppConfigurationService());
        if (!Avalonia.Controls.Design.IsDesignMode) {
            Locator.CurrentMutable.RegisterConstant<IPlatformSettingService>(new AndroidSettingService(this));
        }
        return base.CustomizeAppBuilder(builder)
            .With(new AndroidPlatformOptions {
                RenderingMode = [AndroidRenderingMode.Egl],
            })
            .With(new SkiaOptions {
                MaxGpuResourceSizeBytes = 64 * 1024 * 1024, // 64MB
            })
            .WithInterFont();
    }

    private void SetDefualtLauncher() {
        try {
            Intent intent = new(Settings.ActionHomeSettings);
            intent.SetFlags(ActivityFlags.NewTask);
            StartActivity(intent);
        } catch (Exception) {
#pragma warning disable CA1416 // Validate platform compatibility
            Intent intent = new(Settings.ActionManageDefaultAppsSettings);
#pragma warning restore CA1416 // Validate platform compatibility
            intent.SetFlags(ActivityFlags.NewTask);
            StartActivity(intent);
        }
    }

    private bool IsMyselfDefaultLauncher() {
        Intent intent = new(Intent.ActionMain);
        intent.AddCategory(Intent.CategoryHome);
        var resolveInfo = PackageManager?.ResolveActivity(intent, PackageInfoFlags.MatchDefaultOnly);
        if (resolveInfo?.ActivityInfo != null) {
            return PackageName != null && PackageName.Equals(resolveInfo.ActivityInfo.PackageName);
        }
        return false;
    }
}
