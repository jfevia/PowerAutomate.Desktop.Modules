# OpenTibia Module — Action Reference

Every input/output for the 21 actions in `Modules.OpenTibia.Actions`, grouped by their designer
category. Types and default values are read directly from the action source, not inferred. See
[open-tibia-module.md](open-tibia-module.md) for the session lifecycle, the queue model, and the
hard constraints that apply across all of these.

Unless noted, every action throws `UnknownError` on an unexpected failure, and every action taking
a `GameSession` throws `NotConnectedError` when the session is missing, closed, or faulted.

## Authentication

| Action | Inputs | Outputs | Notes |
| --- | --- | --- | --- |
| `Login` | `Host`, `Port` (7171), `AccountName`, `Password`, `TimeoutMs` (15000) | `LoginSession`, `Motd`, `Characters` (DataTable) | Connects to the login server and authenticates. Throws `ConnectionFailedError`, `AuthenticationFailedError`, or `TimeoutError`. |
| `Logout` | `LoginSession` | — | Closes the login socket; a no-op if called twice. |

## Characters

| Action | Inputs | Outputs | Notes |
| --- | --- | --- | --- |
| `GetCharacterList` | `LoginSession` | `Characters` (DataTable: Name, World, Host, Port) | Re-projects the character list already returned by `Login`. |
| `SelectCharacter` | `LoginSession`, `CharacterName` | `Character` (`TibiaCharacter`) | Case-insensitive lookup. Throws `CharacterNotFoundError` listing the available names when no match exists. |

## Game

| Action | Inputs | Outputs | Notes |
| --- | --- | --- | --- |
| `EnterGame` | `LoginSession`, `Character`, `QueueCapacity` (4096), `TimeoutMs` (15000) | `GameSession` | Opens the game socket, performs the challenge/enter handshake, and waits for the login/pending-state confirmation. Throws `AuthenticationFailedError`, `ConnectionFailedError`, or `TimeoutError`. |
| `ExitGame` | `GameSession` | — | Sends a logout client message if still in game, then disconnects. |
| `Disconnect` | `GameSession` | — | Force-closes the socket and reader thread. Safe to call repeatedly and after a fault; mandatory in every flow, including `ON ERROR`. |
| `IsInGame` | `GameSession` (optional) | `IsInGame` (bool) | A **condition action** usable directly as an `If` condition. Non-throwing: a missing or faulted session simply evaluates to `false`. |

## ClientMessages (outbound)

All require an in-game `GameSession`; `NotConnectedError` otherwise.

| Action | Inputs | Outputs | Notes |
| --- | --- | --- | --- |
| `Move` | `GameSession`, `Direction` | — | Single-step walk. |
| `Turn` | `GameSession`, `Direction` | — | Only the 4 cardinal directions; diagonals raise a protocol error. |
| `AutoWalk` | `GameSession`, `Directions` (list) | — | Multi-step queued walk. |
| `Look` | `GameSession`, `X`, `Y`, `Z`, `ItemId`, `StackPosition` (0) | — | Looks at a tile or item. |
| `UseItem` | `GameSession`, `X`, `Y`, `Z`, `ItemId`, `StackPosition` (0), `ContainerIndex` (0) | — | Uses/opens an item. |
| `Attack` | `GameSession`, `CreatureId` | — | Targets a creature by id. |
| `SendTalk` | `GameSession`, `Type` (`SpeakType.Say`), `Text`, `ReceiverName` (optional), `ChannelId` (optional) | — | Chat message; `Type` selects say/whisper/yell/private/channel semantics. |
| `SendClientMessage` | `GameSession`, `Opcode`, `PayloadHex` (optional, hex string) | — | Escape hatch: sends a raw opcode and hex-encoded payload not otherwise modelled. Not protocol-validated — see known limitations. |

## ServerMessages (inbound)

| Action | Inputs | Outputs | Notes |
| --- | --- | --- | --- |
| `ReceiveServerMessages` | `GameSession`, `MaxCount` (64), `SliceTimeoutMs` (500, clamped ≤ 1000 ms) | `Messages` (DataTable: Opcode, Name, ReceivedAt, Payload), `Count`, `TimedOut` | Drains up to `MaxCount` messages from the queue, waiting at most one clamped slice for the first one. |
| `HasServerMessage` | `GameSession` (optional) | `HasMessage` (bool) | A **wait action** meant to be polled by a wait-loop; non-blocking peek at queue depth. |
| `ClearServerMessages` | `GameSession` | `Cleared` (int) | Discards everything currently queued and reports how many entries were dropped. |
| `SubscribeOpcodes` | `GameSession`, `Opcodes` (list of int, optional) | — | Restricts the queue to the given opcodes; an empty or null list resets to allow-all. |
| `GetStreamStatistics` | `GameSession` | `Depth`, `Capacity`, `Enqueued`, `Dequeued`, `Dropped`, `Filtered`, `MaxDepthSeen` | Point-in-time view of the inbound queue; see [open-tibia-module.md](open-tibia-module.md#inbound-message-queue). |

## Supporting `[Type]` values

* `TibiaCharacter` — `Name`, `World`, `Host`, `Port`. Produced by `SelectCharacter`, consumed by
  `EnterGame`.
* `TibiaServerMessage` — `Opcode` (int), `Name` (resolved from the `GameServerOpcode` enum, or
  `Unknown_0xNN`), `ReceivedAt` (UTC), `Payload` (a `CustomObject` with the decoded message's
  properties flattened to flow-friendly values). One row of the `ReceiveServerMessages` DataTable.
