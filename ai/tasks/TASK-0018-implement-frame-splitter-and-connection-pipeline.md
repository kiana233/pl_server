# TASK-0018 Implement Frame Splitter and Connection Pipeline

## Goal

Implement TCP stream frame splitting and a harder connection receive pipeline in `PlServer.Network`.

## Scope

- Add PacketFrameSplitterOptions, PacketFrameSplitter, PacketFrameReadBuffer, and split result/error models.
- Add ReceivedFrame and ConnectionReceiveLoop.
- Update ReceivePipeline with a complete-frame processing entry point.
- Update TcpServerHost receive loop to split TCP chunks into complete frames before routing.
- Preserve incomplete half packets across chunks.
- Split sticky packets and multiple frames in a single chunk.
- Resynchronize after leading noise before the configured packet header.
- Protect against oversized frames with MaxFrameSize.
- Add tests for splitter behavior, receive loop routing, trace logging, and non-gameplay send behavior.
- Update state documents and create the TASK-0018 report.

## Non-Goals

- Do not implement real AC handlers.
- Do not implement AC0, AC63, or AC06 business behavior.
- Do not implement login, character selection, enter-map, movement, inventory, equipment, NPC, quests, battle, or gameplay systems.
- Do not implement GUI behavior.
- Do not add client resources, reference server source, real traces, databases, binaries, secrets, or tokens.
- Do not mark `reference:muayad` behavior as confirmed.
- Do not treat synthetic stream traffic as target-client trace.
- Do not generate login, enter-map, movement, or gameplay responses.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- Incomplete headers and partial frames remain buffered.
- Complete frames are extracted.
- Sticky frames are split into independent frames.
- Frames split across chunks are completed after later chunks arrive.
- Leading noise is discarded and resynchronized.
- Invalid declared length and oversized frames report errors.
- Malformed bytes do not throw.
- ConnectionReceiveLoop routes each complete frame independently.
- PacketCodec validation errors are preserved.
- AC63/SubAC4 routes to ActionRouter skeleton.
- MovementCandidate in Connected state is rejected through SessionStateGuard.
- StopAsync cancels the TCP receive loop cleanly.
- Synthetic stream tests are not marked `trace:client` or `confirmed`.
- SendPipeline does not generate login responses automatically.
