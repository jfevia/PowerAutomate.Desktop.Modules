// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;

/// <summary>
/// Builds the raw payloads the login and game servers send during a handshake.
/// </summary>
internal static class Frames
{
    public static byte[] GameChallengeFrame()
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.Challenge);
        writer.WriteUInt32(0x11223344);
        writer.WriteByte(0x55);
        return FrameCodec.EncodeInboundPlain(writer.ToArray());
    }

    public static byte[] GamePendingStatePayload()
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.LoginOrPendingState);
        writer.WriteUInt32(0x00001000);
        writer.WriteUInt16(50);
        writer.WriteByte(0);
        return writer.ToArray();
    }

    public static byte[] GameLoginErrorPayload(string text)
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.LoginError);
        writer.WriteString(text);
        return writer.ToArray();
    }

    public static byte[] LoginMotdPayload(string text)
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)LoginServerOpcode.Motd);
        writer.WriteString(text);
        return writer.ToArray();
    }

    public static byte[] LoginCharacterListPayload(params string[] names)
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)LoginServerOpcode.CharacterList);
        writer.WriteByte((byte)names.Length);
        foreach (var name in names)
        {
            writer.WriteString(name);
            writer.WriteString("Antica");
            writer.WriteUInt32(0x0100007F);
            writer.WriteUInt16(7172);
        }

        writer.WriteUInt16(42);
        return writer.ToArray();
    }

    public static byte[] LoginErrorPayload(string text)
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)LoginServerOpcode.Error);
        writer.WriteString(text);
        return writer.ToArray();
    }
}
