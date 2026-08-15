// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.Notifications.Actions.Tests.Fakes;

internal sealed class FakeNotificationService : INotificationService
{
    public Exception? ExceptionToThrow { get; set; }
    public NotificationRequest? LastNotification { get; private set; }

    public void Show(NotificationRequest notification)
    {
        LastNotification = notification;
        if (ExceptionToThrow is not null)
        {
            throw ExceptionToThrow;
        }
    }
}
