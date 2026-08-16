// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;

namespace PowerAutomate.Desktop.OpenTibia.Client.Streaming;

/// <summary>
/// Single-producer, single-consumer FIFO handoff between the socket reader and the flow thread.
/// </summary>
public sealed class ServerMessageQueue
{
    /// <summary>
    /// Waiting longer than this would make a desktop flow unresponsive to Stop.
    /// </summary>
    public static readonly TimeSpan MaxSliceTimeout = TimeSpan.FromMilliseconds(1000);

    private readonly object _gate = new object();
    private readonly BoundedRingBuffer<IProtocolMessage> _buffer;

    private long _enqueued;
    private long _dequeued;
    private long _dropped;
    private long _filtered;
    private int _maxDepthSeen;
    private bool _completed;

    public ServerMessageQueue(int capacity)
    {
        _buffer = new BoundedRingBuffer<IProtocolMessage>(capacity);
    }

    public int Capacity => _buffer.Capacity;

    public int Depth
    {
        get
        {
            lock (_gate)
            {
                return _buffer.Count;
            }
        }
    }

    /// <summary>
    /// Clamps a caller timeout into the range the flow engine can tolerate.
    /// </summary>
    public static TimeSpan ClampTimeout(TimeSpan timeout)
    {
        if (timeout < TimeSpan.Zero)
        {
            return TimeSpan.Zero;
        }

        if (timeout > MaxSliceTimeout)
        {
            return MaxSliceTimeout;
        }

        return timeout;
    }

    public void Enqueue(IProtocolMessage message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        lock (_gate)
        {
            if (!_buffer.Enqueue(message))
            {
                _dropped++;
            }

            _enqueued++;

            if (_buffer.Count > _maxDepthSeen)
            {
                _maxDepthSeen = _buffer.Count;
            }

            Monitor.Pulse(_gate);
        }
    }

    public void RecordFiltered()
    {
        lock (_gate)
        {
            _filtered++;
        }
    }

    /// <summary>
    /// Drains up to <paramref name="maxCount" /> messages, waiting at most one clamped slice for the first.
    /// </summary>
    public IReadOnlyList<IProtocolMessage> DequeueBatch(int maxCount, TimeSpan timeout)
    {
        if (maxCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxCount));
        }

        var batch = new List<IProtocolMessage>();

        lock (_gate)
        {
            if (_buffer.Count == 0 && !_completed)
            {
                Monitor.Wait(_gate, ClampTimeout(timeout));
            }

            while (batch.Count < maxCount && _buffer.TryDequeue(out var message))
            {
                batch.Add(message);
                _dequeued++;
            }
        }

        return batch;
    }

    /// <summary>
    /// Releases any waiter; called when the connection ends so the flow is not left blocking.
    /// </summary>
    public void Complete()
    {
        lock (_gate)
        {
            _completed = true;
            Monitor.PulseAll(_gate);
        }
    }

    public int Clear()
    {
        lock (_gate)
        {
            return _buffer.Clear();
        }
    }

    public StreamStatistics GetStatistics()
    {
        lock (_gate)
        {
            return new StreamStatistics(
                _buffer.Count,
                _buffer.Capacity,
                _enqueued,
                _dequeued,
                _dropped,
                _filtered,
                _maxDepthSeen);
        }
    }
}
