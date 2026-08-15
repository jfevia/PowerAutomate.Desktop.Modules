// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace PowerAutomate.Desktop.Modules.Windows.Notifications.Actions;

// Pure time adapter.
[ExcludeFromCodeCoverage]
internal sealed class SystemClock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}
