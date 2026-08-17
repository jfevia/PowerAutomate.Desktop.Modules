// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenTibia.LiveHarness;

/// <summary>
/// Refuses any target that is not the dedicated test server.
/// </summary>
/// <remarks>
/// Other OpenTibia servers on this machine belong to other work and must never be contacted.
/// </remarks>
public static class TargetAllowList
{
    public const string AllowedHost = "127.0.0.1";

    public static readonly int[] AllowedPorts = { 10101, 10102, 10103, 10104 };

    public static void Ensure(string host, int port, bool overridden)
    {
        if (overridden)
        {
            return;
        }

        if (host != AllowedHost || !AllowedPorts.Contains(port))
        {
            throw new InvalidOperationException(
                $"Refusing to connect to {host}:{port}. This harness is restricted to {AllowedHost} ports "
                + string.Join(", ", AllowedPorts)
                + ". Other servers on this machine belong to other work. Pass --i-know-what-im-doing to override.");
        }
    }
}

/// <summary>
/// Command line configuration for a live acceptance run.
/// </summary>
public sealed class HarnessOptions
{
    public string Host { get; private set; } = TargetAllowList.AllowedHost;

    public int LoginPort { get; private set; } = 10101;

    public string Account { get; private set; } = string.Empty;

    public string Password { get; private set; } = string.Empty;

    public string? Character { get; private set; }

    /// <summary>
    /// login stops after the character list; game enters the world.
    /// </summary>
    public string Phase { get; private set; } = "login";

    public int TimeoutMs { get; private set; } = 15000;

    public int ObserveSeconds { get; private set; } = 20;

    /// <summary>
    /// Slice used when draining the queue; the module clamps anything above 1000 ms.
    /// </summary>
    public int SliceMs { get; private set; } = 300;

    public bool Trace { get; private set; } = true;

    public bool Walk { get; private set; }

    public bool Diagonal { get; private set; }

    public bool AutoWalk { get; private set; }

    public bool Turn { get; private set; }

    public string? Say { get; private set; }

    public string? Look { get; private set; }

    /// <summary>
    /// Inventory slot to use, which opens a container such as the backpack.
    /// </summary>
    public int UseSlot { get; private set; }

    public string? ItemsXml { get; private set; }

    public string? ItemsOtb { get; private set; }

    public string? StackableIds { get; private set; }

    public bool Override { get; private set; }

    public string OutputRoot { get; private set; } = "artifacts/live";

    public static HarnessOptions Parse(string[] args)
    {
        var options = new HarnessOptions();

        for (var index = 0; index < args.Length; index++)
        {
            var key = args[index];
            string Next()
            {
                if (index + 1 >= args.Length)
                {
                    throw new ArgumentException($"Option {key} requires a value.");
                }

                return args[++index];
            }

            switch (key)
            {
                case "--host": options.Host = Next(); break;
                case "--login-port": options.LoginPort = int.Parse(Next(), CultureInfo.InvariantCulture); break;
                case "--account": options.Account = Next(); break;
                case "--password": options.Password = Next(); break;
                case "--character": options.Character = Next(); break;
                case "--phase": options.Phase = Next().ToLowerInvariant(); break;
                case "--timeout-ms": options.TimeoutMs = int.Parse(Next(), CultureInfo.InvariantCulture); break;
                case "--observe-seconds": options.ObserveSeconds = int.Parse(Next(), CultureInfo.InvariantCulture); break;
                case "--slice-ms": options.SliceMs = int.Parse(Next(), CultureInfo.InvariantCulture); break;
                case "--items-xml": options.ItemsXml = Next(); break;
                case "--items-otb": options.ItemsOtb = Next(); break;
                case "--stackable-ids": options.StackableIds = Next(); break;
                case "--out": options.OutputRoot = Next(); break;
                case "--say": options.Say = Next(); break;
                case "--look": options.Look = Next(); break;
                case "--use-slot": options.UseSlot = int.Parse(Next(), CultureInfo.InvariantCulture); break;
                case "--walk": options.Walk = true; break;
                case "--diagonal": options.Diagonal = true; break;
                case "--autowalk": options.AutoWalk = true; break;
                case "--turn": options.Turn = true; break;
                case "--no-trace": options.Trace = false; break;
                case "--i-know-what-im-doing": options.Override = true; break;
                default:
                    throw new ArgumentException($"Unknown option '{key}'.");
            }
        }

        if (options.Account.Length == 0 || options.Password.Length == 0)
        {
            throw new ArgumentException("--account and --password are required.");
        }

        if (options.Phase != "login" && options.Phase != "game")
        {
            throw new ArgumentException("--phase must be login or game.");
        }

        if (options.Phase == "game" && string.IsNullOrEmpty(options.Character))
        {
            throw new ArgumentException("--character is required for --phase game.");
        }

        TargetAllowList.Ensure(options.Host, options.LoginPort, options.Override);
        return options;
    }

    public static IEnumerable<string> Usage()
    {
        yield return "OpenTibia live acceptance harness";
        yield return "  restricted to 127.0.0.1 ports 10101-10104 (the dedicated test server)";
        yield return string.Empty;
        yield return "  --dump-items <otb-path>  offline mode: parse items.otb and print classification counts, no network";
        yield return string.Empty;
        yield return "  --account <name>         account name           REQUIRED";
        yield return "  --password <pw>          account password       REQUIRED";
        yield return "  --character <name>       character to enter as  (required for --phase game)";
        yield return "  --phase login|game       login stops at the character list (default login)";
        yield return "  --host <ip>              default 127.0.0.1";
        yield return "  --login-port <port>      default 10101";
        yield return "  --timeout-ms <ms>        handshake timeout (default 15000)";
        yield return "  --observe-seconds <s>    how long to drain the queue in game (default 20)";
        yield return "  --items-xml <path>       TFS items.xml, for map item decoding";
        yield return "  --items-otb <path>       TFS items.otb, keyed by client id (stackable/fluid/splash)";
        yield return "  --stackable-ids <path>   plain list of stackable item ids";
        yield return "  --say <text>             say this once entry is confirmed; use | to send several in order";
        yield return "  --look <x,y,z>           look at a position";
        yield return "  --walk                   step north, east, south, west";
        yield return "  --diagonal               step north-east, south-east, south-west, north-west";
        yield return "  --autowalk               send a multi step path in one message";
        yield return "  --turn                   turn to each cardinal direction";
        yield return "  --no-trace               do not write the raw wire hex dump";
        yield return "  --out <dir>              output root (default artifacts/live)";
        yield return "  --i-know-what-im-doing   bypass the target allow list";
    }
}
