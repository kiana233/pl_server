# Latest Status

Date: 2026-07-09

## Phase

Phase 14 / Sanitized target-client trace capture guide completed.

## Last Completed Task

`ai/tasks/TASK-0023-implement-sanitized-target-client-trace-capture-guide.md`

## Active Task

None. Waiting for ChatGPT review and TASK-0024 creation.

## Current Goal

Prepare to implement an account repository skeleton or login response contract candidates in TASK-0024.

## Current Constraints

- Do not implement gameplay before protocol framing, logging, replay, session, contract registry, routing, TCP host, stream receive, and connection session foundations.
- Do not implement real AC handlers before explicit handler tasks.
- Do not add client resources or copied reference-server source.
- Protocol facts must keep source labels.
- Reference behavior cannot be marked confirmed without target-client trace.
- Synthetic host smoke tests are not target-client trace.
- TCP receive pipeline routes only to ActionRouter skeleton results and no-op or missing-handler outcomes.
- Connection session updates are candidate-only and do not execute login or gameplay behavior.
- Packet trace events can carry candidate session previous/current state, state-change flag, rejection reason, errors, and notes.
- AC0 and AC63/SubAC4 candidate handlers only record candidate handling and never authenticate accounts or generate response packets.
- Real target-client trace must be sanitized, replayable, and human reviewed before evidence promotion.
- Synthetic tests and reference behavior still cannot be marked confirmed.
- Host smoke tests do not generate login, enter-map, movement, or gameplay response packets.
- Codex may commit and push task branches, but must not merge `main` without ChatGPT review approval.

## Next Suggested Task

`TASK-0024-implement-account-repository-skeleton`
