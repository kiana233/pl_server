# REPORT-0016 Implement ActionRouter Skeleton

## Task ID

TASK-0016-implement-action-router-skeleton

## Summary

Implemented an ActionRouter skeleton in `PlServer.Application`.

The router connects:

- `PacketDecodeResult`
- `LegacyProtocolContractRegistry`
- `SessionPacketClassifier`
- `SessionStateGuard`
- `ActionHandlerRegistry`

The router only returns skeleton routing results. It does not implement TCP Host, GUI behavior, real AC handlers, login, movement, map, inventory, NPC, battle, or gameplay logic.

## Changed Files

- `src/PlServer.Application/ActionRouteRequest.cs`
- `src/PlServer.Application/ActionRouteResult.cs`
- `src/PlServer.Application/ActionRouteStatus.cs`
- `src/PlServer.Application/IActionHandler.cs`
- `src/PlServer.Application/ActionHandlerDescriptor.cs`
- `src/PlServer.Application/IActionHandlerRegistry.cs`
- `src/PlServer.Application/ActionHandlerRegistry.cs`
- `src/PlServer.Application/ActionRouter.cs`
- `src/PlServer.Application/NoOpActionHandler.cs`
- `src/PlServer.Application/MissingActionHandler.cs`
- `src/PlServer.Application/PlServer.Application.csproj`
- `tests/PlServer.Application.Tests/ActionRouterTests.cs`
- `tests/PlServer.Application.Tests/PlServer.Application.Tests.csproj`
- `src/PlServer.Session/*`
- `tests/PlServer.Session.Tests/*`
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/tasks/TASK-0016-implement-action-router-skeleton.md`
- `ai/reports/REPORT-0016-implement-action-router-skeleton.md`

## Implemented Classes

- `ActionRouteRequest`
- `ActionRouteResult`
- `ActionRouteStatus`
- `IActionHandler`
- `ActionHandlerDescriptor`
- `IActionHandlerRegistry`
- `ActionHandlerRegistry`
- `ActionRouter`
- `NoOpActionHandler`
- `MissingActionHandler`

## Routing Flow

1. Reject invalid `PacketDecodeResult` with `InvalidPacket`.
2. Resolve AC/SubAC/direction through `LegacyProtocolContractRegistry`.
3. Classify the packet through `SessionPacketClassifier`.
4. Check the current `SessionState` through `SessionStateGuard`.
5. Return `RejectedBySessionGuard` if the candidate is not allowed.
6. Resolve a handler through `ActionHandlerRegistry`.
7. Return `MissingHandler` if no handler is registered.
8. Route to `NoOpActionHandler` when a skeleton handler is registered.

## Handler Registry Behavior

- Handler descriptors are keyed by `LegacyProtocolKey`.
- Duplicate handler registration throws `InvalidOperationException`.
- Missing handler lookup returns `false` and does not throw.
- Registered no-op handlers can route candidate contracts without executing business logic.

## Session Guard Integration

`ActionRouter` uses `SessionPacketClassifier` and `SessionStateGuard`.

Movement candidates are rejected before `InMap` and allowed in `InMap`. The router does not transition session state or execute gameplay behavior.

## Source Labels

Route results preserve contract source labels such as `reference:muayad`.

Unknown fallback route results preserve `unknown` source labels.

## Evidence Statuses

Route results preserve contract evidence statuses.

`reference:muayad` contracts remain `PendingTargetClientTrace` and are not marked `Confirmed`.

## Dependency Note

The current `main` branch contained TASK-0015 metadata but did not contain the TASK-0014 Session source files referenced by `ai/context/current-state.md`. To keep TASK-0016 aligned with the recorded project foundation, the existing local TASK-0014 Session source and tests were brought into this task branch.

No new gameplay behavior was added in those Session files; they are the previously completed SessionStateMachine / SessionPacketClassifier / SessionStateGuard foundation.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main`
- `git checkout -B task/0016-implement-action-router-skeleton`
- `dotnet --info`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- line-count check
- `git status --short`
- `git diff --stat`

## Test Results

- `dotnet build .\src\PlServer.sln` succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln` succeeded.
- `PlServer.Application.Tests` passed 16 tests.
- `PlServer.Session.Tests` passed 22 tests after bringing in the previously completed Session foundation.

## Line Count Check

- `src/PlServer.Application/ActionRouteRequest.cs`: 13 lines
- `src/PlServer.Application/ActionRouteResult.cs`: 60 lines
- `src/PlServer.Application/ActionRouteStatus.cs`: 12 lines
- `src/PlServer.Application/IActionHandler.cs`: 18 lines
- `src/PlServer.Application/ActionHandlerDescriptor.cs`: 23 lines
- `src/PlServer.Application/IActionHandlerRegistry.cs`: 10 lines
- `src/PlServer.Application/ActionHandlerRegistry.cs`: 26 lines
- `src/PlServer.Application/ActionRouter.cs`: 141 lines
- `src/PlServer.Application/NoOpActionHandler.cs`: 50 lines
- `src/PlServer.Application/MissingActionHandler.cs`: 44 lines
- `tests/PlServer.Application.Tests/ActionRouterTests.cs`: 266 lines
- `src/PlServer.Session/SessionPacketClassifier.cs`: 135 lines
- `src/PlServer.Session/SessionStateGuard.cs`: 56 lines
- `src/PlServer.Session/SessionStateMachine.cs`: 98 lines
- `tests/PlServer.Session.Tests/SessionStateMachineTests.cs`: 324 lines
- `ai/context/current-state.md`: 61 lines
- `ai/context/latest-status.md`: 34 lines
- `ai/tasks/TASK-0016-implement-action-router-skeleton.md`: 44 lines
- `ai/reports/REPORT-0016-implement-action-router-skeleton.md`: 143 lines before this section was added

## Risks

- ActionRouter is intentionally skeleton-only and does not execute real handlers.
- Contract metadata remains candidate-only until target-client traces are available.
- Session files were included because `main` lacked the already documented TASK-0014 foundation.

## Blockers

- Real target-client traces are not yet available.
- Real AC handlers require future explicit tasks and review approval.

## Next Recommended Task

TASK-0017-implement-tcp-host-skeleton

## Branch

`task/0016-implement-action-router-skeleton`

## Commit Hash

Final commit hash will be printed in terminal output.

## Push Result

Final push result will be printed in terminal output.
