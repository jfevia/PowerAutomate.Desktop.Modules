// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;

namespace PowerAutomate.Desktop.OpenTibia.Client.Tests.Transport;

/// <summary>
/// Scripted transport that replays queued reads and records everything written.
/// </summary>
internal sealed class FakeSocketTransport : ISocketTransport
{
    private readonly Queue<byte[]> _reads = new Queue<byte[]>();

    public List<byte[]> Written { get; } = new List<byte[]>();

    public bool IsConnected { get; private set; } = true;

    public bool Disposed { get; private set; }

    public Exception? ReadFailure { get; set; }

    /// <summary>
    /// Number of read-timeout results to emit before the queued reads are served.
    /// </summary>
    public int TimeoutsBeforeData { get; set; }

    /// <summary>
    /// Keeps the connection alive once the scripted reads run out, as a real quiet server does.
    /// </summary>
    public bool KeepAliveWhenDrained { get; set; }

    public void EnqueueRead(byte[] data)
    {
        _reads.Enqueue(data);
    }

    public void Connect(string host, int port, TimeSpan timeout)
    {
        IsConnected = true;
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        if (ReadFailure != null)
        {
            var failure = ReadFailure;
            ReadFailure = null;
            throw failure;
        }

        if (TimeoutsBeforeData > 0)
        {
            TimeoutsBeforeData--;
            return -1;
        }

        if (_reads.Count == 0)
        {
            if (!KeepAliveWhenDrained)
            {
                return 0;
            }

            System.Threading.Thread.Sleep(10);
            return -1;
        }

        var chunk = _reads.Dequeue();
        Array.Copy(chunk, 0, buffer, offset, chunk.Length);
        return chunk.Length;
    }

    public void Write(byte[] data)
    {
        Written.Add(data);
    }

    public void Close()
    {
        IsConnected = false;
    }

    public void Dispose()
    {
        Disposed = true;
        Close();
    }
}
