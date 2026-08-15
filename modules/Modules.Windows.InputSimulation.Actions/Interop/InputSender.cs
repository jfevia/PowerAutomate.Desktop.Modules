// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Threading;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Extensions;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

/// <summary>
/// Builds the message sequences Windows itself would produce for mouse and keyboard input.
/// </summary>
internal static class InputSender
{
    public static void Click(IntPtr handle, MouseButton button, int x, int y, bool isDoubleClick)
    {
        var coordinates = MessageDispatcher.MakeCoordinates(x, y);
        var buttonState = (IntPtr)GetButtonState(button);

        // Controls that track hover state need the move before they accept the press.
        MessageDispatcher.Post(handle, WindowMessages.MouseMove, IntPtr.Zero, coordinates);
        MessageDispatcher.Post(handle, GetButtonDownMessage(button), buttonState, coordinates);
        MessageDispatcher.Post(handle, GetButtonUpMessage(button), IntPtr.Zero, coordinates);

        if (!isDoubleClick)
        {
            return;
        }

        MessageDispatcher.Post(handle, GetButtonDoubleClickMessage(button), buttonState, coordinates);
        MessageDispatcher.Post(handle, GetButtonUpMessage(button), IntPtr.Zero, coordinates);
    }

    public static void SendText(IntPtr handle, string text, int delayMilliseconds)
    {
        foreach (var character in text)
        {
            MessageDispatcher.Post(handle, WindowMessages.Char, (IntPtr)character, (IntPtr)1);

            if (delayMilliseconds > 0)
            {
                Thread.Sleep(delayMilliseconds);
            }
        }
    }

    public static void SendKey(IntPtr handle, VirtualKey key)
    {
        var keyCode = (uint)key;
        var scanCode = NativeMethods.MapVirtualKey(keyCode, NativeMethods.MapVirtualKeyToScanCode);
        var extendedFlag = IsExtendedKey(key) ? 1u << 24 : 0u;

        var downParameter = 1u | ((scanCode & 0xFF) << 16) | extendedFlag;
        var upParameter = downParameter | 0xC0000000;

        MessageDispatcher.Post(handle, WindowMessages.KeyDown, (IntPtr)keyCode, (IntPtr)downParameter);
        MessageDispatcher.Post(handle, WindowMessages.KeyUp, (IntPtr)keyCode, unchecked((IntPtr)(int)upParameter));
    }

    public static void Scroll(IntPtr handle, ScrollDirection direction, int notches)
    {
        var delta = direction == ScrollDirection.Up ? WindowMessages.WheelDelta : -WindowMessages.WheelDelta;

        WindowExtensions.GetCenter(handle, out var centerX, out var centerY);

        // Unlike other mouse messages, the wheel carries screen coordinates.
        WindowExtensions.ClientToScreen(handle, centerX, centerY, out var screenX, out var screenY);
        var coordinates = MessageDispatcher.MakeCoordinates(screenX, screenY);

        for (var index = 0; index < notches; index++)
        {
            var wheelParameter = (IntPtr)((delta & 0xFFFF) << 16);
            MessageDispatcher.Post(handle, WindowMessages.MouseWheel, wheelParameter, coordinates);
        }
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

    private static int GetButtonState(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => WindowMessages.MouseKeyLeftButton,
            MouseButton.Right => WindowMessages.MouseKeyRightButton,
            MouseButton.Middle => WindowMessages.MouseKeyMiddleButton,
            _ => throw new ArgumentOutOfRangeException(nameof(button), button, "Unknown mouse button.")
        };
    }

    private static uint GetButtonDownMessage(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => WindowMessages.LeftButtonDown,
            MouseButton.Right => WindowMessages.RightButtonDown,
            MouseButton.Middle => WindowMessages.MiddleButtonDown,
            _ => throw new ArgumentOutOfRangeException(nameof(button), button, "Unknown mouse button.")
        };
    }

    private static uint GetButtonUpMessage(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => WindowMessages.LeftButtonUp,
            MouseButton.Right => WindowMessages.RightButtonUp,
            MouseButton.Middle => WindowMessages.MiddleButtonUp,
            _ => throw new ArgumentOutOfRangeException(nameof(button), button, "Unknown mouse button.")
        };
    }

    private static uint GetButtonDoubleClickMessage(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => WindowMessages.LeftButtonDoubleClick,
            MouseButton.Right => WindowMessages.RightButtonDoubleClick,
            MouseButton.Middle => WindowMessages.MiddleButtonDoubleClick,
            _ => throw new ArgumentOutOfRangeException(nameof(button), button, "Unknown mouse button.")
        };
    }
}
