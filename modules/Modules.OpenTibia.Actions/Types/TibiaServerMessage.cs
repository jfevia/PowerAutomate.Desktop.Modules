// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Types;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;

/// <summary>
/// One decoded server message as a desktop flow sees it.
/// </summary>
[Type(FriendlyName = nameof(TibiaServerMessage) + "_FriendlyName",
      FriendlyNamePlural = nameof(TibiaServerMessage) + "_FriendlyNamePlural",
      DefaultPropertyVisibility = Visibility.Visible)]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
public sealed class TibiaServerMessage
{
    public TibiaServerMessage(int opcode, string name, DateTime receivedAt, CustomObject payload)
    {
        Opcode = opcode;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ReceivedAt = receivedAt;
        Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    [Property]
    public int Opcode { get; }

    [Property]
    public string Name { get; }

    [Property]
    public DateTime ReceivedAt { get; }

    [Property]
    public CustomObject Payload { get; }

    public override string ToString()
    {
        return $"{Name} (0x{Opcode:X2})";
    }
}
