// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Linq;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Services;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Services;

[TestFixture]
public class WindowServiceTests
{
    private const uint GetTextLength = 0x000E;

    [SetUp]
    public void SetUp()
    {
        _native = new FakeNativeMethods();
        _dispatcher = new FakeMessageDispatcher();
        _clock = new FakeClock();
        _service = new WindowService(_native, _dispatcher, _clock);
        _handle = _native.RegisterWindow(10);
    }

    private FakeClock _clock = null!;
    private FakeMessageDispatcher _dispatcher = null!;
    private IntPtr _handle;
    private FakeNativeMethods _native = null!;
    private WindowService _service = null!;

    [Test]
    public void Constructor_WithoutNativeMethods_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new WindowService(null!, _dispatcher, _clock));

    [Test]
    public void Constructor_WithoutDispatcher_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new WindowService(_native, null!, _clock));

    [Test]
    public void Constructor_WithoutClock_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new WindowService(_native, _dispatcher, null!));

    [Test]
    public void GetClassName_WhenKnown_ReturnsIt()
    {
        _native.ClassNames[_handle] = "Edit";

        Assert.That(_service.GetClassName(_handle), Is.EqualTo("Edit"));
    }

    [Test]
    public void GetClassName_WhenUnknown_ReturnsEmpty()
    {
        Assert.That(_service.GetClassName(_handle), Is.Empty);
    }

    [Test]
    public void GetText_ReadsThroughTheDispatcher()
    {
        _dispatcher.SendResults[GetTextLength] = (IntPtr)5;
        _dispatcher.BufferText = "value";

        Assert.That(_service.GetText(_handle), Is.EqualTo("value"));
    }

    [Test]
    public void GetText_WhenEmpty_ReturnsEmpty()
    {
        _dispatcher.SendResults[GetTextLength] = IntPtr.Zero;

        Assert.That(_service.GetText(_handle), Is.Empty);
    }

    [Test]
    public void GetText_WhenLongerThanTheCap_IsClamped()
    {
        _dispatcher.SendResults[GetTextLength] = (IntPtr)50000;
        _dispatcher.BufferText = "clamped";

        Assert.That(_service.GetText(_handle), Is.EqualTo("clamped"));
    }

    [Test]
    public void GetText_WhenTheWindowCannotAnswer_FallsBackToTheCachedTitle()
    {
        _dispatcher.SendException = new InvalidOperationException("hung");
        _native.WindowTexts[_handle] = "cached";

        Assert.That(_service.GetText(_handle), Is.EqualTo("cached"));
    }

    [Test]
    public void GetText_WhenTheFallbackIsAlsoEmpty_ReturnsEmpty()
    {
        _dispatcher.SendException = new InvalidOperationException("hung");

        Assert.That(_service.GetText(_handle), Is.Empty);
    }

    [Test]
    public void GetText_WhenTheFallbackCopiesNothing_ReturnsEmpty()
    {
        _dispatcher.SendException = new InvalidOperationException("hung");
        _native.WindowTexts[_handle] = "reported";
        _native.FailWindowTextCopy = true;

        Assert.That(_service.GetText(_handle), Is.Empty);
    }

    [Test]
    public void FindControl_WhenTheClassDoesNotMatch_SkipsTheControl()
    {
        var child = _native.RegisterWindow(32);
        _native.ChildWindows.Add(child);
        _native.ClassNames[child] = "Static";

        Assert.That(_service.FindControl(_handle, null!, "Edit", null, TextMatchMode.Equals, true, 0), Is.EqualTo(IntPtr.Zero));
    }

    [Test]
    public void ToWindowObject_ProjectsTheWindowState()
    {
        _native.ClassNames[_handle] = "Button";
        _native.ControlIds[_handle] = 4;
        _native.ProcessIds[_handle] = 88;
        _native.ClientRects[_handle] = new NativeRect { Left = 0, Top = 0, Right = 30, Bottom = 20 };
        _dispatcher.SendResults[GetTextLength] = IntPtr.Zero;

        var window = _service.ToWindowObject(_handle);

        Assert.That(window.Handle, Is.EqualTo(10));
        Assert.That(window.ClassName, Is.EqualTo("Button"));
        Assert.That(window.ControlId, Is.EqualTo(4));
        Assert.That(window.ProcessId, Is.EqualTo(88));
        Assert.That(window.Width, Is.EqualTo(30));
        Assert.That(window.Height, Is.EqualTo(20));
        Assert.That(window.IsEnabled, Is.True);
        Assert.That(window.IsVisible, Is.True);
    }

    [Test]
    public void GetCenter_ReturnsTheClientMidpoint()
    {
        _native.ClientRects[_handle] = new NativeRect { Right = 40, Bottom = 10 };

        _service.GetCenter(_handle, out var x, out var y);

        Assert.That(x, Is.EqualTo(20));
        Assert.That(y, Is.EqualTo(5));
    }

    [Test]
    public void ClientToScreen_AppliesTheWindowOffset()
    {
        _native.ClientToScreenOffsetX = 100;
        _native.ClientToScreenOffsetY = 200;

        _service.ClientToScreen(_handle, 5, 6, out var screenX, out var screenY);

        Assert.That(screenX, Is.EqualTo(105));
        Assert.That(screenY, Is.EqualTo(206));
    }

    [Test]
    public void GetControlId_AndGetParent_ForwardToWin32()
    {
        _native.ControlIds[_handle] = 12;
        _native.Parents[_handle] = (IntPtr)999;

        Assert.That(_service.GetControlId(_handle), Is.EqualTo(12));
        Assert.That(_service.GetParent(_handle), Is.EqualTo((IntPtr)999));
    }

    [TestCase("Hello", "hello", TextMatchMode.Equals, true)]
    [TestCase("Hello", "other", TextMatchMode.Equals, false)]
    [TestCase("Hello world", "LO WO", TextMatchMode.Contains, true)]
    [TestCase("Hello", "zz", TextMatchMode.Contains, false)]
    [TestCase("Hello", "HEL", TextMatchMode.StartsWith, true)]
    [TestCase("Hello", "ello", TextMatchMode.StartsWith, false)]
    public void Matches_ComparesAccordingToTheMode(string candidate, string pattern, TextMatchMode mode, bool expected)
    {
        Assert.That(_service.Matches(candidate, pattern, mode), Is.EqualTo(expected));
    }

    [Test]
    public void Matches_WithAnEmptyPattern_AlwaysMatches()
    {
        Assert.That(_service.Matches("anything", string.Empty, TextMatchMode.Equals), Is.True);
    }

    [Test]
    public void Matches_WithANullCandidate_TreatsItAsEmpty()
    {
        Assert.That(_service.Matches(null!, "x", TextMatchMode.Contains), Is.False);
    }

    [Test]
    public void Matches_WithAnUnknownMode_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.Matches("a", "a", (TextMatchMode)77));
    }

    [Test]
    public void EnumerateTopLevelWindows_ReturnsEveryHandle()
    {
        _native.TopLevelWindows.AddRange(new[] { (IntPtr)1, (IntPtr)2 });

        Assert.That(_service.EnumerateTopLevelWindows(), Is.EqualTo(new[] { (IntPtr)1, (IntPtr)2 }));
    }

    [Test]
    public void EnumerateChildWindows_WhenRecursive_ReturnsTheWholeSubtree()
    {
        _native.ChildWindows.AddRange(new[] { (IntPtr)1, (IntPtr)2 });

        Assert.That(_service.EnumerateChildWindows(_handle, true).Count, Is.EqualTo(2));
    }

    [Test]
    public void EnumerateChildWindows_WhenNotRecursive_KeepsOnlyDirectChildren()
    {
        _native.ChildWindows.AddRange(new[] { (IntPtr)1, (IntPtr)2 });
        _native.Parents[(IntPtr)1] = _handle;
        _native.Parents[(IntPtr)2] = (IntPtr)500;

        Assert.That(_service.EnumerateChildWindows(_handle, false), Is.EqualTo(new[] { (IntPtr)1 }));
    }

    [Test]
    public void FindWindow_ReturnsTheFirstMatch()
    {
        var target = _native.RegisterWindow(20);
        _native.TopLevelWindows.AddRange(new[] { _handle, target });
        _native.ClassNames[_handle] = "Other";
        _native.ClassNames[target] = "Notepad";

        Assert.That(_service.FindWindow(null!, "Notepad", null, TextMatchMode.Equals, 0), Is.EqualTo(target));
    }

    [Test]
    public void FindWindow_FiltersByProcess()
    {
        _native.TopLevelWindows.Add(_handle);
        _native.ProcessIds[_handle] = 5;

        Assert.That(_service.FindWindow(null!, null!, 5, TextMatchMode.Equals, 0), Is.EqualTo(_handle));
        Assert.That(_service.FindWindow(null!, null!, 6, TextMatchMode.Equals, 0), Is.EqualTo(IntPtr.Zero));
    }

    [Test]
    public void FindWindow_FiltersByTitle()
    {
        _native.TopLevelWindows.Add(_handle);
        _dispatcher.SendResults[GetTextLength] = (IntPtr)4;
        _dispatcher.BufferText = "Main";

        Assert.That(_service.FindWindow("Main", null!, null, TextMatchMode.Equals, 0), Is.EqualTo(_handle));
    }

    [Test]
    public void FindWindow_WhenNothingMatches_RetriesUntilTheTimeout()
    {
        _native.TopLevelWindows.Add(_handle);
        _native.ClassNames[_handle] = "Other";

        var result = _service.FindWindow(null!, "Missing", null, TextMatchMode.Equals, 250);

        Assert.That(result, Is.EqualTo(IntPtr.Zero));
        Assert.That(_clock.Sleeps, Is.Not.Empty);
    }

    [Test]
    public void FindControl_MatchesOnClassAndControlId()
    {
        var child = _native.RegisterWindow(30);
        _native.ChildWindows.Add(child);
        _native.ClassNames[child] = "Edit";
        _native.ControlIds[child] = 8;

        Assert.That(_service.FindControl(_handle, null!, "Edit", 8, TextMatchMode.Equals, true, 0), Is.EqualTo(child));
        Assert.That(_service.FindControl(_handle, null!, "Edit", 9, TextMatchMode.Equals, true, 0), Is.EqualTo(IntPtr.Zero));
    }

    [Test]
    public void FindControl_MatchesOnText()
    {
        var child = _native.RegisterWindow(31);
        _native.ChildWindows.Add(child);
        _dispatcher.SendResults[GetTextLength] = (IntPtr)2;
        _dispatcher.BufferText = "Ok";

        Assert.That(_service.FindControl(_handle, "Ok", null!, null, TextMatchMode.Equals, true, 0), Is.EqualTo(child));
    }

    [Test]
    public void DescribeCriteria_SkipsNullAndEmptyValues()
    {
        var description = _service.DescribeCriteria(
            new SearchCriterion("title", "Main"),
            new SearchCriterion("class", null),
            new SearchCriterion("text", string.Empty),
            new SearchCriterion("process", 12));

        Assert.That(description, Is.EqualTo("title 'Main', process '12'"));
    }

    [Test]
    public void DescribeCriteria_WithNothingUseful_FallsBackToAPhrase()
    {
        Assert.That(_service.DescribeCriteria(new SearchCriterion("title", null)), Is.EqualTo("the supplied criteria"));
    }
}
