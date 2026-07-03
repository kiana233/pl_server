# Codex Auto Commit Policy

## Branch Rule

Each task gets one task branch. Codex must not commit task work directly to `main`.

Branch format:

`task/<task-id-lowercase-title>`

Example:

`task/0009-local-intake-and-architecture-v2`

Each task must produce one task branch, one report under `ai/reports/`, one `ai/context/current-state.md` update, and one primary task commit. Follow-up review fixups may add small additional commits on the same task branch.

## Merge Rule

Codex must not automatically merge task branches into `main`. Codex also must not directly push `main`. ChatGPT must review the task branch before any `main` merge is allowed, and the human owner controls the final merge.

## Commit Message Format

Use the task-provided commit message. If none is provided, use:

`TASK-XXXX short task summary`

## Push Target

Push to `origin` using the task branch:

`git push -u origin <task-branch-name>`

## Pre-Commit Checks

Before committing, Codex must run:

- `git status --short`
- `git diff --stat`
- requested build/test commands, or document why they cannot run

Codex must confirm the staged content does not include prohibited files.

## Prohibited Commit Content

Do not commit:

- client binaries or copyrighted client resources
- copied reference server source or large source excerpts
- secrets, tokens, real account passwords, or private packet data
- `bin`, `obj`, `exe`, `dll`
- real databases
- unrelated files outside task scope

## Build/Test Requirement

Every code change must include tests where possible. Documentation-only tasks still run requested checks and record no-op or failure reasons.

## Failure Handling

If commit fails, record the failure and leave the working tree intact for review.

If push fails due to authentication or network issues, record the exact failure in the report and do not repeatedly retry in a way that risks repository state.
