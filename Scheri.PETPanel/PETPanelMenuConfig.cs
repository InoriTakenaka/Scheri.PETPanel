using FluentIcons.Common;
using Scheri.PETPanel.UIComponents;
using System.Collections.Generic;

namespace Scheri.PETPanel;

public class PETPanelMenuConfig : IMenuConfig
{
    public List<Navigate> GetMenuItems()
    {
        return new List<Navigate> {
            new("Overview", Icon.Home, typeof(SystemOverview)),
            new("Device", Icon.DeviceMeetingRoom, typeof(DeviceInfo)),
            new("Camera", Icon.ScanCamera, typeof(CameraInfo)),
            new("Settings", Icon.Settings, typeof(SystemSetting)),
            new("Logs", Icon.BookDatabase, typeof(SystemLog))
        };
    }
}

