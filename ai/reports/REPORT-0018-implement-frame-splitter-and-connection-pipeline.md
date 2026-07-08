# REPORT-0018 Implement Frame Splitter and Connection Pipeline

## Task ID

TASK-0018-implement-frame-splitter-and-connection-pipeline

## Summary

Implemented TCP stream frame splitting and connection receive pipeline hardening in `PlServer.Network`.

The pipeline now supports half packets, sticky packets, multi-frame chunks, leading-noise resynchronization, invalid zero-length frame reporting, and oversized frame protection. Complete frames are routed through the existing skeleton chain only:

`PacketFrameReadBuffer -> ReceivePipeline -> PacketRoutePipeline -> PacketCodec -> ProtocolTraceLogger -> ActionRouter`

No real AC handler, login, map, movement, inventory, NPC, battle, GUI behavior, client resource access, reference server source, real trace, database, secret, token, or auto-generated gameplay response was added.

## Changed Files

- `src/PlServer.Network/PacketFrameSplitterOptions.cs`
- `src/PlServer.Network/PacketFrameSplitter.cs`
- `src/PlServer.Network/PacketFrameSplitResult.cs`
- `src/PlServer.Network/PacketFrameSplitStatus.cs`
- `src/PlServer.Network/PacketFrameReadBuffer.cs`
- `src/PlServer.Network/ReceivedFrame.cs`
- `src/PlServer.Network/FrameSplitError.cs`
- `src/PlServer.Network/FrameSplitErrorCode.cs`
- `src/PlServer.Network/ConnectionReceiveLoop.cs`
- `src/PlServer.Network/ConnectionReceiveResult.cs`
- `src/PlServer.Network/ReceivePipeline.cs`
- `src/PlServer.Network/TcpServerHost.cs`
- `tests/PlServer.Network.Tests/PacketFrameReadBufferTests.cs`
- `tests/PlServer.Network.Tests/TcpHostSkeletonTests.cs`
- `src/PlServer.Network/*` and `tests/PlServer.Network.Tests/*` from the TASK-0017 TCP Host foundation
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/tasks/TASK-0018-implement-frame-splitter-and-connection-pipeline.md`
- `ai/reports/REPORT-0018-implement-frame-splitter-and-connection-pipeline.md`

## Implemented Classes

- `PacketFrameSplitterOptions`
- `PacketFrameSplitter`
- `PacketFrameSplitResult`
- `PacketFrameSplitStatus`
- `PacketFrameReadBuffer`
- `ReceivedFrame`
- `FrameSplitError`
- `FrameSplitErrorCode`
- `ConnectionReceiveLoop`
- `ConnectionReceiveResult`

## Frame Splitter Behavior

- Uses `PacketCodecOptions` for header bytes, length offset, length size, little-endian length, and payload offset.
- Preserves incomplete headers and partial frames in `PacketFrameReadBuffer`.
- Splits sticky frames and multiple complete frames from one TCP chunk.
- Handles frames split across multiple chunks.
- Discards leading noise and resynchronizes to the next `F4 44` header.
- Emits zero-length malformed frames for PacketCodec validation while also recording a split error.
- Discards oversized frames and records a `FrameTooLarge` error.
- Does not throw on malformed bytes.

## Receive Loop Flow

1. `TcpServerHost` reads a TCP chunk from `NetworkStream`.
2. `ConnectionReceiveLoop` appends the chunk to `PacketFrameReadBuffer`.
3. `PacketFrameReadBuffer` returns complete `ReceivedFrame` instances and split errors.
4. Each complete frame is processed independently through `ReceivePipeline.ProcessFrameAsync`.
5. `PacketRoutePipeline` decodes, logs, and routes each frame.
6. Stop and disconnect paths cancel the loop cleanly.

## ActionRouter Integration

Each complete frame is routed through the existing ActionRouter skeleton.

AC63/SubAC4 reaches `LoginRequestCandidate` metadata and returns skeleton route results such as `MissingHandler`. Movement candidates in `Connected` state are rejected by SessionStateGuard.

No login, movement, gameplay, map, inventory, NPC, or battle behavior is executed.

## ProtocolTrace Integration

Each routed complete frame writes a protocol trace event through `ProtocolTraceLogger`.

Synthetic stream tests are not marked `trace:client` or `confirmed`.

## Dependency Note

The current `main` branch did not contain the TASK-0017 TCP Host skeleton files when this task branch was created. The already pushed TASK-0017 `PlServer.Network` and `PlServer.Network.Tests` foundation was brought into this task branch so TASK-0018 could build on the documented TCP Host skeleton.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main`
- `git checkout -B task/0018-implement-frame-splitter-and-connection-pipeline`
- `dotnet --info`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- line-count check
- `git status --short`
- `git diff --stat`

## Test Results

- `dotnet build .\src\PlServer.sln` succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln` succeeded.
- `PlServer.Network.Tests` passed 34 tests.

## Line Count Check

- `src/PlServer.Network/PacketFrameSplitterOptions.cs`: 26 lines
- `src/PlServer.Network/PacketFrameSplitter.cs`: 171 lines
- `src/PlServer.Network/PacketFrameSplitResult.cs`: 8 lines
- `src/PlServer.Network/PacketFrameSplitStatus.cs`: 9 lines
- `src/PlServer.Network/PacketFrameReadBuffer.cs`: 34 lines
- `src/PlServer.Network/ReceivedFrame.cs`: 8 lines
- `src/PlServer.Network/FrameSplitError.cs`: 7 lines
- `src/PlServer.Network/FrameSplitErrorCode.cs`: 9 lines
- `src/PlServer.Network/ConnectionReceiveLoop.cs`: 47 lines
- `src/PlServer.Network/ConnectionReceiveResult.cs`: 6 lines
- `src/PlServer.Network/ReceivePipeline.cs`: 31 lines
- `src/PlServer.Network/TcpServerHost.cs`: 227 lines
- `tests/PlServer.Network.Tests/PacketFrameReadBufferTests.cs`: 294 lines
- `tests/PlServer.Network.Tests/TcpHostSkeletonTests.cs`: 286 lines
- `ai/context/current-state.md`: 68 lines
- `ai/context/latest-status.md`: 34 lines
- `ai/tasks/TASK-0018-implement-frame-splitter-and-connection-pipeline.md`: 48 lines
- `ai/reports/REPORT-0018-implement-frame-splitter-and-connection-pipeline.md`: 135 lines before this section was added

## Risks

- The splitter is still based on pending `reference:muayad` framing assumptions and is not target-client confirmed.
- Session state is not yet advanced from route results.
- TCP Host still routes to skeleton results only.

## Blockers

- Real target-client packet traces are not yet available.
- Connection session update behavior requires a dedicated follow-up task.
- Real handlers require explicit future tasks and review approval.

## Next Recommended Task

TASK-0019-implement-connection-session-update-pipeline

## Branch

`task/0018-implement-frame-splitter-and-connection-pipeline`

## Commit Hash

Final commit hash will be printed in terminal output.

## Push Result

Final push result will be printed in terminal output.
