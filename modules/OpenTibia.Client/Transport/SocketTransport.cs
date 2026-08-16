// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace PowerAutomate.Desktop.OpenTibia.Client.Transport;

/// <summary>
/// Thin forwarding adapter over <see cref="TcpClient" />; all protocol logic lives above it.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class SocketTransport : ISocketTransport
{
    /// <summary>
    /// Short enough that the reader thread notices a shutdown request promptly.
    /// </summary>
    public static readonly TimeSpan ReadTimeout = TimeSpan.FromMilliseconds(500);

    public static readonly TimeSpan WriteTimeout = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Set to a file path to capture a hex dump of every frame in both directions.
    /// </summary>
    public static string? TraceFilePath { get; set; }

    private TcpClient? _client;
    private NetworkStream? _stream;

    public bool IsConnected => _client != null && _client.Connected;

    public void Connect(string host, int port, TimeSpan timeout)
    {
        var client = new TcpClient();
        var async = client.BeginConnect(host, port, null, null);

        if (!async.AsyncWaitHandle.WaitOne(timeout))
        {
            client.Close();
            throw new TimeoutException($"Connecting to {host}:{port} timed out after {timeout.TotalMilliseconds} ms.");
        }

        client.EndConnect(async);
        client.NoDelay = true;

        // Bounded so a dead peer surfaces as an error instead of blocking the reader thread forever.
        client.ReceiveTimeout = (int)ReadTimeout.TotalMilliseconds;
        client.SendTimeout = (int)WriteTimeout.TotalMilliseconds;

        _client = client;
        _stream = client.GetStream();
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        if (_stream == null)
        {
            throw new InvalidOperationException("The transport is not connected.");
        }

        try
        {
            var read = _stream.Read(buffer, offset, count);
            Trace("recv", buffer, offset, read);
            return read;
        }
        catch (IOException exception) when (IsReadTimeout(exception))
        {
            // Silence is normal between server pings; only a real close ends the loop.
            return -1;
        }
        catch (IOException)
        {
            return 0;
        }
    }

    private static bool IsReadTimeout(IOException exception)
    {
        return exception.InnerException is SocketException socket
               && socket.SocketErrorCode == SocketError.TimedOut;
    }

    /// <summary>
    /// Appends a hex dump when <see cref="TraceFilePath" /> is set; the only practical way to
    /// diagnose a wire mismatch against a live server.
    /// </summary>
    private static void Trace(string direction, byte[] buffer, int offset, int count)
    {
        var path = TraceFilePath;
        if (string.IsNullOrEmpty(path) || count <= 0)
        {
            return;
        }

        var hex = new StringBuilder(count * 3);
        for (var index = 0; index < count; index++)
        {
            hex.Append(buffer[offset + index].ToString("X2")).Append(' ');
        }

        File.AppendAllText(path, $"{DateTime.UtcNow:O} {direction} {count}: {hex}{Environment.NewLine}");
    }

    public void Write(byte[] data)
    {
        if (_stream == null)
        {
            throw new InvalidOperationException("The transport is not connected.");
        }

        _stream.Write(data, 0, data.Length);
        _stream.Flush();
        Trace("send", data, 0, data.Length);
    }

    public void Close()
    {
        _stream?.Dispose();
        _client?.Close();
        _stream = null;
        _client = null;
    }

    public void Dispose()
    {
        Close();
    }
}
