// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.OpenTibia.Client.Streaming;

/// <summary>
/// Point-in-time view of the inbound queue, used to detect the flow falling behind.
/// </summary>
public readonly struct StreamStatistics
{
    public StreamStatistics(
        int depth,
        int capacity,
        long enqueued,
        long dequeued,
        long dropped,
        long filtered,
        int maxDepthSeen)
    {
        Depth = depth;
        Capacity = capacity;
        Enqueued = enqueued;
        Dequeued = dequeued;
        Dropped = dropped;
        Filtered = filtered;
        MaxDepthSeen = maxDepthSeen;
    }

    public int Depth { get; }

    public int Capacity { get; }

    public long Enqueued { get; }

    public long Dequeued { get; }

    /// <summary>
    /// Messages discarded because the buffer was full when they arrived.
    /// </summary>
    public long Dropped { get; }

    /// <summary>
    /// Messages discarded before the buffer because their opcode was not subscribed.
    /// </summary>
    public long Filtered { get; }

    public int MaxDepthSeen { get; }

    public override string ToString()
    {
        return $"depth {Depth}/{Capacity}, dropped {Dropped}, filtered {Filtered}";
    }
}
