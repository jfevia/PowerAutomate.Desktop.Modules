// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.OpenTibia.Client.Streaming;

/// <summary>
/// Fixed-capacity FIFO ring that overwrites the oldest entry when it is full.
/// </summary>
public sealed class BoundedRingBuffer<T>
{
    private readonly T[] _items;
    private int _head;
    private int _count;

    public BoundedRingBuffer(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        _items = new T[capacity];
        _head = 0;
        _count = 0;
    }

    public int Capacity => _items.Length;

    public int Count => _count;

    /// <summary>
    /// Appends an item, returning false when the oldest entry had to be discarded.
    /// </summary>
    public bool Enqueue(T item)
    {
        var tail = (_head + _count) % _items.Length;
        _items[tail] = item;

        if (_count == _items.Length)
        {
            _head = (_head + 1) % _items.Length;
            return false;
        }

        _count++;
        return true;
    }

    public bool TryDequeue(out T item)
    {
        if (_count == 0)
        {
            item = default!;
            return false;
        }

        item = _items[_head];
        _items[_head] = default!;
        _head = (_head + 1) % _items.Length;
        _count--;
        return true;
    }

    /// <summary>
    /// Discards everything and reports how many entries were dropped.
    /// </summary>
    public int Clear()
    {
        var discarded = _count;
        while (TryDequeue(out _))
        {
        }

        return discarded;
    }
}
