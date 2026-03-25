using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Scheri.PETPanel.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Scheri.PETPanel.ViewModels
{
    public partial class SystemOverviewViewModel : ViewModelBase
    {
        public ObservableCollection<LogRecord> LogRecords { get; } = [];     
        public SystemOverviewViewModel() 
        {
            LogRecords.Add(new LogRecord(DateTime.Now, NLog.LogLevel.Info, "test log 1"));
            AppLogger.Info("test log 2");
        }


    }
}
