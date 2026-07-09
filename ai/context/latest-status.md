# Latest Status

Date: 2026-07-09

## Phase

Phase 15 / Account repository skeleton completed.

## Last Completed Task

`ai/tasks/TASK-0024-implement-account-repository-skeleton.md`

## Active Task

None. Waiting for ChatGPT review and TASK-0025 creation.

## Current Goal

Prepare to define login request field parser candidates without treating reference or synthetic behavior as confirmed target-client protocol facts.

## Current Constraints

- Do not implement gameplay before protocol framing, logging, replay, session, contract registry, routing, TCP host, stream receive, and connection session foundations.
- Do not implement real AC handlers before explicit handler tasks.
- Do not add client resources or copied reference-server source.
- Protocol facts must keep source labels.
- Reference behavior cannot be marked confirmed without target-client trace.
- Synthetic host smoke tests are not target-client trace.
- LoginRequestCandidateHandler keeps payload bytes opaque and does not invoke account authentication.
- AccountLoginCandidateService is candidate-only and does not generate protocol login success, login response packets, or character list responses.
- No real account authentication, real database connection, plaintext password handling, token handling, session key handling, or private trace handling is implemented.
- Real target-client trace must be sanitized, replayable, and human reviewed before evidence promotion.
- Synthetic tests and reference behavior still cannot be marked confirmed.
- Codex may commit and push task branches, but must not merge `main` without ChatGPT review approval.

## Next Suggested Task

`TASK-0025-implement-login-request-field-parser-candidates`
