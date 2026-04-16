using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FluentIcons.Common;
using System;
using System.Collections.ObjectModel;

namespace Scheri.PETPanel.UIComponents;

public partial class Navigate(string Text, Icon Icon, NavigateType Route):ObservableObject
{
    public string Text { get; set; } = Text;
    public Icon Icon { get; set; } = Icon;
    public NavigateType Type { get; set; } = Route;
    [ObservableProperty]
    private bool _isActive;
}
public partial class MenuControlViewModel:ObservableObject
{
    public ObservableCollection<Navigate> MenuItems { get; set; }
    public void NavigateToView(Navigate? navigate)
    {
        if (navigate == null) return;
        foreach(var item in MenuItems)
        {
            item.IsActive = (item == navigate);
        }
        if (!ViewRoute.ViewRoutes.TryGetValue(navigate.Type, out var viewFactory)) return;
        WeakReferenceMessenger.Default.Send(new NavigateTypeMessage(viewFactory));
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
