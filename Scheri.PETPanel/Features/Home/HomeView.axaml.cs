using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibVLCSharp;
using LibVLCSharp.Shared;

namespace Scheri.PETPanel.Features;

public partial class HomeView : UserControl
{
    private readonly HomeViewModel _viewModel = new HomeViewModel();
    public HomeView()
    {
        InitializeComponent();
        DataContext = _viewModel;
    }
}