# REPORT-0021 Implement Protocol Trace State Change Enrichment

## Task ID

TASK-0021-implement-protocol-trace-state-change-enrichment

## Summary

Implemented protocol trace state-change enrichment for received packets. Trace events are now written after connection session update so the same packet trace can include candidate session previous/current state, packet kind, state-change flag, rejection reason, transition errors, and notes.

No real AC handlers, login responses, enter-map responses, movement broadcasts, gameplay logic, GUI behavior, client resources, reference server source, binaries, databases, secrets, or real target-client traces were added.

## Changed Files

- `src/PlServer.Diagnostics/ProtocolTraceStateChange.cs`
- `src/PlServer.Diagnostics/ProtocolTraceEvent.cs`
- `src/PlServer.Diagnostics/ProtocolTraceFormatter.cs`
- `src/PlServer.Diagnostics/ProtocolTraceLogger.cs`
- `src/PlServer.Network/PacketRoutePipeline.cs`
- `src/PlServer.Network/ReceivePipeline.cs`
- `src/PlServer.Network/ReceivedPacketContext.cs`
- `tests/PlServer.Diagnostics.Tests/ProtocolTraceTests.cs`
- `tests/PlServer.Network.Tests/HostSmokeTests.cs`
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/tasks/TASK-0021-implement-protocol-trace-state-change-enrichment.md`
- `ai/reports/REPORT-0021-implement-protocol-trace-state-change-enrichment.md`

## Implemented Classes

- Extended `ProtocolTraceStateChange`.
- Added `ProtocolTraceStateChangeError`.
- Extended `ProtocolTraceEvent` with `RouteStatus` and `WithStateChange`.
- Extended `ProtocolTraceLogger` with `CreatePacketDecodeEvent`.

## Trace State Change Fields

Each enriched trace state-change object includes:

- `previousState`
- `currentState`
- `packetKind`
- `wasStateChanged`
- `rejectionReason`
- `errors`
- `notes`

## Pipeline Integration

- `PacketRoutePipeline` now decodes and routes packets, then creates a base trace event without writing it immediately.
- `ReceivePipeline` applies `ConnectionSessionUpdater`, converts `ConnectionSessionUpdateResult` into `ProtocolTraceStateChange`, enriches the base event, and writes the completed trace event.
- Existing PacketCodec, ActionRouter skeleton, and SessionStateMachine behavior were preserved.

## JSON Output Behavior

- JSON Lines output remains one event per line.
- State-change data is serialized under `stateChange`.
- Route status is serialized under `routeStatus`.
- Validation errors are still preserved.
- No password field was added.

## Synthetic Trace Limitations

- Synthetic host smoke trace is not target-client trace.
- Synthetic traffic is not marked `trace:client`.
- Synthetic traffic is not marked `confirmed`.
- State-change records are candidate-only until target-client traces are available.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main`
- `git checkout -B task/0021-implement-protocol-trace-state-change-enrichment`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- `dotnet --info`
- `git status --short`
- `git diff --stat`
- line-count check for modified C# and Markdown files
- `git add src/PlServer.Diagnostics src/PlServer.Network tests/PlServer.Diagnostics.Tests tests/PlServer.Network.Tests src/PlServer.sln ai/context/current-state.md ai/context/latest-status.md ai/tasks/TASK-0021-implement-protocol-trace-state-change-enrichment.md ai/reports/REPORT-0021-implement-protocol-trace-state-change-enrichment.md`
- `git commit -m "TASK-0021 implement protocol trace state change enrichment"`
- `git push -u origin task/0021-implement-protocol-trace-state-change-enrichment`

## Test Results

Passed:

- `dotnet build .\src\PlServer.sln`: succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln`: succeeded. Diagnostics tests passed 19 tests; Network tests passed 65 tests.

## Line Count Check

- `src/PlServer.Diagnostics/ProtocolTraceStateChange.cs`: 14 lines
- `src/PlServer.Diagnostics/ProtocolTraceEvent.cs`: 83 lines
- `src/PlServer.Diagnostics/ProtocolTraceFormatter.cs`: 143 lines
- `src/PlServer.Diagnostics/ProtocolTraceLogger.cs`: 96 lines
- `src/PlServer.Network/PacketRoutePipeline.cs`: 67 lines
- `src/PlServer.Network/ReceivePipeline.cs`: 82 lines
- `src/PlServer.Network/ReceivedPacketContext.cs`: 12 lines
- `tests/PlServer.Diagnostics.Tests/ProtocolTraceTests.cs`: 285 lines
- `tests/PlServer.Network.Tests/HostSmokeTests.cs`: 268 lines
- `ai/context/current-state.md`: 76 lines
- `ai/context/latest-status.md`: 37 lines
- `ai/tasks/TASK-0021-implement-protocol-trace-state-change-enrichment.md`: 40 lines
- `ai/reports/REPORT-0021-implement-protocol-trace-state-change-enrichment.md`: 116 lines before adding this line-count section

## Risks

- State-change enrichment is based on candidate session classification and synthetic tests, not target-client confirmation.
- Trace events now write after session update; future code that expects pre-update trace timing should account for this enriched write point.

## Blockers

- Real target-client packet traces are not yet available.
- Real handlers require explicit future tasks and review approval.

## Next Recommended Task

`TASK-0022-implement-login-handshake-candidate`

## Branch

`task/0021-implement-protocol-trace-state-change-enrichment`

## Commit Hash

Final task commit hash is printed in the terminal completion output.

## Push Result

Final push result is printed in the terminal completion output.
