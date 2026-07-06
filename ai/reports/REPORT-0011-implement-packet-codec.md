# REPORT-0011: Implement Packet Codec

## Task ID

TASK-0011-implement-packet-codec

## Summary

Implemented the basic Protocol-layer PacketCodec for old-client-compatible frame encoding and decoding. This task implements only the protocol frame layer and supporting binary helpers. It does not implement AC handlers, login, TCP Host, GUI behavior, gameplay logic, client resources, reference server source, real traces, binaries, or databases.

## Changed Files

- `src/PlServer.Protocol/PacketCodecOptions.cs`
- `src/PlServer.Protocol/XorScope.cs`
- `src/PlServer.Protocol/PacketValidationErrorCode.cs`
- `src/PlServer.Protocol/PacketValidationError.cs`
- `src/PlServer.Protocol/PacketHeader.cs`
- `src/PlServer.Protocol/PacketFrame.cs`
- `src/PlServer.Protocol/PacketDecodeResult.cs`
- `src/PlServer.Protocol/PacketEncodeResult.cs`
- `src/PlServer.Protocol/XorCodec.cs`
- `src/PlServer.Protocol/PacketReader.cs`
- `src/PlServer.Protocol/PacketWriter.cs`
- `src/PlServer.Protocol/PacketCodec.cs`
- `tests/PlServer.Protocol.Tests/PacketCodecTests.cs`
- `tests/PlServer.Protocol.Tests/PacketReaderWriterTests.cs`
- `ai/context/current-state.md`
- `ai/tasks/TASK-0011-implement-packet-codec.md`
- `ai/reports/REPORT-0011-implement-packet-codec.md`

## Implemented Classes

- `PacketCodecOptions`
- `XorScope`
- `PacketValidationErrorCode`
- `PacketValidationError`
- `PacketHeader`
- `PacketFrame`
- `PacketDecodeResult`
- `PacketEncodeResult`
- `XorCodec`
- `PacketReader`
- `PacketWriter`
- `PacketCodec`

## Protocol Rules Implemented

- Default header bytes: `F4 44`.
- Default length field offset: 2.
- Default length field size: 2 bytes.
- Default length endian: little-endian.
- Default payload offset: 4.
- Length means payload length and excludes the 4-byte header/length prefix.
- AC is read from payload offset 0 when present.
- SubAC is read from payload offset 1 when present.
- Missing SubAC is allowed when payload length is 1.
- XOR key defaults to `0xAD`.
- XOR is configurable and applies to the whole frame when enabled.
- Decode returns validation errors instead of throwing for ordinary malformed protocol frames.
- PacketReader and PacketWriter support byte, UInt16 little-endian, UInt32 little-endian, byte arrays, and remaining byte count.

## Source Labels

- Header `F4 44`: `reference:muayad`
- Little-endian length at bytes 2 and 3: `reference:muayad`
- Payload offset 4 and payload length semantics: `reference:muayad`
- AC/SubAC payload offsets: `reference:muayad`
- Whole-frame XOR with key `0xAD`: `reference:muayad`, `pending-target-client-trace`
- Target-client status: pending trace verification; not confirmed.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main`
- `git checkout -B task/0011-implement-packet-codec`
- `Select-String` against `D:\pl\server\NetWork\Packet.cs`
- `Select-String` against `D:\pl\server\NetWork\Server.cs`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- `dotnet --info`

## Test Results

- `dotnet --info`: succeeded. SDK `8.0.421`; host runtime `8.0.27`.
- Initial build/test failed because nullable byte assertions in new tests used ambiguous xUnit overloads. Tests were corrected to assert `HasValue` and compare `.Value`.
- Final `dotnet build .\src\PlServer.sln`: succeeded with 0 warnings and 0 errors.
- Final `dotnet test .\src\PlServer.sln`: succeeded. `PlServer.Protocol.Tests` ran 17 tests successfully; full solution tests succeeded.

## Risks

- Packet framing rules come from `reference:muayad`, not target-client packet traces.
- Whole-frame XOR behavior is implemented according to `reference:muayad` and must be verified against sanitized target-client traces.
- The codec currently decodes a single complete frame. TCP stream splitting/reassembly is intentionally left for a future task.

## Blockers

- No sanitized target-client traces are present to mark these protocol rules confirmed.
- TCP Host, frame splitter, packet logger, replay behavior, session state machine, and AC handlers are not implemented yet.

## Next Recommended Task

TASK-0012-implement-protocol-trace-logger

## Branch

`task/0011-implement-packet-codec`

## Commit Hash

Final commit hash is printed in terminal output after commit.

## Push Result

Pending until push is attempted. Final push result is printed in terminal output because the report must be committed before the push can occur.

## ChatGPT Review Fixup

ChatGPT reviewed the remote task branch and requested a fixup before merging TASK-0011 into `main`.

Fixes made:

* Updated `ai/context/latest-status.md` so TASK-0011 is the latest completed task and TASK-0012 is the next suggested task.
* Ran `dotnet format .\src\PlServer.sln`; the TASK-0011 C# source and test files were already in normal readable multi-line C# format, so no semantic code changes were required.
* Preserved PacketCodec behavior and tests.
* No AC handlers, login, TCP Host, GUI behavior, gameplay logic, client resources, reference server source, binaries, databases, secrets, or real traces were added.

## Fixup Commands Run

* `git checkout task/0011-implement-packet-codec`: succeeded; branch was already checked out.
* `git pull origin task/0011-implement-packet-codec`: succeeded; branch was already up to date.
* `git status --short`: succeeded; initial working tree was clean.
* `Get-Content ai/context/latest-status.md`: succeeded; confirmed stale TASK-0002 status before this fixup.
* `Get-Content ai/tasks/TASK-0011-implement-packet-codec.md`: succeeded.
* `dotnet format .\src\PlServer.sln`: succeeded.
* `git restore --worktree -- src/PlServer.Gui/AssemblyInfo.cs`: succeeded; removed an out-of-scope line-ending-only change caused by `dotnet format`.
* `git status --short`: succeeded after restoring the out-of-scope GUI formatting change.
* `git diff --stat`: succeeded; only `ai/context/latest-status.md` and this report were changed before build/test.
* `dotnet build .\src\PlServer.sln`: succeeded with 0 warnings and 0 errors.
* `dotnet test .\src\PlServer.sln`: succeeded. `PlServer.Protocol.Tests` passed 17 tests; the full solution test run passed.

## Fixup Push Result

Succeeded. The fixup branch was pushed to `origin/task/0011-implement-packet-codec`.

## ChatGPT Review Fixup

ChatGPT reviewed the remote task branch and requested a fixup before merging TASK-0011 into `main`.

Fixes made:

- Updated `ai/context/latest-status.md` so TASK-0011 is the latest completed task and TASK-0012 is the next suggested task.
- Verified TASK-0011 C# source and test files are normal readable multi-line C# files and are not effectively single-line files.
- Preserved PacketCodec behavior and tests.
- Confirmed PacketCodec behavior remains `reference:muayad` and `pending-target-client-trace`.
- No AC handlers, login, TCP Host, GUI behavior, gameplay logic, client resources, reference server source, binaries, databases, secrets, or real traces were added.

## Fixup Commands Run

- `git checkout task/0011-implement-packet-codec`: succeeded; branch was already checked out.
- `git pull origin task/0011-implement-packet-codec`: failed twice because GitHub HTTPS connection to port 443 timed out after about 21 seconds. The local working tree was clean and remained intact.
- `git status --short --branch`: succeeded; branch was clean and tracking `origin/task/0011-implement-packet-codec`.
- `Get-Content ai/context/latest-status.md`: succeeded.
- `Get-Content ai/tasks/TASK-0011-implement-packet-codec.md`: succeeded.
- `dotnet format .\src\PlServer.sln`: succeeded; it did not change the TASK-0011 protocol files, which were already multi-line.
- `git restore --worktree -- src/PlServer.Gui/AssemblyInfo.cs`: succeeded; removed an out-of-scope formatting-only change produced by `dotnet format`.
- TASK-0011 C# line count check: succeeded; every checked file had more than 3 lines.
- `git status --short`: succeeded; only `ai/context/latest-status.md` and this report were modified.
- `git diff --stat`: succeeded; only `ai/context/latest-status.md` and this report were modified.
- `dotnet build .\src\PlServer.sln`: succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln`: succeeded. `PlServer.Protocol.Tests` passed 17 tests; the full solution test run passed.

## Fixup Push Result

Succeeded. Commit `09debad` was pushed to `origin/task/0011-implement-packet-codec` with:

`git push -u origin task/0011-implement-packet-codec`

This report update records the successful push result; the final branch push is also verified in terminal output.
