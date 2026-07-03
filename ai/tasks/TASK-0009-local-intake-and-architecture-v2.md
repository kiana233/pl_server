# TASK-0009: Local Intake and Architecture v2

## Goal

Capture local repository, target client, and reference server intake facts; update architecture/workflow/policy documentation; generate a report; commit and push the task branch.

## Branch

`task/0009-local-intake-and-architecture-v2`

## Commit Message

`TASK-0009 local intake and architecture v2`

## Scope

- Scan `D:\Wonderland\client` as read-only metadata only.
- Scan `D:\pl\server` as read-only metadata only.
- Scan `D:\Wonderland\pl_server`.
- Create or update:
  - `ai/context/current-state.md`
  - `ai/context/client-resource-inventory.md`
  - `ai/context/reference-server-inventory.md`
  - `ai/context/project-inventory.md`
  - `docs/architecture.md`
  - `docs/gui-design.md`
  - `docs/development-workflow.md`
  - `docs/protocol-source-policy.md`
  - `docs/task-report-standard.md`
  - `docs/codex-auto-commit-policy.md`
  - `ai/reports/REPORT-0009-local-intake-and-architecture-v2.md`

## Prohibited

- Do not modify `D:\Wonderland\client`.
- Do not modify `D:\pl\server`.
- Do not copy client resource files into the repository.
- Do not copy reference server source into the repository.
- Do not implement `PacketCodec`.
- Do not implement TCP host, login, character selection, enter-map, inventory, equipment, NPC, quest, battle, or GUI code.
- Do not commit secrets, tokens, account passwords, private packet data, binaries, build outputs, or real databases.
- Do not mark `reference:muayad` or `reference:wlophoenix` behavior as `confirmed`.

## Acceptance Criteria

- Required context inventory files exist.
- Required architecture/workflow/policy docs exist.
- Report exists under `ai/reports/`.
- Task branch is created.
- Task is committed and pushed unless authentication/network failure prevents push.
- No client or reference server files are modified.
- No server business code or GUI code is implemented.

## Next Recommended Task

TASK-0010-create-dotnet-project-skeleton
