using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Scheri.PETPanel.UIComponents;

public enum NavigateType
{
    Home,
    BedPosAdjust,
    Camera,
    Settings,
    Device,
    Logs
}

public record NavigateTypeMessage(Func<UserControl> ViewFactory){
    public Func<UserControl> GetViewFactory { get; } = ViewFactory ?? throw new ArgumentNullException(nameof(ViewFactory));
}

