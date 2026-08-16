// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Client;
using PowerAutomate.Desktop.OpenTibia.Client.Handshake;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;

/// <summary>
/// Builds login and game sessions wired to a fake transport, bypassing the real actions.
/// </summary>
internal static class SessionFactory
{
    public static readonly uint[] LoginKey = { 1u, 2u, 3u, 4u };
    public static readonly uint[] GameKey = { 9u, 8u, 7u, 6u };
    public static readonly TimeSpan Patience = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Authenticates a login session against a fake transport already scripted with its answer.
    /// </summary>
    public static TibiaLoginSession AuthenticateLoginSession(
        FakeSocketTransport transport,
        string accountName = "account",
        string password = "secret",
        string host = "login.example",
        int port = 7171)
    {
        var client = new TibiaLoginClient(transport, () => LoginKey);
        var options = new LoginOptions(host, port, accountName, password);
        var result = client.Authenticate(options, Patience);

        return new TibiaLoginSession(transport, client, host, port, accountName, password, result);
    }

    /// <summary>
    /// A login session scripted with a Motd and the given characters, ready to use.
    /// </summary>
    public static TibiaLoginSession CreateLoginSession(FakeSocketTransport transport, params string[] characterNames)
    {
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(Frames.LoginMotdPayload("welcome"), LoginKey));
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(
            Frames.LoginCharacterListPayload(characterNames.Length == 0 ? new[] { "Rook" } : characterNames),
            LoginKey));

        return AuthenticateLoginSession(transport);
    }

    /// <summary>
    /// A game session that has already completed EnterGame and stays healthy (reads block once exhausted).
    /// </summary>
    public static TibiaGameSession CreateInGameSession(
        FakeSocketTransport transport,
        string characterName = "Rook",
        string world = "Antica",
        int queueCapacity = 64)
    {
        transport.BlockWhenExhausted();
        transport.EnqueueRead(Frames.GameChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(Frames.GamePendingStatePayload(), GameKey));

        var client = new TibiaGameClient(transport, GameServerRegistryFactory.CreateDefault(), () => GameKey);
        var options = new GameOptions("game.example", 7172, "account", characterName, "secret")
        {
            QueueCapacity = queueCapacity
        };
        client.EnterGame(options, Patience);

        return new TibiaGameSession(client, characterName, world);
    }

    /// <summary>
    /// A game session that has entered the game but whose transport closes right after, causing a fault.
    /// </summary>
    public static TibiaGameSession CreateInGameSessionThatWillFault(
        FakeSocketTransport transport,
        string characterName = "Rook",
        string world = "Antica")
    {
        transport.EnqueueRead(Frames.GameChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(Frames.GamePendingStatePayload(), GameKey));

        var client = new TibiaGameClient(transport, GameServerRegistryFactory.CreateDefault(), () => GameKey);
        var options = new GameOptions("game.example", 7172, "account", characterName, "secret");
        client.EnterGame(options, Patience);

        return new TibiaGameSession(client, characterName, world);
    }

    /// <summary>
    /// A game session whose client was constructed but never entered the game.
    /// </summary>
    public static TibiaGameSession CreateDisconnectedGameSession(
        FakeSocketTransport transport,
        string characterName = "Rook",
        string world = "Antica")
    {
        var client = new TibiaGameClient(transport, GameServerRegistryFactory.CreateDefault(), () => GameKey);
        return new TibiaGameSession(client, characterName, world);
    }
}
