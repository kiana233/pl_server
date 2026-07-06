# Current State

Date: 2026-07-06

## Current Phase

Phase 1 / .NET Project Skeleton

## Repository Paths

- Main repository: `D:\Wonderland\pl_server`
- Target client, read-only: `D:\Wonderland\client`
- Reference server, read-only: `D:\pl\server`

## Current GitHub Visible State

- Remote: `origin https://github.com/kiana233/pl_server.git`
- Task branch: `task/0010-create-dotnet-project-skeleton`
- `git pull origin main` failed during branch preparation with `OpenSSL SSL_read: Connection was reset, errno 10054`; local `main` was already aligned with `origin/main` before the pull attempt.

## Current Local Scan State

- TASK-0010 created a real .NET solution and project skeleton.
- `src/PlServer.sln` contains the new source and test projects.
- Existing placeholder `.gitkeep` files remain harmless placeholders where they already existed.

## Created Source Projects

- `src/PlServer.Core`
- `src/PlServer.Application`
- `src/PlServer.Protocol`
- `src/PlServer.LegacyProtocol`
- `src/PlServer.Network`
- `src/PlServer.Session`
- `src/PlServer.Resources`
- `src/PlServer.Persistence`
- `src/PlServer.Diagnostics`
- `src/PlServer.Replay`
- `src/PlServer.Gui`
- `src/PlServer.Host`

## Created Test Projects

- `tests/PlServer.Core.Tests`
- `tests/PlServer.Application.Tests`
- `tests/PlServer.Protocol.Tests`
- `tests/PlServer.LegacyProtocol.Tests`
- `tests/PlServer.Network.Tests`
- `tests/PlServer.Session.Tests`
- `tests/PlServer.Resources.Tests`
- `tests/PlServer.Diagnostics.Tests`
- `tests/PlServer.Replay.Tests`

## Implemented Content

- SDK-style project files exist for the source and test projects.
- Class library and console projects target `net8.0`.
- GUI project targets `net8.0-windows` with `UseWPF=true`.
- Each source project has a minimal assembly marker or minimal host/WPF shell.
- Each test project has one xUnit smoke test.

## Not Implemented

- `PacketCodec` is not implemented.
- TCP host/frame splitter is not implemented.
- XOR codec is not implemented.
- Packet logger is not implemented.
- Replay framework behavior is not implemented beyond the project skeleton.
- Session state machine is not implemented.
- AC0, AC63, login, character selection, enter-map, movement, inventory, equipment, NPC, quests, warp, and battle are not implemented.
- GUI management functionality is not implemented beyond a minimal WPF shell title.

## Current Blockers

- Real target-client packet traces are not yet available in this repository, so protocol behavior cannot be marked `confirmed`.
- TASK-0010 is intentionally skeleton-only; protocol and runtime behavior require follow-up tasks.

## Next Suggested Task

TASK-0011-implement-packet-codec
