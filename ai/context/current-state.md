# Current State

Date: 2026-07-06

## Current Phase

Phase 4 / Replay Framework

## Repository Paths

- Main repository: `D:\Wonderland\pl_server`
- Target client, read-only: `D:\Wonderland\client`
- Reference server, read-only: `D:\pl\server`

## Current GitHub Visible State

- Remote: `origin https://github.com/kiana233/pl_server.git`
- Task branch: `task/0013-implement-replay-framework`
- Base branch: `main`

## Current Local Scan State

- TASK-0010 created a real .NET solution and project skeleton.
- TASK-0011 implemented the basic Protocol-layer PacketCodec.
- TASK-0012 implemented protocol trace logging infrastructure in `PlServer.Diagnostics`.
- TASK-0013 implemented the replay framework in `PlServer.Replay`.
- PacketCodec exists and remains the source for decoded packet frame data.
- ProtocolTraceLogger exists and can emit JSON Lines trace events.
- Replay currently verifies only frame validity and AC/SubAC expectations.
- Replay does not validate session state.
- Replay data is synthetic or sanitized JSONL and is not target-client confirmation.

## Implemented Content

- `PlServer.Protocol` contains configurable packet codec options, frame encode/decode, packet validation results, XOR helper, packet reader, and packet writer.
- `PlServer.Diagnostics` contains structured protocol trace events, JSON Lines formatting, JSON Lines sink, protocol trace logger, and candidate-only behavior descriptors.
- `PlServer.Replay` can import TASK-0012 JSON Lines trace records into replay steps.
- `PlServer.Replay` records import errors for invalid JSON, missing direction, missing packet hex, and invalid hex.
- `PlServer.Replay` can parse uppercase spaced hex into bytes.
- `PlServer.Replay` can run replay steps through `PacketCodec`.
- `PlServer.Replay` reports decoded AC/SubAC, expected AC/SubAC matches, packet validation errors, and run summaries.

## Not Implemented

- TCP Host is not implemented.
- SessionStateMachine is not implemented.
- ActionRouter is not implemented.
- AC handler dispatch is not implemented.
- AC0, AC63, AC06, login, character selection, enter-map, movement, inventory, equipment, NPC, quests, warp, and battle are not implemented.
- GUI management functionality is not implemented beyond the existing minimal WPF shell.
- Replay does not execute real client capture and does not confirm target-client behavior.

## Current Blockers

- Real target-client packet traces are not yet available in this repository, so protocol behavior cannot be marked `confirmed`.
- Replay framework is limited to frame and AC/SubAC verification until session state and action routing foundations are implemented.

## Next Suggested Task

TASK-0014-implement-session-state-machine
