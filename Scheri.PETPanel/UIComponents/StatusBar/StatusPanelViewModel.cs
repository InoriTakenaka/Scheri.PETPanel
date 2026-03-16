using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Scheri.PETPanel.UIComponents;

public partial class StatusPanelViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isConnected = true;
}

public class BoolToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Avalonia.Media.Brushes.Green : Avalonia.Media.Brushes.Red;
        }
        return Avalonia.Media.Brushes.Yellow;
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}