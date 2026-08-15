// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Linq;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Interop;

[TestFixture]
public class InputSenderTests
{
    [SetUp]
    public void SetUp()
    {
        _dispatcher = new FakeMessageDispatcher();
        _native = new FakeNativeMethods();
        _clock = new FakeClock();
        _sender = new InputSender(_dispatcher, _native, _clock);
        _handle = (IntPtr)10;
    }

    private FakeClock _clock = null!;
    private FakeMessageDispatcher _dispatcher = null!;
    private IntPtr _handle;
    private FakeNativeMethods _native = null!;
    private InputSender _sender = null!;

    [Test]
    public void Constructor_WithoutDispatcher_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new InputSender(null!, _native, _clock));

    [Test]
    public void Constructor_WithoutNativeMethods_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new InputSender(_dispatcher, null!, _clock));

    [Test]
    public void Constructor_WithoutClock_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new InputSender(_dispatcher, _native, null!));

    [TestCase(MouseButton.Left, 0x0201u, 0x0202u, 1)]
    [TestCase(MouseButton.Right, 0x0204u, 0x0205u, 2)]
    [TestCase(MouseButton.Middle, 0x0207u, 0x0208u, 16)]
    public void Click_SendsHoverThenPressAndRelease(MouseButton button, uint down, uint up, int keyState)
    {
        _sender.Click(_handle, button, 5, 6, false);

        Assert.That(_dispatcher.Posts.Select(p => p.Message), Is.EqualTo(new[] { 0x0200u, down, up }));
        Assert.That(_dispatcher.Posts[1].WParam, Is.EqualTo((IntPtr)keyState));
        Assert.That(_dispatcher.Posts[1].LParam, Is.EqualTo((IntPtr)0x00060005));
    }

    [Test]
    public void Click_WhenDoubleClicking_AppendsTheDoubleClickPair()
    {
        _sender.Click(_handle, MouseButton.Left, 0, 0, true);

        Assert.That(_dispatcher.Posts.Select(p => p.Message), Is.EqualTo(new[] { 0x0200u, 0x0201u, 0x0202u, 0x0203u, 0x0202u }));
    }

    [Test]
    public void Click_WithAnUnknownButton_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _sender.Click(_handle, (MouseButton)99, 0, 0, false));
    }

    [Test]
    public void SendText_PostsOneCharacterMessagePerCharacter()
    {
        _sender.SendText(_handle, "hi", 0);

        Assert.That(_dispatcher.Posts.Count, Is.EqualTo(2));
        Assert.That(_dispatcher.Posts[0].WParam, Is.EqualTo((IntPtr)'h'));
        Assert.That(_dispatcher.Posts[1].WParam, Is.EqualTo((IntPtr)'i'));
        Assert.That(_clock.Sleeps, Is.Empty);
    }

    [Test]
    public void SendText_WithADelay_WaitsBetweenCharacters()
    {
        _sender.SendText(_handle, "ab", 15);

        Assert.That(_clock.Sleeps, Is.EqualTo(new[] { 15, 15 }));
    }

    [Test]
    public void SendText_WithAnEmptyString_PostsNothing()
    {
        _sender.SendText(_handle, string.Empty, 5);

        Assert.That(_dispatcher.Posts, Is.Empty);
    }

    [Test]
    public void SendText_WithNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _sender.SendText(_handle, null!, 0));
    }

    [Test]
    public void SendKey_PostsDownThenUpWithTheScanCode()
    {
        _native.ScanCode = 0x1C;

        _sender.SendKey(_handle, VirtualKey.Enter);

        Assert.That(_dispatcher.Posts.Select(p => p.Message), Is.EqualTo(new[] { 0x0100u, 0x0101u }));
        Assert.That(_dispatcher.Posts[0].LParam, Is.EqualTo((IntPtr)0x001C0001));
    }

    [Test]
    public void SendKey_ForAnExtendedKey_SetsTheExtendedFlag()
    {
        _native.ScanCode = 0x4B;

        _sender.SendKey(_handle, VirtualKey.ArrowLeft);

        Assert.That(_dispatcher.Posts[0].LParam, Is.EqualTo((IntPtr)0x014B0001));
    }

    [TestCase(ScrollDirection.Up, 0x00780000)]
    [TestCase(ScrollDirection.Down, unchecked((int)0xFF880000))]
    public void Scroll_PostsOneWheelMessagePerNotch(ScrollDirection direction, int expectedWheelParameter)
    {
        _sender.Scroll(_handle, direction, 2, 100, 200);

        Assert.That(_dispatcher.Posts.Count, Is.EqualTo(2));
        Assert.That(_dispatcher.Posts[0].Message, Is.EqualTo(0x020Au));
        Assert.That(_dispatcher.Posts[0].WParam, Is.EqualTo((IntPtr)expectedWheelParameter));
        Assert.That(_dispatcher.Posts[0].LParam, Is.EqualTo((IntPtr)0x00C80064));
    }

    [Test]
    public void Scroll_WithAnUnknownDirection_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _sender.Scroll(_handle, (ScrollDirection)42, 1, 0, 0));
    }

    [Test]
    public void Scroll_WithZeroNotches_PostsNothing()
    {
        _sender.Scroll(_handle, ScrollDirection.Up, 0, 0, 0);

        Assert.That(_dispatcher.Posts, Is.Empty);
    }
}
