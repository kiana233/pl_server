# TASK-0005: Packet Codec Implementation

## Goal

Implement the initial protocol codec after TASK-0004 is complete.

## Scope

Create or update source and tests for:

- packet frame constants
- frame encode/decode
- AC/SubAC accessors
- length validation
- basic XOR encode/decode placeholder behavior if verified

## Acceptance Criteria

- Unit tests cover single complete frame, invalid header, invalid length, and AC/SubAC reads.
- Any unverified XOR behavior is left behind a documented contract.
