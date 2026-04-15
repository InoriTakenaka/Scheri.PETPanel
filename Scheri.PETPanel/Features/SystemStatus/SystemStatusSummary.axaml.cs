using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Scheri.PETPanel.UIComponents;

namespace Scheri.PETPanel.Features;

public partial class SystemStatusSummary : UserControl
{
    private readonly StatusPanelViewModel _viewModel = new();
    public SystemStatusSummary()
    {
        InitializeComponent();
        DataContext = _viewModel;
    }
}