# Current State

Date: 2026-07-08

## Current Phase

Phase 13 / Login And Handshake Candidate Handlers

## Repository Paths

- Main repository: `D:\Wonderland\pl_server`
- Target client, read-only: `D:\Wonderland\client`
- Reference server, read-only: `D:\pl\server`

## Current GitHub Visible State

- Remote: `origin https://github.com/kiana233/pl_server.git`
- Task branch: `task/0022-implement-login-handshake-candidate`
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
- TASK-0020 implemented Host-level smoke tests and a synthetic TCP client test utility.
- TASK-0021 implemented ProtocolTrace state-change enrichment.
- TASK-0022 implements login / handshake candidate handlers.

## Implemented Content

- `PlServer.Protocol` contains PacketCodec, packet encode/decode, validation errors, XOR helper, packet reader, and packet writer.
- `PlServer.Diagnostics` contains ProtocolTraceLogger, state-change trace models, and JSON Lines trace sink support.
- `PlServer.Replay` can replay trace-derived packet steps through PacketCodec.
- `PlServer.Session` contains session classification, state machine, and SessionStateGuard.
- `PlServer.LegacyProtocol` contains protocol contract metadata and seeded candidate contracts.
- `PlServer.Application` contains ActionRouter skeleton, candidate handler result models, AC0 HandshakeCandidate handler, and AC63/SubAC4 LoginRequestCandidate handler.
- `PlServer.Network` contains TCP host skeleton, frame splitter, connection receive loop, receive pipeline, packet route pipeline, and connection session updater.
- `tests/PlServer.Network.Tests` contains synthetic TCP client host smoke coverage.

## Login And Handshake Candidate Handler Scope

- Synthetic TCP client tests connect to `TcpServerHost` on loopback port 0.
- Host smoke tests exercise `TcpServerHost -> ConnectionReceiveLoop -> PacketFrameReadBuffer -> ReceivePipeline -> PacketCodec -> ProtocolTraceLogger -> ActionRouter skeleton -> ConnectionSessionUpdater`.
- AC0 handshake candidate synthetic packets advance `Connected` to `HandshakeDone`.
- AC63/SubAC4 login request candidate synthetic packets advance `HandshakeDone` to `LoginPending`.
- AC0 HandshakeCandidateHandler records candidate-only handling and does not generate response packets.
- AC63/SubAC4 LoginRequestCandidateHandler records candidate-only handling, AC/SubAC, and payload length while keeping the payload opaque.
- Candidate handlers return handler status and notes with pending-target-client-trace wording.
- Trace events include handler name, handler status, and handler notes.
- Sticky frames and partial frame chunks are covered through the real host socket path.
- Malformed bytes are covered without crashing the host.
- Protocol trace events are written after connection session update so the same event can include state-change details.
- Each packet trace can carry session previous state, current state, packet kind, state-change flag, rejection reason, transition errors, and notes.
- Movement-before-map and invalid-packet traces record no false state change.
- Synthetic client traffic is explicitly not target-client trace and does not confirm protocol facts.

## Not Implemented

- Real AC handler business dispatch is not implemented beyond candidate-only AC0 and AC63/SubAC4 handler skeletons.
- Real account authentication, login response, character list response, character selection, enter-map response, movement broadcast, inventory, equipment, NPC, quests, warp, and battle are not implemented.
- GUI management functionality is not implemented beyond the existing minimal WPF shell.
- TCP receive, session update, and host smoke tests use synthetic traffic only and do not confirm target-client behavior.
- SendPipeline does not generate login, enter-map, movement, or gameplay responses.
- ProtocolTrace state-change enrichment records candidate state transitions only; it does not confirm target-client behavior.

## Current Blockers

- Real target-client packet traces are not yet available.
- Real handlers require explicit future tasks and review approval.

## Next Suggested Task

TASK-0023-implement-sanitized-target-client-trace-capture-guide
