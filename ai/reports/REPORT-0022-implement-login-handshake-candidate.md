# REPORT-0022 Implement Login Handshake Candidate

## Task ID

TASK-0022-implement-login-handshake-candidate

## Summary

Implemented minimal candidate-only action handlers for AC0 handshake and AC63/SubAC4 login request packets. The handlers record candidate handling details and return handler status/notes through `ActionRouteResult` and protocol trace events.

No real account authentication, plaintext password parsing, login response, character list, enter-map, movement, gameplay logic, GUI behavior, client resources, reference server source, binaries, databases, secrets, or real target-client traces were added.

## Changed Files

- `src/PlServer.Application/ActionHandlerContext.cs`
- `src/PlServer.Application/ActionHandlerResult.cs`
- `src/PlServer.Application/ActionHandlerStatus.cs`
- `src/PlServer.Application/CandidateActionHandlerCatalog.cs`
- `src/PlServer.Application/HandshakeCandidateHandler.cs`
- `src/PlServer.Application/LoginRequestCandidateHandler.cs`
- `src/PlServer.Application/ActionRouteResult.cs`
- `src/PlServer.Application/ActionRouteStatus.cs`
- `src/PlServer.Application/NoOpActionHandler.cs`
- `src/PlServer.Diagnostics/ProtocolTraceEvent.cs`
- `src/PlServer.Diagnostics/ProtocolTraceFormatter.cs`
- `src/PlServer.Diagnostics/ProtocolTraceLogger.cs`
- `src/PlServer.Network/PacketRoutePipeline.cs`
- `src/PlServer.Network/TcpServerHost.cs`
- `tests/PlServer.Application.Tests/ActionRouterTests.cs`
- `tests/PlServer.Network.Tests/HostSmokeTestFixture.cs`
- `tests/PlServer.Network.Tests/HostSmokeTests.cs`
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/tasks/TASK-0022-implement-login-handshake-candidate.md`
- `ai/reports/REPORT-0022-implement-login-handshake-candidate.md`

## Implemented Classes

- `ActionHandlerContext`
- `ActionHandlerResult`
- `ActionHandlerStatus`
- `CandidateActionHandlerCatalog`
- `HandshakeCandidateHandler`
- `LoginRequestCandidateHandler`

## Candidate Handler Behavior

- `HandshakeCandidateHandler` handles AC0 candidate packets and records candidate-only notes.
- `LoginRequestCandidateHandler` handles AC63/SubAC4 candidate packets and records AC/SubAC plus payload length.
- Login payload remains opaque.
- No password field was added.
- No response packets are generated.
- No account authentication or database lookup is performed.

## ActionRouter Integration

- `ActionRouteResult` can carry `ActionHandlerResult`, `HandlerStatus`, and `HandlerNotes`.
- `ActionRouteStatus.CandidateHandled` identifies candidate handler routing.
- `ActionRouter` still respects `SessionStateGuard`.
- Guard rejection returns before handler resolution or invocation.
- Missing handler behavior is preserved when no handler is registered.

## Trace Integration

- `ProtocolTraceEvent` now includes `HandlerStatus` and `HandlerNotes`.
- JSON trace output includes `handlerStatus` and `handlerNotes`.
- `PacketRoutePipeline` passes handler name/status/notes into trace events.
- Synthetic traffic remains non-`trace:client` and non-`confirmed`.

## Synthetic Host Test Coverage

- Synthetic AC0 host smoke invokes `HandshakeCandidateHandler`.
- Synthetic AC63/SubAC4 host smoke invokes `LoginRequestCandidateHandler`.
- Trace events include handler name/status/notes.
- Session state changes remain visible in trace events.
- Synthetic client receives no login response.

## Security Notes

- No plaintext password parsing, storage, or output was added.
- No token, account password, real trace, database, binary, or client resource files were added.
- Candidate handlers do not authenticate accounts and do not create identity state.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main` (first attempt failed with schannel TLS handshake error, retry succeeded)
- `git checkout -B task/0022-implement-login-handshake-candidate`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- `dotnet --info`
- `git status --short`
- `git diff --stat`
- line-count check for modified C# and Markdown files
- `git add src/PlServer.Application src/PlServer.Network src/PlServer.Diagnostics tests/PlServer.Application.Tests tests/PlServer.Network.Tests src/PlServer.sln ai/context/current-state.md ai/context/latest-status.md ai/tasks/TASK-0022-implement-login-handshake-candidate.md ai/reports/REPORT-0022-implement-login-handshake-candidate.md`
- `git commit -m "TASK-0022 implement login handshake candidate"`
- `git push -u origin task/0022-implement-login-handshake-candidate`

## Test Results

Passed:

- `dotnet build .\src\PlServer.sln`: succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln`: succeeded. Application tests passed 23 tests, Diagnostics tests passed 19 tests, and Network tests passed 66 tests.

## Line Count Check

- `src/PlServer.Application/ActionHandlerContext.cs`: 36 lines
- `src/PlServer.Application/ActionHandlerResult.cs`: 27 lines
- `src/PlServer.Application/ActionHandlerStatus.cs`: 7 lines
- `src/PlServer.Application/CandidateActionHandlerCatalog.cs`: 28 lines
- `src/PlServer.Application/HandshakeCandidateHandler.cs`: 48 lines
- `src/PlServer.Application/LoginRequestCandidateHandler.cs`: 52 lines
- `src/PlServer.Application/ActionRouteResult.cs`: 99 lines
- `src/PlServer.Application/ActionRouteStatus.cs`: 13 lines
- `src/PlServer.Application/NoOpActionHandler.cs`: 54 lines
- `src/PlServer.Diagnostics/ProtocolTraceEvent.cs`: 89 lines
- `src/PlServer.Diagnostics/ProtocolTraceFormatter.cs`: 145 lines
- `src/PlServer.Diagnostics/ProtocolTraceLogger.cs`: 104 lines
- `src/PlServer.Network/PacketRoutePipeline.cs`: 69 lines
- `src/PlServer.Network/TcpServerHost.cs`: 227 lines
- `tests/PlServer.Application.Tests/ActionRouterTests.cs`: 391 lines
- `tests/PlServer.Network.Tests/HostSmokeTestFixture.cs`: 202 lines
- `tests/PlServer.Network.Tests/HostSmokeTests.cs`: 299 lines
- `ai/context/current-state.md`: 81 lines
- `ai/context/latest-status.md`: 38 lines
- `ai/tasks/TASK-0022-implement-login-handshake-candidate.md`: 42 lines
- `ai/reports/REPORT-0022-implement-login-handshake-candidate.md`: 156 lines after final report update

## Risks

- Handler behavior remains candidate-only and is not target-client confirmed.
- AC63/SubAC4 login payload fields remain opaque until sanitized target-client traces are available.
- Future real authentication must be implemented in a separate reviewed task.

## Blockers

- Real target-client packet traces are not yet available.
- Account repository and authentication policy are not yet defined.

## Next Recommended Task

`TASK-0023-implement-sanitized-target-client-trace-capture-guide`

## Branch

`task/0022-implement-login-handshake-candidate`

## Commit Hash

Final task commit hash is printed in the terminal completion output.

## Push Result

Final push result is printed in the terminal completion output.
