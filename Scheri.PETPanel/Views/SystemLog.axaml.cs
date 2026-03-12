using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Scheri.PETPanel.ViewModels;

namespace Scheri.PETPanel;

public partial class SystemLog : UserControl
{
    private readonly SystemOverviewViewModel _viewModel = new();
    public SystemLog()
    {
        InitializeComponent();
        DataContext = _viewModel;
    }
}