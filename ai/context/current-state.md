# Current State

Date: 2026-07-06

## Current Phase

Phase 2 / Protocol Frame Codec

## Repository Paths

- Main repository: `D:\Wonderland\pl_server`
- Target client, read-only: `D:\Wonderland\client`
- Reference server, read-only: `D:\pl\server`

## Current GitHub Visible State

- Remote: `origin https://github.com/kiana233/pl_server.git`
- Task branch: `task/0011-implement-packet-codec`
- Base branch: `main`

## Current Local Scan State

- TASK-0010 created a real .NET solution and project skeleton.
- TASK-0011 implemented the basic Protocol-layer PacketCodec.
- PacketCodec rules are based on local reference server observations from `D:\pl\server\NetWork\Packet.cs` and `D:\pl\server\NetWork\Server.cs`.
- PacketCodec source label: `reference:muayad`.
- PacketCodec status: `pending-target-client-trace`, not `confirmed`.

## Implemented Content

- `PlServer.Protocol` contains configurable packet codec options.
- `PlServer.Protocol` can encode and decode the basic old-client-compatible frame shape.
- Header defaults to `F4 44`.
- Length defaults to 2-byte little-endian payload length at offset 2.
- Payload defaults to offset 4.
- AC defaults to payload offset 0.
- SubAC defaults to payload offset 1 when present.
- XOR helper supports whole-frame XOR with key `0xAD`.
- PacketReader and PacketWriter support basic little-endian binary operations.
- Protocol tests cover encode/decode, malformed frames, XOR roundtrip, configurable header, and reader/writer little-endian behavior.

## Not Implemented

- TCP Host is not implemented.
- TCP frame splitter is not implemented.
- Packet logger is not implemented.
- Replay framework behavior is not implemented beyond the project skeleton.
- Session state machine is not implemented.
- AC0, AC63, AC06, login, character selection, enter-map, movement, inventory, equipment, NPC, quests, warp, and battle are not implemented.
- GUI management functionality is not implemented beyond a minimal WPF shell title.

## Current Blockers

- Real target-client packet traces are not yet available in this repository, so protocol behavior cannot be marked `confirmed`.
- XOR scope is based on `reference:muayad` and remains pending target-client trace verification.

## Next Suggested Task

TASK-0012-implement-protocol-trace-logger
