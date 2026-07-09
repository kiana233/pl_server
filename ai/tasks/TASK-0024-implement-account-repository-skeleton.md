# TASK-0024 Implement Account Repository Skeleton

## Task ID

TASK-0024-implement-account-repository-skeleton

## Branch

`task/0024-implement-account-repository-skeleton`

## Goal

Create a safe account repository skeleton and candidate-only account lookup/authentication service without implementing real authentication, real database access, login responses, character list responses, gameplay, GUI behavior, or target-client protocol confirmation.

## Scope

- Add account identity and record model types in `PlServer.Core`.
- Add account repository and credential verifier interfaces in `PlServer.Application`.
- Add `AccountLoginCandidateService` to classify candidate account lookup results.
- Add a test-only in-memory repository in `PlServer.Persistence`.
- Add application and persistence tests for skeleton behavior.
- Update task status and report documentation.

## Non-Goals

- No real account authentication.
- No real database connection.
- No plaintext password storage, output, or validation.
- No token, session key, cookie, or private credential handling.
- No login response packet or character list response generation.
- No character selection, enter-map, movement, inventory, equipment, NPC, quest, battle, or GUI behavior.
- No client resources, copied reference server source, binary files, databases, or real traces.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- Account lookup can return not found, disabled, invalid credentials, and candidate authenticated outcomes.
- CandidateAuthenticated remains explicitly not a protocol login success.
- Candidate account service generates no response packets.
- LoginRequestCandidateHandler keeps payload opaque and does not invoke the account repository.
- InMemoryAccountRepository is in-memory only, supports seeded lookup, case-insensitive name lookup, id lookup, missing lookup, and duplicate rejection.
- Documentation records the candidate-only status and next TASK-0025 direction.
