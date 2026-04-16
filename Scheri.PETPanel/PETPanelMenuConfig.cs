using FluentIcons.Common;
using Scheri.PETPanel.UIComponents;
using System.Collections.Generic;

namespace Scheri.PETPanel;

public class PETPanelMenuConfig : IMenuConfig
{
    public List<Navigate> GetMenuItems()
    {
        return new List<Navigate> {
            new("Overview", Icon.Home, NavigateType.Home),
            new("Device", Icon.DeviceMeetingRoom, NavigateType.Device),
            new("Camera", Icon.ScanCamera, NavigateType.Camera),
            new("Settings", Icon.Settings, NavigateType.Settings),
            new("Logs", Icon.BookDatabase, NavigateType.Logs)
        };
    }
}

