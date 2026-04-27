using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Scheri.PETPanel.Utils;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Scheri.PETPanel.Features;

public partial class BedMovementView : UserControl
{
    private bool _isClosed = false;
    private BedMovementViewModel _viewModel = new();
    public BedMovementView()
    {
        InitializeComponent();
        Forward.AddHandler(PointerPressedEvent, Forward_PointerPressed, handledEventsToo: true);
        Forward.AddHandler(PointerReleasedEvent, Forward_PointerReleased, handledEventsToo: true);
        Backward.AddHandler(PointerPressedEvent, Backward_PointerPressed, handledEventsToo: true);
        Backward.AddHandler(PointerReleasedEvent, Backward_PointerReleased, handledEventsToo: true);

        Loaded+= BedMovementView_Loaded;   
        Unloaded += BedMovementView_Unloaded;
        DataContext = _viewModel;
    }

    private void BedMovementView_Unloaded(object? sender, RoutedEventArgs e)
    {
        _isClosed = true;
    }

    private async void BedMovementView_Loaded(object? sender, RoutedEventArgs e)
    {
        _ = UpdateBedPositionLoopAsync();
    }

    private async Task UpdateBedPositionLoopAsync()
    {
        while (!_isClosed)
        {
            try
            {
                await Task.Delay(100);
                await Dispatcher.UIThread.InvokeAsync(() => {
                    BedPosition.Text = ((int)Math.Round(ScanTableManager.Instance.SActualPos, MidpointRounding.AwayFromZero))
                                     .ToString(CultureInfo.InvariantCulture);
                });
            }
            catch 
            {
                //ignore the exception
            }
        }
    }

    private async void Backward_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        try
        {
            // only effective when debug compiled, so it won't affect release performance
            System.Diagnostics.Debug.WriteLine("Backward_PointerPressed");

            ScanTableManager.Instance.CheckFloat();
            await ScanTableManager.Instance.ToBBackward(true);
        }
        catch (Exception ex) { 
            AppLogger.Error(ex.Message, nameof(Backward_PointerPressed));
        }
    }

    private async void Backward_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Backward_PointerReleased");
            ScanTableManager.Instance.CheckFloat();
            await ScanTableManager.Instance.ToBBackward(false);
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex.Message, nameof(Backward_PointerReleased));
        }
    }

    private async void Forward_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Forward_PointerPressed");
            ScanTableManager.Instance.CheckFloat();
            await ScanTableManager.Instance.ToBForward(true);
        }
        catch (Exception ex)
        {
            AppLogger.Error(ex.Message, nameof(Forward_PointerPressed));
        }
    }

    private async void Forward_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Forward_PointerReleased");
            ScanTableManager.Instance.CheckFloat();
            await ScanTableManager.Instance.ToBForward(false);
        }
        catch (Exception ex)
        {
             AppLogger.Error(ex.Message, nameof(Forward_PointerReleased));  
        }
    }

}