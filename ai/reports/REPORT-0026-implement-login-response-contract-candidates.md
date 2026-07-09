# REPORT-0026 Implement Login Response Contract Candidates

## Task ID

TASK-0026-implement-login-response-contract-candidates

## Summary

Implemented a candidate-only login response contract framework. The new metadata and planner describe possible S2C login response candidates while preserving the hard safety boundary: no real response bytes are generated, no PacketWriter is used, no character list response is generated, and all evidence remains pending-target-client-trace or unknown.

## Changed Files

- `src/PlServer.LegacyProtocol/LoginResponseKind.cs`
- `src/PlServer.LegacyProtocol/LoginResponseContractStatus.cs`
- `src/PlServer.LegacyProtocol/LoginResponseContractPlanStatus.cs`
- `src/PlServer.LegacyProtocol/LoginResponseFieldDescriptor.cs`
- `src/PlServer.LegacyProtocol/LoginResponseContractCandidate.cs`
- `src/PlServer.LegacyProtocol/LoginResponseContractPlan.cs`
- `src/PlServer.LegacyProtocol/LoginResponseContractCandidateRegistry.cs`
- `src/PlServer.Application/LoginResponseCandidatePlanStatus.cs`
- `src/PlServer.Application/LoginResponseCandidatePlanResult.cs`
- `src/PlServer.Application/LoginResponseCandidatePlanner.cs`
- `src/PlServer.Application/LoginRequestParseResult.cs`
- `src/PlServer.Application/LoginRequestCandidateHandler.cs`
- `tests/PlServer.LegacyProtocol.Tests/LoginResponseContractCandidateRegistryTests.cs`
- `tests/PlServer.Application.Tests/LoginResponseCandidatePlannerTests.cs`
- `tests/PlServer.Network.Tests/HostSmokeTests.cs`
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/tasks/TASK-0026-implement-login-response-contract-candidates.md`
- `ai/reports/REPORT-0026-implement-login-response-contract-candidates.md`

## Implemented Classes

- `LoginResponseKind`
- `LoginResponseContractStatus`
- `LoginResponseContractPlanStatus`
- `LoginResponseFieldDescriptor`
- `LoginResponseContractCandidate`
- `LoginResponseContractPlan`
- `LoginResponseContractCandidateRegistry`
- `LoginResponseCandidatePlanStatus`
- `LoginResponseCandidatePlanResult`
- `LoginResponseCandidatePlanner`

## Seeded Response Candidates

- `LoginSuccessCandidate`
- `LoginFailureCandidate`
- `CharacterListFollowsCandidate`

All seeded response candidates are S2C, not confirmed, and pending target-client trace.

## Planner Behavior

- Opaque or unknown LoginRequest layout returns `CannotPlanUnknownRequestLayout`.
- Missing authentication result returns `CannotPlanWithoutAuthentication`.
- Candidate response selection returns `CandidateOnlyNoPacketGenerated`.
- `ShouldGeneratePacket` is always false.
- `GeneratedBytes` is always empty.
- Planner does not hold or use PacketWriter.
- Planner does not change session state.
- Planner does not generate character list bytes.

## Handler Integration

- `LoginRequestCandidateHandler` now calls `LoginResponseCandidatePlanner`.
- Handler notes include response planning status, no response generated, pending target-client trace, and not confirmed.
- Handler response packets remain empty.
- CandidateAuthenticated is not treated as client login success.

## Trace Notes

- Existing trace flow carries handler notes into `ProtocolTraceEvent.HandlerNotes`.
- Synthetic login candidate traces can show response planning status and no response generated.
- Synthetic traffic remains non-`trace:client` and non-confirmed.

## Security Notes

- No real account authentication was implemented.
- No real database connection was implemented.
- No plaintext password handling was added.
- No token, cookie, or session key handling was added.
- No real login success/failure response bytes were generated.
- No character list response bytes were generated.
- No real trace, pcap, pcapng, har, database, binary, client resource, or reference server source was added.
- No character selection, enter-map, movement, inventory, equipment, NPC, task, battle, or GUI behavior was implemented.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main`
- `git checkout -B task/0026-implement-login-response-contract-candidates`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- `dotnet --info`
- `git diff --stat`
- line-count check script for TASK-0026 source, tests, and Markdown files

## Test Results

- `dotnet build .\src\PlServer.sln`: succeeded, 0 warnings, 0 errors.
- `dotnet test .\src\PlServer.sln`: succeeded, 229 tests passed, 0 failed, 0 skipped.
- `.NET SDK`: 8.0.421.

## Line Count Check

- `src/PlServer.LegacyProtocol/LoginResponseKind.cs`: 11 lines
- `src/PlServer.LegacyProtocol/LoginResponseContractStatus.cs`: 9 lines
- `src/PlServer.LegacyProtocol/LoginResponseContractPlanStatus.cs`: 10 lines
- `src/PlServer.LegacyProtocol/LoginResponseFieldDescriptor.cs`: 48 lines
- `src/PlServer.LegacyProtocol/LoginResponseContractCandidate.cs`: 66 lines
- `src/PlServer.LegacyProtocol/LoginResponseContractPlan.cs`: 26 lines
- `src/PlServer.LegacyProtocol/LoginResponseContractCandidateRegistry.cs`: 117 lines
- `src/PlServer.Application/LoginResponseCandidatePlanStatus.cs`: 10 lines
- `src/PlServer.Application/LoginResponseCandidatePlanResult.cs`: 93 lines
- `src/PlServer.Application/LoginResponseCandidatePlanner.cs`: 54 lines
- `src/PlServer.Application/LoginRequestParseResult.cs`: 100 lines
- `src/PlServer.Application/LoginRequestCandidateHandler.cs`: 93 lines
- `tests/PlServer.LegacyProtocol.Tests/LoginResponseContractCandidateRegistryTests.cs`: 140 lines
- `tests/PlServer.Application.Tests/LoginResponseCandidatePlannerTests.cs`: 233 lines
- `tests/PlServer.Network.Tests/HostSmokeTests.cs`: 378 lines
- `ai/context/current-state.md`: 96 lines
- `ai/context/latest-status.md`: 42 lines
- `ai/tasks/TASK-0026-implement-login-response-contract-candidates.md`: 46 lines
- `ai/reports/REPORT-0026-implement-login-response-contract-candidates.md`: 134 lines before adding this line-count section.

## Risks

- Login response packet structure is still unknown.
- Login request field structure is still unknown.
- Candidate response metadata must not be treated as sendable protocol frames.
- Future response contract changes must not be promoted to confirmed without sanitized target-client trace review.

## Blockers

- Real sanitized target-client LoginRequest/LoginResponse traces are not available.
- Character list contract candidates require a separate reviewed task.
- Real authentication and real response generation remain blocked until safe field extraction and reviewed response contracts exist.

## Next Recommended Task

`TASK-0027-implement-character-list-contract-candidates`

## Branch

`task/0026-implement-login-response-contract-candidates`

## Commit Hash

Final commit hash is printed in terminal output after commit.

## Push Result

Final push result is printed in terminal output after push.
