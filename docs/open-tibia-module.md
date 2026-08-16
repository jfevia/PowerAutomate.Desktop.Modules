# OpenTibia Module

Automates an OpenTibia protocol 8.60 game client from a Power Automate desktop flow: login/logout,
character selection, entering/exiting the game world, sending outbound client messages, and
receiving inbound server messages.

Implementation lives in three projects: [`OpenTibia.Protocol`](/modules/OpenTibia.Protocol) (wire
format, crypto, ~90 message classes), [`OpenTibia.Client`](/modules/OpenTibia.Client) (socket
client, framing, inbound queue), and
[`Modules.OpenTibia.Actions`](/modules/Modules.OpenTibia.Actions) (the 21 PAD actions). The full
action reference lives in [open-tibia-actions.md](open-tibia-actions.md); the deploy procedure
lives in [open-tibia-packaging.md](open-tibia-packaging.md).

## Legal and usage note

Automating a game client can violate a given server's terms of service. This module is intended
for **private or self-hosted OpenTibia servers** that you are authorized to automate. Check the
rules of any server before connecting a bot to it.

## What it does

* **Authentication** — `Login`, `Logout` against the login server.
* **Characters** — `GetCharacterList`, `SelectCharacter`.
* **Game** — `EnterGame`, `ExitGame`, `Disconnect`, `IsInGame`.
* **ClientMessages** (outbound) — `Move`, `Turn`, `AutoWalk`, `Look`, `UseItem`, `Attack`,
  `SendTalk`, `SendClientMessage` (raw opcode escape hatch).
* **ServerMessages** (inbound) — `ReceiveServerMessages`, `HasServerMessage`,
  `ClearServerMessages`, `SubscribeOpcodes`, `GetStreamStatistics`.

See [open-tibia-actions.md](open-tibia-actions.md) for every action's inputs and outputs.

## Session lifecycle

Two `[Type]` values carry a live connection between actions as flow variables:

* `TibiaLoginSession` — returned by `Login`; wraps the login socket and the account/character-list
  result. Consumed by `GetCharacterList`, `SelectCharacter`, `EnterGame`, `Logout`.
* `TibiaGameSession` — returned by `EnterGame`; wraps the game socket, the inbound queue, and the
  opcode filter. Consumed by every `Game`/`ClientMessages`/`ServerMessages` action.

Both are held by **live reference**, not by value: neither class implements `ICloneable` (enforced
by a unit test), so a flow must never treat a session as data that can be safely copied or
persisted.

`TibiaGameClient.State` is a `ConnectionState`:

| Value | Meaning |
| --- | --- |
| `Disconnected` | Initial state, and the state after `Disconnect`/`ExitGame`. |
| `Connecting` | TCP connect in progress. |
| `Authenticating` | Waiting for the game server's challenge. |
| `CharacterList` | Reserved; never set by this client. |
| `EnteringGame` | Challenge answered, waiting for the login/pending-state confirmation. |
| `InGame` | Confirmed; `ClientMessages` actions are now allowed. |
| `Disconnecting` | `ExitGame` sent a logout message and is closing the socket. |
| `Faulted` | Reserved; never set directly — see below. |

The reader thread never sets `State` to `Faulted`. Instead, `TibiaGameClient.FaultReason` (e.g.
"The server closed the connection.") is tracked separately; the flow-visible
`TibiaGameSession.State` reports `"Faulted"` whenever `FaultReason` is non-null, overriding
whatever the last real `ConnectionState` was. `IsInGame` and every in-game action guard check
`FaultReason` as well as `State`.

The first `ReceiveServerMessages` call after `EnterGame` also returns the login/pending-state
message that confirmed entry: the opcode filter defaults to allow-all, and the inbound pipeline
enqueues every decoded message before the client's own handshake logic observes it, so that
confirmation message is never consumed internally — it is left for the flow to read.

## Canonical reference flow

```
Login → GetCharacterList → SelectCharacter → EnterGame → SubscribeOpcodes →
Loop while IsInGame { ReceiveServerMessages(64, 500) → flow logic → Send* } →
Disconnect
```

## Inbound message queue

Every decoded server message passes through, in order:

1. **Opcode filter** (`SubscribeOpcodes`) — an allow-list applied *before* the queue, so
   unsubscribed opcodes never consume queue capacity; they only increment a `Filtered` counter.
   Everything is allowed until a flow calls `SubscribeOpcodes`.
2. **Bounded ring buffer** (capacity set by `EnterGame`'s `QueueCapacity`, default 4096) — a
   fixed-size FIFO. When full, the **oldest** entry is silently overwritten (**DropOldest**) and a
   `Dropped` counter increments; the queue never blocks the socket reader thread and never grows
   unbounded.

`GetStreamStatistics` exposes `Depth`, `Capacity`, `Enqueued`, `Dequeued`, `Dropped`, `Filtered`,
`MaxDepthSeen` so a flow can detect it is falling behind.

## Hard constraints

These follow from how Power Automate Desktop actually runs a flow, and are the most important part
of this document:

* **No state survives a run.** A fresh `PAD.RobotV2.exe` is spawned per flow run; nothing is
  preserved between runs, so every run must re-authenticate (`Login` → ... → `EnterGame`).
* **Stop is cooperative and only checked between actions.** A long-blocking call inside a single
  action ignores Stop until it returns; the orchestrator then hard-kills the robot after a ~2
  second grace period. This is why `ReceiveServerMessages`'s `SliceTimeoutMs` is clamped to
  **1000 ms max** regardless of the value requested, and why the flow's own timeout should be
  disabled or set very high — a flow killed mid-action never reaches `Disconnect`.
* **The engine never calls `Dispose()`.** `DisconnectAction` is the only way the socket and reader
  thread are released; call it unconditionally, including from an `ON ERROR` block.
* **Sessions are live references, never cloneable.** `TibiaLoginSession` and `TibiaGameSession`
  must never implement `ICloneable` — copying one would duplicate a socket handle, not the
  connection itself.
* **The post-`EnterGame` confirmation message is delivered, not swallowed.** See "Session
  lifecycle" above — expect it as the first row of the first `ReceiveServerMessages` call.

## Known limitations

* Passing a `TibiaLoginSession`/`TibiaGameSession` variable into a **child desktop flow (subflow)
  is unverified** and may not survive Power Automate's parameter marshalling between flows. Design
  flows to reconnect inside any subflow rather than relying on a session crossing a flow boundary.
* `SendClientMessage`'s raw opcode/hex payload is not validated against the protocol; a malformed
  payload can desynchronize the connection (surfaces as a protocol error on the next read).
* There is no reconnect/retry built into any action; a dropped connection must be detected
  (`IsInGame`, `GetStreamStatistics`) and recovered by re-running the login sequence.
