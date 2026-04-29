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
        /// <summary>
        /// Current position of the bed.
        /// Max: 380.0  Min:4.9
        /// </summary>
        [ObservableProperty]
        private float _bedPosition;
        private const int BED_POSITION_OFFSET = 40;
         
        public BedMovementViewModel()
        {
            var dispatcherTime = new DispatcherTimer {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            dispatcherTime.Tick += DispatcherTime_Tick;
            dispatcherTime.Start();
        }

        /// <summary>
        /// Handles the timer tick event to update the bed position based on the current scan table display position.
        /// </summary>
        /// <remarks>This method is intended to be used as an event handler for a timer's Tick event. It
        /// recalculates and clamps the bed position on each tick.</remarks>
        /// <param name="sender">The source of the event, typically the timer that triggered the tick.</param>
        /// <param name="e">An object that contains the event data.</param>
        private void DispatcherTime_Tick(object? sender, EventArgs e)
        {
            float calculated = (386f - ScanTableManager.Instance.SDisplayPos) / 3.5f + BED_POSITION_OFFSET;
            BedPosition = Math.Clamp(calculated, 0, 400);
             
        }
    }
}
