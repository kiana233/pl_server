# TASK-0026 Implement Login Response Contract Candidates

## Task ID

TASK-0026-implement-login-response-contract-candidates

## Branch

`task/0026-implement-login-response-contract-candidates`

## Goal

Create a candidate-only login response contract framework that records S2C response candidate metadata and planning status without generating real login success/failure bytes or character list response bytes.

## Scope

- Add login response candidate metadata in `PlServer.LegacyProtocol`.
- Add a login response candidate registry and no-packet contract plan.
- Add a candidate-only response planner in `PlServer.Application`.
- Integrate response planning notes into `LoginRequestCandidateHandler`.
- Add LegacyProtocol, Application, and Network tests for candidate-only behavior.
- Update current status, latest status, task, and report documentation.

## Non-Goals

- No hardcoded login success response structure.
- No hardcoded login failure response structure.
- No real login success/failure response bytes.
- No character list response bytes.
- No real account authentication.
- No real database access.
- No password, token, cookie, or session key handling.
- No character selection, enter-map, movement, inventory, equipment, NPC, task, battle, or GUI behavior.
- No client resources, copied reference server source, binary files, databases, or real traces.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- Registry seeds LoginSuccessCandidate, LoginFailureCandidate, and CharacterListFollowsCandidate.
- Seeded response candidates are S2C, pending/unknown, and not confirmed.
- Planner does not generate packets or bytes.
- Planner does not call PacketWriter.
- Handler notes include response planning status and no response generated.
- Synthetic host smoke tests still reach LoginPending without receiving login or character list response bytes.
- Synthetic traffic is not `trace:client` and does not confirm response contracts.
