# AGENTS.md

## Project

This repository is for rebuilding a stable Wonderland Online compatible server foundation.

The project should be driven by verified protocol facts, replay tests, and small implementation tasks.

## Roles

ChatGPT:
- architecture planning
- protocol analysis
- task design
- acceptance criteria
- report review

Codex:
- local implementation
- tests
- reports
- Git commits
- branch updates

Human owner:
- provides client observations
- provides sanitized packet traces
- reviews risky changes
- controls final merge

## Hard Rules

1. Always read `ai/context/latest-status.md` before starting.
2. Always read the active task file under `ai/tasks/`.
3. Do not modify unrelated files.
4. Do not commit game client binaries or copyrighted client resources.
5. Do not commit secrets, tokens, account passwords, or private packet data.
6. Do not guess protocol behavior without recording the assumption.
7. Every task must produce a report under `ai/reports/`.
8. Every code change must include tests where possible.
9. Prefer small tasks and small commits.
10. Do not work directly on `main`.
11. Do not implement gameplay systems before protocol framing, logging, and replay tests exist.

## Technical Priority

Current rebuild order:

1. Repository workflow
2. Reference server analysis
3. Protocol fact table
4. PacketCodec
5. TCP frame splitter
6. XOR encode/decode
7. Packet logger
8. Replay test framework
9. Session state machine
10. AC0 handshake
11. AC63 login
12. Character list and selection
13. Minimal enter-map flow
14. AC06 movement
15. Inventory
16. Equipment
17. NPC
18. Warp
19. Battle

## Report Format

Each report must include:

- Task ID
- Summary
- Files changed
- Tests run
- Result
- Known risks
- Remaining issues
- Suggested next task
