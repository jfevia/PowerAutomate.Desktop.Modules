// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.Windows.Notifications.Actions;

public interface INotificationService
{
    void Show(NotificationRequest notification);
}
