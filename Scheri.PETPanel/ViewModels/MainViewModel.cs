using CommunityToolkit.Mvvm.ComponentModel;

namespace Scheri.PETPanel.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
