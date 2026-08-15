// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.Notifications.Actions;

public interface IClock
{
    DateTimeOffset Now { get; }
}
