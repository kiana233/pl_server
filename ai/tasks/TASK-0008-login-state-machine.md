# TASK-0008: Login State Machine

## Goal

Design the first explicit session state machine for login and map entry.

## Scope

Define allowed state transitions for AC0, AC63/4, AC63/2, and AC06/1.

## Acceptance Criteria

- AC handlers cannot run in unrelated states.
- Movement is not accepted before `InMap`.
- Gameplay systems are still deferred.
