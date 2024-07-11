// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PowerAutomate.Desktop.Modules.Async.Actions;

public delegate Task AsyncOperationDelegate(CancellationToken cancellationToken);

public sealed class AsyncScheduler
{
    private static readonly Lazy<AsyncScheduler> Lazy = new(() => new AsyncScheduler());
    private readonly BlockingCollection<AsyncOperationDelegate> _blockingCollection = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly SemaphoreSlim _semaphoreSlim = new(0, 1);
    private bool _waitingForCompletion;

    public static AsyncScheduler Default => Lazy.Value;

    public AsyncScheduler()
    {
        _ = Task.Run(ProcessAsync, _cancellationTokenSource.Token);
    }

    public void CancelAll()
    {
        _cancellationTokenSource.Cancel();
    }

    public void Queue(AsyncOperationDelegate action)
    {
        _waitingForCompletion = false;
        _blockingCollection.Add(action);
    }

    public void WaitForCompletion()
    {
        _waitingForCompletion = true;
        _semaphoreSlim.Wait();
    }

    private async Task ProcessAsync()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            if (_waitingForCompletion && _blockingCollection.Count == 0)
            {
                _semaphoreSlim.Release();
                break;
            }

            var item = _blockingCollection.Take();
            await item.Invoke(_cancellationTokenSource.Token);
        }
    }
}