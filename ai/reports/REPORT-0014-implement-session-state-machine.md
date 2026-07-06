# REPORT-0014 Implement Session State Machine

## Task ID

TASK-0014-implement-session-state-machine

## Summary

Implemented the first minimal session-state foundation in `PlServer.Session`.

The implementation adds candidate packet classification, session transition checks, state guard validation, and snapshot structures only. No TCP Host, ActionRouter, AC handler, login business logic, GUI behavior, gameplay logic, client resources, reference server source, binaries, databases, secrets, tokens, or real traces were added.

## Changed Files

- `src/PlServer.Session/PlServer.Session.csproj`
- `src/PlServer.Session/SessionState.cs`
- `src/PlServer.Session/SessionPacketKind.cs`
- `src/PlServer.Session/SessionPacketClassification.cs`
- `src/PlServer.Session/SessionTransitionResult.cs`
- `src/PlServer.Session/SessionTransitionError.cs`
- `src/PlServer.Session/SessionTransitionErrorCode.cs`
- `src/PlServer.Session/SessionStateGuardResult.cs`
- `src/PlServer.Session/SessionPacketClassifier.cs`
- `src/PlServer.Session/SessionStateMachine.cs`
- `src/PlServer.Session/SessionStateGuard.cs`
- `src/PlServer.Session/SessionContextSnapshot.cs`
- `tests/PlServer.Session.Tests/SessionStateMachineTests.cs`
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/tasks/TASK-0014-implement-session-state-machine.md`
- `ai/reports/REPORT-0014-implement-session-state-machine.md`

## Implemented Classes

- `SessionState`
- `SessionPacketKind`
- `SessionPacketClassification`
- `SessionTransitionResult`
- `SessionTransitionError`
- `SessionTransitionErrorCode`
- `SessionStateGuardResult`
- `SessionPacketClassifier`
- `SessionStateMachine`
- `SessionStateGuard`
- `SessionContextSnapshot`

## Session States

- `Disconnected`
- `Connected`
- `HandshakeDone`
- `LoginPending`
- `LoginAccepted`
- `CharacterListShown`
- `CharacterSelected`
- `EnteringMap`
- `InMap`
- `Rejected`

## Packet Classification Rules

- Invalid `PacketDecodeResult` -> `InvalidPacket`
- AC0 / any SubAC -> `HandshakeCandidate`
- AC63/SubAC4 -> `LoginRequestCandidate`
- AC63/SubAC1 -> `CharacterListCandidate`
- AC63/SubAC2 -> `CharacterSelectCandidate`
- AC12 / any SubAC -> `EnterMapCandidate`
- AC20 / any SubAC -> `EnterMapCandidate`
- AC06/SubAC1 -> `MovementCandidate`
- Other AC/SubAC values -> `Unknown`

## Transition Rules

- Initial state: `Connected`
- `Connected + HandshakeCandidate -> HandshakeDone`
- `HandshakeDone + LoginRequestCandidate -> LoginPending`
- `LoginPending + LoginAcceptedCandidate -> LoginAccepted`
- `LoginAccepted + CharacterListCandidate -> CharacterListShown`
- `CharacterListShown + CharacterSelectCandidate -> CharacterSelected`
- `CharacterSelected + EnterMapCandidate -> EnteringMap`
- `EnteringMap + InMapReadyCandidate -> InMap`
- `InMap + MovementCandidate -> InMap`
- Any state + `DisconnectCandidate -> Disconnected`
- Invalid packets keep the current state and record an error.
- Unknown packets keep the current state and record an unknown packet note without blocking.
- `MovementCandidate` is rejected before `InMap`.

## Source Labels

- AC0, AC63/SubAC4, AC63/SubAC1, AC63/SubAC2, and AC06/SubAC1 are labeled `reference:muayad`.
- AC12, AC20, invalid packets, unknown packets, and explicit synthetic test events are labeled `assumption`.
- All classifications use `pending-target-client-trace`.
- No reference behavior was marked `confirmed`.
- Synthetic tests were not treated as target-client traces.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main` - failed due to GitHub TLS handshake/network timeout; local `main` and `origin/main` both pointed to `69ed9f2205565b57f669593637663c90864bd24a`.
- `git checkout -B task/0014-implement-session-state-machine`
- `dotnet --info`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- `git status --short`
- `git diff --stat`

## Test Results

- `dotnet build .\src\PlServer.sln` succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln` succeeded.
- `PlServer.Session.Tests` passed 22 tests.

## Risks

- State rules are candidate-only and are not confirmed by real target-client traces.
- AC63/SubAC1 is treated as `CharacterListCandidate` until target-client traces can disambiguate it from any login-accepted behavior.
- `InMapReadyCandidate` is currently an explicit synthetic event for tests and does not assert a real protocol AC/SubAC mapping.

## Blockers

- Real target-client traces are not available in this repository.
- Session rules cannot be marked confirmed until target-client trace evidence is added and reviewed.

## Next Recommended Task

`TASK-0015-implement-protocol-contract-registry` or `TASK-0015-implement-action-router-skeleton`

## Branch

`task/0014-implement-session-state-machine`

## Commit Hash

Implementation commit: `1423292`

Final branch tip commit is printed in terminal output after recording this push result.

## Push Result

Initial push succeeded:

```text
To https://github.com/kiana233/pl_server.git
 * [new branch]      task/0014-implement-session-state-machine -> task/0014-implement-session-state-machine
Branch 'task/0014-implement-session-state-machine' set up to track remote branch 'task/0014-implement-session-state-machine' from 'origin'.
```
