# OpenTibia module — live validation

What was proven against a real server, what was not, and how to reproduce it.

## Test environment

A dedicated TFS 0.4 (protocol 8.60) instance, isolated from every other server on the machine:

| Setting | Value |
| --- | --- |
| Path | `C:\Users\jfevi\RiderProjects\tfs-old-svn_padm` |
| Login / game port | **10101 / 10102** |
| Manager+admin / status | 10103 / 10104 |
| `worldId` | 1 |
| Database | `theforgottenserver_padm.s3db` (SQLite; 13 `world_id` tables migrated to 1) |
| Accounts | `11`/`1` → BotTester (player) · `22`/`2` → BotTester2 (God) |

The server needs `C:\msys64\mingw64\bin` on `PATH` for its MinGW DLLs. `config.lua` must stay
**BOM-free** — TFS uses Lua 5.1, which does not skip a byte order mark.

The harness refuses any target other than `127.0.0.1:10101-10104` unless `--i-know-what-im-doing`
is passed. Other OpenTibia servers on this machine belong to other work.

## Reproducing

```powershell
.\.tools\Run-LiveAcceptance.ps1 -Scenario login
.\.tools\Run-LiveAcceptance.ps1 -Scenario full -Account 22 -Password 2 -Character BotTester2
```

Item classification is mandatory for anywhere but an empty room:

```powershell
--items-otb C:\Users\jfevi\RiderProjects\tfs-old-svn_padm\data\items\items.otb
```

## What passed

| Area | Evidence |
| --- | --- |
| Login, both accounts | 88 ms and 33 ms; MOTD and character list decoded; game port reported as 10102 |
| Wrong password | clean `LoginRejectedException: Invalid password.` |
| World entry | 28–70 ms, `state=InGame`, zero faults |
| Repeatability | three consecutive enter/exit cycles, all PASS |
| Full map | `FullMap` at (95,117,7) and (50,50,7), matching the database rows |
| Walking | each cardinal step produced `MoveCreature` plus the matching `MapTopRow` / `MapRightRow` / `MapBottomRow` / `MapLeftRow` |
| Diagonals | each diagonal produced **two** row messages, e.g. NorthEast gave top + right |
| Autowalk | one message drove a six step path, with row messages for every step |
| Chat | our `Talk` echoed back with speaker, type and text; a Dragon's `MonsterYell` decoded |
| Containers | `OpenContainer name="backpack" Capacity=20` with a stackable shown as `x30` |
| Blocked movement | `CancelWalk` decoded with the corrected facing direction |
| Inventory | `SetInventory` / `DeleteInventory` for every slot |
| Soak | 124 s, **1245 messages, zero dropped**, peak queue depth 41 of 4096 |
| Abrupt server death | client records the fault and disconnects cleanly, no unhandled exception |
| Server restart | rebinds cleanly, login works again |

Opcodes observed decoding live include `LoginOrPendingState`, `FullMap`, the four map rows,
`UpdateTile`, `CreateOnMap`, `ChangeOnMap`, `DeleteOnMap`, `MoveCreature`, `PlayerData`,
`PlayerSkills`, `PlayerState`, `CreatureHealth`, `CreatureLight`, `CreatureSquare`, `Talk`,
`TextMessage`, `TextEffect`, `GraphicalEffect`, `MissleEffect`, `Ambient`, `VipAdd`,
`SetInventory`, `DeleteInventory`, `OpenContainer`, `CancelWalk` and `Ping`.

## Bugs that only live testing could find

Each of these passed the entire unit suite and would have failed on first contact with a server.

1. **Inbound inner length prefix.** Server-to-client frames carry an inner `u16` length even when
   not XTEA encrypted; outbound first packets do not. The challenge therefore read as opcode
   `0x06`. Fixture frames were built the same wrong way, so the tests agreed with each other and
   with nothing else.
2. **Creature marker compared as one byte.** Creature entries begin with the word `0x0061`/`0x0062`,
   but only the low byte was peeked, so town item `0x0E61` was misread as a creature and the map
   desynchronized. An empty room contains no such ids, which is why a control experiment passed
   while a populated town failed.
3. **Missing item classification.** Stackable, fluid and splash items carry an extra byte. Without
   `items.otb` those bytes were never consumed. This and (2) were independent: fixing only one
   moved the failure offset without curing it.
4. **Logout written to a dead socket.** After the peer vanished, `ExitGame` still attempted the
   logout write and a raw `IOException` escaped — which a flow calling `DisconnectAction` from an
   `ON ERROR` block would have hit.
5. **Silence treated as disconnect.** A short receive timeout combined with returning `0` on
   `IOException` made normal quiet look like a closed connection.
6. **Coalesced handshake bytes discarded.** Bytes arriving in the same TCP read as the challenge
   were dropped, so entry would always have timed out.

## Not validated

- **`FloorChangeUp` / `FloorChangeDown` cannot be produced by this server at all.** This is now a
  proven statement, not a gap in the testing. `Tile::moveCreature` sets `teleport = true` whenever
  the move is not `Position::areInRange<1,1,0>`, and that template requires `|Δz| ≤ 0`
  (`tile.cpp` 449-451, `position.h` 51). `ProtocolGame::sendMoveCreature` only reaches
  `MoveUpCreature` / `MoveDownCreature` inside its `else` branch, which requires `teleport == false`,
  while those calls require `newPos.z != oldPos.z` (`protocolgame.cpp` 2297-2318). The two conditions
  are mutually exclusive, so `0xBE` / `0xBF` are unreachable in this revision.
  Confirmed live twice by walking onto real map tiles rather than teleporting:

  | walk                      | tile                          | wire sequence                          | z      |
  | ------------------------- | ----------------------------- | -------------------------------------- | ------ |
  | east onto stairs up       | `101,114,7` item 5258 `west`  | `0x6D` `0x66` `0x6C` **`0x64 FullMap`** | 7 → 6  |
  | east onto a hole down     | `94,145,7` item 409 `down`    | `0x6D` `0x6C` **`0x64 FullMap`**        | 7 → 8  |

  Both walks are a normal same-floor step onto the tile, then a second server-side move that is
  flagged as a teleport and therefore re-describes the whole map. `MapFloorTracker` followed both.
  The opcodes remain covered by byte-exact unit tests whose pre-reveal counts come from
  tfs-old-svn r3884 `protocolgame.cpp` (`MoveUpCreature` 2984-3020, `MoveDownCreature` 3027-3062).
  A stock Tibia server does send them, which is why the decoders stay in.
- **Trade, quest log, editable text and rule violation messages.** Implemented and unit tested, but
  no live scenario triggered them.
- **PAD designer and portal upload.** No Power Automate Desktop installation or tenant is available
  here, so the signed cab was produced and verified locally but never uploaded or smoke tested.

## Notes for whoever runs this next

- `items.xml` on this server has **no** stackable flags; they live in binary `items.otb`. The
  harness can read either that or, preferably, a client `Tibia.dat` via `--items-dat`, which is
  what a real client classifies from and needs no server files. Both agree exactly on this
  install: 11,604 items, 480 stackable, 28 fluid, 12 splash. Note the two id spaces differ, so
  gold coin is server id 2148 but client id 3031.
- **Item classification is mandatory.** Without it every stackable, fluid and splash item decodes
  one byte short and the map description desynchronizes, surfacing as a bogus "opcode has no
  reader" error somewhere later in the stream. Reproduced deliberately: entering at the temple
  with no item data fails with `Opcode 0xE8 has no reader`, and the same character with
  `--items-dat` enters in 41 ms with 0 dropped messages. The PAD module therefore requires the
  Load item database action and fails fast with a clear message when it is missing.
- BotTester spawns at the temple surrounded by dragons and loses health continuously; movement
  there is blocked. BotTester2 is the better character for movement tests.
- GM talkactions (`/arenatp`, `/arenaclean`, `/arenaspawn`, `/arenagive`) are available on account
  `22` and can build a deterministic scenario. Chain them with `--say "cmd1|cmd2"`.
- To find a stair or hole to walk onto, use the offline map scan, which needs no network:
  `--find-floorchange <map.otbm> <items.xml> [x,y,z]`. It reads the floorchange ids out of
  `items.xml`, walks the OTBM node tree and lists matching tiles nearest a position. On this map it
  finds 608 such tiles. Then drive the walk with `--say "/arenatp <char>,<x>,<y>,<z>" --steps e`.
- A slice of 500 ms is the measured sweet spot; smaller slices only multiply action invocations.
