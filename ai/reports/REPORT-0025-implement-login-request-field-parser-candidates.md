# REPORT-0025 Implement Login Request Field Parser Candidates

## Task ID

TASK-0025-implement-login-request-field-parser-candidates

## Summary

Implemented a candidate-only LoginRequest field parser framework. The default parser keeps AC63/SubAC4 payloads opaque, records safe parser metadata, forces sensitive values through redaction, and integrates parser status into `LoginRequestCandidateHandler` notes so trace output can show safe parser status without exposing account credentials or confirming target-client behavior.

## Changed Files

- `src/PlServer.Application/ILoginRequestCandidateParser.cs`
- `src/PlServer.Application/LoginRequestParseResult.cs`
- `src/PlServer.Application/LoginRequestParseStatus.cs`
- `src/PlServer.Application/LoginRequestField.cs`
- `src/PlServer.Application/LoginRequestFieldKind.cs`
- `src/PlServer.Application/LoginRequestFieldSensitivity.cs`
- `src/PlServer.Application/LoginRequestCandidateParserRegistry.cs`
- `src/PlServer.Application/OpaqueLoginRequestCandidateParser.cs`
- `src/PlServer.Application/RedactedFieldValue.cs`
- `src/PlServer.Application/LoginRequestCandidateHandler.cs`
- `tests/PlServer.Application.Tests/LoginRequestCandidateParserTests.cs`
- `tests/PlServer.Network.Tests/HostSmokeTests.cs`
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/tasks/TASK-0025-implement-login-request-field-parser-candidates.md`
- `ai/reports/REPORT-0025-implement-login-request-field-parser-candidates.md`

## Implemented Classes

- `ILoginRequestCandidateParser`
- `LoginRequestParseResult`
- `LoginRequestParseStatus`
- `LoginRequestField`
- `LoginRequestFieldKind`
- `LoginRequestFieldSensitivity`
- `LoginRequestCandidateParserRegistry`
- `OpaqueLoginRequestCandidateParser`
- `RedactedFieldValue`

## Parser Behavior

- Default parser is `OpaqueLoginRequestCandidateParser`.
- Default status is `OpaquePayload` for valid LoginRequest candidates.
- Invalid packet input returns `InvalidPacket` without throwing.
- Parser records payload length, AC, SubAC, source label, evidence status, and notes.
- Parser does not parse account identifier fields.
- Parser does not parse password fields.
- Parser does not output raw payload bytes.
- Parser result `IsConfirmed` is always false.
- Reference and synthetic behavior remains pending-target-client-trace or unknown.

## Redaction Rules

- `SecretCandidate`, `TokenCandidate`, `UnknownSensitive`, and `Redacted` sensitivities return `[redacted]`.
- Opaque payload field value is redacted.
- Parser and handler notes do not include raw password, token, cookie, session key, or payload values.

## LoginRequestCandidateHandler Integration

- `LoginRequestCandidateHandler` now uses `LoginRequestCandidateParserRegistry`.
- The default registry contains `OpaqueLoginRequestCandidateParser`.
- Handler notes include parser status, source, evidence status, confirmation flag, payload length, field layout unknown, pending target-client trace, and no password/token emitted.
- Handler continues to return candidate-only handling.
- Handler does not invoke `AccountLoginCandidateService` when field layout is unknown.
- Handler does not generate login response packets.
- Handler does not generate character list response packets.

## Trace Notes

- Existing network trace flow carries handler notes into `ProtocolTraceEvent.HandlerNotes`.
- Synthetic login candidate host smoke trace now includes parser status and opaque layout notes.
- Synthetic traffic remains non-`trace:client` and non-confirmed.

## Security Notes

- No real account authentication was implemented.
- No real database connection was implemented.
- No plaintext password handling was added.
- No token, cookie, or session key handling was added.
- No real trace, pcap, pcapng, har, database, binary, client resource, or reference server source was added.
- No login, character list, enter-map, movement, inventory, equipment, NPC, task, battle, or GUI behavior was implemented.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main`
- `git checkout -B task/0025-implement-login-request-field-parser-candidates`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- `dotnet --info`
- `git diff --stat`
- line-count check script for TASK-0025 source, tests, and Markdown files

## Test Results

- `dotnet build .\src\PlServer.sln`: succeeded, 0 warnings, 0 errors.
- `dotnet test .\src\PlServer.sln`: succeeded, 209 tests passed, 0 failed, 0 skipped.
- `.NET SDK`: 8.0.421.

## Line Count Check

- `src/PlServer.Application/ILoginRequestCandidateParser.cs`: 10 lines
- `src/PlServer.Application/LoginRequestParseResult.cs`: 81 lines
- `src/PlServer.Application/LoginRequestParseStatus.cs`: 10 lines
- `src/PlServer.Application/LoginRequestField.cs`: 50 lines
- `src/PlServer.Application/LoginRequestFieldKind.cs`: 13 lines
- `src/PlServer.Application/LoginRequestFieldSensitivity.cs`: 11 lines
- `src/PlServer.Application/LoginRequestCandidateParserRegistry.cs`: 49 lines
- `src/PlServer.Application/OpaqueLoginRequestCandidateParser.cs`: 78 lines
- `src/PlServer.Application/RedactedFieldValue.cs`: 47 lines
- `src/PlServer.Application/LoginRequestCandidateHandler.cs`: 71 lines
- `tests/PlServer.Application.Tests/LoginRequestCandidateParserTests.cs`: 239 lines
- `tests/PlServer.Network.Tests/HostSmokeTests.cs`: 352 lines
- `ai/context/current-state.md`: 92 lines
- `ai/context/latest-status.md`: 40 lines
- `ai/tasks/TASK-0025-implement-login-request-field-parser-candidates.md`: 48 lines
- `ai/reports/REPORT-0025-implement-login-request-field-parser-candidates.md`: 129 lines before adding this line-count section.

## Risks

- Login request field positions are still unknown.
- Parser output is intentionally conservative until sanitized target-client trace exists.
- Future parser rules must not be promoted to confirmed without target-client trace review.

## Blockers

- Real sanitized target-client LoginRequest traces are not available.
- Real login response contract candidates require a separate reviewed task.
- Real authentication remains blocked until safe field extraction and response contracts exist.

## Next Recommended Task

`TASK-0026-implement-login-response-contract-candidates`

## Branch

`task/0025-implement-login-request-field-parser-candidates`

## Commit Hash

Final commit hash is printed in terminal output after commit.

## Push Result

Final push result is printed in terminal output after push.
