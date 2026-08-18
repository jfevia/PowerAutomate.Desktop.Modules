// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

/// <summary>
/// Growable little-endian writer.
/// </summary>
public sealed class PacketWriter
{
    private const int DefaultCapacity = 64;

    private byte[] _buffer;
    private int _length;

    public PacketWriter() : this(DefaultCapacity)
    {
    }

    public PacketWriter(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        _buffer = new byte[capacity];
        _length = 0;
    }

    public int Length => _length;

    public void WriteByte(byte value)
    {
        EnsureCapacity(1);
        _buffer[_length++] = value;
    }

    public void WriteUInt16(ushort value)
    {
        EnsureCapacity(2);
        _buffer[_length++] = (byte)value;
        _buffer[_length++] = (byte)(value >> 8);
    }

    public void WriteUInt32(uint value)
    {
        EnsureCapacity(4);
        _buffer[_length++] = (byte)value;
        _buffer[_length++] = (byte)(value >> 8);
        _buffer[_length++] = (byte)(value >> 16);
        _buffer[_length++] = (byte)(value >> 24);
    }

    public void WriteUInt64(ulong value)
    {
        WriteUInt32((uint)value);
        WriteUInt32((uint)(value >> 32));
    }

    public void WriteBytes(byte[] value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        WriteBytes(value, 0, value.Length);
    }

    public void WriteBytes(byte[] value, int offset, int count)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        if (offset + count > value.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        EnsureCapacity(count);
        Array.Copy(value, offset, _buffer, _length, count);
        _length += count;
    }

    public void WriteString(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var bytes = Latin1.GetBytes(value);
        WriteUInt16((ushort)bytes.Length);
        WriteBytes(bytes);
    }

    public byte[] ToArray()
    {
        var result = new byte[_length];
        Array.Copy(_buffer, 0, result, 0, _length);
        return result;
    }

    private void EnsureCapacity(int additional)
    {
        var required = _length + additional;
        if (required <= _buffer.Length)
        {
            return;
        }

        var capacity = _buffer.Length * 2;
        while (capacity < required)
        {
            capacity *= 2;
        }

        var grown = new byte[capacity];
        Array.Copy(_buffer, 0, grown, 0, _length);
        _buffer = grown;
    }
}
