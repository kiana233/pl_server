# Latest Status

Date: 2026-07-09

## Phase

Phase 18 / Character list contract candidates completed.

## Last Completed Task

`ai/tasks/TASK-0027-implement-character-list-contract-candidates.md`

## Active Task

None. Waiting for ChatGPT review and TASK-0028 creation.

## Current Goal

Prepare to define a character repository skeleton without generating real character list, character selection, enter-map, movement, or gameplay response packets.

## Current Constraints

- Do not implement gameplay before protocol framing, logging, replay, session, contract registry, routing, TCP host, stream receive, and connection session foundations.
- Do not implement real AC handlers before explicit handler tasks.
- Do not add client resources or copied reference-server source.
- Protocol facts must keep source labels.
- Reference behavior cannot be marked confirmed without target-client trace.
- Synthetic host smoke tests are not target-client trace.
- LoginRequestCandidateHandler invokes only candidate parser, login response candidate planner, and character list candidate planner flow.
- Login request, login response, and character list field layouts remain unknown and pending target-client trace.
- AccountLoginCandidateService is candidate-only and is not connected to handler authentication flow.
- CandidateAuthenticated is not client login success and is not character-list-ready.
- Character repository is not implemented.
- No real account authentication, real database connection, plaintext password handling, token handling, cookie handling, session key handling, character data handling, or private trace handling is implemented.
- No login response packet, character list response, character selection response, enter-map response, movement response, or gameplay response is generated.
- Real target-client trace must be sanitized, replayable, and human reviewed before evidence promotion.
- Synthetic tests and reference behavior still cannot be marked confirmed.
- Codex may commit and push task branches, but must not merge `main` without ChatGPT review approval.

## Next Suggested Task

`TASK-0028-implement-character-repository-skeleton`
