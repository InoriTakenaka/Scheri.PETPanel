using Avalonia.Controls;
using Scheri.PETPanel.Network;
using Scheri.PETPanel.Utils;
using Scheri.PETPanel.ViewModels;
using Avalonia.Diagnostics;
using System.Reflection.Metadata;
using Avalonia.Controls.Documents;
using System.Threading.Tasks;
namespace Scheri.PETPanel.Views;

public partial class MainWindow : Window
{
    private readonly string _plc = "192.168.1.88";
    private readonly string _workstation = "192.168.1.80";

    //private readonly MainViewModel _viewModel = new();
    public MainWindow()
    {
        InitializeComponent();
        //connect to PLC
        ScanTableManager.Init(_plc);
        //connect to PetTools 
        PetToolsManager.Init("127.0.0.1");
        if(PetToolsManager.IsConnected)
            System.Diagnostics.Debug.WriteLine("Connected to PetTools successfully.");
        else
            System.Diagnostics.Debug.WriteLine("Failed to connect to PetTools.");
    }
}