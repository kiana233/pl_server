# TASK-0010: Create .NET Project Skeleton

## Goal

Create a real buildable and testable .NET solution skeleton for `pl_server` without implementing server business behavior.

## Branch

`task/0010-create-dotnet-project-skeleton`

## Commit Message

`TASK-0010 create dotnet project skeleton`

## Scope

- Create source projects under `src`.
- Create xUnit test projects under `tests`.
- Add projects to `src/PlServer.sln`.
- Add minimal project references.
- Add assembly markers and smoke tests only.
- Update `ai/context/current-state.md`.
- Create `ai/reports/REPORT-0010-create-dotnet-project-skeleton.md`.

## Prohibited

- Do not implement `PacketCodec`.
- Do not implement TCP host/frame splitting.
- Do not implement login, character selection, enter-map, inventory, equipment, NPC, quests, battle, or other gameplay systems.
- Do not implement GUI page logic.
- Do not modify `D:\Wonderland\client`.
- Do not modify `D:\pl\server`.
- Do not copy client resources or reference server source.
- Do not add real database files, secrets, binaries, or build outputs.
- Do not mark reference behavior as confirmed without target-client trace evidence.

## Acceptance Criteria

- `src/PlServer.sln` includes all created source and test projects.
- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- Every test project has a smoke test.
- Report exists under `ai/reports`.
- Task branch is committed and pushed unless network/authentication failure prevents push.

## Next Recommended Task

TASK-0011-implement-packet-codec
