# REPORT-0013: Implement Replay Framework

## Task ID

TASK-0013-implement-replay-framework

## Summary

Implemented offline replay infrastructure in `PlServer.Replay` for importing TASK-0012 JSON Lines trace records, converting them to replay steps, decoding packets through `PacketCodec`, preserving validation errors, and summarizing frame/AC/SubAC replay results.

This task does not implement TCP Host, real client capture, SessionStateMachine, ActionRouter, AC handlers, login, gameplay, GUI behavior, client resources, reference server source, binaries, databases, secrets, or real target-client traces.

## Changed Files

- `src/PlServer.Replay/ReplayDirection.cs`
- `src/PlServer.Replay/ReplayStep.cs`
- `src/PlServer.Replay/ReplayCase.cs`
- `src/PlServer.Replay/ReplayImportError.cs`
- `src/PlServer.Replay/ReplayImportResult.cs`
- `src/PlServer.Replay/ReplayJsonLinesImporter.cs`
- `src/PlServer.Replay/ReplayPacketResult.cs`
- `src/PlServer.Replay/ReplayRunResult.cs`
- `src/PlServer.Replay/ReplayRunner.cs`
- `src/PlServer.Replay/ReplayExpectation.cs`
- `tests/PlServer.Replay.Tests/PlServer.Replay.Tests.csproj`
- `tests/PlServer.Replay.Tests/ReplayFrameworkTests.cs`
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/tasks/TASK-0013-implement-replay-framework.md`
- `ai/reports/REPORT-0013-implement-replay-framework.md`

## Implemented Classes

- `ReplayDirection`
- `ReplayStep`
- `ReplayCase`
- `ReplayImportError`
- `ReplayImportResult`
- `ReplayJsonLinesImporter`
- `ReplayPacketResult`
- `ReplayRunResult`
- `ReplayRunner`
- `ReplayExpectation`

## Replay JSONL Fields Supported

- `timestamp` is tolerated but not used for validation in this task.
- `direction`
- `connectionId` is tolerated but not used for validation in this task.
- `sessionState` is tolerated but not used for validation in this task.
- `rawHex`
- `decodedHex`
- `ac`
- `subAc`
- `protocolName`
- `behavior`
- `sourceLabel`
- `status`
- `validationErrors` is tolerated but PacketCodec validation is recomputed during replay.

## Commands Run

- `Get-Content ai/context/latest-status.md`
- `git status --short --branch`
- `git checkout main`
- `git pull origin main`
- `git checkout -B task/0013-implement-replay-framework`
- `dotnet test .\tests\PlServer.Replay.Tests\PlServer.Replay.Tests.csproj`
- `dotnet --info`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- `git status --short`
- `git diff --stat`
- Prohibited path scan for `bin`, `obj`, `exe`, `dll`, databases, client paths, token, password, and secret.
- Real trace/capture path scan for `pcap`, `pcapng`, `cap`, `har`, trace, and capture directories.
- `rg -n "token|secret|password" ai src tests -g "!**/bin/**" -g "!**/obj/**"`

## Test Results

- `dotnet --info`: succeeded. SDK `8.0.421`; host runtime `8.0.27`.
- Initial Replay test build failed because nullable byte assertions used ambiguous xUnit overloads. Tests were corrected to assert `HasValue` and compare `.Value`.
- `dotnet test .\tests\PlServer.Replay.Tests\PlServer.Replay.Tests.csproj`: succeeded. 15 tests passed.
- `dotnet build .\src\PlServer.sln`: succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln`: succeeded. `PlServer.Replay.Tests` passed 15 tests; `PlServer.Diagnostics.Tests` passed 14 tests; `PlServer.Protocol.Tests` passed 17 tests; full solution test run passed.
- Pre-commit prohibited path scan: passed.
- Real trace/capture path scan: passed.
- Secret keyword scan: no actual secrets found. Matches were documentation prohibitions, an existing password-field absence test, and the local `tokens` variable used by hex parsing.

## Risks

- Replay only validates packet frame decode and AC/SubAC expectations.
- Replay does not validate session state, action routing, or handler behavior.
- Synthetic replay and sanitized JSONL are not target-client confirmation.
- `sourceLabel` and `status` are preserved as strings and are not promoted to confirmed facts.

## Blockers

- Real sanitized target-client traces are not present.
- SessionStateMachine, ActionRouter, SessionStateGuard, TCP Host, and AC handlers remain future work.

## Next Recommended Task

TASK-0014-implement-session-state-machine

## Branch

`task/0013-implement-replay-framework`

## Commit Hash

Final commit hash is printed in terminal output after commit.

## Push Result

Pending until push is attempted. Final push result is printed in terminal output because the report must be committed before the push can occur.
