// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.Notifications.Actions;

public sealed class NotificationRequest
{
    public NotificationRequest(string text, string appUserModelId, DateTimeOffset expirationTime)
    {
        Text = text;
        AppUserModelId = appUserModelId;
        ExpirationTime = expirationTime;
    }

    public string AppUserModelId { get; }
    public DateTimeOffset ExpirationTime { get; }
    public string Text { get; }
}
