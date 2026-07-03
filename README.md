# pl_server

This repository is the central workspace for rebuilding a stable Wonderland Online compatible server foundation.

The project is intentionally driven by:

- verified protocol facts
- small task files under `ai/tasks/`
- execution reports under `ai/reports/`
- replay tests before broad gameplay work

## Workflow

1. ChatGPT creates plans, task files, and acceptance criteria.
2. Codex works locally from one active task at a time.
3. Codex updates code, docs, tests, and an execution report.
4. Gitee stores the full history of plans, tasks, reports, branches, and commits.
5. The human owner provides client observations, sanitized traces, and review.

## Current Phase

Phase 0: AI-Codex workflow bootstrap.

No server implementation should be added until the repository workflow, protocol facts, logging format, and replay-test direction are documented.

## Safety Boundary

Do not commit complete client binaries, copyrighted client data/resources, account credentials, tokens, or private packet captures.

Allowed materials include sanitized protocol logs, self-written protocol analysis, server code, mock test data, minimal sample frames, and hash/size/version records.
