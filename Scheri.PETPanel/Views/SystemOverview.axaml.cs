using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Scheri.PETPanel.ViewModels;

namespace Scheri.PETPanel;

public partial class SystemOverview : UserControl
{
    private readonly SystemOverviewViewModel _viewModel = new();
    public SystemOverview()
    {
        InitializeComponent();
        DataContext = _viewModel;
    }
}