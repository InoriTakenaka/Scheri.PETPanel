using Avalonia.Threading;
using Scheri.PETPanel.Interfaces;
using Avalonia.Controls.Notifications;
using System;

namespace Scheri.PETPanel.Services
{
    public class AppNotificationService : INotificationService
    {
        public void Show(string title, string message, TimeSpan? duration = null)
        {
            Dispatcher.UIThread.Post(() => {
                var expiration = duration ?? TimeSpan.FromMilliseconds(500);
                var notification = new Notification(
                    title,
                    message,
                    NotificationType.Success,
                    expiration
                    );

                App.NotificationManager?.Show(notification);
            });
        }
    }
}
