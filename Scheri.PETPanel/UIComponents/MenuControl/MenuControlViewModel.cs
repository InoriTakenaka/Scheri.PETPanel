using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FluentIcons.Common;
using System;
using System.Collections.ObjectModel;

namespace Scheri.PETPanel.UIComponents;

public class Navigate(string Text, Icon Icon,Type Route)
{
    public string Text { get; set; } = Text;
    public Icon Icon { get; set; } = Icon;
    public Type Route { get; set; } = Route;
}
public partial class MenuControlViewModel:ObservableObject
{
    public ObservableCollection<Navigate> MenuItems { get; set; }
    public void NavigateToView(Navigate? navigate)
    {
        if (navigate == null) return;
        WeakReferenceMessenger.Default.Send(new NavigateTypeMessage(navigate.Route));
    }
    public MenuControlViewModel(IMenuConfig menuConfig)
    {
        MenuItems = [];
        foreach(var item in menuConfig.GetMenuItems())
        {
            MenuItems.Add(item);
        }
    }
}
