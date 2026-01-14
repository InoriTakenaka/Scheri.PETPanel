using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentIcons.Common;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Scheri.PETPanel.UIComponents.ViewModels
{
    public class Navigate(string Text, Icon Icon,string Route)
    {
        public string Text { get; set; } = Text;
        public Icon Icon { get; set; } = Icon;
        public string Route { get; set; } = Route;
    }
    public partial class MenuControlViewModel:ObservableObject
    {
        [ObservableProperty]
        private StatusPanel _statusPanel = new();
        public ObservableCollection<Navigate> MenuItems { get; set; } = [];
    }
}
