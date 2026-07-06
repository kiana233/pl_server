# Current State

Date: 2026-07-06

## Current Phase

Phase 6 / Protocol Contract Registry

## Repository Paths

- Main repository: `D:\Wonderland\pl_server`
- Target client, read-only: `D:\Wonderland\client`
- Reference server, read-only: `D:\pl\server`

## Current GitHub Visible State

- Remote: `origin https://github.com/kiana233/pl_server.git`
- Task branch: `task/0015-implement-protocol-contract-registry`
- Base branch: `main`

## Current Local Scan State

- TASK-0010 created a real .NET solution and project skeleton.
- TASK-0011 implemented the basic Protocol-layer PacketCodec.
- TASK-0012 implemented protocol trace logging infrastructure in `PlServer.Diagnostics`.
- TASK-0013 implemented the replay framework in `PlServer.Replay`.
- TASK-0014 created the SessionStateMachine / SessionPacketClassifier / SessionStateGuard foundation on its task branch.
- TASK-0015 implements the LegacyProtocol ProtocolContractRegistry.
- PacketCodec exists and remains the source for decoded packet frame data.
- ProtocolTraceLogger exists and can emit JSON Lines trace events.
- Replay currently verifies frame validity and AC/SubAC expectations.
- Protocol contracts are metadata only and do not execute handlers.

## Implemented Content

- `PlServer.Protocol` contains configurable packet codec options, frame encode/decode, packet validation results, XOR helper, packet reader, and packet writer.
- `PlServer.Diagnostics` contains structured protocol trace events, JSON Lines formatting, JSON Lines sink, protocol trace logger, and candidate-only behavior descriptors.
- `PlServer.Replay` can import TASK-0012 JSON Lines trace records into replay steps and run replay steps through `PacketCodec`.
- `PlServer.LegacyProtocol` contains protocol source labels, evidence statuses, packet directions, protocol keys, contract metadata, field descriptors, session requirement metadata, a contract registry, lookup results, and a seeded catalog.
- Legacy protocol contracts describe AC/SubAC candidates only; they do not implement AC handlers.
- Protocol contracts remain based on `reference:muayad`, `assumption`, or `unknown` evidence and are `pending-target-client-trace` unless explicitly unknown.

## Not Implemented

- TCP Host is not implemented.
- ActionRouter is not implemented.
- AC handler dispatch is not implemented.
- AC0, AC63, AC06, login, character selection, enter-map, movement, inventory, equipment, NPC, quests, warp, and battle are not implemented.
- GUI management functionality is not implemented beyond the existing minimal WPF shell.
- Protocol contracts do not execute business behavior.
- Replay does not execute real client capture and does not confirm target-client behavior.

## Current Blockers

- Real target-client packet traces are not yet available in this repository, so protocol behavior cannot be marked `confirmed`.
- Contract metadata cannot become handler behavior until ActionRouter and AC handler tasks are explicitly created and reviewed.

## Next Suggested Task

TASK-0016-implement-action-router-skeleton
