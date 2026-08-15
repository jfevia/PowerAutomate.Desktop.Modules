// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Text;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Exceptions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Interop;

[TestFixture]
public class MessageDispatcherTests
{
    private const uint Message = 0x0201;

    [SetUp]
    public void SetUp()
    {
        _native = new FakeNativeMethods();
        _handle = _native.RegisterWindow(10);
        _dispatcher = new MessageDispatcher(_native);
    }

    private MessageDispatcher _dispatcher = null!;
    private IntPtr _handle;
    private FakeNativeMethods _native = null!;

    [Test]
    public void Constructor_WithoutNativeMethods_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MessageDispatcher(null!));
    }

    [Test]
    public void MakeCoordinates_PacksYIntoTheHighWord()
    {
        Assert.That(MessageDispatcher.MakeCoordinates(3, 4), Is.EqualTo((IntPtr)0x00040003));
    }

    [Test]
    public void MakeNotification_PacksTheCodeIntoTheHighWord()
    {
        Assert.That(MessageDispatcher.MakeNotification(9, 1), Is.EqualTo((IntPtr)0x00010009));
    }

    [Test]
    public void Post_WhenAccepted_RecordsTheMessage()
    {
        _dispatcher.Post(_handle, Message, IntPtr.Zero, IntPtr.Zero);

        Assert.That(_native.Posted.Count, Is.EqualTo(1));
        Assert.That(_native.Posted[0].Message, Is.EqualTo(Message));
    }

    [Test]
    public void Post_WhenRejectedWithAccessDenied_Throws()
    {
        _native.PostMessageResult = false;
        _native.LastError = 5;

        Assert.Throws<AccessDeniedException>(() => _dispatcher.Post(_handle, Message, IntPtr.Zero, IntPtr.Zero));
    }

    [Test]
    public void Post_WhenRejectedWithoutError_ReturnsQuietly()
    {
        _native.PostMessageResult = false;
        _native.LastError = 0;

        Assert.DoesNotThrow(() => _dispatcher.Post(_handle, Message, IntPtr.Zero, IntPtr.Zero));
    }

    [Test]
    public void Post_WhenRejectedWithAnotherError_ReportsTheCode()
    {
        _native.PostMessageResult = false;
        _native.LastError = 1400;

        var exception = Assert.Throws<MessageDeliveryException>(() => _dispatcher.Post(_handle, Message, IntPtr.Zero, IntPtr.Zero))!;
        Assert.That(exception.Message, Does.Contain("1400"));
    }

    [Test]
    public void Post_ToAMissingWindow_Throws()
    {
        Assert.Throws<MessageDeliveryException>(() => _dispatcher.Post((IntPtr)999, Message, IntPtr.Zero, IntPtr.Zero));
    }

    [Test]
    public void Post_ToTheNullHandle_Throws()
    {
        Assert.Throws<MessageDeliveryException>(() => _dispatcher.Post(IntPtr.Zero, Message, IntPtr.Zero, IntPtr.Zero));
    }

    [Test]
    public void Send_WhenAnswered_ReturnsTheResult()
    {
        _native.SendResult = (IntPtr)7;

        Assert.That(_dispatcher.Send(_handle, Message, IntPtr.Zero, IntPtr.Zero), Is.EqualTo((IntPtr)7));
    }

    [Test]
    public void Send_WhenTimedOut_Throws()
    {
        _native.SendOutcome = IntPtr.Zero;
        _native.LastError = 1460;

        var exception = Assert.Throws<MessageDeliveryException>(() => _dispatcher.Send(_handle, Message, IntPtr.Zero, IntPtr.Zero))!;
        Assert.That(exception.Message, Does.Contain("in time"));
    }

    [Test]
    public void Send_WhenHandlerReturnsZeroWithoutError_ReportsZero()
    {
        _native.SendOutcome = IntPtr.Zero;
        _native.LastError = 0;

        Assert.That(_dispatcher.Send(_handle, Message, IntPtr.Zero, IntPtr.Zero), Is.EqualTo(IntPtr.Zero));
    }

    [Test]
    public void SendText_WhenAnswered_PassesTheStringThrough()
    {
        _native.SendResult = (IntPtr)1;

        _dispatcher.SendText(_handle, Message, IntPtr.Zero, "payload");

        Assert.That(_native.Sent[0].Text, Is.EqualTo("payload"));
    }

    [Test]
    public void SendText_WhenDenied_Throws()
    {
        _native.SendOutcome = IntPtr.Zero;
        _native.LastError = 5;

        Assert.Throws<AccessDeniedException>(() => _dispatcher.SendText(_handle, Message, IntPtr.Zero, "x"));
    }

    [Test]
    public void SendText_ToAMissingWindow_Throws()
    {
        Assert.Throws<MessageDeliveryException>(() => _dispatcher.SendText((IntPtr)55, Message, IntPtr.Zero, "x"));
    }

    [Test]
    public void SendBuffer_FillsTheCallerBuffer()
    {
        _native.BufferText = "filled";
        _native.SendResult = (IntPtr)6;
        var buffer = new StringBuilder(16);

        _dispatcher.SendBuffer(_handle, Message, (IntPtr)16, buffer);

        Assert.That(buffer.ToString(), Is.EqualTo("filled"));
    }

    [Test]
    public void SendBuffer_WhenDenied_Throws()
    {
        _native.SendOutcome = IntPtr.Zero;
        _native.LastError = 5;

        Assert.Throws<AccessDeniedException>(() => _dispatcher.SendBuffer(_handle, Message, IntPtr.Zero, new StringBuilder()));
    }

    [Test]
    public void SendBuffer_ToAMissingWindow_Throws()
    {
        Assert.Throws<MessageDeliveryException>(() => _dispatcher.SendBuffer((IntPtr)77, Message, IntPtr.Zero, new StringBuilder()));
    }

    [Test]
    public void Send_ToAMissingWindow_Throws()
    {
        Assert.Throws<MessageDeliveryException>(() => _dispatcher.Send((IntPtr)66, Message, IntPtr.Zero, IntPtr.Zero));
    }
}
