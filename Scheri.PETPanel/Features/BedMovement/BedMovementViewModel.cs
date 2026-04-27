using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Scheri.PETPanel.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheri.PETPanel.Features
{
    public partial class BedMovementViewModel: ObservableObject
    {
        [ObservableProperty]
        private float _bedPosition;
         
        public BedMovementViewModel()
        {
            var dispatcherTime = new DispatcherTimer {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            dispatcherTime.Tick += DispatcherTime_Tick;
            dispatcherTime.Start();
        }

        private void DispatcherTime_Tick(object? sender, EventArgs e)
        {
            BedPosition = (395.659f - ScanTableManager.Instance.SDisplayPos) / 3.5f + 150; 
        }
    }
}
