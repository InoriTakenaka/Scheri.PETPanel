using Avalonia.Controls;
using FluentIcons.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Scheri.PETPanel.UIComponents;

public partial class MenuControl : UserControl
{
    public MenuControl()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider!.GetService<MenuControlViewModel>();
    }
}