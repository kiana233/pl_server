# Current State

Date: 2026-07-08

## Current Phase

Phase 9 / Frame Splitter and Connection Receive Pipeline

## Repository Paths

- Main repository: `D:\Wonderland\pl_server`
- Target client, read-only: `D:\Wonderland\client`
- Reference server, read-only: `D:\pl\server`

## Current GitHub Visible State

- Remote: `origin https://github.com/kiana233/pl_server.git`
- Task branch: `task/0018-implement-frame-splitter-and-connection-pipeline`
- Base branch: `main`

## Current Local Scan State

- TASK-0010 created a real .NET solution and project skeleton.
- TASK-0011 implemented the basic Protocol-layer PacketCodec.
- TASK-0012 implemented protocol trace logging infrastructure in `PlServer.Diagnostics`.
- TASK-0013 implemented the replay framework in `PlServer.Replay`.
- TASK-0014 created the SessionStateMachine / SessionPacketClassifier / SessionStateGuard foundation.
- TASK-0015 implemented the LegacyProtocol ProtocolContractRegistry.
- TASK-0016 implemented the ActionRouter skeleton.
- TASK-0017 implemented the TCP Host skeleton.
- TASK-0018 implements frame splitting and connection receive pipeline hardening.

## Implemented Content

- `PlServer.Protocol` contains PacketCodec, packet encode/decode, validation errors, XOR helper, packet reader, and packet writer.
- `PlServer.Diagnostics` contains ProtocolTraceLogger and JSON Lines trace sink support.
- `PlServer.Replay` can replay trace-derived packet steps through PacketCodec.
- `PlServer.Session` contains session classification, state machine, and SessionStateGuard.
- `PlServer.LegacyProtocol` contains protocol contract metadata and seeded candidate contracts.
- `PlServer.Application` contains ActionRouter skeleton and no-op or missing-handler route results.
- `PlServer.Network` contains TcpServerHost, connection registry, connection contexts, receive pipeline, packet route pipeline, send pipeline skeleton, runtime result types, frame splitter, frame read buffer, and connection receive loop.

## TCP Stream Receive Scope

- PacketFrameReadBuffer preserves incomplete half packets across chunks.
- Sticky packets and multiple frames in a single chunk are split into independent complete frames.
- Leading noise bytes are discarded and the splitter resynchronizes to the next configured `F4 44` header.
- Invalid zero-length frames are reported and still passed to PacketCodec so validation errors are preserved.
- Oversized frames are reported and discarded/resynchronized according to `MaxFrameSize`.
- ConnectionReceiveLoop routes each complete frame independently through `ReceivePipeline -> PacketRoutePipeline -> PacketCodec -> ProtocolTraceLogger -> ActionRouter`.

## Not Implemented

- Real AC handler dispatch is not implemented.
- AC0, AC63, AC06, login, character selection, enter-map, movement, inventory, equipment, NPC, quests, warp, and battle are not implemented.
- GUI management functionality is not implemented beyond the existing minimal WPF shell.
- TCP receive tests use synthetic stream traffic only and do not confirm target-client behavior.
- SendPipeline does not generate login, enter-map, movement, or gameplay responses.

## Current Blockers

- Real target-client packet traces are not yet available in this repository.
- Connection session state is not yet advanced from route results.
- Real handlers require explicit future tasks and review approval.

## Next Suggested Task

TASK-0019-implement-connection-session-update-pipeline
