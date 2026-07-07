# Current State

Date: 2026-07-07

## Current Phase

Phase 8 / TCP Host Skeleton

## Repository Paths

- Main repository: `D:\Wonderland\pl_server`
- Target client, read-only: `D:\Wonderland\client`
- Reference server, read-only: `D:\pl\server`

## Current GitHub Visible State

- Remote: `origin https://github.com/kiana233/pl_server.git`
- Task branch: `task/0017-implement-tcp-host-skeleton`
- Base branch: `main`

## Current Local Scan State

- TASK-0010 created a real .NET solution and project skeleton.
- TASK-0011 implemented the basic Protocol-layer PacketCodec.
- TASK-0012 implemented protocol trace logging infrastructure in `PlServer.Diagnostics`.
- TASK-0013 implemented the replay framework in `PlServer.Replay`.
- TASK-0014 created the SessionStateMachine / SessionPacketClassifier / SessionStateGuard foundation.
- TASK-0015 implemented the LegacyProtocol ProtocolContractRegistry.
- TASK-0016 implemented the ActionRouter skeleton.
- TASK-0017 implements the TCP Host skeleton.

## Implemented Content

- `PlServer.Protocol` contains PacketCodec, packet encode/decode, validation errors, XOR helper, packet reader, and packet writer.
- `PlServer.Diagnostics` contains ProtocolTraceLogger and JSON Lines trace sink support.
- `PlServer.Replay` can replay trace-derived packet steps through PacketCodec.
- `PlServer.Session` contains session classification, state machine, and SessionStateGuard.
- `PlServer.LegacyProtocol` contains protocol contract metadata and seeded candidate contracts.
- `PlServer.Application` contains ActionRouter skeleton and no-op or missing-handler route results.
- `PlServer.Network` now contains TcpServerHost, connection registry, connection contexts, receive pipeline, packet route pipeline, send pipeline skeleton, and runtime result types.
- `PlServer.Host` provides a minimal console entry point that prints `梦幻服务端 Host 骨架` and starts listening only when `--listen` is passed.

## TCP Host Scope

- TCP Host can listen on a local address, including loopback port `0` for tests.
- TCP Host accepts local clients and tracks connection lifecycle through a registry.
- Stop closes active connections and clears the registry.
- ReceivePipeline treats the input byte array as one complete frame for this task.
- PacketRoutePipeline connects `PacketCodec -> ProtocolTraceLogger -> ActionRouter`.
- SendPipeline queues outgoing bytes only and does not generate gameplay responses.

## Not Implemented

- Full TCP sticky-packet and half-packet frame splitting is not implemented.
- Real AC handler dispatch is not implemented.
- AC0, AC63, AC06, login, character selection, enter-map, movement, inventory, equipment, NPC, quests, warp, and battle are not implemented.
- GUI management functionality is not implemented beyond the existing minimal WPF shell.
- TCP Host does not connect to real client resources and does not record real client traces.
- Protocol behavior remains pending target-client trace confirmation.

## Current Blockers

- Real target-client packet traces are not yet available in this repository.
- Complete stream frame splitting is required before real TCP client compatibility testing.
- Real handlers require explicit future tasks and review approval.

## Next Suggested Task

TASK-0018-implement-frame-splitter-and-connection-pipeline
