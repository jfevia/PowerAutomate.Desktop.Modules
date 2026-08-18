// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Framing;

[TestFixture]
public class FrameBufferTests
{
    private static byte[] FrameOf(params byte[] body)
    {
        var frame = new byte[FrameCodec.LengthPrefixSize + body.Length];
        frame[0] = (byte)body.Length;
        frame[1] = (byte)(body.Length >> 8);
        Array.Copy(body, 0, frame, FrameCodec.LengthPrefixSize, body.Length);
        return frame;
    }

    [Test]
    public void TryReadFrame_WithCompleteFrame_ReturnsBody()
    {
        var buffer = new FrameBuffer();
        var frame = FrameOf(1, 2, 3);

        buffer.Append(frame, 0, frame.Length);

        Assert.Multiple(() =>
        {
            Assert.That(buffer.TryReadFrame(out var body), Is.True);
            Assert.That(body, Is.EqualTo(new byte[] { 1, 2, 3 }));
            Assert.That(buffer.BufferedBytes, Is.Zero);
        });
    }

    [Test]
    public void TryReadFrame_WithByteAtATimeDelivery_YieldsTheSameFrame()
    {
        var buffer = new FrameBuffer();
        var frame = FrameOf(7, 8, 9, 10);

        for (var index = 0; index < frame.Length - 1; index++)
        {
            buffer.Append(frame, index, 1);
            Assert.That(buffer.TryReadFrame(out _), Is.False, $"Frame completed early at byte {index}.");
        }

        buffer.Append(frame, frame.Length - 1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(buffer.TryReadFrame(out var body), Is.True);
            Assert.That(body, Is.EqualTo(new byte[] { 7, 8, 9, 10 }));
        });
    }

    [Test]
    public void TryReadFrame_WithThreeFramesInOneRead_YieldsAllInOrder()
    {
        var buffer = new FrameBuffer();
        var first = FrameOf(1);
        var second = FrameOf(2, 2);
        var third = FrameOf(3, 3, 3);
        var combined = new byte[first.Length + second.Length + third.Length];
        Array.Copy(first, 0, combined, 0, first.Length);
        Array.Copy(second, 0, combined, first.Length, second.Length);
        Array.Copy(third, 0, combined, first.Length + second.Length, third.Length);

        buffer.Append(combined, 0, combined.Length);

        Assert.Multiple(() =>
        {
            Assert.That(buffer.TryReadFrame(out var a), Is.True);
            Assert.That(a, Is.EqualTo(new byte[] { 1 }));
            Assert.That(buffer.TryReadFrame(out var b), Is.True);
            Assert.That(b, Is.EqualTo(new byte[] { 2, 2 }));
            Assert.That(buffer.TryReadFrame(out var c), Is.True);
            Assert.That(c, Is.EqualTo(new byte[] { 3, 3, 3 }));
            Assert.That(buffer.TryReadFrame(out _), Is.False);
        });
    }

    [Test]
    public void TryReadFrame_WithoutEnoughBytesForPrefix_ReturnsFalse()
    {
        var buffer = new FrameBuffer();

        buffer.Append(new byte[] { 5 }, 0, 1);

        Assert.That(buffer.TryReadFrame(out _), Is.False);
    }

    [Test]
    public void TryReadFrame_WithOversizedDeclaredLength_ThrowsProtocolException()
    {
        var buffer = new FrameBuffer(8);

        buffer.Append(new byte[] { 0xFF, 0xFF }, 0, 2);

        Assert.Throws<ProtocolException>(() => buffer.TryReadFrame(out _));
    }

    [Test]
    public void Append_BeyondInitialCapacity_GrowsTheBuffer()
    {
        var buffer = new FrameBuffer(8);

        buffer.Append(new byte[512], 0, 512);

        Assert.That(buffer.BufferedBytes, Is.EqualTo(512));
    }

    [Test]
    public void Constructor_WithNonPositiveMaxFrameSize_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new FrameBuffer(0));
    }

    [Test]
    public void Append_WithNullData_Throws()
    {
        var buffer = new FrameBuffer();

        Assert.Throws<ArgumentNullException>(() => buffer.Append(null!, 0, 0));
    }

    [Test]
    public void Append_WithNegativeOffset_Throws()
    {
        var buffer = new FrameBuffer();

        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Append(new byte[1], -1, 0));
    }

    [Test]
    public void Append_WithNegativeCount_Throws()
    {
        var buffer = new FrameBuffer();

        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Append(new byte[1], 0, -1));
    }

    [Test]
    public void Append_WithRangeBeyondData_Throws()
    {
        var buffer = new FrameBuffer();

        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Append(new byte[1], 1, 1));
    }
}
