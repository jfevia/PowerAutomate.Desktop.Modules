// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

/// <summary>
/// Records what the actions asked to deliver, and can be told to fail on demand.
/// </summary>
internal sealed class FakeMessageDispatcher : IMessageDispatcher
{
    public List<Call> Posts { get; } = new();
    public List<Call> Sends { get; } = new();

    public Exception? PostException { get; set; }
    public Exception? SendException { get; set; }

    /// <summary>
    /// Answers returned by Send, keyed by message; anything unlisted returns <see cref="DefaultSendResult" />.
    /// </summary>
    public Dictionary<uint, IntPtr> SendResults { get; } = new();

    public IntPtr DefaultSendResult { get; set; } = IntPtr.Zero;

    public string BufferText { get; set; } = string.Empty;

    public void Post(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam)
    {
        Posts.Add(new Call(handle, message, wParam, lParam, null));

        if (PostException is not null)
        {
            throw PostException;
        }
    }

    public IntPtr Send(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam)
    {
        Sends.Add(new Call(handle, message, wParam, lParam, null));

        if (SendException is not null)
        {
            throw SendException;
        }

        return Resolve(message);
    }

    public IntPtr SendText(IntPtr handle, uint message, IntPtr wParam, string lParam)
    {
        Sends.Add(new Call(handle, message, wParam, IntPtr.Zero, lParam));

        if (SendException is not null)
        {
            throw SendException;
        }

        return Resolve(message);
    }

    public IntPtr SendBuffer(IntPtr handle, uint message, IntPtr wParam, StringBuilder lParam)
    {
        Sends.Add(new Call(handle, message, wParam, IntPtr.Zero, null));

        if (SendException is not null)
        {
            throw SendException;
        }

        lParam.Append(BufferText);
        return Resolve(message);
    }

    private IntPtr Resolve(uint message) => SendResults.TryGetValue(message, out var value) ? value : DefaultSendResult;

    internal readonly struct Call
    {
        public Call(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam, string? text)
        {
            Handle = handle;
            Message = message;
            WParam = wParam;
            LParam = lParam;
            Text = text;
        }

        public IntPtr Handle { get; }
        public IntPtr LParam { get; }
        public uint Message { get; }
        public string? Text { get; }
        public IntPtr WParam { get; }
    }
}
