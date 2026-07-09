# TASK-0025 Implement Login Request Field Parser Candidates

## Task ID

TASK-0025-implement-login-request-field-parser-candidates

## Branch

`task/0025-implement-login-request-field-parser-candidates`

## Goal

Create a candidate-only LoginRequest field parser framework that can safely summarize opaque AC63/SubAC4 login payloads without guessing account or password field locations, exposing sensitive values, authenticating accounts, or generating login/character response packets.

## Scope

- Add parser interfaces, parse result models, redacted field models, and a parser registry in `PlServer.Application`.
- Add a default `OpaqueLoginRequestCandidateParser`.
- Integrate parser invocation into `LoginRequestCandidateHandler`.
- Add Application tests for parser behavior and redaction.
- Add Network smoke tests verifying parser status appears in trace notes without promoting synthetic traffic to confirmed evidence.
- Update current status, latest status, task, and report documentation.

## Non-Goals

- No hardcoded AC63/SubAC4 account field location.
- No hardcoded AC63/SubAC4 password field location.
- No plaintext password output, storage, or logging.
- No token, cookie, or session key output, storage, or logging.
- No real account authentication.
- No real database access.
- No login success/failure response packets.
- No character list response packets.
- No gameplay, GUI, client resources, reference-server source, real traces, databases, or binaries.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- Default parser returns `OpaquePayload` or safe invalid result.
- Parser records payload length and AC63/SubAC4 context.
- Parser does not expose password, token, cookie, or session key fields.
- Sensitive field values are redacted.
- `LoginRequestCandidateHandler` invokes the parser and records parser status in notes.
- Handler does not invoke real account authentication when parser is opaque.
- Handler does not generate login or character list response packets.
- Parser evidence remains pending-target-client-trace or unknown and is never confirmed.
- Synthetic host smoke trace includes parser status without becoming `trace:client` confirmed evidence.
