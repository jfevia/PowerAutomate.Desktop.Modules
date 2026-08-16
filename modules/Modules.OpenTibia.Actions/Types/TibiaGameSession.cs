// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;
using PowerAutomate.Desktop.OpenTibia.Client;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;

/// <summary>
/// A live game-server connection carried between actions by a flow variable.
/// </summary>
[Type(FriendlyName = nameof(TibiaGameSession) + "_FriendlyName",
      FriendlyNamePlural = nameof(TibiaGameSession) + "_FriendlyNamePlural",
      DefaultPropertyVisibility = Visibility.Visible)]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
public sealed class TibiaGameSession
{
    public TibiaGameSession(TibiaGameClient client, string characterName, string world)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
        CharacterName = characterName ?? throw new ArgumentNullException(nameof(characterName));
        World = world ?? throw new ArgumentNullException(nameof(world));
    }

    [PropertyIgnore]
    public TibiaGameClient Client { get; }

    [Property]
    public string CharacterName { get; }

    [Property]
    public string World { get; }

    /// <summary>
    /// The underlying client never reports Faulted itself, so a stalled reader is synthesised here.
    /// </summary>
    [Property]
    public string State => Client.FaultReason != null ? "Faulted" : Client.State.ToString();

    [Property]
    public int QueueDepth => Client.Queue?.Depth ?? 0;

    [Property]
    public long Dropped => Client.Queue?.GetStatistics().Dropped ?? 0;

    public override string ToString()
    {
        return $"{CharacterName} @ {World} ({State})";
    }
}
