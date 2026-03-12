using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Scheri.PETPanel.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Scheri.PETPanel.ViewModels
{
    public class SystemOverviewViewModel : ViewModelBase
    {
        public ObservableCollection<LogRecord> LogRecords { get; } = [];

        public SystemOverviewViewModel() 
        {
            LogRecords.Add(new LogRecord(DateTime.Now, NLog.LogLevel.Info, "test log 1"));
            MsLogger.Log(new LogRecord(DateTime.Now, NLog.LogLevel.Info, "test log 2"));
        }
    }
}
