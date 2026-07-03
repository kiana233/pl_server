# Codex Auto Commit Policy

This document defines how Codex may commit and push task work for this repository.

## Branch Rule

Each task gets one task branch.

Codex must not commit task work directly to `main`.

Branch format:

`task/<task-id-lowercase-title>`

Example:

`task/0009-local-intake-and-architecture-v2`

## Merge Rule

Codex must not automatically merge task branches into `main`.

The human owner controls the final merge after ChatGPT review approval.

## Commit Message Format

Use the task-provided commit message.

If none is provided, use:

`TASK-XXXX short task summary`

Example:

`TASK-0009 local intake and architecture v2`

## Push Target

Push to `origin` using the current task branch:

`git push -u origin <task-branch-name>`

Example:

`git push -u origin task/0009-local-intake-and-architecture-v2`

## Required Files Per Task

Each task should produce or update:

* one task file under `ai/tasks/`
* one report under `ai/reports/`
* `ai/context/current-state.md`
* implementation, tests, or documentation required by the task

Follow-up review fixups may add small additional commits on the same task branch.

## Pre-Commit Checks

Before committing, Codex must run:

* `git status --short`
* `git diff --stat`
* requested build/test commands, or document why they cannot run

Codex must confirm the staged content does not include prohibited files.

## Prohibited Commit Content

Do not commit:

* client binaries or copyrighted client resources
* copied reference server source or large source excerpts
* secrets, tokens, real account passwords, or private packet data
* `bin`, `obj`, `exe`, `dll`
* real databases
* unrelated files outside task scope

## Build/Test Requirement

Every code change must include tests where possible.

Documentation-only tasks still run requested checks and record no-op or failure reasons.

## Failure Handling

If commit fails, record the failure and leave the working tree intact for review.

If push fails due to authentication or network issues, record the exact failure in the report and do not repeatedly retry in a way that risks repository state.

## Main Branch Protection

Codex must not push directly to `main`.

Codex must not merge task branches into `main` unless ChatGPT explicitly says the task is approved for merge.
