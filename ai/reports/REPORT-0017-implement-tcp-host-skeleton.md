# REPORT-0017 Implement TCP Host Skeleton

## Task ID

TASK-0017-implement-tcp-host-skeleton

## Summary

Implemented a TCP Host skeleton in `PlServer.Network` and a minimal opt-in console entry point in `PlServer.Host`.

The TCP Host skeleton supports local listening, connection lifecycle tracking, complete-frame receive processing, PacketCodec decode, ProtocolTraceLogger entry logging, ActionRouter skeleton routing, and a send queue skeleton.

No real AC handler, login, map, movement, inventory, NPC, battle, GUI behavior, client resource access, reference server source, real trace, database, secret, or token was added.

## Changed Files

- `src/PlServer.Network/TcpServerOptions.cs`
- `src/PlServer.Network/TcpServerHost.cs`
- `src/PlServer.Network/ITcpServerHost.cs`
- `src/PlServer.Network/ClientConnection.cs`
- `src/PlServer.Network/ClientConnectionContext.cs`
- `src/PlServer.Network/ConnectionIdGenerator.cs`
- `src/PlServer.Network/IConnectionRegistry.cs`
- `src/PlServer.Network/ConnectionRegistry.cs`
- `src/PlServer.Network/ReceivePipeline.cs`
- `src/PlServer.Network/SendPipeline.cs`
- `src/PlServer.Network/ReceivedPacketContext.cs`
- `src/PlServer.Network/PacketRoutePipeline.cs`
- `src/PlServer.Network/NetworkRuntimeResult.cs`
- `src/PlServer.Network/NetworkRuntimeStatus.cs`
- `src/PlServer.Network/NullProtocolTraceSink.cs`
- `src/PlServer.Network/PlServer.Network.csproj`
- `src/PlServer.Host/Program.cs`
- `tests/PlServer.Network.Tests/TcpHostSkeletonTests.cs`
- `tests/PlServer.Network.Tests/PlServer.Network.Tests.csproj`
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/tasks/TASK-0017-implement-tcp-host-skeleton.md`
- `ai/reports/REPORT-0017-implement-tcp-host-skeleton.md`

## Implemented Classes

- `TcpServerOptions`
- `TcpServerHost`
- `ITcpServerHost`
- `ClientConnection`
- `ClientConnectionContext`
- `ConnectionIdGenerator`
- `IConnectionRegistry`
- `ConnectionRegistry`
- `ReceivePipeline`
- `SendPipeline`
- `ReceivedPacketContext`
- `PacketRoutePipeline`
- `NetworkRuntimeResult`
- `NetworkRuntimeStatus`
- `NullProtocolTraceSink`

## TCP Lifecycle

- `TcpServerHost.StartAsync` starts a `TcpListener` on the configured local host and port.
- Port `0` is supported for test-time ephemeral binding.
- `BoundEndPoint` is populated after start.
- Accepted clients receive a unique connection ID and a `ClientConnectionContext`.
- Active connections are registered in `ConnectionRegistry`.
- `StopAsync` cancels the accept loop, stops the listener, closes connected clients, and removes registry entries.
- Normal cancellation and listener disposal are treated as expected shutdown paths.

## Receive Pipeline Flow

1. `ReceivePipeline` accepts a `byte[]` that represents one complete frame.
2. `PacketRoutePipeline` decodes the frame through `PacketCodec`.
3. The decode result is written through `ProtocolTraceLogger`.
4. An `ActionRouteRequest` is created from the connection context and decode result.
5. `ActionRouter` returns skeleton route results such as `InvalidPacket`, `UnknownPacket`, `RejectedBySessionGuard`, or `MissingHandler`.

Full sticky-packet and half-packet stream splitting is intentionally not implemented in this task.

## ActionRouter Integration

`PacketRoutePipeline` calls the TASK-0016 `ActionRouter` with the current connection session state, account name, character name, decoded packet, and C2S direction.

The TCP Host does not register real AC handlers and therefore does not execute login or gameplay behavior.

## ProtocolTrace Integration

`PacketRoutePipeline` logs every decoded receive attempt through `ProtocolTraceLogger`.

Tests use an in-memory sink. The default host pipeline uses `NullProtocolTraceSink` unless `TraceOutputPath` is configured.

Synthetic network test traffic remains synthetic and is not marked `trace:client` or `confirmed`.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main`
- `git checkout -B task/0017-implement-tcp-host-skeleton`
- `dotnet --info`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- line-count check
- `git status --short`
- `git diff --stat`

## Test Results

- `dotnet build .\src\PlServer.sln` succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln` succeeded.
- `PlServer.Network.Tests` passed 17 tests.

## Line Count Check

- `src/PlServer.Network/TcpServerOptions.cs`: 20 lines
- `src/PlServer.Network/TcpServerHost.cs`: 220 lines
- `src/PlServer.Network/ITcpServerHost.cs`: 14 lines
- `src/PlServer.Network/ClientConnection.cs`: 52 lines
- `src/PlServer.Network/ClientConnectionContext.cs`: 30 lines
- `src/PlServer.Network/ConnectionIdGenerator.cs`: 12 lines
- `src/PlServer.Network/IConnectionRegistry.cs`: 12 lines
- `src/PlServer.Network/ConnectionRegistry.cs`: 52 lines
- `src/PlServer.Network/ReceivePipeline.cs`: 21 lines
- `src/PlServer.Network/SendPipeline.cs`: 37 lines
- `src/PlServer.Network/ReceivedPacketContext.cs`: 10 lines
- `src/PlServer.Network/PacketRoutePipeline.cs`: 59 lines
- `src/PlServer.Network/NetworkRuntimeResult.cs`: 10 lines
- `src/PlServer.Network/NetworkRuntimeStatus.cs`: 14 lines
- `src/PlServer.Network/NullProtocolTraceSink.cs`: 19 lines
- `src/PlServer.Host/Program.cs`: 63 lines
- `tests/PlServer.Network.Tests/TcpHostSkeletonTests.cs`: 286 lines
- `ai/context/current-state.md`: 69 lines
- `ai/context/latest-status.md`: 34 lines
- `ai/tasks/TASK-0017-implement-tcp-host-skeleton.md`: 46 lines
- `ai/reports/REPORT-0017-implement-tcp-host-skeleton.md`: 139 lines before this section was added

## Risks

- ReceivePipeline currently treats each input byte array as one complete frame.
- Full stream-safe frame splitting is required before real client compatibility testing.
- TCP Host uses skeleton route results and does not update session state from route results.
- No real AC handler behavior exists.

## Blockers

- Real target-client traces are not yet available.
- Sticky-packet and half-packet frame splitting needs a dedicated next task.
- Real handler tasks require explicit review and scope approval.

## Next Recommended Task

TASK-0018-implement-frame-splitter-and-connection-pipeline

## Branch

`task/0017-implement-tcp-host-skeleton`

## Commit Hash

Final commit hash will be printed in terminal output.

## Push Result

Final push result will be printed in terminal output.
