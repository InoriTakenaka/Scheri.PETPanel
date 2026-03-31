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
        if (PetToolsManager.StatusInfo != null)
        {
            var lastedState = PetToolsManager.StatusInfo.Cache.Take(3);
            var statusInfos = lastedState as StatusInfo[] ?? lastedState.ToArray();
            if (statusInfos.Length == 3)
            {
                var promptCountSum = statusInfos.LastOrDefault().collect_prompt_count -
                                     statusInfos.FirstOrDefault().collect_prompt_count;
                var randomCountSum = statusInfos.LastOrDefault().collect_random_count -
                                     statusInfos.FirstOrDefault().collect_random_count;
                var duration =
                    (statusInfos.LastOrDefault().last_update_ts - statusInfos.FirstOrDefault().last_update_ts) / 1000;

                PromptCountValue = promptCountSum / duration / 1000f;

                if (PromptCountValue < 0)
                    PromptCountValue = 0;

                PromptCount = PromptCountValue.ToString("0.00") + "K";

            }

            CrystalAvgTemprature = PetToolsManager.StatusInfo.temp_avg.ToString("0.0") + "℃";
            IsConnected = PetToolsManager.IsConnected && ScanTableManager.Instance.IsConnected;
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