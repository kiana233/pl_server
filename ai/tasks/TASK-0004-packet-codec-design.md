# TASK-0004: Packet Codec Design

## Goal

Design the protocol-layer shape before implementation.

## Scope

Create or update:

- `docs/02-client-protocol-analysis.md`
- `ai/context/protocol-known-facts.md`
- `ai/reports/REPORT-0004-packet-codec-design.md`

## Acceptance Criteria

- Packet frame, reader, writer, splitter, XOR codec, and trace responsibilities are separated.
- Unknown protocol behavior is recorded as assumption, not implemented as fact.
- No gameplay systems are added.
