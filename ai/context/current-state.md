# Current State

Date: 2026-07-09

## Current Phase

Phase 18 / Character List Contract Candidates

## Repository Paths

- Main repository: `D:\Wonderland\pl_server`
- Target client, read-only: `D:\Wonderland\client`
- Reference server, read-only: `D:\pl\server`

## Current GitHub Visible State

- Remote: `origin https://github.com/kiana233/pl_server.git`
- Task branch: `task/0027-implement-character-list-contract-candidates`
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
- TASK-0025 implemented a LoginRequest field parser candidate framework.
- TASK-0026 implemented Login Response Contract Candidates.
- TASK-0027 implements Character List Contract Candidates.

## Implemented Content

- `PlServer.LegacyProtocol` now contains S2C character list candidate contract metadata, registry lookup, field descriptors, and no-packet contract plans.
- Seeded character list candidates include CharacterListFollowsLoginCandidate, EmptyCharacterListCandidate, and CharacterListUnavailableCandidate.
- `PlServer.Application` now contains `CharacterListCandidatePlanner`, plan result models, and candidate-only plan statuses.
- `LoginRequestCandidateHandler` records character list planning status in handler notes.
- Existing trace flow can surface character list planning notes through `ProtocolTraceEvent.HandlerNotes`.
- `tests/PlServer.LegacyProtocol.Tests` covers seeded character list candidates, S2C direction, not-confirmed evidence, duplicate rejection, unknown lookup safety, field descriptors, and sanitized-trace notes.
- `tests/PlServer.Application.Tests` covers planner no-response behavior, no PacketWriter dependency, CandidateAuthenticated not being character-list-ready, no sensitive fields, and handler notes.
- `tests/PlServer.Network.Tests` covers synthetic host smoke trace notes, LoginPending candidate state, no response bytes, no character list bytes, no selection or enter-map response notes, and non-confirmed synthetic traffic.

## Character List Candidate Scope

- The current framework only establishes S2C character list candidate contracts and planner metadata.
- Character list response layout is unknown because sanitized target-client trace is still absent.
- Login request and login response field layouts are also still unknown.
- Planner does not generate bytes.
- Planner does not call PacketWriter.
- Planner does not change SessionState.
- Planner does not generate login success or failure response packets.
- Planner does not generate character list response packets.
- Planner does not generate character selection or enter-map response packets.
- CandidateAuthenticated does not equal client login success and does not make the session character-list-ready.
- Character list candidate conclusions remain `assumption` / `unknown` and `pending-target-client-trace` / `unknown`.
- Character list candidates are never marked confirmed.

## Sanitized Target Client Trace Scope

- Synthetic TCP client tests connect to `TcpServerHost` on loopback port 0.
- Host smoke tests use synthetic traffic only and do not confirm target-client behavior.
- Synthetic login candidate traffic can reach `LoginPending` session state through candidate-only session rules.
- Synthetic login candidate trace notes can include parser status, response planning status, and character list planning status.
- Synthetic traffic is not `trace:client` and cannot promote character list contracts to confirmed.
- Real target-client trace is still absent from the repository.
- Real target-client trace remains the prerequisite for confirming login request, login response, and character list field layouts.
- Any trace entering the repository must be sanitized first.

## Not Implemented

- Real account authentication is not implemented.
- Real character repository access is not implemented.
- Real database access is not implemented.
- Real login request account/password field layout is not confirmed.
- Real login success/failure response layout is not confirmed.
- Real character list response layout is not confirmed.
- Real login response generation is not implemented.
- Real character list response generation is not implemented.
- Character selection, enter-map response, movement broadcast, inventory, equipment, NPC, quests, warp, and battle are not implemented.
- GUI management functionality is not implemented beyond the existing minimal WPF shell.
- No real account, character name, password, token, cookie, session key, database, trace, pcap, pcapng, har, IP, device identifier, or personal data was added.

## Current Blockers

- Real target-client packet traces are not yet available.
- Login request, login response, and character list field layouts are still unknown and must not be guessed as confirmed.
- Real character data access requires an explicit future repository task.
- Real authentication and response packet generation require explicit future tasks and review approval.

## Next Suggested Task

TASK-0028-implement-character-repository-skeleton
