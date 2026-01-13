using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Scheri.PETPanel.UIComponents.ViewModels
{
    public class Navigate(string Text, string IconPath,string Route)
    {
        public string Text { get; set; } = Text;
        public string IconPath { get; set; } = IconPath;
        public string Route { get; set; } = Route;
    }
    public class MenuControlViewModel
    {        
        public ObservableCollection<Navigate> MenuItems { get; set; } = [];
        public ICommand NavigateCommand => new RelayCommand<Navigate>(navigate => {
             
        });
    }
}
