using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Scheri.PETPanel.Features;
using Scheri.PETPanel.UIComponents;
using Scheri.PETPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Scheri.PETPanel;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}

public static class ViewRoute
{
    public static readonly Dictionary<NavigateType, Func<UserControl>> ViewRoutes = new() {
        {NavigateType.Home, () => new HomeView()},
        {NavigateType.BedPosAdjust, () => new BedMovementView()},
        {NavigateType.Camera, () => new CameraPlay()},
        {NavigateType.Settings, () => new SettingsView()}
    };
}