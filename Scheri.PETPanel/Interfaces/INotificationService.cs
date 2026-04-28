using System;

namespace Scheri.PETPanel.Interfaces;

public interface INotificationService
{
    void Show(string title, string message, TimeSpan? duration = null);
}

