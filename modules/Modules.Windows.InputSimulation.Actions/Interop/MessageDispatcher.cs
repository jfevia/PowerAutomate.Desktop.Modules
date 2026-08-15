// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Text;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Exceptions;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

/// <inheritdoc />
public sealed class MessageDispatcher : IMessageDispatcher
{
    private const uint TimeoutMilliseconds = 5000;
    private const int ErrorAccessDenied = 5;
    private const int ErrorTimeout = 1460;
    private const uint AbortIfHung = 0x0002;

    private readonly INativeMethods _nativeMethods;

    public MessageDispatcher(INativeMethods nativeMethods)
    {
        _nativeMethods = nativeMethods ?? throw new ArgumentNullException(nameof(nativeMethods));
    }

    /// <summary>
    /// Packs client-area coordinates the way mouse messages expect them.
    /// </summary>
    public static IntPtr MakeCoordinates(int x, int y) => (IntPtr)(((y & 0xFFFF) << 16) | (x & 0xFFFF));

    public static IntPtr MakeNotification(int controlId, int notificationCode) => (IntPtr)(((notificationCode & 0xFFFF) << 16) | (controlId & 0xFFFF));

    public void Post(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam)
    {
        EnsureWindow(handle);

        if (_nativeMethods.PostMessage(handle, message, wParam, lParam))
        {
            return;
        }

        Throw(_nativeMethods.GetLastError(), message);
    }

    public IntPtr Send(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam)
    {
        EnsureWindow(handle);

        var outcome = _nativeMethods.SendMessageTimeout(handle, message, wParam, lParam, AbortIfHung, TimeoutMilliseconds, out var result);
        return outcome == IntPtr.Zero ? Reject(message) : result;
    }

    public IntPtr SendText(IntPtr handle, uint message, IntPtr wParam, string lParam)
    {
        EnsureWindow(handle);

        var outcome = _nativeMethods.SendMessageTimeoutText(handle, message, wParam, lParam, AbortIfHung, TimeoutMilliseconds, out var result);
        return outcome == IntPtr.Zero ? Reject(message) : result;
    }

    public IntPtr SendBuffer(IntPtr handle, uint message, IntPtr wParam, StringBuilder lParam)
    {
        EnsureWindow(handle);

        var outcome = _nativeMethods.SendMessageTimeoutBuffer(handle, message, wParam, lParam, AbortIfHung, TimeoutMilliseconds, out var result);
        return outcome == IntPtr.Zero ? Reject(message) : result;
    }

    private IntPtr Reject(uint message)
    {
        Throw(_nativeMethods.GetLastError(), message);

        // A zero result with no recorded error means the handler legitimately returned zero.
        return IntPtr.Zero;
    }

    private void EnsureWindow(IntPtr handle)
    {
        if (handle == IntPtr.Zero || !_nativeMethods.IsWindow(handle))
        {
            throw new MessageDeliveryException("The target window no longer exists. Locate it again before sending input to it.");
        }
    }

    private static void Throw(int errorCode, uint message)
    {
        switch (errorCode)
        {
            case ErrorAccessDenied:
                throw new AccessDeniedException();
            case ErrorTimeout:
                throw new MessageDeliveryException($"The target window did not process message 0x{message:X4} in time. It may be busy or not responding.");
            case 0:
                return;
            default:
                throw new MessageDeliveryException($"Windows rejected message 0x{message:X4} with error code {errorCode}.");
        }
    }
}
