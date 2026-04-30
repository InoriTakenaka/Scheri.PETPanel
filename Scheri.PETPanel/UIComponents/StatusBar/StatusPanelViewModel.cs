using Avalonia.Data.Converters;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Scheri.PETPanel.Network.Contract;
using Scheri.PETPanel.Utils;
using System;
using System.Linq;

namespace Scheri.PETPanel.UIComponents;

public partial class StatusPanelViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isConnected = true;
    [ObservableProperty]
    private string _CrystalAvgTemprature = string.Empty;
    [ObservableProperty]
    private string _promptCount = string.Empty;
    public double PromptCountValue { get; set; }

    public StatusPanelViewModel()
    {
        var timer = new DispatcherTimer {
            Interval = TimeSpan.FromSeconds(1)
        };
        timer.Tick += (sender, args) => {
            // TestData();
            UpdateStatus();
        };
        timer.Start();
        //TestData();
        UpdateStatus();
    }

    void TestData()
    {
        CrystalAvgTemprature = "34.7 ℃";
        PromptCount = "100.13K";
        IsConnected = true;
    }

    void UpdateStatus()
    {
        var cachedStatus = DeviceManager.Instance.Cache.ToArray();
        int count = cachedStatus.Length;
        if (count > 0)
        {
            var latest = cachedStatus[count - 1];
            CrystalAvgTemprature = latest.temp_avg.ToString("0.0") + " ℃";
            IsConnected = DeviceManager.Instance.IsConnected;

            if (count >= 3)
            {
                var first = cachedStatus[count - 3];
                var last = cachedStatus[count - 1];
                var duration = (last.last_update_ts - first.last_update_ts) / 1000.0;
                if (duration > 0)
                {
                    var promptCountSum = last.collect_prompt_count - first.collect_prompt_count;
                    PromptCountValue = (float)(promptCountSum / duration / 1000.0);
                    if (PromptCountValue < 0) PromptCountValue = 0;
                    PromptCount = PromptCountValue.ToString("0.00") + " K/s";
                }
            }
            else
            {
                PromptCount = "0.00 K/s";
            }
            IsConnected = DeviceManager.Instance.IsConnected;
        }
        else
        {
            IsConnected = false;
            CrystalAvgTemprature = "N/A";
            PromptCount = "N/A";
        }
    }
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