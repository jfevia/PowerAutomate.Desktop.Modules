// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Abstractions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

/// <inheritdoc />
public sealed class InputSender : IInputSender
{
    private const uint MapVirtualKeyToScanCode = 0;

    private readonly IClock _clock;
    private readonly IMessageDispatcher _messageDispatcher;
    private readonly INativeMethods _nativeMethods;

    public InputSender(IMessageDispatcher messageDispatcher, INativeMethods nativeMethods, IClock clock)
    {
        _messageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
        _nativeMethods = nativeMethods ?? throw new ArgumentNullException(nameof(nativeMethods));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public void Click(IntPtr handle, MouseButton button, int x, int y, bool isDoubleClick)
    {
        var messages = GetButtonMessages(button);
        var coordinates = MessageDispatcher.MakeCoordinates(x, y);
        var buttonState = (IntPtr)messages.KeyState;

        // Controls that track hover state need the move before they accept the press.
        _messageDispatcher.Post(handle, WindowMessages.MouseMove, IntPtr.Zero, coordinates);
        _messageDispatcher.Post(handle, messages.Down, buttonState, coordinates);
        _messageDispatcher.Post(handle, messages.Up, IntPtr.Zero, coordinates);

        if (!isDoubleClick)
        {
            return;
        }

        _messageDispatcher.Post(handle, messages.DoubleClick, buttonState, coordinates);
        _messageDispatcher.Post(handle, messages.Up, IntPtr.Zero, coordinates);
    }

    public void SendText(IntPtr handle, string text, int delayMilliseconds)
    {
        if (text is null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        foreach (var character in text)
        {
            _messageDispatcher.Post(handle, WindowMessages.Char, (IntPtr)character, (IntPtr)1);

            if (delayMilliseconds > 0)
            {
                _clock.Sleep(delayMilliseconds);
            }
        }
    }

    public void SendKey(IntPtr handle, VirtualKey key)
    {
        var keyCode = (uint)key;
        var scanCode = _nativeMethods.MapVirtualKey(keyCode, MapVirtualKeyToScanCode);
        var extendedFlag = IsExtendedKey(key) ? 1u << 24 : 0u;

        var downParameter = 1u | ((scanCode & 0xFF) << 16) | extendedFlag;
        var upParameter = downParameter | 0xC0000000;

        _messageDispatcher.Post(handle, WindowMessages.KeyDown, (IntPtr)keyCode, (IntPtr)downParameter);
        _messageDispatcher.Post(handle, WindowMessages.KeyUp, (IntPtr)keyCode, unchecked((IntPtr)(int)upParameter));
    }

    public void Scroll(IntPtr handle, ScrollDirection direction, int notches, int screenX, int screenY)
    {
        var delta = GetWheelDelta(direction);

        // Unlike other mouse messages, the wheel carries screen coordinates.
        var coordinates = MessageDispatcher.MakeCoordinates(screenX, screenY);
        var wheelParameter = (IntPtr)((delta & 0xFFFF) << 16);

        for (var index = 0; index < notches; index++)
        {
            _messageDispatcher.Post(handle, WindowMessages.MouseWheel, wheelParameter, coordinates);
        }
    }

    private static int GetWheelDelta(ScrollDirection direction)
    {
        return direction switch
        {
            ScrollDirection.Up => WindowMessages.WheelDelta,
            ScrollDirection.Down => -WindowMessages.WheelDelta,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown scroll direction.")
        };
    }

    private static bool IsExtendedKey(VirtualKey key)
    {
        switch (key)
        {
            case VirtualKey.Insert:
            case VirtualKey.Delete:
            case VirtualKey.Home:
            case VirtualKey.End:
            case VirtualKey.PageUp:
            case VirtualKey.PageDown:
            case VirtualKey.ArrowLeft:
            case VirtualKey.ArrowUp:
            case VirtualKey.ArrowRight:
            case VirtualKey.ArrowDown:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Resolved in one place so an unknown button cannot leave part of the sequence undefined.
    /// </summary>
    private static ButtonMessages GetButtonMessages(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => new ButtonMessages(WindowMessages.LeftButtonDown, WindowMessages.LeftButtonUp, WindowMessages.LeftButtonDoubleClick, WindowMessages.MouseKeyLeftButton),
            MouseButton.Right => new ButtonMessages(WindowMessages.RightButtonDown, WindowMessages.RightButtonUp, WindowMessages.RightButtonDoubleClick, WindowMessages.MouseKeyRightButton),
            MouseButton.Middle => new ButtonMessages(WindowMessages.MiddleButtonDown, WindowMessages.MiddleButtonUp, WindowMessages.MiddleButtonDoubleClick, WindowMessages.MouseKeyMiddleButton),
            _ => throw new ArgumentOutOfRangeException(nameof(button), button, "Unknown mouse button.")
        };
    }

    private readonly struct ButtonMessages
    {
        public ButtonMessages(uint down, uint up, uint doubleClick, int keyState)
        {
            Down = down;
            Up = up;
            DoubleClick = doubleClick;
            KeyState = keyState;
        }

        public uint Down { get; }
        public uint DoubleClick { get; }
        public int KeyState { get; }
        public uint Up { get; }
    }
}
