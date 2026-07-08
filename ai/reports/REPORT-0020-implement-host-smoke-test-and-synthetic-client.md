# REPORT-0020 Implement Host Smoke Test And Synthetic Client

## Task ID

TASK-0020-implement-host-smoke-test-and-synthetic-client

## Summary

Implemented test-only Host smoke coverage using a synthetic TCP client. The tests validate the current socket-to-session-update skeleton path without implementing real gameplay behavior, real AC handlers, login responses, GUI behavior, client resources, reference server source, real traces, or confirmed protocol facts.

## Changed Files

- `tests/PlServer.Network.Tests/SyntheticTcpClient.cs`
- `tests/PlServer.Network.Tests/HostSmokeTestFixture.cs`
- `tests/PlServer.Network.Tests/HostSmokeTests.cs`
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/tasks/TASK-0020-implement-host-smoke-test-and-synthetic-client.md`
- `ai/reports/REPORT-0020-implement-host-smoke-test-and-synthetic-client.md`

## Implemented Test Utilities

- `SyntheticTcpClient` supports `ConnectAsync`, `SendAsync`, `SendChunksAsync`, `ReadAvailableAsync`, `DisconnectAsync`, and `DisposeAsync`.
- `HostSmokeTestFixture` starts `TcpServerHost` on loopback port 0 with a test-only in-memory protocol trace sink.
- The fixture observes connection state through `ConnectionRegistry.GetAll()`.
- The fixture uses an empty recording action-handler registry so no real AC handler is registered or invoked.

## Host Smoke Test Coverage

- Synthetic client connects to `TcpServerHost`.
- Synthetic AC0 handshake candidate reaches `HandshakeDone`.
- Synthetic AC63/SubAC4 login request candidate reaches `LoginPending` after handshake.
- Sticky frames in one TCP write advance state sequentially.
- Partial frame chunks do not update state until completed.
- Malformed bytes do not crash the host and later valid frames still process.
- Movement candidate before `InMap` is rejected and keeps `Connected`.
- Protocol trace events include AC/SubAC values.
- Trace events are not marked `trace:client` and are not `confirmed`.
- The host does not send a login response automatically.
- `StopAsync` shuts down the host and clears active synthetic connections.
- The smoke tests use an in-memory sink and do not write temporary trace files.

## Synthetic Client Limitations

- Synthetic traffic is test-generated and is not a real Wonderland target-client trace.
- Synthetic smoke tests do not confirm protocol behavior.
- Synthetic tests only verify the current server skeleton wiring and candidate session transitions.
- No login, enter-map, movement, or gameplay response packets are generated.

## Trace Handling

- Trace events are written to an in-memory test sink.
- No trace files are committed.
- Trace source labels remain non-`trace:client`.
- Trace statuses remain non-`confirmed`.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main`
- `git checkout -B task/0020-implement-host-smoke-test-and-synthetic-client`
- `dotnet --info`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- `git status --short`
- `git diff --stat`
- line-count check for modified C# and Markdown files
- `git add src/PlServer.Network tests/PlServer.Network.Tests src/PlServer.Host src/PlServer.sln ai/context/current-state.md ai/context/latest-status.md ai/tasks/TASK-0020-implement-host-smoke-test-and-synthetic-client.md ai/reports/REPORT-0020-implement-host-smoke-test-and-synthetic-client.md`
- `git commit -m "TASK-0020 implement host smoke test and synthetic client"`
- `git push -u origin task/0020-implement-host-smoke-test-and-synthetic-client`

## Test Results

Passed:

- `dotnet build .\src\PlServer.sln`: succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln`: succeeded with 167 tests passed.

## Line Count Check

- `tests/PlServer.Network.Tests/SyntheticTcpClient.cs`: 64 lines
- `tests/PlServer.Network.Tests/HostSmokeTestFixture.cs`: 175 lines
- `tests/PlServer.Network.Tests/HostSmokeTests.cs`: 196 lines
- `ai/context/current-state.md`: 74 lines
- `ai/context/latest-status.md`: 36 lines
- `ai/tasks/TASK-0020-implement-host-smoke-test-and-synthetic-client.md`: 46 lines
- `ai/reports/REPORT-0020-implement-host-smoke-test-and-synthetic-client.md`: 105 lines before adding this line-count section

## Risks

- Host smoke tests use synthetic packets only and cannot prove compatibility with the real target client.
- Protocol behavior remains candidate-only until sanitized target-client traces are available.
- Trace state-change enrichment is still a future task.

## Blockers

- Real target-client packet traces are not yet available.
- Real AC handler behavior is intentionally blocked until explicit future tasks.

## Next Recommended Task

`TASK-0021-implement-protocol-trace-state-change-enrichment`

## Branch

`task/0020-implement-host-smoke-test-and-synthetic-client`

## Commit Hash

Final task commit hash is printed in the terminal completion output.

## Push Result

Final push result is printed in the terminal completion output.
