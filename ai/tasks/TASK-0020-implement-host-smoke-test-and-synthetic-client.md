# TASK-0020 Implement Host Smoke Test And Synthetic Client

## Task ID

TASK-0020-implement-host-smoke-test-and-synthetic-client

## Branch

`task/0020-implement-host-smoke-test-and-synthetic-client`

## Goal

Implement Host-level smoke tests and a synthetic TCP client test utility that can verify the current server skeleton path end to end:

`Synthetic TCP client -> TcpServerHost -> ConnectionReceiveLoop -> PacketFrameReadBuffer -> ReceivePipeline -> PacketCodec -> ProtocolTraceLogger -> ActionRouter skeleton -> ConnectionSessionUpdater`

## Scope

- Create test-only synthetic TCP client utilities under `tests/PlServer.Network.Tests`.
- Verify loopback `TcpServerHost` startup on port 0.
- Verify synthetic AC0 handshake candidate processing.
- Verify synthetic AC63/SubAC4 login request candidate processing after handshake.
- Verify sticky frame and partial frame behavior through the host socket path.
- Verify malformed bytes do not crash the host.
- Verify protocol trace events are written to an in-memory sink.
- Verify synthetic traffic is not marked `trace:client` or `confirmed`.
- Verify the host does not generate login responses automatically.
- Verify no real AC handler is registered or invoked.

## Out Of Scope

- Real AC handlers.
- AC0, AC63, or AC06 business logic.
- Login, character selection, enter-map, movement, inventory, equipment, NPC, quest, or battle behavior.
- GUI behavior.
- Real target-client trace capture or confirmation.
- Client resources, reference server source, binaries, databases, secrets, or private packet data.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- New and modified C# files are readable multi-line files.
- New and modified Markdown files are readable multi-line files.
- The report records changed files, test coverage, commands, results, risks, branch, commit, and push status.
- The task branch is committed and pushed to GitHub.
