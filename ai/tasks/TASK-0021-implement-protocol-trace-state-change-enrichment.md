# TASK-0021 Implement Protocol Trace State Change Enrichment

## Task ID

TASK-0021-implement-protocol-trace-state-change-enrichment

## Branch

`task/0021-implement-protocol-trace-state-change-enrichment`

## Goal

Enrich protocol trace events so each received packet trace can include the connection-level Session state-change result calculated by `ConnectionSessionUpdater`.

## Scope

- Extend `ProtocolTraceStateChange` to include previous state, current state, packet kind, state-change flag, rejection reason, transition errors, and notes.
- Extend `ProtocolTraceEvent` JSON output to include state-change data and route status.
- Refactor `PacketRoutePipeline` and `ReceivePipeline` so trace events are written after session update enrichment.
- Add Diagnostics JSON tests for enriched state-change fields.
- Add Network host smoke tests that verify enriched trace state changes for synthetic packets.

## Out Of Scope

- Real AC handlers.
- AC0, AC63, or AC06 business behavior.
- Login, character selection, enter-map, movement, inventory, equipment, NPC, quest, or battle behavior.
- GUI behavior.
- Login, enter-map, movement, or gameplay response packets.
- Real target-client trace capture or confirmation.
- Client resources, reference server source, binaries, databases, secrets, or private packet data.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- JSON Lines trace output remains one event per line.
- Synthetic trace events are not marked `trace:client` or `confirmed`.
- No password field is added to trace JSON.
- The task branch is committed and pushed to GitHub.
