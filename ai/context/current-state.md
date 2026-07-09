# Current State

Date: 2026-07-09

## Current Phase

Phase 15 / Account Repository Skeleton

## Repository Paths

- Main repository: `D:\Wonderland\pl_server`
- Target client, read-only: `D:\Wonderland\client`
- Reference server, read-only: `D:\pl\server`

## Current GitHub Visible State

- Remote: `origin https://github.com/kiana233/pl_server.git`
- Task branch: `task/0024-implement-account-repository-skeleton`
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
- TASK-0024 creates an account repository skeleton and candidate-only account lookup service.

## Implemented Content

- `PlServer.Core` contains AccountId, AccountName, AccountStatus, and AccountRecord model types.
- `PlServer.Application` contains ActionRouter skeleton, candidate handler result models, AC0 HandshakeCandidate handler, AC63/SubAC4 LoginRequestCandidate handler, account repository interfaces, credential verifier interface, and AccountLoginCandidateService.
- `PlServer.Persistence` contains an in-memory account repository for skeleton and test use only.
- `PlServer.Protocol` contains PacketCodec, packet encode/decode, validation errors, XOR helper, packet reader, and packet writer.
- `PlServer.Diagnostics` contains ProtocolTraceLogger, state-change trace models, and JSON Lines trace sink support.
- `PlServer.Replay` can replay trace-derived packet steps through PacketCodec.
- `PlServer.Session` contains session classification, state machine, and SessionStateGuard.
- `PlServer.LegacyProtocol` contains protocol contract metadata and seeded candidate contracts.
- `PlServer.Network` contains TCP host skeleton, frame splitter, connection receive loop, receive pipeline, packet route pipeline, and connection session updater.
- `tests/PlServer.Application.Tests` covers account candidate service outcomes and confirms login payload remains opaque.
- `tests/PlServer.Persistence.Tests` covers seeded in-memory lookup, case-insensitive lookup, duplicate rejection, missing lookup, id lookup, and no real credential/database behavior.

## Account Repository Skeleton Scope

- Account repository behavior is skeleton-only and uses fake in-memory data for tests.
- AccountLoginCandidateService can return AccountNotFound, AccountDisabled, InvalidCredentials, or CandidateAuthenticated.
- CandidateAuthenticated is not a protocol login success.
- CandidateAuthenticated does not generate login response packets or character list responses.
- LoginRequestCandidateHandler still keeps payload bytes opaque and does not invoke the account repository.
- Credential verification is an interface only; no plaintext credential validation is implemented.
- AccountRecord does not include plaintext password, token, cookie, or session key fields.
- InMemoryAccountRepository has no real database connection and no persistent storage.
- Account lookup behavior remains candidate-only and does not confirm target-client protocol behavior.

## Sanitized Target Client Trace Scope

- Synthetic TCP client tests connect to `TcpServerHost` on loopback port 0.
- Host smoke tests use synthetic traffic only and do not confirm target-client behavior.
- Real target-client trace is still absent from the repository.
- Any trace entering the repository must be sanitized first.
- Synthetic tests still cannot be promoted to confirmed.
- Only sanitized, replayable, human-reviewed target-client trace can support evidence promotion.

## Not Implemented

- Real account authentication is not implemented.
- Real database access is not implemented.
- Real login response generation is not implemented.
- Character list response, character selection, enter-map response, movement broadcast, inventory, equipment, NPC, quests, warp, and battle are not implemented.
- GUI management functionality is not implemented beyond the existing minimal WPF shell.
- Login request field parsing remains opaque and pending target-client trace.
- No real account, password, token, session key, cookie, database, trace, pcap, pcapng, har, IP, device identifier, or personal data was added.

## Current Blockers

- Real target-client packet traces are not yet available.
- Login request field layout is still unknown and must not be guessed as confirmed.
- Real handlers require explicit future tasks and review approval.

## Next Suggested Task

TASK-0025-implement-login-request-field-parser-candidates
