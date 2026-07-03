# Client Trace Index

No sanitized client traces have been added yet.

## Rules

- Store only sanitized traces.
- Remove accounts, passwords, tokens, machine identifiers, and private personal data.
- Prefer minimal reproductions tied to one task.
- Record client version, file hash facts, and reproduction steps separately from raw payload data.

## Suggested Trace IDs

- `TRACE-0001-handshake`
- `TRACE-0002-login-failed`
- `TRACE-0003-login-success`
- `TRACE-0004-character-list`
- `TRACE-0005-character-select`
- `TRACE-0006-enter-map`
- `TRACE-0007-movement`
