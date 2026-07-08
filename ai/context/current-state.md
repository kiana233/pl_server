# Current State

Date: 2026-07-08

## Current Phase

Phase 11 / Host Smoke Test and Synthetic Client

## Repository Paths

- Main repository: `D:\Wonderland\pl_server`
- Target client, read-only: `D:\Wonderland\client`
- Reference server, read-only: `D:\pl\server`

## Current GitHub Visible State

- Remote: `origin https://github.com/kiana233/pl_server.git`
- Task branch: `task/0020-implement-host-smoke-test-and-synthetic-client`
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
- TASK-0018 implemented frame splitting and connection receive pipeline hardening.
- TASK-0019 implemented connection-level SessionState update pipeline.
- TASK-0020 implements Host-level smoke tests and a synthetic TCP client test utility.

## Implemented Content

- `PlServer.Protocol` contains PacketCodec, packet encode/decode, validation errors, XOR helper, packet reader, and packet writer.
- `PlServer.Diagnostics` contains ProtocolTraceLogger and JSON Lines trace sink support.
- `PlServer.Replay` can replay trace-derived packet steps through PacketCodec.
- `PlServer.Session` contains session classification, state machine, and SessionStateGuard.
- `PlServer.LegacyProtocol` contains protocol contract metadata and seeded candidate contracts.
- `PlServer.Application` contains ActionRouter skeleton and no-op or missing-handler route results.
- `PlServer.Network` contains TCP host skeleton, frame splitter, connection receive loop, receive pipeline, packet route pipeline, and connection session updater.
- `tests/PlServer.Network.Tests` contains synthetic TCP client host smoke coverage.

## Host Smoke Test Scope

- Synthetic TCP client tests connect to `TcpServerHost` on loopback port 0.
- Host smoke tests exercise `TcpServerHost -> ConnectionReceiveLoop -> PacketFrameReadBuffer -> ReceivePipeline -> PacketCodec -> ProtocolTraceLogger -> ActionRouter skeleton -> ConnectionSessionUpdater`.
- AC0 handshake candidate synthetic packets advance `Connected` to `HandshakeDone`.
- AC63/SubAC4 login request candidate synthetic packets advance `HandshakeDone` to `LoginPending`.
- Sticky frames and partial frame chunks are covered through the real host socket path.
- Malformed bytes are covered without crashing the host.
- Protocol trace events are written to an in-memory sink for tests.
- Synthetic client traffic is explicitly not target-client trace and does not confirm protocol facts.

## Not Implemented

- Real AC handler dispatch is not implemented.
- AC0, AC63, AC06, login response, character selection, enter-map response, movement broadcast, inventory, equipment, NPC, quests, warp, and battle are not implemented.
- GUI management functionality is not implemented beyond the existing minimal WPF shell.
- TCP receive, session update, and host smoke tests use synthetic traffic only and do not confirm target-client behavior.
- SendPipeline does not generate login, enter-map, movement, or gameplay responses.
- ProtocolTraceLogger state-change enrichment is not yet updated; session update results carry the state transition details.

## Current Blockers

- Real target-client packet traces are not yet available.
- ProtocolTraceLogger state-change enrichment should be handled in a later focused task.
- Real handlers require explicit future tasks and review approval.

## Next Suggested Task

TASK-0021-implement-protocol-trace-state-change-enrichment
