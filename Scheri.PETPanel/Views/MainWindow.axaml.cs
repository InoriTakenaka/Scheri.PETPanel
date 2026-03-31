using Avalonia.Controls;
using Scheri.PETPanel.Network;
using Scheri.PETPanel.Utils;
using Scheri.PETPanel.ViewModels;

namespace Scheri.PETPanel.Views;

public partial class MainWindow : Window
{
    private readonly string _ip = "192.168.1.88";
    private readonly MainViewModel _viewModel = new();
    public MainWindow()
    {
        InitializeComponent();
        //connect to PLC
        ScanTableManager.Init(_ip);
        //connect to PetTools 
        PetToolsManager.Init(_ip);

        DataContext = _viewModel;
    }
}