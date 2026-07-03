# TASK-0002: Reference Server Analysis

## Background

There are two known reference repositories:

- `muayad-mahmoud/Wonderland-Online-Private-Server`
- `wlophoenix/Wonderland-Private-Server`

The project should not blindly copy either repository.

## Goal

Create a reference analysis document explaining which parts are useful, uncertain, or risky.

## Scope

Create or update:

- `docs/01-reference-servers-analysis.md`
- `references/wlophoenix-notes.md`
- `references/muayad-notes.md`
- `ai/context/reference-server-analysis.md`
- `ai/reports/REPORT-0002-reference-analysis.md`

## Required Analysis

Cover at least:

1. Packet structure
2. AC0 handshake
3. AC63 login
4. AC06 movement
5. World/map structure
6. Player/Character/Inventory model
7. Known incompleteness
8. What must be verified against the real client

## Acceptance Criteria

- Document clearly separates verified facts from assumptions.
- Document explains why random AC patching is not allowed.
- Report includes suggested next task.
