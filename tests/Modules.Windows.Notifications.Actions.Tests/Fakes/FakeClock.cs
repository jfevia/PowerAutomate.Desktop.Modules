// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.Notifications.Actions.Tests.Fakes;

internal sealed class FakeClock : IClock
{
    public DateTimeOffset Now { get; set; } = new(2026, 8, 15, 11, 28, 0, TimeSpan.FromHours(3));
}
