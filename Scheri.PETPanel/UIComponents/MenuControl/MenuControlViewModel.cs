using CommunityToolkit.Mvvm.ComponentModel;
using FluentIcons.Common;
using System.Collections.ObjectModel;

namespace Scheri.PETPanel.UIComponents;

public class Navigate(string Text, Icon Icon,string Route)
{
    public string Text { get; set; } = Text;
    public Icon Icon { get; set; } = Icon;
    public string Route { get; set; } = Route;
}
public partial class MenuControlViewModel:ObservableObject
{
    [ObservableProperty]
    private StatusPanelViewModel _statusPanel = new();
    public ObservableCollection<Navigate> MenuItems { get; set; } = [];
}
