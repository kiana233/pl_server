# Development Workflow

## Roles

- Human owner provides client observations, sanitized traces, and final review.
- ChatGPT plans architecture, protocol analysis, task design, acceptance criteria, and report review.
- Codex performs local implementation, tests, reports, commits, branch updates, and pushes task branches.

## Continue Flow

When the user sends "继续", ChatGPT should actively inspect the current GitHub repository state, review the latest task/report status, and generate the next small task.

Codex is responsible for reading local paths, following `AGENTS.md`, executing the task, running checks, updating local context, writing a report, committing, and pushing the task branch.

## Required Codex Steps Per Task

1. Read `ai/context/latest-status.md`.
2. Read the active task file under `ai/tasks/`.
3. Confirm the working tree is clean before branch setup.
4. Do not work directly on `main`.
5. Create or switch to the task branch.
6. Modify only task-scoped files.
7. Update `ai/context/current-state.md`.
8. Generate a report under `ai/reports/`.
9. Run tests or explain why tests cannot run.
10. Run pre-commit checks.
11. Commit with the task commit message.
12. Push to the task branch.

## Merge Policy

Codex must not automatically merge to `main`. ChatGPT reviews the GitHub-visible state after push and prepares the next task or review notes. The human owner controls final merge.

## Testing Policy

Every code change must include tests where possible. Documentation-only tasks must still run the requested build/test commands and record failure or no-op reasons.

## Context Policy

Codex must update `current-state` each task so future turns can start from local facts. Reports must include commands run, results, branch, commit/push outcome, known risks, blockers, and the next recommended task.
