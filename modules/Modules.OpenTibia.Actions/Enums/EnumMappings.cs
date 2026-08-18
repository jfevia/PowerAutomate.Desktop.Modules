// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using ProtocolDirection = PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement.Direction;
using ProtocolSpeakType = PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat.SpeakType;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Enums;

/// <summary>
/// Converts the module's own enums to the protocol ones.
/// </summary>
/// <remarks>
/// The two sets are kept value for value identical, which EnumParityTests enforces.
/// </remarks>
public static class EnumMappings
{
    public static ProtocolDirection ToProtocol(this Direction direction)
    {
        return (ProtocolDirection)(byte)direction;
    }

    public static ProtocolSpeakType ToProtocol(this SpeakType speakType)
    {
        return (ProtocolSpeakType)(byte)speakType;
    }

    public static IReadOnlyList<ProtocolDirection> ToProtocol(this IEnumerable<Direction> directions)
    {
        return directions.Select(direction => direction.ToProtocol()).ToList();
    }
}
