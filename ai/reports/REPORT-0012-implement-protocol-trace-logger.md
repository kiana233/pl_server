# REPORT-0012: Implement Protocol Trace Logger

## Task ID

TASK-0012-implement-protocol-trace-logger

## Summary

Implemented protocol trace logging infrastructure in `PlServer.Diagnostics` with structured trace events, source/status labels, JSON Lines output, packet decode result logging, and candidate-only packet behavior descriptors.

This task does not implement TCP Host, GUI behavior, ReplayRunner, SessionStateMachine, AC handlers, login, gameplay, client resources, reference server source, binaries, databases, secrets, or real traces.

## Changed Files

- `src/PlServer.Diagnostics/PlServer.Diagnostics.csproj`
- `src/PlServer.Diagnostics/ProtocolTraceDirection.cs`
- `src/PlServer.Diagnostics/ProtocolTraceSourceLabel.cs`
- `src/PlServer.Diagnostics/ProtocolTraceStatus.cs`
- `src/PlServer.Diagnostics/ProtocolTraceResourceCheck.cs`
- `src/PlServer.Diagnostics/ProtocolTraceStateChange.cs`
- `src/PlServer.Diagnostics/ProtocolTraceEvent.cs`
- `src/PlServer.Diagnostics/ProtocolTraceFormatter.cs`
- `src/PlServer.Diagnostics/IProtocolTraceSink.cs`
- `src/PlServer.Diagnostics/JsonLinesProtocolTraceSink.cs`
- `src/PlServer.Diagnostics/ProtocolTraceLogger.cs`
- `src/PlServer.Diagnostics/PacketBehaviorDescriptor.cs`
- `src/PlServer.Diagnostics/PacketBehaviorRegistry.cs`
- `tests/PlServer.Diagnostics.Tests/PlServer.Diagnostics.Tests.csproj`
- `tests/PlServer.Diagnostics.Tests/ProtocolTraceTests.cs`
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/tasks/TASK-0012-implement-protocol-trace-logger.md`
- `ai/reports/REPORT-0012-implement-protocol-trace-logger.md`

## Implemented Classes

- `ProtocolTraceEvent`
- `ProtocolTraceDirection`
- `ProtocolTraceSourceLabel`
- `ProtocolTraceStatus`
- `ProtocolTraceResourceCheck`
- `ProtocolTraceStateChange`
- `ProtocolTraceFormatter`
- `IProtocolTraceSink`
- `JsonLinesProtocolTraceSink`
- `ProtocolTraceLogger`
- `PacketBehaviorDescriptor`
- `PacketBehaviorRegistry`

## Behavior Registry Entries

- AC0 / unknown sub: `HandshakeCandidate`
- AC63/4: `LoginRequestCandidate`
- AC63/2: `CharacterSelectCandidate`
- AC06/1: `MovementCandidate`
- Unknown AC/SubAC: `UnknownPacket`

These are behavior descriptions only, not handlers.

## Source Labels

- Candidate behavior entries use `ReferenceMuayad`.
- JSON output serializes `ReferenceMuayad` as `reference:muayad`.
- Candidate behavior status uses `PendingTargetClientTrace`.
- JSON output serializes `PendingTargetClientTrace` as `pending-target-client-trace`.
- No reference behavior is marked confirmed.

## Commands Run

- `Get-Content ai/context/latest-status.md`
- `git status --short --branch`
- `git checkout -B task/0012-implement-protocol-trace-logger`
- `dotnet test .\tests\PlServer.Diagnostics.Tests\PlServer.Diagnostics.Tests.csproj`
- `dotnet --info`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- `git status --short`
- `git diff --stat`
- Prohibited path scan for `bin`, `obj`, `exe`, `dll`, databases, client paths, token, password, and secret.
- `rg -n "token|secret|password" ai src tests -g "!**/bin/**" -g "!**/obj/**"`

## Test Results

- `dotnet --info`: succeeded. SDK `8.0.421`; host runtime `8.0.27`.
- `dotnet test .\tests\PlServer.Diagnostics.Tests\PlServer.Diagnostics.Tests.csproj`: succeeded. 14 tests passed.
- `dotnet build .\src\PlServer.sln`: succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln`: succeeded. `PlServer.Diagnostics.Tests` passed 14 tests; `PlServer.Protocol.Tests` passed 17 tests; full solution test run passed.
- Pre-commit prohibited path scan: passed. Trace-related source file names are task scope, not real client trace data.
- Secret keyword scan: no actual secrets found. Matches were documentation prohibitions and the test that asserts `ProtocolTraceEvent` has no password field.

## Risks

- Behavior registry entries are based on `reference:muayad` and remain pending target-client trace verification.
- JSON field shape is intentionally minimal and may evolve after replay/session requirements become concrete.
- The logger records packet decode results but does not implement packet capture, TCP hosting, replay execution, or AC dispatch.

## Blockers

- No sanitized target-client traces are present to mark packet behavior as confirmed.
- TCP Host, ReplayRunner, SessionStateMachine, ActionRouter, SessionStateGuard, and AC handlers remain future work.

## Next Recommended Task

TASK-0013-implement-replay-framework

## Branch

`task/0012-implement-protocol-trace-logger`

## Commit Hash

Final commit hash is printed in terminal output after commit.

## Push Result

Pending until push is attempted. Final push result is printed in terminal output because the report must be committed before the push can occur.
