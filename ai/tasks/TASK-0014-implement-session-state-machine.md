# TASK-0014 Implement Session State Machine

## Goal

Implement the first minimal session-state foundation in `PlServer.Session`.

This task adds only candidate packet classification, session transition checks, state guard validation, and snapshot structures. It does not implement TCP hosting, ActionRouter dispatch, AC handlers, login business behavior, GUI behavior, or gameplay systems.

## Scope

- Add session state and packet-kind enums.
- Add packet classification result structures with source labels and pending trace status.
- Add a candidate-only `SessionPacketClassifier` based on decoded AC/SubAC values.
- Add `SessionStateMachine` for minimal state transitions.
- Add `SessionStateGuard` for validating whether a state can accept a packet kind.
- Add `SessionContextSnapshot` for future diagnostics and GUI display.
- Add unit tests for classification, state transitions, invalid packets, unknown packets, disconnects, and movement gating.
- Update repository status documents and create the task report.

## Non-Goals

- Do not implement TCP Host.
- Do not implement ActionRouter.
- Do not implement any AC handler.
- Do not implement AC0, AC63, AC06 business behavior.
- Do not implement login, character selection, enter-map, movement, inventory, equipment, NPC, quests, battle, or GUI behavior.
- Do not add client resources, reference server source, real traces, databases, binaries, secrets, or tokens.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- Session packet classification keeps source labels and `pending-target-client-trace` status.
- Movement is rejected before `InMap`.
- Unknown packets do not advance state and do not block.
- Invalid packets do not advance state and record an error.
- The task branch is committed and pushed as `task/0014-implement-session-state-machine`.
