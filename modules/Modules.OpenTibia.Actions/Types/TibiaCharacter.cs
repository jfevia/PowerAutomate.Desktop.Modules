// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;

/// <summary>
/// A character offered by the login server.
/// </summary>
[Type(FriendlyName = nameof(TibiaCharacter) + "_FriendlyName",
      FriendlyNamePlural = nameof(TibiaCharacter) + "_FriendlyNamePlural",
      DefaultPropertyVisibility = Visibility.Visible)]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
public sealed class TibiaCharacter
{
    public TibiaCharacter(string name, string world, string host, int port)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        World = world ?? throw new ArgumentNullException(nameof(world));
        Host = host ?? throw new ArgumentNullException(nameof(host));
        Port = port;
    }

    [Property]
    public string Name { get; }

    [Property]
    public string World { get; }

    [Property]
    public string Host { get; }

    [Property]
    public int Port { get; }

    public override string ToString()
    {
        return $"{Name} @ {World} ({Host}:{Port})";
    }
}
