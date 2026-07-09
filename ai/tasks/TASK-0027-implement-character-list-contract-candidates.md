# TASK-0027 Implement Character List Contract Candidates

## Branch

`task/0027-implement-character-list-contract-candidates`

## Goal

Create a candidate-only character list contract framework without generating real character list response bytes.

## Scope

- Add character list request/response candidate contract metadata.
- Add candidate plan and planner models.
- Add registry lookup for character list response candidates.
- Add handler notes that character list response bytes are not generated.
- Add tests proving no login, character list, selection, enter-map, movement, or gameplay response bytes are generated.

## Constraints

- Do not hard guess character list response structure.
- Do not generate real character list response bytes.
- Do not generate real login response bytes.
- Do not implement a real character repository.
- Do not connect a real database.
- Do not commit real accounts, character names, passwords, tokens, cookies, session keys, traces, pcaps, or binary files.
- Do not modify `D:\Wonderland\client`.
- Do not modify `D:\pl\server`.
- Do not copy client resources or reference server source.
- Do not mark synthetic traffic, assumptions, or reference behavior as confirmed target-client facts.

## Acceptance Criteria

- Character list candidate contracts exist under `PlServer.LegacyProtocol`.
- Character list candidate planner exists under `PlServer.Application`.
- `LoginRequestCandidateHandler` records character list planning notes without response packets.
- Tests cover registry behavior, planner behavior, handler notes, no PacketWriter dependency, and synthetic trace no-response behavior.
- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.

## Next Suggested Task

`TASK-0028-implement-character-repository-skeleton`
