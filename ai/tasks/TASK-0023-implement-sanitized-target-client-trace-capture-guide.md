# TASK-0023 Implement Sanitized Target Client Trace Capture Guide

## Task ID

TASK-0023-implement-sanitized-target-client-trace-capture-guide

## Branch

`task/0023-implement-sanitized-target-client-trace-capture-guide`

## Goal

Create the documentation, template, sanitization rules, and evidence promotion workflow needed before any real target-client trace can be committed or used to confirm protocol behavior.

## Scope

- Add target-client trace capture guidance.
- Add trace sanitization policy.
- Add trace evidence promotion policy.
- Add target-client trace requirements context.
- Add sanitized trace JSONL schema templates with fake sample data only.
- Update `.gitignore` to block raw, private, unreviewed, real, and binary capture formats.
- Update current state and latest status.

## Out Of Scope

- Real capture tooling.
- Real target-client traces.
- Raw `.pcap`, `.pcapng`, `.har`, database, or binary capture files.
- Account names, passwords, tokens, real IP addresses, device identifiers, or personal data.
- Real account authentication.
- Login response, character-list response, enter-map, movement, inventory, equipment, NPC, quest, battle, or GUI behavior.

## Acceptance Criteria

- Documentation clearly states what can and cannot be captured or committed.
- Templates use fake sample data only.
- `.gitignore` blocks raw/private/unreviewed trace paths and capture file extensions.
- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- Task branch is committed and pushed.
