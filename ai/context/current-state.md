# Current State

Date: 2026-07-09

## Current Phase

Phase 16 / Login Request Field Parser Candidates

## Repository Paths

- Main repository: `D:\Wonderland\pl_server`
- Target client, read-only: `D:\Wonderland\client`
- Reference server, read-only: `D:\pl\server`

## Current GitHub Visible State

- Remote: `origin https://github.com/kiana233/pl_server.git`
- Task branch: `task/0025-implement-login-request-field-parser-candidates`
- Base branch: `main`

## Current Local Scan State

- TASK-0010 created a real .NET solution and project skeleton.
- TASK-0011 implemented the basic Protocol-layer PacketCodec.
- TASK-0012 implemented protocol trace logging infrastructure in `PlServer.Diagnostics`.
- TASK-0013 implemented the replay framework in `PlServer.Replay`.
- TASK-0014 created the SessionStateMachine / SessionPacketClassifier / SessionStateGuard foundation.
- TASK-0015 implemented the LegacyProtocol ProtocolContractRegistry.
- TASK-0016 implemented the ActionRouter skeleton.
- TASK-0017 implemented the TCP Host skeleton.
- TASK-0018 implemented frame splitting and connection receive pipeline hardening.
- TASK-0019 implemented connection-level SessionState update pipeline.
- TASK-0020 implemented Host-level smoke tests and a synthetic TCP client test utility.
- TASK-0021 implemented ProtocolTrace state-change enrichment.
- TASK-0022 implemented login / handshake candidate handlers.
- TASK-0023 established sanitized target-client trace capture, sanitization, and evidence promotion guidance.
- TASK-0024 created an account repository skeleton and candidate-only account lookup service.
- TASK-0025 implements a LoginRequest field parser candidate framework.

## Implemented Content

- `PlServer.Application` now contains `ILoginRequestCandidateParser`, login parse result models, redacted field value models, `LoginRequestCandidateParserRegistry`, and `OpaqueLoginRequestCandidateParser`.
- `LoginRequestCandidateHandler` safely invokes the parser registry and records parser status in handler notes.
- The default LoginRequest parser remains opaque / unknown layout.
- The parser records payload length, AC, SubAC, source label, evidence status, and safe notes only.
- SecretCandidate, TokenCandidate, UnknownSensitive, and Redacted values are forced through redacted display values.
- `tests/PlServer.Application.Tests` covers parser status, payload length, AC/SubAC context, redaction, not-confirmed evidence, invalid packet safety, and handler integration.
- `tests/PlServer.Network.Tests` covers synthetic host smoke trace parser notes, LoginPending state, no real account authentication, no response bytes, and non-confirmed synthetic evidence.

## Login Request Parser Scope

- The current parser does not parse account fields.
- The current parser does not parse password fields.
- The current parser does not output raw payload bytes.
- The current parser does not output token, cookie, or session key values.
- The current parser does not authenticate accounts.
- The current parser does not generate login response packets.
- The current parser does not generate character list response packets.
- AccountLoginCandidateService is still not connected to real handler flow because safe login field extraction does not exist yet.
- Parser conclusions remain `reference:muayad` / `unknown` and `pending-target-client-trace` / `unknown`.
- Parser results are never marked confirmed.

## Sanitized Target Client Trace Scope

- Synthetic TCP client tests connect to `TcpServerHost` on loopback port 0.
- Host smoke tests use synthetic traffic only and do not confirm target-client behavior.
- Synthetic login candidate traffic can reach `LoginPending` session state through candidate-only session rules.
- Synthetic login candidate trace notes can include parser status and payload length.
- Synthetic traffic is not `trace:client` and cannot promote parser behavior to confirmed.
- Real target-client trace is still absent from the repository.
- Real target-client trace remains the prerequisite for confirming login request field layout.
- Any trace entering the repository must be sanitized first.

## Not Implemented

- Real account authentication is not implemented.
- Real database access is not implemented.
- Real login request account/password field layout is not confirmed.
- Real login response generation is not implemented.
- Character list response, character selection, enter-map response, movement broadcast, inventory, equipment, NPC, quests, warp, and battle are not implemented.
- GUI management functionality is not implemented beyond the existing minimal WPF shell.
- No real account, password, token, cookie, session key, database, trace, pcap, pcapng, har, IP, device identifier, or personal data was added.

## Current Blockers

- Real target-client packet traces are not yet available.
- Login request field layout is still unknown and must not be guessed as confirmed.
- Real authentication and response packet generation require explicit future tasks and review approval.

## Next Suggested Task

TASK-0026-implement-login-response-contract-candidates
