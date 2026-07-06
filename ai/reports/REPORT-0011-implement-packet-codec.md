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
