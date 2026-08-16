// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Client.Tests.Transport;

[TestFixture]
public class FrameReaderLoopTests
{
    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(5);

    private static GameServerMessageRegistry Registry()
    {
        var registry = new GameServerMessageRegistry();
        registry.Register(GameServerOpcode.Ping, (opcode, reader) => new PayloadlessMessage(opcode));
        return registry;
    }

    private static FrameReaderLoop Build(
        FakeSocketTransport transport,
        ServerMessageQueue queue,
        List<IProtocolMessage> observed)
    {
        var pipeline = new InboundPipeline(Registry(), new OpcodeFilter(), queue);
        return new FrameReaderLoop(transport, pipeline, queue, observed.Add);
    }

    [Test]
    public void Run_DecodesFramesAndObservesEveryMessage()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(FrameCodec.EncodeInboundPlain(new[] { (byte)GameServerOpcode.Ping }));
        var queue = new ServerMessageQueue(8);
        var observed = new List<IProtocolMessage>();

        using var loop = Build(transport, queue, observed);
        loop.Start();
        loop.WaitForCompletion(Patience);

        Assert.Multiple(() =>
        {
            Assert.That(observed, Has.Count.EqualTo(1));
            Assert.That(queue.Depth, Is.EqualTo(1));
        });
    }

    [Test]
    public void Run_WhenReadTimesOut_KeepsRunningInsteadOfFaulting()
    {
        var transport = new FakeSocketTransport { TimeoutsBeforeData = 3 };
        transport.EnqueueRead(FrameCodec.EncodeInboundPlain(new[] { (byte)GameServerOpcode.Ping }));
        var queue = new ServerMessageQueue(8);
        var observed = new List<IProtocolMessage>();

        using var loop = Build(transport, queue, observed);
        loop.Start();
        loop.WaitForCompletion(Patience);

        Assert.Multiple(() =>
        {
            Assert.That(observed, Has.Count.EqualTo(1),
                "A read timeout means silence, not a closed connection.");
            Assert.That(loop.FaultReason, Is.EqualTo("The server closed the connection."));
        });
    }

    [Test]
    public void Run_WhenPeerClosesConnection_RecordsFaultAndCompletesQueue()
    {
        var transport = new FakeSocketTransport();
        var queue = new ServerMessageQueue(8);
        var observed = new List<IProtocolMessage>();

        using var loop = Build(transport, queue, observed);
        loop.Start();
        loop.WaitForCompletion(Patience);

        Assert.Multiple(() =>
        {
            Assert.That(loop.FaultReason, Is.EqualTo("The server closed the connection."));
            Assert.That(loop.IsRunning, Is.False);
            Assert.That(queue.DequeueBatch(1, TimeSpan.FromSeconds(3)), Is.Empty,
                "Complete must release the waiter instead of blocking for the whole slice.");
        });
    }

    [Test]
    public void Run_WhenTransportThrows_RecordsTheExceptionMessage()
    {
        var transport = new FakeSocketTransport { ReadFailure = new InvalidOperationException("socket exploded") };
        var queue = new ServerMessageQueue(8);
        var observed = new List<IProtocolMessage>();

        using var loop = Build(transport, queue, observed);
        loop.Start();
        loop.WaitForCompletion(Patience);

        Assert.That(loop.FaultReason, Is.EqualTo("socket exploded"));
    }

    [Test]
    public void Start_WhenAlreadyRunning_Throws()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(FrameCodec.EncodeInboundPlain(new[] { (byte)GameServerOpcode.Ping }));
        var queue = new ServerMessageQueue(8);
        var observed = new List<IProtocolMessage>();
        var pipeline = new InboundPipeline(Registry(), new OpcodeFilter(), queue);
        var blocking = new BlockingTransport();

        using var loop = new FrameReaderLoop(blocking, pipeline, queue, observed.Add);
        loop.Start();

        try
        {
            Assert.Throws<InvalidOperationException>(() => loop.Start());
        }
        finally
        {
            blocking.Release();
            loop.Stop();
        }
    }

    [Test]
    public void Stop_WhenNeverStarted_ReturnsImmediately()
    {
        var transport = new FakeSocketTransport();
        var queue = new ServerMessageQueue(8);
        var observed = new List<IProtocolMessage>();

        var loop = Build(transport, queue, observed);
        loop.Stop();

        Assert.Multiple(() =>
        {
            Assert.That(loop.IsRunning, Is.False);
            Assert.That(loop.FaultReason, Is.Null);
        });

        loop.Dispose();
    }

    [Test]
    public void Constructor_WithNullTransport_Throws()
    {
        var queue = new ServerMessageQueue(1);
        var pipeline = new InboundPipeline(Registry(), new OpcodeFilter(), queue);

        Assert.Throws<ArgumentNullException>(() => new FrameReaderLoop(null!, pipeline, queue, _ => { }));
    }

    [Test]
    public void Constructor_WithNullPipeline_Throws()
    {
        var queue = new ServerMessageQueue(1);

        Assert.Throws<ArgumentNullException>(
            () => new FrameReaderLoop(new FakeSocketTransport(), null!, queue, _ => { }));
    }

    [Test]
    public void Constructor_WithNullQueue_Throws()
    {
        var pipeline = new InboundPipeline(Registry(), new OpcodeFilter(), new ServerMessageQueue(1));

        Assert.Throws<ArgumentNullException>(
            () => new FrameReaderLoop(new FakeSocketTransport(), pipeline, null!, _ => { }));
    }

    [Test]
    public void Constructor_WithNullObserver_Throws()
    {
        var queue = new ServerMessageQueue(1);
        var pipeline = new InboundPipeline(Registry(), new OpcodeFilter(), queue);

        Assert.Throws<ArgumentNullException>(
            () => new FrameReaderLoop(new FakeSocketTransport(), pipeline, queue, null!));
    }

    /// <summary>
    /// Blocks inside Read until released, so the loop is observably running.
    /// </summary>
    private sealed class BlockingTransport : ISocketTransport
    {
        private readonly System.Threading.ManualResetEventSlim _gate = new System.Threading.ManualResetEventSlim(false);

        public bool IsConnected => true;

        public void Connect(string host, int port, TimeSpan timeout)
        {
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            _gate.Wait(TimeSpan.FromSeconds(5));
            return 0;
        }

        public void Write(byte[] data)
        {
        }

        public void Close()
        {
        }

        public void Release()
        {
            _gate.Set();
        }

        public void Dispose()
        {
            _gate.Dispose();
        }
    }
}
