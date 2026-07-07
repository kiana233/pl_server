# TASK-0017 Implement TCP Host Skeleton

## Goal

Create a testable TCP Host skeleton that connects TCP receive bytes to PacketCodec, ProtocolTraceLogger, and ActionRouter skeleton routing.

## Scope

- Add TCP host options, host interface, and TCP listener lifecycle.
- Add connection IDs, connection contexts, and connection registry.
- Add receive pipeline for complete-frame byte arrays.
- Add packet route pipeline for `PacketCodec -> ProtocolTraceLogger -> ActionRouter`.
- Add send pipeline skeleton that queues outgoing bytes without generating gameplay responses.
- Update Host console entry point to default to no listener unless `--listen` is provided.
- Add tests for lifecycle, local TCP connection acceptance, registry behavior, packet decode, route outcomes, trace logging, and non-gameplay behavior.

## Non-Goals

- Do not implement real AC handlers.
- Do not implement AC0, AC63, or AC06 business behavior.
- Do not implement login, character selection, enter-map, movement, inventory, equipment, NPC, quests, battle, or gameplay systems.
- Do not implement GUI behavior.
- Do not add client resources, reference server source, real traces, databases, binaries, secrets, or tokens.
- Do not mark `reference:muayad` behavior as confirmed.
- Do not treat synthetic test traffic as target-client trace.
- Do not implement full sticky-packet or half-packet stream splitting in this task.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- TcpServerHost starts and stops on loopback port `0`.
- TcpServerHost exposes a bound endpoint after start.
- TcpServerHost accepts at least one local TCP client.
- Stop closes active connections and clears the registry.
- ConnectionRegistry registers and removes connection contexts.
- ConnectionIdGenerator returns unique IDs.
- ReceivePipeline decodes valid PacketCodec frames.
- ReceivePipeline returns invalid route results for malformed frames without throwing.
- ReceivePipeline routes AC63/SubAC4 to ActionRouter skeleton.
- MovementCandidate is rejected in Connected state through SessionStateGuard.
- PacketCodec validation errors are preserved.
- PacketRoutePipeline writes a protocol trace event.
- SendPipeline queues outgoing bytes without generating gameplay response.
- TCP Host does not implement login business logic and does not require GUI.
- Synthetic network test traffic is not marked `trace:client` or `confirmed`.
