# TASK-0003: Protocol Known Facts Table

## Goal

Build the first protocol fact table for the server rebuild.

## Scope

Create or update:

- `ai/context/protocol-known-facts.md`
- `docs/02-client-protocol-analysis.md`
- `docs/05-protocol-log-format.md`
- `ai/reports/REPORT-0003-protocol-facts.md`

## Required Sections

1. Packet frame format
2. Header
3. Length field
4. XOR encoding
5. AC/SubAC position
6. AC0 known behavior
7. AC63 known behavior
8. AC06 known behavior
9. Unknowns requiring client traces

## Acceptance Criteria

- Every fact has a source label:
  - `reference:wlophoenix`
  - `reference:muayad`
  - `trace:client`
  - `assumption`
- Unknowns are listed separately.
- No gameplay code is added.
