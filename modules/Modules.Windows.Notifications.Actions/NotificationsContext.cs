// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.Notifications.Actions;

public sealed class NotificationsContext
{
    public NotificationsContext(INotificationService notificationService, IClock clock)
    {
        NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        Clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public IClock Clock { get; }
    public INotificationService NotificationService { get; }

    public static NotificationsContext CreateDefault() => new(new WindowsToastNotificationService(), new SystemClock());
}
