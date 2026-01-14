using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;

namespace Scheri.PETPanel.UIComponents.Views;

public partial class AppTitle : UserControl
{
    private readonly DispatcherTimer _timer;
    public AppTitle()
    {
        InitializeComponent();
        _timer = new DispatcherTimer {
            Interval = TimeSpan.FromSeconds(1)
        };

        _timer.Tick += UpdateCurrentTime;

        Loaded += (s, e) => {
            UpdateCurrentTime(s, e);
            _timer.Start();
        };

        Unloaded += (s, e) => {
            _timer.Stop();
            _timer.Tick -= UpdateCurrentTime;
        };
    }

    private void UpdateCurrentTime(object? o,EventArgs e)
    {
        SystemTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}