# Project Inventory

Task ID: TASK-0009-local-intake-and-architecture-v2

Scope: metadata scan of `D:\Wonderland\pl_server`.

## Repository Structure

### `src`

- `src\PlServer.Game`
- `src\PlServer.Host`
- `src\PlServer.Network`
- `src\PlServer.Persistence`
- `src\PlServer.Protocol`
- `src\PlServer.sln`

### `tests`

- `tests\PlServer.Game.Tests`
- `tests\PlServer.Network.Tests`
- `tests\PlServer.Protocol.Tests`
- `tests\PlServer.Replay.Tests`

## Implementation State

| Question | Result |
| --- | --- |
| Has `.sln` | yes, `src\PlServer.sln` |
| Has root-level `.sln` | no |
| Has `.csproj` | no |
| Has `.cs` implementation | no |
| Has `PacketCodec` implementation | no |
| Has packet logger implementation | no |
| Has replay implementation | no |
| Has `SessionStateMachine` implementation | no |
| Has GUI implementation | no |
| Still placeholder project | yes |

## Notes

- Existing source/test directories currently contain `.gitkeep` placeholders.
- Existing docs and task files describe the rebuild roadmap, but no server implementation has started.
- `dotnet build` from the repository root fails because there is no root project or solution file.
- `dotnet build .\src\PlServer.sln` succeeds with a warning that no restoreable projects exist.
