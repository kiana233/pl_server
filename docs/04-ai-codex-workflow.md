# 04 AI-Codex Workflow

## Responsibilities

ChatGPT:

- analyze client behavior
- analyze reference servers
- design protocol direction
- write tasks and acceptance criteria
- review Codex reports
- decide the next task

Codex:

- pull the repository
- read `AGENTS.md`
- read `ai/context/latest-status.md`
- read the active task
- implement only that task
- run relevant tests
- write a report under `ai/reports/`
- commit and push only when explicitly instructed

Gitee:

- store code, plans, tasks, reports, branches, and pull requests

Human owner:

- provide observations and sanitized traces
- review risky changes
- control final merge

## Per-Task Flow

1. Read `AGENTS.md`.
2. Read `ai/context/latest-status.md`.
3. Read exactly one active task.
4. Make scoped changes.
5. Run relevant tests or explain why none exist.
6. Write or update the task report.
7. Summarize risks and next task.
