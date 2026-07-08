# TASK-0019 Implement Connection Session Update Pipeline

## Goal

Implement connection-level SessionState update after packet decode, route lookup, and ActionRouter skeleton routing.

## Scope

- Add connection session update policy, status, result, and updater.
- Attach SessionUpdateResult to each received packet result.
- Aggregate FinalSessionState in ConnectionReceiveResult.
- Update ReceivePipeline to apply candidate SessionStateMachine transitions.
- Keep invalid, unknown, and rejected packets inspectable without crashing or disconnecting.
- Add tests for candidate state transitions, sticky-frame sequential updates, partial-frame behavior, invalid and unknown packets, rejected movement, final session state, synthetic trace labels, no generated login response, and no real AC handler invocation.

## Non-Goals

- Do not implement real AC handlers.
- Do not implement AC0, AC63, or AC06 business behavior.
- Do not implement login, character selection, enter-map responses, movement broadcast, inventory, equipment, NPC, quests, battle, or gameplay systems.
- Do not implement GUI behavior.
- Do not add client resources, reference server source, real traces, databases, binaries, secrets, or tokens.
- Do not mark `reference:muayad` behavior as confirmed.
- Do not treat synthetic session update traffic as target-client trace.
- Do not generate login, enter-map, movement, or gameplay responses.

## Acceptance Criteria

- `dotnet build .\src\PlServer.sln` succeeds.
- `dotnet test .\src\PlServer.sln` succeeds.
- Handshake advances Connected to HandshakeDone.
- Login request advances HandshakeDone to LoginPending.
- Character list advances LoginAccepted to CharacterListShown.
- Character select advances CharacterListShown to CharacterSelected.
- Enter-map candidate advances CharacterSelected to EnteringMap.
- Explicit InMapReadyCandidate advances EnteringMap to InMap.
- Movement in InMap keeps InMap.
- Movement in Connected is rejected and does not update state.
- Invalid and unknown packets do not update state.
- Sticky frames update state sequentially.
- Partial frames do not update state until complete.
- Received packet results include SessionUpdateResult.
- ConnectionReceiveResult exposes FinalSessionState.
- Synthetic tests are not trace-client confirmed.
- No login response is generated and no real AC handler is invoked.
