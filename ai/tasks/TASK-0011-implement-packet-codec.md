# TASK-0011: Implement Packet Codec

## Goal

Implement the basic Wonderland Online old-client-compatible protocol frame codec in `PlServer.Protocol`.

## Branch

`task/0011-implement-packet-codec`

## Commit Message

`TASK-0011 implement packet codec`

## Scope

- Implement configurable packet frame encode/decode.
- Implement packet validation result objects.
- Implement whole-frame XOR helper.
- Implement minimal little-endian packet reader and writer.
- Add protocol tests covering frame encoding, decoding, validation, XOR, and reader/writer behavior.
- Update `ai/context/current-state.md`.
- Create `ai/reports/REPORT-0011-implement-packet-codec.md`.

## Source Labels

- Packet frame rules: `reference:muayad`
- XOR whole-frame behavior: `reference:muayad`, `pending-target-client-trace`
- No behavior in this task is marked `trace:client confirmed`.

## Prohibited

- Do not implement AC0, AC63, AC06, or any AC handler.
- Do not implement login, character selection, enter-map, movement, inventory, equipment, NPC, quests, battle, or gameplay domain models.
- Do not implement TCP Host.
- Do not implement GUI behavior.
- Do not modify `D:\Wonderland\client`.
- Do not modify `D:\pl\server`.
- Do not copy client resources or reference server source.
- Do not add real traces, secrets, binaries, databases, or build outputs.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- Tests cover the required encode/decode, validation, XOR, options, reader, and writer scenarios.
- Report exists under `ai/reports`.
- Task branch is committed and pushed unless network/authentication failure prevents push.

## Next Recommended Task

TASK-0012-implement-protocol-trace-logger
