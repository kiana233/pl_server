# TASK-0015 Implement Protocol Contract Registry

## Goal

Implement a protocol contract registry in `PlServer.LegacyProtocol` for Wonderland Online legacy-client compatible AC/SubAC candidate metadata.

This task adds metadata only. It does not implement TCP Host, GUI behavior, ActionRouter, AC handlers, login business logic, map logic, inventory, equipment, NPCs, quests, or battle.

## Scope

- Add protocol source labels and display strings.
- Add evidence statuses.
- Add packet direction metadata.
- Add protocol keys with AC/SubAC/direction matching.
- Add protocol contract and field descriptor metadata.
- Add lightweight session requirement metadata.
- Add registry lookup with exact, AC-only, and unknown fallback resolution.
- Add seeded legacy protocol candidate contracts.
- Add tests for lookup behavior, duplicate rejection, source labels, evidence status, field descriptors, directions, session requirements, catalog entries, and absence of handler behavior.
- Update repository state documents and create the task report.

## Non-Goals

- Do not implement TCP Host.
- Do not implement GUI behavior.
- Do not implement ActionRouter.
- Do not implement AC handlers.
- Do not implement AC0, AC63, AC06 business behavior.
- Do not implement login, character selection, enter-map, movement, inventory, equipment, NPC, quests, battle, or gameplay systems.
- Do not add client resources, reference server source, real traces, databases, binaries, secrets, or tokens.
- Do not mark `reference:muayad` or synthetic replay behavior as confirmed.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- The registry resolves exact AC/SubAC contracts before AC-only contracts.
- The registry resolves unknown packets without throwing.
- Seeded `reference:muayad` contracts are `PendingTargetClientTrace`, not `Confirmed`.
- Contracts expose source label strings such as `reference:muayad`.
- Contracts can carry field descriptors, allowed directions, and session requirement metadata.
- No contract creates or invokes an AC handler.
- The task branch is committed and pushed as `task/0015-implement-protocol-contract-registry`.
