// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Text;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Exceptions;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

/// <summary>
/// Delivers window messages straight to a target window queue, bypassing the system input stack so
/// the physical cursor and global key state are never touched.
/// </summary>
internal static class MessageDispatcher
{
    private const uint DefaultTimeoutMilliseconds = 5000;

    /// <summary>
    /// Packs client-area coordinates the way mouse messages expect them.
    /// </summary>
    public static IntPtr MakeCoordinates(int x, int y)
    {
        return (IntPtr)(((y & 0xFFFF) << 16) | (x & 0xFFFF));
    }

    public static IntPtr MakeNotification(int controlId, int notificationCode)
    {
        return (IntPtr)(((notificationCode & 0xFFFF) << 16) | (controlId & 0xFFFF));
    }

    /// <summary>
    /// Queues a message and returns immediately; used for input that the target processes on its own thread.
    /// </summary>
    public static void Post(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam)
    {
        EnsureWindow(handle);

        if (NativeMethods.PostMessage(handle, message, wParam, lParam))
        {
            return;
        }

        ThrowForLastError(Marshal.GetLastWin32Error(), message);
    }

    /// <summary>
    /// Sends a message and waits for the answer, aborting instead of hanging when the target stops pumping.
    /// </summary>
    public static IntPtr Send(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam, uint timeoutMilliseconds = DefaultTimeoutMilliseconds)
    {
        EnsureWindow(handle);

        var result = NativeMethods.SendMessageTimeout(handle, message, wParam, lParam, NativeMethods.SendMessageTimeoutAbortIfHung, timeoutMilliseconds, out var response);
        if (result == IntPtr.Zero)
        {
            ThrowForLastError(Marshal.GetLastWin32Error(), message);
        }

        return response;
    }

    public static IntPtr SendText(IntPtr handle, uint message, IntPtr wParam, string lParam, uint timeoutMilliseconds = DefaultTimeoutMilliseconds)
    {
        EnsureWindow(handle);

        var result = NativeMethods.SendMessageTimeoutText(handle, message, wParam, lParam, NativeMethods.SendMessageTimeoutAbortIfHung, timeoutMilliseconds, out var response);
        if (result == IntPtr.Zero)
        {
            ThrowForLastError(Marshal.GetLastWin32Error(), message);
        }

        return response;
    }

    public static IntPtr SendBuffer(IntPtr handle, uint message, IntPtr wParam, StringBuilder lParam, uint timeoutMilliseconds = DefaultTimeoutMilliseconds)
    {
        EnsureWindow(handle);

        var result = NativeMethods.SendMessageTimeoutBuffer(handle, message, wParam, lParam, NativeMethods.SendMessageTimeoutAbortIfHung, timeoutMilliseconds, out var response);
        if (result == IntPtr.Zero)
        {
            ThrowForLastError(Marshal.GetLastWin32Error(), message);
        }

        return response;
    }

    private static void EnsureWindow(IntPtr handle)
    {
        if (handle == IntPtr.Zero || !NativeMethods.IsWindow(handle))
        {
            throw new MessageDeliveryException("The target window no longer exists. Locate it again before sending input to it.");
        }
    }

    private static void ThrowForLastError(int errorCode, uint message)
    {
        switch (errorCode)
        {
            case NativeMethods.ErrorAccessDenied:
                throw new AccessDeniedException();
            case NativeMethods.ErrorTimeout:
                throw new MessageDeliveryException($"The target window did not process message 0x{message:X4} in time. It may be busy or not responding.");
            case 0:
                // SendMessageTimeout reports a zero result for handlers that legitimately return zero.
                return;
            default:
                throw new MessageDeliveryException($"Windows rejected message 0x{message:X4} with error code {errorCode}.");
        }
    }
}
