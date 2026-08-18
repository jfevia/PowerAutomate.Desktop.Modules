// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

/// <summary>
/// Forward-only little-endian reader over a byte range.
/// </summary>
public sealed class PacketReader
{
    private readonly byte[] _buffer;
    private readonly int _origin;
    private readonly int _length;
    private int _position;

    public PacketReader(byte[] buffer) : this(buffer, 0, Length(buffer))
    {
    }

    public PacketReader(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        if (offset + count > buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        _buffer = buffer;
        _origin = offset;
        _length = count;
        _position = 0;
    }

    public int Position => _position;

    public int Remaining => _length - _position;

    public byte PeekByte()
    {
        EnsureAvailable(1);
        return _buffer[_origin + _position];
    }

    /// <summary>
    /// Reads a little-endian word without consuming it.
    /// </summary>
    public ushort PeekUInt16()
    {
        EnsureAvailable(2);
        return (ushort)(_buffer[_origin + _position] | (_buffer[_origin + _position + 1] << 8));
    }

    public byte ReadByte()
    {
        EnsureAvailable(1);
        return _buffer[_origin + _position++];
    }

    public ushort ReadUInt16()
    {
        EnsureAvailable(2);
        var value = (ushort)(_buffer[_origin + _position] | (_buffer[_origin + _position + 1] << 8));
        _position += 2;
        return value;
    }

    public uint ReadUInt32()
    {
        EnsureAvailable(4);
        var value = (uint)(_buffer[_origin + _position]
                           | (_buffer[_origin + _position + 1] << 8)
                           | (_buffer[_origin + _position + 2] << 16)
                           | (_buffer[_origin + _position + 3] << 24));
        _position += 4;
        return value;
    }

    public ulong ReadUInt64()
    {
        var low = ReadUInt32();
        var high = ReadUInt32();
        return low | ((ulong)high << 32);
    }

    public byte[] ReadBytes(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        EnsureAvailable(count);
        var bytes = new byte[count];
        Array.Copy(_buffer, _origin + _position, bytes, 0, count);
        _position += count;
        return bytes;
    }

    public string ReadString()
    {
        var count = ReadUInt16();
        EnsureAvailable(count);
        var value = Latin1.GetString(_buffer, _origin + _position, count);
        _position += count;
        return value;
    }

    public void Skip(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        EnsureAvailable(count);
        _position += count;
    }

    private static int Length(byte[] buffer)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        return buffer.Length;
    }

    private void EnsureAvailable(int count)
    {
        if (count > Remaining)
        {
            throw new ProtocolException(
                $"Attempted to read {count} byte(s) at offset {_position} but only {Remaining} remain.");
        }
    }
}
