// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

/// <summary>
/// Accumulates socket reads and yields whole frame bodies once the length prefix is satisfied.
/// </summary>
public sealed class FrameBuffer
{
    private readonly int _maxFrameSize;
    private byte[] _buffer;
    private int _length;

    public FrameBuffer() : this(FrameCodec.MaxFrameSize)
    {
    }

    public FrameBuffer(int maxFrameSize)
    {
        if (maxFrameSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxFrameSize));
        }

        _maxFrameSize = maxFrameSize;
        _buffer = new byte[FrameCodec.LengthPrefixSize + maxFrameSize];
        _length = 0;
    }

    /// <summary>
    /// Bytes held that do not yet form a complete frame.
    /// </summary>
    public int BufferedBytes => _length;

    public void Append(byte[] data, int offset, int count)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        if (offset + count > data.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        EnsureCapacity(_length + count);
        Array.Copy(data, offset, _buffer, _length, count);
        _length += count;
    }

    /// <summary>
    /// Returns the next frame body, excluding the length prefix.
    /// </summary>
    public bool TryReadFrame(out byte[] body)
    {
        body = null!;

        if (_length < FrameCodec.LengthPrefixSize)
        {
            return false;
        }

        var declared = _buffer[0] | (_buffer[1] << 8);
        if (declared > _maxFrameSize)
        {
            throw new ProtocolException(
                $"A frame declaring {declared} byte(s) exceeds the {_maxFrameSize} byte limit.");
        }

        var total = FrameCodec.LengthPrefixSize + declared;
        if (_length < total)
        {
            return false;
        }

        body = new byte[declared];
        Array.Copy(_buffer, FrameCodec.LengthPrefixSize, body, 0, declared);

        var leftover = _length - total;
        Array.Copy(_buffer, total, _buffer, 0, leftover);
        _length = leftover;
        return true;
    }

    private void EnsureCapacity(int required)
    {
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
