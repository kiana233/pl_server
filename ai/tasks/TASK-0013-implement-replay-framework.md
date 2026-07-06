# TASK-0013: Implement Replay Framework

## Goal

Implement offline replay infrastructure in `PlServer.Replay` for reading JSON Lines protocol traces, converting them into replay steps, decoding packets through `PacketCodec`, and reporting frame/AC/SubAC replay results.

## Branch

`task/0013-implement-replay-framework`

## Commit Message

`TASK-0013 implement replay framework`

## Scope

- Add replay direction, step, case, import error, import result, packet result, run result, and expectation models.
- Add `ReplayJsonLinesImporter` for TASK-0012 JSON Lines trace format.
- Add `ReplayRunner` for PacketCodec-based frame and AC/SubAC verification.
- Add Replay tests for import, invalid JSON, missing packet hex, hex parsing, direction preservation, PacketCodec validation errors, expected AC match/mismatch, multiple steps, summaries, synthetic replay labels, and no TCP/session dependency.
- Update current repository state and latest status documentation.

## Source Labels

- Replay imports `sourceLabel` and `status` as strings from JSONL.
- Replay does not mark `reference:muayad` as confirmed.
- Synthetic replay is not treated as `trace:client` confirmation.

## Prohibited

- Do not implement TCP Host.
- Do not implement GUI behavior.
- Do not implement SessionStateMachine.
- Do not implement ActionRouter.
- Do not implement AC0, AC63, AC06, or any AC handler.
- Do not implement login, character selection, enter-map, movement, inventory, equipment, NPC, quests, battle, or gameplay domain models.
- Do not modify `D:\Wonderland\client`.
- Do not modify `D:\pl\server`.
- Do not copy client resources or reference server source.
- Do not add real traces, secrets, binaries, databases, or build outputs.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- Replay tests cover the required import, error handling, decode, validation preservation, summary, and synthetic trace scenarios.
- Report exists under `ai/reports`.
- Task branch is committed and pushed unless network/authentication failure prevents push.

## Next Recommended Task

TASK-0014-implement-session-state-machine
