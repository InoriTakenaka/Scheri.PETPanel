using Avalonia.Controls;
using Scheri.PETPanel.Network;
using Scheri.PETPanel.Utils;

namespace Scheri.PETPanel.Views;

public partial class MainWindow : Window
{
    private readonly string _ip = "192.168.1.88";
    public MainWindow()
    {
        InitializeComponent();
        //connect to PLC
        ScanTableManager.Init(_ip);
        //connect to PetTools 
        PetToolsManager.Init(_ip);
    }
}