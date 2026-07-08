# REPORT-0019 Implement Connection Session Update Pipeline

## Task ID

TASK-0019-implement-connection-session-update-pipeline

## Summary

Implemented connection-level SessionState updates in `PlServer.Network`.

The receive path now attaches a `ConnectionSessionUpdateResult` to every received packet result and updates `ClientConnectionContext.CurrentSessionState` for allowed candidate transitions. Invalid, unknown, and rejected packets keep the previous state and remain visible for tracing and test assertions.

No real AC handler, login, map, movement, inventory, NPC, battle, GUI behavior, client resource access, reference server source, real trace, database, secret, token, or auto-generated gameplay response was added.

## Changed Files

- `src/PlServer.Network/ConnectionSessionUpdatePolicy.cs`
- `src/PlServer.Network/ConnectionSessionUpdateResult.cs`
- `src/PlServer.Network/ConnectionSessionUpdateStatus.cs`
- `src/PlServer.Network/ConnectionSessionUpdater.cs`
- `src/PlServer.Network/ReceivedPacketResult.cs`
- `src/PlServer.Network/ConnectionReceiveResult.cs`
- `src/PlServer.Network/ReceivePipeline.cs`
- `src/PlServer.Network/ConnectionReceiveLoop.cs`
- `src/PlServer.Session/SessionStateMachine.cs`
- `tests/PlServer.Network.Tests/ConnectionSessionUpdateTests.cs`
- `src/PlServer.Network/*` and `tests/PlServer.Network.Tests/*` from TASK-0018 stream receive foundation
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/tasks/TASK-0019-implement-connection-session-update-pipeline.md`
- `ai/reports/REPORT-0019-implement-connection-session-update-pipeline.md`

## Implemented Classes

- `ConnectionSessionUpdatePolicy`
- `ConnectionSessionUpdateResult`
- `ConnectionSessionUpdateStatus`
- `ConnectionSessionUpdater`
- `ReceivedPacketResult`

## Session Update Flow

1. `PacketRoutePipeline` decodes, logs, and routes the packet to ActionRouter skeleton.
2. `ReceivePipeline` passes the decoded packet and route result to `ConnectionSessionUpdater`.
3. `ConnectionSessionUpdater` classifies the packet and applies `SessionStateMachine` from the connection's current state.
4. Allowed candidate transitions update `ClientConnectionContext.CurrentSessionState`.
5. Invalid, unknown, and rejected packets do not update the connection state.
6. `ReceivedPacketResult` exposes DecodeResult, RouteResult, and SessionUpdateResult.
7. `ConnectionReceiveResult` exposes FinalSessionState after all complete frames in the chunk are processed.

## State Transition Rules Applied

- Connected + HandshakeCandidate -> HandshakeDone.
- HandshakeDone + LoginRequestCandidate -> LoginPending.
- LoginAccepted + CharacterListCandidate -> CharacterListShown.
- CharacterListShown + CharacterSelectCandidate -> CharacterSelected.
- CharacterSelected + EnterMapCandidate -> EnteringMap.
- EnteringMap + explicit InMapReadyCandidate -> InMap.
- InMap + MovementCandidate remains InMap.
- MovementCandidate before InMap is rejected and does not update state.
- InvalidPacket and Unknown packets do not update state.

These transitions are candidate-only and do not confirm target-client behavior.

## Receive Pipeline Integration

`ReceivePipeline` now applies session updates after each routed packet. Sticky frames update the connection state sequentially because each complete frame is processed in order.

Partial frames do not update state until the frame splitter emits a complete frame.

## ProtocolTrace Integration

No Diagnostics API expansion was added in this task. `ConnectionSessionUpdateResult` records previous/current state, packet kind, status, rejection reason, errors, and notes.

ProtocolTraceLogger state-change enrichment is left for a later focused task.

## Dependency Note

The current `main` branch did not contain the TASK-0018 frame splitter and TCP receive foundation when this task branch was created. The already pushed TASK-0018 `PlServer.Network` and `PlServer.Network.Tests` foundation was brought into this task branch so TASK-0019 could build on the documented stream receive pipeline.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main`
- `git checkout -B task/0019-implement-connection-session-update-pipeline`
- `dotnet --info`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- line-count check
- `git status --short`
- `git diff --stat`

## Test Results

- `dotnet build .\src\PlServer.sln` succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln` succeeded.
- `PlServer.Network.Tests` passed 52 tests.

## Line Count Check

- `src/PlServer.Network/ConnectionSessionUpdatePolicy.cs`: 10 lines
- `src/PlServer.Network/ConnectionSessionUpdateResult.cs`: 13 lines
- `src/PlServer.Network/ConnectionSessionUpdateStatus.cs`: 10 lines
- `src/PlServer.Network/ConnectionSessionUpdater.cs`: 104 lines
- `src/PlServer.Network/ReceivedPacketResult.cs`: 11 lines
- `src/PlServer.Network/ConnectionReceiveResult.cs`: 7 lines
- `src/PlServer.Network/ReceivePipeline.cs`: 58 lines
- `src/PlServer.Network/ConnectionReceiveLoop.cs`: 51 lines
- `src/PlServer.Session/SessionStateMachine.cs`: 108 lines
- `tests/PlServer.Network.Tests/ConnectionSessionUpdateTests.cs`: 314 lines
- `ai/context/current-state.md`: 71 lines
- `ai/context/latest-status.md`: 35 lines
- `ai/tasks/TASK-0019-implement-connection-session-update-pipeline.md`: 45 lines
- `ai/reports/REPORT-0019-implement-connection-session-update-pipeline.md`: 126 lines before this section was added

## Risks

- Session transitions are candidate-only and not target-client confirmed.
- ProtocolTraceLogger state-change enrichment is not implemented in this task.
- No real AC handlers exist; ActionRouter still returns skeleton results such as MissingHandler.

## Blockers

- Real target-client packet traces are not yet available.
- Real handler tasks require explicit future tasks and review approval.
- Host-level smoke tests with a synthetic client should verify end-to-end behavior next.

## Next Recommended Task

TASK-0020-implement-host-smoke-test-and-synthetic-client

## Branch

`task/0019-implement-connection-session-update-pipeline`

## Commit Hash

Final commit hash will be printed in terminal output.

## Push Result

Final push result will be printed in terminal output.
