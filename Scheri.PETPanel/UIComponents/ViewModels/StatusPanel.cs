using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheri.PETPanel.UIComponents.ViewModels
{
    public partial class StatusPanel:ObservableObject
    {
        [ObservableProperty]
        private bool _isConnected = true;     
    }
}
