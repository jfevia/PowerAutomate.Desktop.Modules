// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using PowerAutomate.Desktop.OpenTibia.Client;
using PowerAutomate.Desktop.OpenTibia.Client.Handshake;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;
using PowerAutomate.Desktop.OpenTibia.Protocol;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.ItemActions;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

namespace OpenTibia.LiveHarness;

public static class Program
{
    private static StreamWriter _log = StreamWriter.Null;
    private static int _sliceMs = 300;
    private static long _drainCalls;

    public static int Main(string[] args)
    {
        if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
        {
            foreach (var line in HarnessOptions.Usage())
            {
                Console.WriteLine(line);
            }

            return 2;
        }

        if (string.Equals(args[0], "--dump-items", StringComparison.Ordinal))
        {
            return RunDumpItems(args);
        }

        if (string.Equals(args[0], "--find-floorchange", StringComparison.Ordinal))
        {
            return RunFindFloorChange(args);
        }

        HarnessOptions options;
        try
        {
            options = HarnessOptions.Parse(args);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("Refused: " + exception.Message);
            return 2;
        }

        var runDirectory = Path.Combine(options.OutputRoot, DateTime.Now.ToString("yyyyMMdd-HHmmss"));
        Directory.CreateDirectory(runDirectory);

        using (_log = new StreamWriter(Path.Combine(runDirectory, "messages.log"), false) { AutoFlush = true })
        {
            if (options.Trace)
            {
                SocketTransport.TraceFilePath = Path.Combine(runDirectory, "wire.log");
            }

            Say($"run directory : {Path.GetFullPath(runDirectory)}");
            Say($"target        : {options.Host}:{options.LoginPort}  account={options.Account}  phase={options.Phase}");
            _sliceMs = options.SliceMs;

            try
            {
                return options.Phase == "login" ? RunLogin(options) : RunGame(options, runDirectory);
            }
            catch (Exception exception)
            {
                Report(exception);
                return 1;
            }
        }
    }

    private static int RunLogin(HarnessOptions options)
    {
        var result = Authenticate(options);

        Say(string.Empty);
        Say($"motd          : {result.Motd.Replace("\n", " | ")}");
        Say($"premium days  : {result.PremiumDays}");
        Say($"characters    : {result.Characters.Count}");

        foreach (var character in result.Characters)
        {
            Say($"  {character.Name,-20} {character.World,-14} {character.HostName}:{character.Port}");
        }

        Say(string.Empty);
        Say("PHASE 1 PASSED: RSA, XTEA, Adler32, framing and the login protocol round-tripped against a real server.");
        return 0;
    }

    private static LoginResult Authenticate(HarnessOptions options)
    {
        var clock = Stopwatch.StartNew();

        using (var transport = new SocketTransport())
        {
            var client = new TibiaLoginClient(transport);
            var result = client.Authenticate(
                new LoginOptions(options.Host, options.LoginPort, options.Account, options.Password),
                TimeSpan.FromMilliseconds(options.TimeoutMs));

            Say($"login ok in {clock.ElapsedMilliseconds} ms");
            return result;
        }
    }

    private static int RunGame(HarnessOptions options, string runDirectory)
    {
        var login = Authenticate(options);

        var character = login.Characters
            .FirstOrDefault(entry => string.Equals(entry.Name, options.Character, StringComparison.OrdinalIgnoreCase));

        if (character == null)
        {
            Say($"character '{options.Character}' not found. Available: "
                + string.Join(", ", login.Characters.Select(entry => entry.Name)));
            return 1;
        }

        TargetAllowList.Ensure(character.HostName, character.Port, options.Override);

        var itemTypes = BuildItemTypes(options);
        var floors = new MapFloorTracker();
        var registry = GameServerRegistryFactory.CreateDefault(itemTypes, floors);
        var observed = new List<IProtocolMessage>();

        Say(string.Empty);
        Say($"entering game as {character.Name} at {character.HostName}:{character.Port}");

        using (var transport = new SocketTransport())
        using (var client = new TibiaGameClient(transport, registry, XteaKeyGenerator.Generate, floors))
        {
            var gameOptions = new GameOptions(
                character.HostName,
                character.Port,
                options.Account,
                character.Name,
                options.Password);

            var clock = Stopwatch.StartNew();
            client.EnterGame(gameOptions, TimeSpan.FromMilliseconds(options.TimeoutMs));
            Say($"ENTERED GAME in {clock.ElapsedMilliseconds} ms  state={client.State}  floor z={floors.CurrentZ}");

            Drain(client, observed, TimeSpan.FromSeconds(3), floors);
            PerformScriptedActions(options, client, observed, floors);
            Observe(options, client, observed, floors);

            // A fault recorded before we ask to leave means the session was interrupted.
            var faultBeforeExit = client.FaultReason;

            Say(string.Empty);
            Say("logging out");
            client.ExitGame();

            WriteSummary(runDirectory, observed, client, faultBeforeExit);
            return faultBeforeExit == null ? 0 : 1;
        }
    }

    private static void PerformScriptedActions(
        HarnessOptions options,
        TibiaGameClient client,
        ICollection<IProtocolMessage> observed,
        MapFloorTracker floors)
    {
        if (!string.IsNullOrEmpty(options.Say))
        {
            // Several messages can be chained with |, which is how GM talkactions are driven.
            foreach (var line in options.Say!.Split('|'))
            {
                var text = line.Trim();
                if (text.Length == 0)
                {
                    continue;
                }

                Say($"ACTION say: {text}");
                client.Send(new ClientTalkMessage(SpeakType.Say, null, null, text));
                Drain(client, observed, TimeSpan.FromSeconds(2), floors);
            }
        }

        if (!string.IsNullOrEmpty(options.Steps))
        {
            foreach (var token in options.Steps!.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!TryParseDirection(token.Trim(), out var direction))
                {
                    Say($"skipping step '{token}', expected one of n, e, s, w, ne, se, sw, nw");
                    continue;
                }

                var before = floors.CurrentZ;
                Say($"ACTION step: {direction}  (floor z={before})");
                client.Send(new ClientWalkMessage(direction));
                Drain(client, observed, TimeSpan.FromMilliseconds(1200), floors);
                Say($"    floor z={floors.CurrentZ}{(floors.CurrentZ == before ? string.Empty : "   FLOOR CHANGED")}");
            }
        }

        if (options.Turn)
        {
            foreach (var direction in new[] { Direction.North, Direction.East, Direction.South, Direction.West })
            {
                Say($"ACTION turn: {direction}");
                client.Send(new ClientTurnMessage(direction));
                Drain(client, observed, TimeSpan.FromMilliseconds(700), floors);
            }
        }

        if (options.Walk)
        {
            foreach (var direction in new[] { Direction.North, Direction.East, Direction.South, Direction.West })
            {
                Say($"ACTION walk: {direction}");
                client.Send(new ClientWalkMessage(direction));
                Drain(client, observed, TimeSpan.FromMilliseconds(900), floors);
                Say($"    floor z={floors.CurrentZ}");
            }
        }

        if (options.Diagonal)
        {
            foreach (var direction in new[]
                     {
                         Direction.NorthEast, Direction.SouthEast, Direction.SouthWest, Direction.NorthWest
                     })
            {
                Say($"ACTION diagonal: {direction}");
                client.Send(new ClientWalkMessage(direction));
                Drain(client, observed, TimeSpan.FromMilliseconds(1100), floors);
            }
        }

        if (options.AutoWalk)
        {
            var path = new[] { Direction.North, Direction.North, Direction.East, Direction.South, Direction.South, Direction.West };
            Say($"ACTION autowalk: {string.Join(",", path)}");
            client.Send(new ClientAutoWalkMessage(path));
            Drain(client, observed, TimeSpan.FromSeconds(5), floors);
            Say($"    floor z={floors.CurrentZ}");
        }

        if (options.UseSlot > 0)
        {
            // An inventory item lives at the pseudo position 0xFFFF with the slot in x.
            Say($"ACTION use inventory slot {options.UseSlot}");
            client.Send(new ClientUseItemMessage(
                new Position(0xFFFF, (ushort)options.UseSlot, 0), 0, 0, 0));
            Drain(client, observed, TimeSpan.FromSeconds(3), floors);
        }

        if (options.AttackFirst)
        {
            var target = FindCreatureId(observed);
            if (target == 0)
            {
                Say("ACTION attack: skipped, no creature id seen yet");
            }
            else
            {
                Say($"ACTION attack: creature {target}");
                client.Send(new ClientAttackMessage(target, 1));
                Drain(client, observed, TimeSpan.FromSeconds(4), floors);
            }
        }

        if (!string.IsNullOrEmpty(options.Look))
        {
            var parts = options.Look!.Split(',');
            if (parts.Length == 3
                && ushort.TryParse(parts[0], out var x)
                && ushort.TryParse(parts[1], out var y)
                && byte.TryParse(parts[2], out var z))
            {
                Say($"ACTION look: {x},{y},{z}");
                client.Send(new ClientLookMessage(new Position(x, y, z), 0, 0));
                Drain(client, observed, TimeSpan.FromSeconds(2), floors);
            }
            else
            {
                Say($"skipping --look, expected x,y,z but got '{options.Look}'");
            }
        }
    }

    /// <summary>
    /// Picks a creature id out of anything already observed, ignoring the player's own id.
    /// </summary>
    private static bool TryParseDirection(string token, out Direction direction)
    {
        switch (token.ToLowerInvariant())
        {
            case "n": direction = Direction.North; return true;
            case "e": direction = Direction.East; return true;
            case "s": direction = Direction.South; return true;
            case "w": direction = Direction.West; return true;
            case "ne": direction = Direction.NorthEast; return true;
            case "se": direction = Direction.SouthEast; return true;
            case "sw": direction = Direction.SouthWest; return true;
            case "nw": direction = Direction.NorthWest; return true;
            default: direction = Direction.North; return false;
        }
    }

    private static uint FindCreatureId(IEnumerable<IProtocolMessage> observed)
    {
        uint own = 0;
        var candidates = new List<uint>();

        foreach (var message in observed)
        {
            foreach (var property in message.GetType().GetProperties())
            {
                if (property.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                if (property.Name == "PlayerId" && property.GetValue(message, null) is uint playerId)
                {
                    own = playerId;
                }

                if (property.Name == "CreatureId" && property.GetValue(message, null) is uint creatureId)
                {
                    candidates.Add(creatureId);
                }
            }
        }

        foreach (var candidate in candidates)
        {
            if (candidate != own)
            {
                return candidate;
            }
        }

        return 0;
    }

    private static void Observe(
        HarnessOptions options,
        TibiaGameClient client,
        ICollection<IProtocolMessage> observed,
        MapFloorTracker floors)
    {
        Say(string.Empty);
        Say($"observing for {options.ObserveSeconds}s");
        Drain(client, observed, TimeSpan.FromSeconds(options.ObserveSeconds), floors);
        Say($"observed {observed.Count} message(s) in total, floor z={floors.CurrentZ}");
    }

    private static void Drain(
        TibiaGameClient client,
        ICollection<IProtocolMessage> observed,
        TimeSpan duration,
        MapFloorTracker floors)
    {
        var clock = Stopwatch.StartNew();

        while (clock.Elapsed < duration)
        {
            _drainCalls++;
            var batch = client.Queue!.DequeueBatch(64, TimeSpan.FromMilliseconds(_sliceMs));

            foreach (var message in batch)
            {
                observed.Add(message);
                Say("  " + MessageRenderer.Describe(message));
            }

            // Any recorded fault ends observation immediately; spinning out the clock hides it.
            if (client.FaultReason != null)
            {
                Say($"CONNECTION LOST: {client.FaultReason}");
                return;
            }
        }
    }

    private static readonly ushort[] WellKnownClientIds = { 2148, 2152, 2160 };

    /// <summary>
    /// Offline mode: finds map tiles whose items move a walker between floors, no network involved.
    /// </summary>
    private static int RunFindFloorChange(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Refused: --find-floorchange requires <map.otbm> <items.xml> [x,y,z]");
            return 2;
        }

        IReadOnlyDictionary<ushort, string> wanted;
        IReadOnlyList<FloorChangeTile> tiles;
        try
        {
            wanted = OtbmMapScanner.LoadFloorChangeIds(args[2]);
            tiles = OtbmMapScanner.FindTiles(args[1], wanted);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Refused: {exception.Message}");
            return 1;
        }

        Console.WriteLine($"map                : {args[1]}");
        Console.WriteLine($"floorchange ids    : {wanted.Count}");
        Console.WriteLine($"floorchange tiles  : {tiles.Count}");
        Console.WriteLine();
        Console.WriteLine("by change type:");
        foreach (var group in tiles.GroupBy(tile => tile.Change).OrderByDescending(group => group.Count()))
        {
            Console.WriteLine($"  {group.Key,-14} {group.Count(),6}");
        }

        var ordered = tiles.AsEnumerable();
        if (args.Length > 3 && TryParsePosition(args[3], out var near))
        {
            Console.WriteLine();
            Console.WriteLine($"nearest to {near.X},{near.Y},{near.Z}:");
            ordered = tiles
                .Where(tile => tile.Z == near.Z)
                .OrderBy(tile => Math.Max(Math.Abs(tile.X - near.X), Math.Abs(tile.Y - near.Y)));
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("first candidates:");
        }

        foreach (var tile in ordered.Take(25))
        {
            Console.WriteLine($"  {tile}");
        }

        return 0;
    }

    private static bool TryParsePosition(string text, out Position position)
    {
        position = default;
        var parts = text.Split(',');
        if (parts.Length != 3
            || !ushort.TryParse(parts[0], out var x)
            || !ushort.TryParse(parts[1], out var y)
            || !byte.TryParse(parts[2], out var z))
        {
            return false;
        }

        position = new Position(x, y, z);
        return true;
    }

    /// <summary>
    /// Offline mode: parses an items.otb and prints classification counts, no network involved.
    /// </summary>
    private static int RunDumpItems(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Refused: --dump-items requires a path to items.otb.");
            return 2;
        }

        var path = args[1];
        IReadOnlyDictionary<ushort, OtbItemRecord> items;
        try
        {
            items = OtbItemDatabase.Load(path);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Refused: failed to parse '{path}': {exception.Message}");
            return 1;
        }

        var stackable = items.Values.Count(item => item.IsStackable);
        var fluid = items.Values.Count(item => item.IsFluidContainer);
        var splash = items.Values.Count(item => item.IsSplash);

        Console.WriteLine($"items.otb          : {path}");
        Console.WriteLine($"total items parsed : {items.Count}");
        Console.WriteLine($"stackable          : {stackable}");
        Console.WriteLine($"fluid container    : {fluid}");
        Console.WriteLine($"splash             : {splash}");
        Console.WriteLine();
        Console.WriteLine("well known 8.60 client ids:");

        foreach (var clientId in WellKnownClientIds)
        {
            if (items.TryGetValue(clientId, out var item))
            {
                Console.WriteLine(
                    $"  client id {clientId,-6} server id {item.ServerId,-6} group={item.Group,-10} "
                    + $"stackable={item.IsStackable} fluid={item.IsFluidContainer} splash={item.IsSplash}");
            }
            else
            {
                Console.WriteLine($"  client id {clientId,-6} not found");
            }
        }

        Console.WriteLine();
        Console.WriteLine("sample fluid container items (group=Fluid):");
        foreach (var item in items.Values.Where(item => item.IsFluidContainer).Take(5))
        {
            Console.WriteLine($"  client id {item.ClientId,-6} server id {item.ServerId}");
        }

        Console.WriteLine("sample splash items (group=Splash):");
        foreach (var item in items.Values.Where(item => item.IsSplash).Take(5))
        {
            Console.WriteLine($"  client id {item.ClientId,-6} server id {item.ServerId}");
        }

        return 0;
    }

    private static IItemTypeProvider BuildItemTypes(HarnessOptions options)
    {
        var provider = new ConfiguredItemTypeProvider();

        if (options.ItemsOtb != null)
        {
            provider.LoadItemsOtb(options.ItemsOtb);
            Say($"items.otb: stackable={provider.StackableCount} fluid={provider.FluidCount} splash={provider.SplashCount}");
        }

        if (options.ItemsXml != null)
        {
            provider.LoadItemsXml(options.ItemsXml);
            Say($"items.xml: stackable={provider.StackableCount} fluid={provider.FluidCount} splash={provider.SplashCount}");
        }

        if (options.StackableIds != null)
        {
            provider.LoadIdList(options.StackableIds, "stackable");
            Say($"stackable id list loaded: total stackable={provider.StackableCount}");
        }

        if (provider.IsEmpty)
        {
            Say("WARNING: no item classification supplied. A stackable, fluid or splash item on a visible tile");
            Say("         will be mis-read and the map parse will desynchronize. Use --items-xml or --stackable-ids.");
        }

        return provider;
    }

    private static void WriteSummary(
        string runDirectory,
        IReadOnlyList<IProtocolMessage> observed,
        TibiaGameClient client,
        string? faultBeforeExit)
    {
        var statistics = client.Queue!.GetStatistics();

        var lines = new List<string>
        {
            "messages observed : " + observed.Count,
            "queue depth       : " + statistics.Depth + "/" + statistics.Capacity,
            "enqueued          : " + statistics.Enqueued,
            "dequeued          : " + statistics.Dequeued,
            "dropped           : " + statistics.Dropped,
            "filtered          : " + statistics.Filtered,
            "max depth seen    : " + statistics.MaxDepthSeen,
            "drain calls       : " + _drainCalls + "  at slice " + _sliceMs + " ms",
            "interrupted       : " + (faultBeforeExit ?? "no"),
            "logout write      : " + (client.ExitFailure ?? "ok"),
            "verdict           : " + (faultBeforeExit == null ? "PASS" : "FAIL (session interrupted)"),
            string.Empty,
            "opcode histogram:"
        };

        lines.AddRange(MessageRenderer.Histogram(observed));

        File.WriteAllLines(Path.Combine(runDirectory, "summary.txt"), lines);

        Say(string.Empty);
        foreach (var line in lines)
        {
            Say(line);
        }
    }

    private static void Report(Exception exception)
    {
        Say(string.Empty);
        Say("=== FAILURE ===");

        var decode = exception as PayloadDecodeException ?? exception.InnerException as PayloadDecodeException;

        if (decode != null)
        {
            Say("A frame passed its checksum but a message body could not be parsed.");
            Say($"  reason  : {decode.Message}");
            Say($"  first byte (opcode) : 0x{decode.Payload[0]:X2} {MessageRenderer.NameOf(decode.Payload[0])}");
            Say($"  payload ({decode.Payload.Length} bytes):");
            Say("    " + decode.PayloadHex);
            Say(string.Empty);
            Say("Likely causes, most probable first:");
            Say("  1. An opcode whose reader has the wrong field widths for this server build.");
            Say("  2. Missing item classification, so a stackable or fluid item consumed the wrong byte count.");
            Say("  3. An opcode this module does not implement at all.");
        }
        else if (exception is ProtocolException)
        {
            Say("Protocol failure before any message could be parsed (framing, checksum or crypto).");
            Say($"  {exception.Message}");
        }
        else
        {
            Say($"{exception.GetType().Name}: {exception.Message}");
        }

        Say(string.Empty);
        Say(exception.ToString());
    }

    private static void Say(string line)
    {
        Console.WriteLine(line);
        _log.WriteLine(line);
    }
}
