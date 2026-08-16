// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Player;

/// <summary>
/// Server notice of the local player's 7 fighting/fishing skills (s2c 0xA1).
/// </summary>
public sealed class GameServerPlayerSkillsMessage : IProtocolMessage
{
    public GameServerPlayerSkillsMessage(
        SkillLevel fist,
        SkillLevel club,
        SkillLevel sword,
        SkillLevel axe,
        SkillLevel distance,
        SkillLevel shielding,
        SkillLevel fishing)
    {
        Fist = fist;
        Club = club;
        Sword = sword;
        Axe = axe;
        Distance = distance;
        Shielding = shielding;
        Fishing = fishing;
        Skills = new[] { fist, club, sword, axe, distance, shielding, fishing };
    }

    public byte Opcode => (byte)GameServerOpcode.PlayerSkills;

    /// <summary>
    /// All 7 skills, in TFS's fixed wire order.
    /// </summary>
    public SkillLevel[] Skills { get; }

    public SkillLevel Fist { get; }

    public SkillLevel Club { get; }

    public SkillLevel Sword { get; }

    public SkillLevel Axe { get; }

    public SkillLevel Distance { get; }

    public SkillLevel Shielding { get; }

    public SkillLevel Fishing { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var fist = SkillLevel.Read(reader);
        var club = SkillLevel.Read(reader);
        var sword = SkillLevel.Read(reader);
        var axe = SkillLevel.Read(reader);
        var distance = SkillLevel.Read(reader);
        var shielding = SkillLevel.Read(reader);
        var fishing = SkillLevel.Read(reader);
        return new GameServerPlayerSkillsMessage(fist, club, sword, axe, distance, shielding, fishing);
    }
}
