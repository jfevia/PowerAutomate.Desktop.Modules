// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

namespace PowerAutomate.Desktop.OpenTibia.Client.Streaming;

/// <summary>
/// Tracks the local player's current floor so the stateless registry can decode floor-context map messages.
/// </summary>
public sealed class MapFloorTracker
{
    /// <summary>
    /// Ground level, assumed until the first full map fixes the real value.
    /// </summary>
    public const byte GroundFloor = 7;

    private byte _currentZ = GroundFloor;

    public byte CurrentZ => _currentZ;

    /// <summary>
    /// Updates the tracked floor from a decoded message; call only from the single reader thread.
    /// </summary>
    public void Observe(IProtocolMessage message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        switch (message)
        {
            case GameServerFullMapMessage fullMap:
                _currentZ = fullMap.OwnPosition.Z;
                break;
            case GameServerFloorChangeUpMessage _:
                _currentZ--;
                break;
            case GameServerFloorChangeDownMessage _:
                _currentZ++;
                break;
        }
    }
}
