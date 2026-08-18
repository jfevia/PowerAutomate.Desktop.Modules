// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;

/// <summary>
/// Scripted transport that replays queued reads and records everything written.
/// </summary>
internal sealed class FakeSocketTransport : ISocketTransport
{
    private readonly Queue<byte[]> _reads = new Queue<byte[]>();
    private readonly ManualResetEventSlim _blockSignal = new ManualResetEventSlim(false);
    private bool _blockWhenExhausted;

    public List<byte[]> Written { get; } = new List<byte[]>();

    public bool IsConnected { get; private set; } = true;

    public bool Disposed { get; private set; }

    public Exception? ReadFailure { get; set; }

    /// <summary>
    /// Makes the next Connect call throw instead of succeeding.
    /// </summary>
    public Exception? ConnectFailure { get; set; }

    public void EnqueueRead(byte[] data)
    {
        _reads.Enqueue(data);
    }

    /// <summary>
    /// Makes reads block like an idle socket once the script runs out, instead of signalling EOF.
    /// </summary>
    public void BlockWhenExhausted()
    {
        _blockWhenExhausted = true;
    }

    public void Connect(string host, int port, TimeSpan timeout)
    {
        if (ConnectFailure != null)
        {
            var failure = ConnectFailure;
            ConnectFailure = null;
            throw failure;
        }

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

        if (_reads.Count > 0)
        {
            var chunk = _reads.Dequeue();
            Array.Copy(chunk, 0, buffer, offset, chunk.Length);
            return chunk.Length;
        }

        if (_blockWhenExhausted)
        {
            _blockSignal.Wait();
        }

        return 0;
    }

    public void Write(byte[] data)
    {
        Written.Add(data);
    }

    public void Close()
    {
        IsConnected = false;
        _blockSignal.Set();
    }

    public void Dispose()
    {
        Disposed = true;
        Close();
        _blockSignal.Dispose();
    }
}
