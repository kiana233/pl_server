# REPORT-0010: Create .NET Project Skeleton

## Task ID

TASK-0010-create-dotnet-project-skeleton

## Summary

Created a real .NET solution skeleton with source projects, xUnit test projects, minimal project references, assembly marker classes, a minimal console host, and a minimal WPF shell. No protocol, networking, GUI behavior, login, gameplay, client resources, reference server source, binaries, or databases were implemented or copied.

## Changed Files

- `src/PlServer.sln`
- `src/PlServer.*/*.csproj`
- `src/PlServer.*/*AssemblyMarker.cs`
- `src/PlServer.Host/Program.cs`
- `src/PlServer.Gui/App.xaml`
- `src/PlServer.Gui/App.xaml.cs`
- `src/PlServer.Gui/MainWindow.xaml`
- `src/PlServer.Gui/MainWindow.xaml.cs`
- `tests/PlServer.*.Tests/*.csproj`
- `tests/PlServer.*.Tests/SmokeTest.cs`
- `ai/context/current-state.md`
- `ai/tasks/TASK-0010-create-dotnet-project-skeleton.md`
- `ai/reports/REPORT-0010-create-dotnet-project-skeleton.md`

## Created Projects

- `PlServer.Core` (`net8.0`, class library)
- `PlServer.Application` (`net8.0`, class library)
- `PlServer.Protocol` (`net8.0`, class library)
- `PlServer.LegacyProtocol` (`net8.0`, class library)
- `PlServer.Network` (`net8.0`, class library)
- `PlServer.Session` (`net8.0`, class library)
- `PlServer.Resources` (`net8.0`, class library)
- `PlServer.Persistence` (`net8.0`, class library)
- `PlServer.Diagnostics` (`net8.0`, class library)
- `PlServer.Replay` (`net8.0`, class library)
- `PlServer.Gui` (`net8.0-windows`, WPF, `UseWPF=true`)
- `PlServer.Host` (`net8.0`, console)

## Created Test Projects

- `PlServer.Core.Tests`
- `PlServer.Application.Tests`
- `PlServer.Protocol.Tests`
- `PlServer.LegacyProtocol.Tests`
- `PlServer.Network.Tests`
- `PlServer.Session.Tests`
- `PlServer.Resources.Tests`
- `PlServer.Diagnostics.Tests`
- `PlServer.Replay.Tests`

Each test project contains one xUnit smoke test that verifies its referenced source assembly can be discovered.

## Project References

- `PlServer.Core`: none
- `PlServer.Application` -> `PlServer.Core`
- `PlServer.Protocol`: none
- `PlServer.LegacyProtocol` -> `PlServer.Protocol`, `PlServer.Core`
- `PlServer.Diagnostics` -> `PlServer.Core`
- `PlServer.Network` -> `PlServer.Protocol`, `PlServer.Diagnostics`
- `PlServer.Session` -> `PlServer.Core`, `PlServer.Protocol`
- `PlServer.Resources` -> `PlServer.Core`, `PlServer.Diagnostics`
- `PlServer.Persistence` -> `PlServer.Core`
- `PlServer.Replay` -> `PlServer.Protocol`, `PlServer.Diagnostics`
- `PlServer.Host` -> `PlServer.Application`, `PlServer.LegacyProtocol`, `PlServer.Network`, `PlServer.Session`, `PlServer.Resources`, `PlServer.Persistence`, `PlServer.Diagnostics`, `PlServer.Replay`
- `PlServer.Gui` -> `PlServer.Application`, `PlServer.Diagnostics`, `PlServer.Host`
- Each test project references its corresponding source project.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main`
- `git checkout -B task/0010-create-dotnet-project-skeleton`
- `rg --files`
- `dotnet --info`
- `dotnet new classlib ... --no-restore`
- `dotnet new console ... --no-restore`
- `dotnet new wpf ... --no-restore`
- `dotnet new xunit ... --no-restore`
- `dotnet sln .\src\PlServer.sln add ...`
- `dotnet add ... reference ...`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- `git status --short`
- `git diff --stat`
- `git status --short --ignored`

## Test Results

- `dotnet --info`: succeeded. SDK `8.0.421`; host runtime `8.0.27`.
- Initial `dotnet new wpf -f net8.0-windows`: failed because the installed WPF template does not accept `net8.0-windows` as a template `-f` option. The WPF project was created with `-f net8.0`; the generated project file correctly targets `net8.0-windows` and includes `UseWPF=true`.
- Initial parallel `dotnet test .\src\PlServer.sln`: failed with file locks because it overlapped with `dotnet build`. The checks were rerun sequentially.
- Initial sequential build found a WPF namespace collision between `PlServer.Application` and the generated `App : Application`; it was fixed by inheriting from `System.Windows.Application`.
- Final `dotnet build .\src\PlServer.sln`: succeeded with 0 warnings and 0 errors.
- Final `dotnet test .\src\PlServer.sln`: succeeded. All 9 smoke tests passed.

## Risks

- This branch was created from local `main` after `git pull origin main` hit an SSL reset. Local `main` was already aligned with `origin/main` at the time.
- TASK-0010 is skeleton-only. No protocol framing, PacketCodec, TCP host, packet logging, replay behavior, session behavior, persistence behavior, or GUI management behavior exists yet.
- WPF builds on this Windows development machine; future non-Windows CI may require Windows targeting configuration or a Windows runner.

## Blockers

- Real target-client traces are still needed before protocol behavior can be confirmed.
- No implementation task should build gameplay before protocol framing, logging, replay, and session foundations.

## Next Recommended Task

TASK-0011-implement-packet-codec

## Branch

`task/0010-create-dotnet-project-skeleton`

## Commit Hash

Final commit hash is printed in terminal output after commit.

## Push Result

Pending until push is attempted. Final push result is printed in terminal output because the report must be committed before the push can occur.
