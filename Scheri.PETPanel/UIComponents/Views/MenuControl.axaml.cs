using Avalonia;
using Avalonia.Controls;
using FluentIcons.Common;
using Scheri.PETPanel.UIComponents.ViewModels;

namespace Scheri.PETPanel.UIComponents.Views;

public partial class MenuControl : UserControl
{
    private readonly MenuControlViewModel _menuViewModel =new();
    public MenuControl()
    {
        InitializeComponent();
        InitMenuItems();
        DataContext = _menuViewModel;
    }

    public void InitMenuItems()
    {        
        _menuViewModel.MenuItems.Add(new("Overview", Icon.Home,"/Views/SystemOverview"));
        _menuViewModel.MenuItems.Add(new("Device", Icon.DeviceMeetingRoom, "/Views/DeviceControl"));
        _menuViewModel.MenuItems.Add(new("Camera", Icon.ScanCamera, "/Views/Camera"));
        _menuViewModel.MenuItems.Add(new("Settings", Icon.Settings, "/Views/SystemSettings"));
        _menuViewModel.MenuItems.Add(new("Logs", Icon.BookDatabase, "/Views/Log"));
    }
}