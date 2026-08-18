// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;
using PowerAutomate.Desktop.OpenTibia.Protocol;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

namespace PowerAutomate.Desktop.OpenTibia.Client.Handshake;

/// <summary>
/// Performs the login-server exchange: plaintext request out, XTEA-encrypted character list back.
/// </summary>
public sealed class TibiaLoginClient
{
    private const int ReadBufferSize = 4096;

    private readonly ISocketTransport _transport;
    private readonly Func<uint[]> _keyFactory;

    public TibiaLoginClient(ISocketTransport transport) : this(transport, XteaKeyGenerator.Generate)
    {
    }

    public TibiaLoginClient(ISocketTransport transport, Func<uint[]> keyFactory)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        _keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
    }

    public LoginResult Authenticate(LoginOptions options, TimeSpan timeout)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var key = _keyFactory();

        _transport.Connect(options.Host, options.Port, timeout);

        var request = new ClientLoginRequestMessage(
            options.Os,
            options.Version,
            0,
            0,
            0,
            key,
            options.AccountName,
            options.Password);

        var writer = new PacketWriter();
        request.Write(writer);
        _transport.Write(FrameCodec.EncodePlain(writer.ToArray()));

        return ReadResponse(key, timeout);
    }

    private LoginResult ReadResponse(uint[] key, TimeSpan timeout)
    {
        var frames = new FrameBuffer();
        var buffer = new byte[ReadBufferSize];
        var clock = Stopwatch.StartNew();

        var motd = string.Empty;

        while (clock.Elapsed < timeout)
        {
            var read = _transport.Read(buffer, 0, buffer.Length);

            if (read < 0)
            {
                continue;
            }

            if (read == 0)
            {
                throw new ProtocolException("The login server closed the connection before answering.");
            }

            frames.Append(buffer, 0, read);

            while (frames.TryReadFrame(out var body))
            {
                var payload = FrameCodec.DecodeEncrypted(body, key);

                foreach (var message in LoginServerMessageDispatcher.ReadAll(payload))
                {
                    var completed = Interpret(message, ref motd);
                    if (completed != null)
                    {
                        return completed;
                    }
                }
            }
        }

        throw new TimeoutException($"The login server did not answer within {timeout.TotalMilliseconds} ms.");
    }

    private static LoginResult? Interpret(IProtocolMessage message, ref string motd)
    {
        if (message is LoginServerErrorMessage error)
        {
            throw new LoginRejectedException(error.ErrorText);
        }

        if (message is LoginServerMotdMessage banner)
        {
            motd = banner.Motd;
            return null;
        }

        // The dispatcher only ever yields these three login-server messages.
        var list = (LoginServerCharacterListMessage)message;
        return new LoginResult(list.Characters, motd, list.PremiumDays);
    }
}

/// <summary>
/// Raised when the login server refuses the account.
/// </summary>
public class LoginRejectedException : Exception
{
    public LoginRejectedException(string message) : base(message)
    {
    }
}
