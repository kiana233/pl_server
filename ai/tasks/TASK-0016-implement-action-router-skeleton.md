# TASK-0016 Implement ActionRouter Skeleton

## Goal

Create an ActionRouter skeleton in `PlServer.Application` that connects decoded packet results, legacy protocol contracts, session guard checks, and handler registry lookup.

## Scope

- Add route request, route result, and route status models.
- Add action handler and handler registry abstractions.
- Add duplicate-registration protection for handler descriptors.
- Add ActionRouter orchestration for invalid packets, unknown contracts, session guard rejection, missing handlers, and no-op routing.
- Add NoOpActionHandler and MissingActionHandler skeleton classes.
- Add tests for routing outcomes, metadata preservation, no-op behavior, and non-requirement of TCP Host or GUI.
- Update repository state documents and create the TASK-0016 report.

## Non-Goals

- Do not implement TCP Host.
- Do not implement GUI behavior.
- Do not implement real AC handlers.
- Do not implement AC0, AC63, or AC06 business behavior.
- Do not implement login, character selection, enter-map, movement, inventory, equipment, NPC, quests, battle, or gameplay systems.
- Do not add client resources, reference server source, real traces, databases, binaries, secrets, or tokens.
- Do not mark `reference:muayad` behavior as confirmed.
- Do not treat synthetic replay or synthetic tests as target-client trace confirmation.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- Invalid PacketDecodeResult returns `InvalidPacket`.
- Unknown AC returns `UnknownPacket` or `MissingContract` without throwing.
- AC63/SubAC4 resolves `LoginRequestCandidate`.
- AC63/SubAC4 can route to a no-op handler.
- Missing handler returns `MissingHandler`.
- Movement in `Connected` state is rejected by SessionStateGuard.
- Movement in `InMap` state is allowed by SessionStateGuard.
- Duplicate handler registration is rejected.
- Router preserves source label and evidence status.
- Router does not mark reference behavior as confirmed.
- Router does not execute login business logic.
- Router does not require TCP Host or GUI.
- RouteResult exposes contract protocol name and Chinese behavior text.
