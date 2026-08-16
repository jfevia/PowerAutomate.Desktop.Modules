// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;

namespace PowerAutomate.Desktop.OpenTibia.Client.Streaming;

/// <summary>
/// Opcode allow-list applied before the queue so unsubscribed traffic never consumes capacity.
/// </summary>
public sealed class OpcodeFilter
{
    private readonly bool[] _allowed = new bool[256];

    public OpcodeFilter()
    {
        AllowAll = true;
    }

    /// <summary>
    /// True while no explicit subscription has been set.
    /// </summary>
    public bool AllowAll { get; private set; }

    public void Reset()
    {
        AllowAll = true;
        Array.Clear(_allowed, 0, _allowed.Length);
    }

    /// <summary>
    /// Restricts the stream to the supplied opcodes.
    /// </summary>
    public void Allow(IEnumerable<byte> opcodes)
    {
        if (opcodes == null)
        {
            throw new ArgumentNullException(nameof(opcodes));
        }

        Array.Clear(_allowed, 0, _allowed.Length);
        var any = false;
        foreach (var opcode in opcodes)
        {
            _allowed[opcode] = true;
            any = true;
        }

        if (!any)
        {
            throw new ArgumentException("At least one opcode must be supplied.", nameof(opcodes));
        }

        AllowAll = false;
    }

    public bool IsAllowed(byte opcode)
    {
        return AllowAll || _allowed[opcode];
    }
}
