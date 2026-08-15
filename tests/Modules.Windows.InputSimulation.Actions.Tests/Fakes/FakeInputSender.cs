// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

internal sealed class FakeInputSender : IInputSender
{
    public List<ClickCall> Clicks { get; } = new();
    public List<ScrollCall> Scrolls { get; } = new();
    public List<VirtualKey> Keys { get; } = new();
    public List<TextCall> Texts { get; } = new();

    public void Click(IntPtr handle, MouseButton button, int x, int y, bool isDoubleClick) =>
        Clicks.Add(new ClickCall(handle, button, x, y, isDoubleClick));

    public void Scroll(IntPtr handle, ScrollDirection direction, int notches, int screenX, int screenY) =>
        Scrolls.Add(new ScrollCall(handle, direction, notches, screenX, screenY));

    public void SendKey(IntPtr handle, VirtualKey key) => Keys.Add(key);

    public void SendText(IntPtr handle, string text, int delayMilliseconds) => Texts.Add(new TextCall(handle, text, delayMilliseconds));

    internal readonly struct ClickCall
    {
        public ClickCall(IntPtr handle, MouseButton button, int x, int y, bool isDoubleClick)
        {
            Handle = handle;
            Button = button;
            X = x;
            Y = y;
            IsDoubleClick = isDoubleClick;
        }

        public MouseButton Button { get; }
        public IntPtr Handle { get; }
        public bool IsDoubleClick { get; }
        public int X { get; }
        public int Y { get; }
    }

    internal readonly struct ScrollCall
    {
        public ScrollCall(IntPtr handle, ScrollDirection direction, int notches, int screenX, int screenY)
        {
            Handle = handle;
            Direction = direction;
            Notches = notches;
            ScreenX = screenX;
            ScreenY = screenY;
        }

        public ScrollDirection Direction { get; }
        public IntPtr Handle { get; }
        public int Notches { get; }
        public int ScreenX { get; }
        public int ScreenY { get; }
    }

    internal readonly struct TextCall
    {
        public TextCall(IntPtr handle, string text, int delayMilliseconds)
        {
            Handle = handle;
            Text = text;
            DelayMilliseconds = delayMilliseconds;
        }

        public int DelayMilliseconds { get; }
        public IntPtr Handle { get; }
        public string Text { get; }
    }
}
