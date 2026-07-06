# TASK-0012: Implement Protocol Trace Logger

## Goal

Implement protocol trace logging infrastructure in `PlServer.Diagnostics` and connect it to `PlServer.Protocol` packet decode results.

## Branch

`task/0012-implement-protocol-trace-logger`

## Commit Message

`TASK-0012 implement protocol trace logger`

## Scope

- Add structured protocol trace event types.
- Add source label, status, direction, resource check, and state change models.
- Add uppercase spaced hex formatting.
- Add one-line JSON event formatting.
- Add append-only JSON Lines sink.
- Add `ProtocolTraceLogger` that converts `PacketDecodeResult` to `ProtocolTraceEvent`.
- Add candidate-only packet behavior descriptors and registry entries.
- Add Diagnostics tests for formatting, JSON, sink behavior, validation error preservation, behavior lookup, and sensitive field checks.
- Update current repository state and latest status documentation.

## Source Labels

- Packet behavior registry entries: `reference:muayad`
- Behavior registry status: `pending-target-client-trace`
- No behavior in this task is marked `trace:client confirmed`.

## Prohibited

- Do not implement TCP Host.
- Do not implement GUI behavior.
- Do not implement ReplayRunner.
- Do not implement SessionStateMachine.
- Do not implement AC0, AC63, AC06, or any AC handler.
- Do not implement login, character selection, enter-map, movement, inventory, equipment, NPC, quests, battle, or gameplay domain models.
- Do not modify `D:\Wonderland\client`.
- Do not modify `D:\pl\server`.
- Do not copy client resources or reference server source.
- Do not add real traces, secrets, binaries, databases, or build outputs.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- Diagnostics tests cover hex formatting, JSON line formatting, source/status labels, JSONL sink behavior, logger validation error preservation, behavior registry lookup, unknown behavior, and sensitive field absence.
- Report exists under `ai/reports`.
- Task branch is committed and pushed unless network/authentication failure prevents push.

## Next Recommended Task

TASK-0013-implement-replay-framework
