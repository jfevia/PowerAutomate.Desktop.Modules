// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Net;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

/// <summary>
/// One playable character entry inside a <see cref="LoginServerCharacterListMessage"/>.
/// </summary>
public sealed class CharacterListEntry
{
    public CharacterListEntry(string name, string world, uint address, ushort port)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        World = world ?? throw new ArgumentNullException(nameof(world));
        Address = address;
        Port = port;
    }

    public string Name { get; }

    public string World { get; }

    /// <summary>
    /// The world's server address, packed the way the wire stores it.
    /// </summary>
    public uint Address { get; }

    public ushort Port { get; }

    /// <summary>
    /// <see cref="Address"/> rendered as a dotted-quad host name.
    /// </summary>
    public string HostName => new IPAddress(Address).ToString();

    public void Write(PacketWriter writer)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        writer.WriteString(Name);
        writer.WriteString(World);
        writer.WriteUInt32(Address);
        writer.WriteUInt16(Port);
    }

    public static CharacterListEntry Read(PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var name = reader.ReadString();
        var world = reader.ReadString();
        var address = reader.ReadUInt32();
        var port = reader.ReadUInt16();
        return new CharacterListEntry(name, world, address, port);
    }
}
