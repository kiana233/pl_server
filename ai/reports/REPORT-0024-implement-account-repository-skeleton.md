# REPORT-0024 Implement Account Repository Skeleton

## Task ID

TASK-0024-implement-account-repository-skeleton

## Summary

Implemented a safe account repository skeleton with candidate-only account lookup/authentication classification. The task adds account model types, repository/verifier interfaces, an in-memory repository for skeleton/test use, and tests that preserve the no-real-authentication and no-login-response boundary.

## Changed Files

- `src/PlServer.Core/AccountId.cs`
- `src/PlServer.Core/AccountName.cs`
- `src/PlServer.Core/AccountStatus.cs`
- `src/PlServer.Core/AccountRecord.cs`
- `src/PlServer.Application/IAccountRepository.cs`
- `src/PlServer.Application/AccountLookupResult.cs`
- `src/PlServer.Application/AccountAuthenticationRequest.cs`
- `src/PlServer.Application/AccountAuthenticationResult.cs`
- `src/PlServer.Application/AccountAuthenticationStatus.cs`
- `src/PlServer.Application/IAccountCredentialVerifier.cs`
- `src/PlServer.Application/AccountLoginCandidateService.cs`
- `src/PlServer.Application/LoginRequestCandidateHandler.cs`
- `src/PlServer.Persistence/PlServer.Persistence.csproj`
- `src/PlServer.Persistence/InMemoryAccountRepository.cs`
- `src/PlServer.Persistence/InMemoryAccountRepositoryOptions.cs`
- `src/PlServer.sln`
- `tests/PlServer.Application.Tests/PlServer.Application.Tests.csproj`
- `tests/PlServer.Application.Tests/AccountLoginCandidateServiceTests.cs`
- `tests/PlServer.Persistence.Tests/PlServer.Persistence.Tests.csproj`
- `tests/PlServer.Persistence.Tests/SmokeTest.cs`
- `tests/PlServer.Persistence.Tests/InMemoryAccountRepositoryTests.cs`
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/tasks/TASK-0024-implement-account-repository-skeleton.md`
- `ai/reports/REPORT-0024-implement-account-repository-skeleton.md`

## Created Models and Services

- `AccountId`
- `AccountName`
- `AccountStatus`
- `AccountRecord`
- `IAccountRepository`
- `AccountLookupResult`
- `AccountAuthenticationRequest`
- `AccountAuthenticationResult`
- `AccountAuthenticationStatus`
- `IAccountCredentialVerifier`
- `AccountLoginCandidateService`
- `InMemoryAccountRepository`
- `InMemoryAccountRepositoryOptions`

## Created Tests

- `AccountLoginCandidateServiceTests`
- `InMemoryAccountRepositoryTests`
- `PlServer.Persistence.Tests` smoke test

## Project References

- `PlServer.Application` already references `PlServer.Core`.
- `PlServer.Persistence` now references `PlServer.Application` and `PlServer.Core`.
- `PlServer.Application.Tests` now explicitly references `PlServer.Core` in addition to existing Application, LegacyProtocol, Protocol, and Session references.
- `PlServer.Persistence.Tests` references `PlServer.Persistence`, `PlServer.Application`, and `PlServer.Core`.
- No Core-to-Application or Core-to-Persistence reference was added.
- No Protocol-to-account or Network-to-Persistence dependency was added.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main`
- `git checkout -B task/0024-implement-account-repository-skeleton`
- `dotnet sln .\src\PlServer.sln add .\tests\PlServer.Persistence.Tests\PlServer.Persistence.Tests.csproj`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- `dotnet --info`
- `git diff --stat`
- line-count check script for TASK-0024 source and test files

## Test Results

- `dotnet build .\src\PlServer.sln`: succeeded, 0 warnings, 0 errors.
- `dotnet test .\src\PlServer.sln`: succeeded, 196 tests passed, 0 failed, 0 skipped.
- `.NET SDK`: 8.0.421.

## Line Count Check

- `src/PlServer.Core/AccountId.cs`: 17 lines
- `src/PlServer.Core/AccountName.cs`: 21 lines
- `src/PlServer.Core/AccountStatus.cs`: 7 lines
- `src/PlServer.Core/AccountRecord.cs`: 32 lines
- `src/PlServer.Application/IAccountRepository.cs`: 22 lines
- `src/PlServer.Application/AccountLookupResult.cs`: 27 lines
- `src/PlServer.Application/AccountAuthenticationRequest.cs`: 16 lines
- `src/PlServer.Application/AccountAuthenticationResult.cs`: 81 lines
- `src/PlServer.Application/AccountAuthenticationStatus.cs`: 9 lines
- `src/PlServer.Application/IAccountCredentialVerifier.cs`: 11 lines
- `src/PlServer.Application/AccountLoginCandidateService.cs`: 50 lines
- `src/PlServer.Application/LoginRequestCandidateHandler.cs`: 53 lines
- `src/PlServer.Persistence/InMemoryAccountRepository.cs`: 99 lines
- `src/PlServer.Persistence/InMemoryAccountRepositoryOptions.cs`: 10 lines
- `tests/PlServer.Application.Tests/AccountLoginCandidateServiceTests.cs`: 206 lines
- `tests/PlServer.Persistence.Tests/InMemoryAccountRepositoryTests.cs`: 111 lines
- `tests/PlServer.Persistence.Tests/SmokeTest.cs`: 10 lines
- `tests/PlServer.Persistence.Tests/PlServer.Persistence.Tests.csproj`: 29 lines

## Safety Confirmation

- No real account authentication was implemented.
- No real database connection was implemented.
- No plaintext password storage, output, or validation was added.
- No token, cookie, or session key handling was added.
- No login response packet or character list response was generated.
- No selected-character, enter-map, movement, inventory, equipment, NPC, quest, battle, or GUI behavior was implemented.
- No client resources, copied reference server source, binaries, database files, real traces, pcap, pcapng, har, account passwords, or tokens were added.
- CandidateAuthenticated remains candidate-only and is not a protocol login success.
- LoginRequestCandidateHandler still keeps payload bytes opaque and does not invoke account repository logic.

## Risks

- Account lookup semantics are skeleton-only and may change once sanitized target-client traces and explicit login field parser tasks exist.
- In-memory lookup is for tests and local skeleton wiring only; it is not production persistence.
- CandidateAuthenticated names a local candidate-service outcome only and must not be used as client-visible login success.

## Blockers

- Real target-client traces are not available.
- Login request field layout remains unknown and must not be guessed.
- Real authentication and response packet generation require explicit future tasks and review approval.

## Next Recommended Task

`TASK-0025-implement-login-request-field-parser-candidates`

## Branch

`task/0024-implement-account-repository-skeleton`

## Commit Hash

Final commit hash is printed in terminal output after commit.

## Push Result

Final push result is printed in terminal output after push.
