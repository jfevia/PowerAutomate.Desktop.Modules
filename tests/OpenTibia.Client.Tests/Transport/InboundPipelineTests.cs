// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Linq;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Client.Tests.Transport;

[TestFixture]
public class InboundPipelineTests
{
    private static readonly uint[] Key = { 11u, 22u, 33u, 44u };

    private static GameServerMessageRegistry Registry()
    {
        var registry = new GameServerMessageRegistry();
        registry.Register(GameServerOpcode.Ping, (opcode, reader) => new PayloadlessMessage(opcode));
        registry.Register(GameServerOpcode.PlayerData, (opcode, reader) => new PayloadlessMessage(opcode));
        return registry;
    }

    [Test]
    public void Push_WithPlainFrame_DecodesAndEnqueues()
    {
        var queue = new ServerMessageQueue(8);
        var pipeline = new InboundPipeline(Registry(), new OpcodeFilter(), queue);
        var frame = FrameCodec.EncodePlain(new[] { (byte)GameServerOpcode.Ping });

        var decoded = pipeline.Push(frame, 0, frame.Length);

        Assert.Multiple(() =>
        {
            Assert.That(decoded, Has.Count.EqualTo(1));
            Assert.That(queue.Depth, Is.EqualTo(1));
            Assert.That(pipeline.IsEncrypted, Is.False);
        });
    }

    [Test]
    public void Push_AfterUseEncryption_DecodesEncryptedFrames()
    {
        var queue = new ServerMessageQueue(8);
        var pipeline = new InboundPipeline(Registry(), new OpcodeFilter(), queue);
        pipeline.UseEncryption(Key);
        var frame = FrameCodec.EncodeEncrypted(new[] { (byte)GameServerOpcode.PlayerData }, Key);

        var decoded = pipeline.Push(frame, 0, frame.Length);

        Assert.Multiple(() =>
        {
            Assert.That(pipeline.IsEncrypted, Is.True);
            Assert.That(decoded.Single().Opcode, Is.EqualTo((byte)GameServerOpcode.PlayerData));
        });
    }

    [Test]
    public void Push_WithPartialFrame_DecodesNothingUntilComplete()
    {
        var queue = new ServerMessageQueue(8);
        var pipeline = new InboundPipeline(Registry(), new OpcodeFilter(), queue);
        var frame = FrameCodec.EncodePlain(new[] { (byte)GameServerOpcode.Ping });

        var first = pipeline.Push(frame, 0, frame.Length - 1);
        var second = pipeline.Push(frame, frame.Length - 1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(first, Is.Empty);
            Assert.That(second, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void Push_WithFilteredOpcode_CountsItWithoutQueueing()
    {
        var queue = new ServerMessageQueue(8);
        var filter = new OpcodeFilter();
        filter.Allow(new[] { (byte)GameServerOpcode.PlayerData });
        var pipeline = new InboundPipeline(Registry(), filter, queue);
        var frame = FrameCodec.EncodePlain(new[] { (byte)GameServerOpcode.Ping });

        var decoded = pipeline.Push(frame, 0, frame.Length);

        Assert.Multiple(() =>
        {
            Assert.That(decoded, Has.Count.EqualTo(1), "Filtered messages are still returned so ping can be answered.");
            Assert.That(queue.Depth, Is.Zero);
            Assert.That(queue.GetStatistics().Filtered, Is.EqualTo(1));
        });
    }

    [Test]
    public void Constructor_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new InboundPipeline(null!, new OpcodeFilter(), new ServerMessageQueue(1)));
    }

    [Test]
    public void Constructor_WithNullFilter_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new InboundPipeline(Registry(), null!, new ServerMessageQueue(1)));
    }

    [Test]
    public void Constructor_WithNullQueue_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new InboundPipeline(Registry(), new OpcodeFilter(), null!));
    }

    [Test]
    public void UseEncryption_WithNullKey_Throws()
    {
        var pipeline = new InboundPipeline(Registry(), new OpcodeFilter(), new ServerMessageQueue(1));

        Assert.Throws<ArgumentNullException>(() => pipeline.UseEncryption(null!));
    }
}

[TestFixture]
public class FrameWriterTests
{
    private static readonly uint[] Key = { 1u, 2u, 3u, 4u };

    [Test]
    public void Send_WritesAPlainFrameBeforeEncryptionIsEnabled()
    {
        var transport = new FakeSocketTransport();
        var writer = new FrameWriter(transport);

        writer.Send(new PayloadlessMessage(ClientOpcode.PingBack));

        var payload = FrameCodec.DecodePlain(
            transport.Written.Single().Skip(FrameCodec.LengthPrefixSize).ToArray());

        Assert.Multiple(() =>
        {
            Assert.That(writer.IsEncrypted, Is.False);
            Assert.That(payload, Is.EqualTo(new[] { (byte)ClientOpcode.PingBack }));
        });
    }

    [Test]
    public void Send_AfterUseEncryption_WritesAnEncryptedFrame()
    {
        var transport = new FakeSocketTransport();
        var writer = new FrameWriter(transport);
        writer.UseEncryption(Key);

        writer.Send(new PayloadlessMessage(ClientOpcode.LeaveGame));

        var payload = FrameCodec.DecodeEncrypted(
            transport.Written.Single().Skip(FrameCodec.LengthPrefixSize).ToArray(), Key);

        Assert.Multiple(() =>
        {
            Assert.That(writer.IsEncrypted, Is.True);
            Assert.That(payload, Is.EqualTo(new[] { (byte)ClientOpcode.LeaveGame }));
        });
    }

    [Test]
    public void Constructor_WithNullTransport_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new FrameWriter(null!));
    }

    [Test]
    public void UseEncryption_WithNullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new FrameWriter(new FakeSocketTransport()).UseEncryption(null!));
    }

    [Test]
    public void Send_WithNullMessage_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new FrameWriter(new FakeSocketTransport()).Send(null!));
    }

    [Test]
    public void SendPayload_WithNullPayload_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new FrameWriter(new FakeSocketTransport()).SendPayload(null!));
    }
}
