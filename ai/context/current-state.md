# Current State

Date: 2026-07-07

## Current Phase

Phase 7 / ActionRouter Skeleton

## Repository Paths

- Main repository: `D:\Wonderland\pl_server`
- Target client, read-only: `D:\Wonderland\client`
- Reference server, read-only: `D:\pl\server`

## Current GitHub Visible State

- Remote: `origin https://github.com/kiana233/pl_server.git`
- Task branch: `task/0016-implement-action-router-skeleton`
- Base branch: `main`

## Current Local Scan State

- TASK-0010 created a real .NET solution and project skeleton.
- TASK-0011 implemented the basic Protocol-layer PacketCodec.
- TASK-0012 implemented protocol trace logging infrastructure in `PlServer.Diagnostics`.
- TASK-0013 implemented the replay framework in `PlServer.Replay`.
- TASK-0014 created the SessionStateMachine / SessionPacketClassifier / SessionStateGuard foundation on its task branch.
- TASK-0015 implemented the LegacyProtocol ProtocolContractRegistry.
- TASK-0016 implements the ActionRouter skeleton.
- PacketCodec, ProtocolTraceLogger, Replay, SessionStateMachine, SessionStateGuard, and ProtocolContractRegistry now exist for routing-foundation tests.

## Implemented Content

- `PlServer.Protocol` contains configurable packet codec options, frame encode/decode, packet validation results, XOR helper, packet reader, and packet writer.
- `PlServer.Diagnostics` contains structured protocol trace events, JSON Lines formatting, JSON Lines sink, protocol trace logger, and candidate-only behavior descriptors.
- `PlServer.Replay` can import TASK-0012 JSON Lines trace records into replay steps and run replay steps through `PacketCodec`.
- `PlServer.Session` contains session states, packet classification, a session state machine, and a SessionStateGuard for candidate packet gating.
- `PlServer.LegacyProtocol` contains protocol source labels, evidence statuses, packet directions, protocol keys, contract metadata, field descriptors, session requirement metadata, a contract registry, lookup results, and a seeded catalog.
- `PlServer.Application` contains an ActionRouter skeleton that connects decoded packets, protocol contracts, SessionStateGuard checks, and handler registry lookup.
- ActionRouter only returns `InvalidPacket`, `UnknownPacket`, `RejectedBySessionGuard`, `MissingHandler`, or no-op routed results.
- NoOpActionHandler returns skeleton-only no-op results and does not execute gameplay or login behavior.
- Handler registry rejects duplicate registrations and returns missing-handler results without throwing during route lookup.

## Not Implemented

- TCP Host is not implemented.
- Real AC handler dispatch is not implemented.
- AC0, AC63, AC06, login, character selection, enter-map, movement, inventory, equipment, NPC, quests, warp, and battle are not implemented.
- GUI management functionality is not implemented beyond the existing minimal WPF shell.
- Protocol contracts do not execute business behavior.
- ActionRouter does not run login, map, inventory, NPC, battle, or gameplay logic.
- Replay does not execute real client capture and does not confirm target-client behavior.

## Current Blockers

- Real target-client packet traces are not yet available in this repository, so protocol behavior cannot be marked `confirmed`.
- ActionRouter is a skeleton only; real handler implementation requires explicit future tasks and review.

## Next Suggested Task

TASK-0017-implement-tcp-host-skeleton
