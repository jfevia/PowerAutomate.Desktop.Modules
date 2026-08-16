// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Threading;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;

namespace PowerAutomate.Desktop.OpenTibia.Client.Transport;

/// <summary>
/// Owns the socket read side on a dedicated background thread, never touching the flow thread.
/// </summary>
public sealed class FrameReaderLoop : IDisposable
{
    private const int ReadBufferSize = 8192;

    private readonly ISocketTransport _transport;
    private readonly InboundPipeline _pipeline;
    private readonly ServerMessageQueue _queue;
    private readonly Action<IProtocolMessage> _observer;
    private readonly ManualResetEventSlim _stopped = new ManualResetEventSlim(true);

    private Thread? _thread;
    private volatile bool _running;

    public FrameReaderLoop(
        ISocketTransport transport,
        InboundPipeline pipeline,
        ServerMessageQueue queue,
        Action<IProtocolMessage> observer)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        _observer = observer ?? throw new ArgumentNullException(nameof(observer));
    }

    public bool IsRunning => _running;

    /// <summary>
    /// Why the loop ended, or null while it is healthy.
    /// </summary>
    public string? FaultReason { get; private set; }

    public void Start()
    {
        if (_running)
        {
            throw new InvalidOperationException("The reader loop is already running.");
        }

        _running = true;
        FaultReason = null;
        _stopped.Reset();

        // Background so a finished flow is never held open by this thread.
        _thread = new Thread(Run) { IsBackground = true, Name = "OpenTibia frame reader" };
        _thread.Start();
    }

    public void Stop()
    {
        _running = false;
        _stopped.Wait(TimeSpan.FromMilliseconds(500));
    }

    public void Dispose()
    {
        Stop();
        _stopped.Dispose();
    }

    /// <summary>
    /// Blocks until the loop has exited, for tests and deterministic shutdown.
    /// </summary>
    public bool WaitForCompletion(TimeSpan timeout)
    {
        return _stopped.Wait(timeout);
    }

    private void Run()
    {
        var buffer = new byte[ReadBufferSize];

        try
        {
            while (_running)
            {
                var read = _transport.Read(buffer, 0, buffer.Length);

                if (read < 0)
                {
                    // Read timeout: no data yet, but the connection is alive.
                    continue;
                }

                if (read == 0)
                {
                    FaultReason = "The server closed the connection.";
                    break;
                }

                foreach (var message in _pipeline.Push(buffer, 0, read))
                {
                    _observer(message);
                }
            }
        }
        catch (Exception exception)
        {
            FaultReason = exception.Message;
        }
        finally
        {
            _running = false;
            _queue.Complete();
            _stopped.Set();
        }
    }
}
