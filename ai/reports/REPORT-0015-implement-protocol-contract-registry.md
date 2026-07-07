# REPORT-0015 Implement Protocol Contract Registry

## Task ID

TASK-0015-implement-protocol-contract-registry

## Summary

Implemented the first `PlServer.LegacyProtocol` protocol contract registry for AC/SubAC candidate metadata.

The implementation is metadata-only. It does not implement TCP Host, GUI behavior, ActionRouter, AC handlers, login, gameplay logic, client resources, reference server source, binaries, databases, secrets, tokens, or real traces.

## Changed Files

- `src/PlServer.LegacyProtocol/ProtocolSourceLabel.cs`
- `src/PlServer.LegacyProtocol/ProtocolEvidenceStatus.cs`
- `src/PlServer.LegacyProtocol/LegacyPacketDirection.cs`
- `src/PlServer.LegacyProtocol/LegacyProtocolSessionRequirement.cs`
- `src/PlServer.LegacyProtocol/LegacyProtocolKey.cs`
- `src/PlServer.LegacyProtocol/LegacyProtocolFieldDescriptor.cs`
- `src/PlServer.LegacyProtocol/LegacyProtocolContract.cs`
- `src/PlServer.LegacyProtocol/LegacyProtocolContractMatchKind.cs`
- `src/PlServer.LegacyProtocol/LegacyProtocolContractLookupResult.cs`
- `src/PlServer.LegacyProtocol/LegacyProtocolContractRegistry.cs`
- `src/PlServer.LegacyProtocol/LegacyProtocolContractCatalog.cs`
- `tests/PlServer.LegacyProtocol.Tests/LegacyProtocolContractRegistryTests.cs`
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/tasks/TASK-0015-implement-protocol-contract-registry.md`
- `ai/reports/REPORT-0015-implement-protocol-contract-registry.md`

## Implemented Classes

- `ProtocolSourceLabel`
- `ProtocolSourceLabelExtensions`
- `ProtocolEvidenceStatus`
- `LegacyPacketDirection`
- `LegacyProtocolSessionRequirement`
- `LegacyProtocolKey`
- `LegacyProtocolFieldDescriptor`
- `LegacyProtocolContract`
- `LegacyProtocolContractMatchKind`
- `LegacyProtocolContractLookupResult`
- `LegacyProtocolContractRegistry`
- `LegacyProtocolContractCatalog`

## Seeded Contracts

- AC0 / any SubAC -> `HandshakeCandidate`
- AC63/SubAC4 -> `LoginRequestCandidate`
- AC63/SubAC1 -> `CharacterListCandidate`
- AC63/SubAC2 -> `CharacterSelectCandidate`
- AC06/SubAC1 -> `MovementCandidate`
- AC12 / any SubAC -> `EnterMapCandidate`
- AC20 / any SubAC -> `EnterMapCandidate`
- Unknown fallback -> `Unknown`

## Source Labels

- `TraceClient` -> `trace:client`
- `ReferenceMuayad` -> `reference:muayad`
- `ReferenceWlophoenix` -> `reference:wlophoenix`
- `Assumption` -> `assumption`
- `Unknown` -> `unknown`

## Evidence Statuses

- Seeded `reference:muayad` contracts use `PendingTargetClientTrace`.
- AC12 and AC20 assumption contracts use `PendingTargetClientTrace`.
- Unknown fallback uses `Unknown`.
- No reference behavior is marked `Confirmed`.
- No synthetic replay behavior is treated as a real target-client trace.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main` using a one-command local proxy for GitHub reachability
- `git checkout -B task/0015-implement-protocol-contract-registry`
- `dotnet --info`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- `git status --short`
- `git diff --stat`

## Test Results

- `dotnet build .\src\PlServer.sln` succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln` succeeded.
- `PlServer.LegacyProtocol.Tests` passed 17 tests.

## Risks

- Contract metadata is candidate-only and cannot confirm protocol behavior without target-client traces.
- AC12 and AC20 remain assumptions until target-client traces confirm their role.
- Session requirement metadata does not execute session logic or enforce runtime behavior.

## Blockers

- Real target-client traces are not available in this repository.
- ActionRouter and AC handler tasks have not been created or approved.

## Next Recommended Task

`TASK-0016-implement-action-router-skeleton`

## Branch

`task/0015-implement-protocol-contract-registry`

## Commit Hash

Implementation commit: `426ea68`

Final branch tip commit is printed in terminal output after recording this push result.

## Push Result

Initial push succeeded:

```text
To https://github.com/kiana233/pl_server.git
 * [new branch]      task/0015-implement-protocol-contract-registry -> task/0015-implement-protocol-contract-registry
Branch 'task/0015-implement-protocol-contract-registry' set up to track remote branch 'task/0015-implement-protocol-contract-registry' from 'origin'.
```

## ChatGPT Review Fixup

ChatGPT reviewed the remote task branch and confirmed that the previous single-line formatting issue has been resolved for TASK-0015 source, test, and status files.

Fixes verified:

- TASK-0015 LegacyProtocol source files are readable multi-line C#.
- TASK-0015 LegacyProtocol tests are readable multi-line C#.
- `ai/context/latest-status.md` is readable multi-line Markdown.
- `ai/context/current-state.md` is readable multi-line Markdown.
- `ai/reports/REPORT-0015-implement-protocol-contract-registry.md` is readable multi-line Markdown.
- LegacyProtocolContractRegistry and seeded contract behavior were preserved.
- Protocol contracts remain metadata-only and `pending-target-client-trace`.
- No TCP Host, GUI behavior, ActionRouter, AC handlers, login, gameplay logic, client resources, reference server source, binaries, databases, secrets, or real traces were added.

## Fixup Line Count Check

Recorded line-count verification after formatting fixup:

- `src/PlServer.LegacyProtocol/ProtocolSourceLabel.cs`: 26 lines
- `src/PlServer.LegacyProtocol/ProtocolEvidenceStatus.cs`: 10 lines
- `src/PlServer.LegacyProtocol/LegacyPacketDirection.cs`: 8 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolSessionRequirement.cs`: 15 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolKey.cs`: 11 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolFieldDescriptor.cs`: 48 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolContract.cs`: 73 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolContractMatchKind.cs`: 8 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolContractLookupResult.cs`: 6 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolContractRegistry.cs`: 81 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolContractCatalog.cs`: 125 lines
- `tests/PlServer.LegacyProtocol.Tests/LegacyProtocolContractRegistryTests.cs`: 233 lines
- `ai/context/latest-status.md`: 33 lines
- `ai/context/current-state.md`: 60 lines
- `ai/reports/REPORT-0015-implement-protocol-contract-registry.md`: 125 lines before this Add-Content append

## Fixup Commands Run

- `git checkout task/0015-implement-protocol-contract-registry` - succeeded.
- `git pull origin task/0015-implement-protocol-contract-registry` - succeeded; branch was already up to date.
- `git status --short` - clean after pull.
- line-count check script - succeeded and recorded the line counts above.
- `git diff --stat` - showed only this report changed, with 53 inserted lines before this result update.
- `dotnet build .\src\PlServer.sln` - succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln` - succeeded; all solution tests passed, including 17 `PlServer.LegacyProtocol.Tests`.

## Fixup Test Results

- `dotnet build .\src\PlServer.sln` succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln` succeeded; all solution tests passed.
- `PlServer.LegacyProtocol.Tests` passed 17 tests.

## Fixup Push Result

Pending final push for this Add-Content report-only fixup; final push result will be printed in terminal output.

## ChatGPT Review Fixup

ChatGPT reviewed the remote task branch and confirmed that the previous single-line formatting issue has been resolved for TASK-0015 source, test, and status files.

Fixes verified:

- TASK-0015 LegacyProtocol source files are readable multi-line C#.
- TASK-0015 LegacyProtocol tests are readable multi-line C#.
- `ai/context/latest-status.md` is readable Markdown.
- `ai/context/current-state.md` is readable Markdown.
- LegacyProtocolContractRegistry and seeded contract behavior were preserved.
- Protocol contracts remain metadata-only and `pending-target-client-trace`.
- No TCP Host, GUI behavior, ActionRouter, AC handlers, login, gameplay logic, client resources, reference server source, binaries, databases, secrets, or real traces were added.

## Fixup Line Count Check

- `src/PlServer.LegacyProtocol/ProtocolSourceLabel.cs`: 26 lines
- `src/PlServer.LegacyProtocol/ProtocolEvidenceStatus.cs`: 10 lines
- `src/PlServer.LegacyProtocol/LegacyPacketDirection.cs`: 8 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolSessionRequirement.cs`: 15 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolKey.cs`: 11 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolFieldDescriptor.cs`: 48 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolContract.cs`: 73 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolContractMatchKind.cs`: 8 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolContractLookupResult.cs`: 6 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolContractRegistry.cs`: 81 lines
- `src/PlServer.LegacyProtocol/LegacyProtocolContractCatalog.cs`: 125 lines
- `tests/PlServer.LegacyProtocol.Tests/LegacyProtocolContractRegistryTests.cs`: 233 lines
- `ai/context/latest-status.md`: 33 lines
- `ai/context/current-state.md`: 60 lines
- `ai/reports/REPORT-0015-implement-protocol-contract-registry.md`: 180 lines

## Fixup Commands Run

- `git checkout task/0015-implement-protocol-contract-registry`
- `git pull origin task/0015-implement-protocol-contract-registry`
- `git status --short`
- line-count generation script
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- `git diff --stat`

## Fixup Test Results

- `dotnet build .\src\PlServer.sln` succeeded.
- `dotnet test .\src\PlServer.sln` succeeded.

## Fixup Push Result

Push result must be recorded after `git push` completes.
