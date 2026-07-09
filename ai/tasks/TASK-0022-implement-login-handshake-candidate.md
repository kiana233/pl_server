# TASK-0022 Implement Login Handshake Candidate

## Task ID

TASK-0022-implement-login-handshake-candidate

## Branch

`task/0022-implement-login-handshake-candidate`

## Goal

Implement minimal candidate-only handler skeletons for AC0 handshake and AC63/SubAC4 login request packets on top of the existing ActionRouter skeleton.

## Scope

- Add action handler result/status/context models.
- Add `HandshakeCandidateHandler` for AC0 candidate packets.
- Add `LoginRequestCandidateHandler` for AC63/SubAC4 candidate packets.
- Add a candidate action handler catalog for default registration.
- Attach handler result/status/notes to `ActionRouteResult`.
- Expose handler name/status/notes in protocol trace events.
- Update synthetic host smoke tests to verify candidate handler invocation.

## Out Of Scope

- Real account authentication.
- Password parsing or plaintext password output.
- Real login success or failure response packets.
- Character list generation.
- Character selection, enter-map, movement, inventory, equipment, NPC, quest, or battle behavior.
- GUI behavior.
- Client resources, reference server source, binaries, databases, secrets, or real target-client traces.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- Candidate handlers return pending-target-client-trace notes.
- Candidate handlers do not generate response packets.
- Synthetic trace remains non-`trace:client` and non-`confirmed`.
- The task branch is committed and pushed to GitHub.
