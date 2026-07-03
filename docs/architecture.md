# Architecture

## Project Goal

`pl_server` is the main development repository for rebuilding a stable Wonderland Online compatible server foundation. The goal is compatibility through verified protocol facts, replay tests, small implementation tasks, and clear source labels.

## Compatibility Strategy

`D:\Wonderland\client` is the final target client and must be treated as read-only. Real target-client traces are the highest-trust source for protocol behavior. Local reference servers are useful for orientation, but they are not proof that the target client behaves the same way.

All protocol facts must carry one of these source labels:

- `trace:client`
- `reference:muayad`
- `reference:wlophoenix`
- `assumption`
- `unknown`

Behavior without a real target-client trace must remain `pending-target-client-trace` and cannot be marked `confirmed`.

## Layering Principle

The old client AC/SubAC protocol belongs in a `LegacyProtocol` compatibility layer. `GameCore` and `Application` must not depend on AC/SubAC numbers. Internal behavior should use command/event models so the domain remains testable and usable by future gateways.

## Module Responsibilities

| Module | Responsibility |
| --- | --- |
| Core | Domain rules, entities, value objects, and pure game concepts without socket, database, GUI, or AC/SubAC dependencies. |
| Application | Use cases, commands, events, validation, orchestration, and GM command execution. |
| Protocol | Version-neutral protocol abstractions and shared packet contracts. |
| LegacyProtocol | Wonderland old-client AC/SubAC compatibility, decode/encode decisions, and source-labeled handlers. |
| Network | TCP connections, frame splitting, transport lifecycle, and network errors. |
| Session | Connection/session state machine and account/character/session transitions. |
| Resources | Read-only loading and validation of game resource metadata and parsed resource models. |
| Persistence | Storage abstractions and database implementations, isolated from GUI and protocol adapters. |
| Diagnostics | Structured logs, packet logs, metrics, and behavior explanations. |
| Replay | Sanitized trace fixtures, replay tests, and regression comparison tools. |
| Gui | Chinese management console that observes and commands through Application APIs only. |
| Host | Composition root for headless server deployment and optional local GUI embedding. |

## Why Not Directly Copy muayad

The muayad-style server is a valuable historical reference, but direct copying would import unverified behavior, legacy coupling, unknown bugs, possible resource/database assumptions, and legal/maintenance risk. The rebuild must preserve knowledge only as source-labeled evidence and reimplement behavior through small tested tasks.

## Why muayad Is the Primary Reference

The local reference server at `D:\pl\server` has the expected muayad-style structure: `Packet.cs`, `Server.cs`, `NetWork\ACS`, AC handlers including `AC0`, `AC06`, `AC12`, `AC20`, `AC22`, `AC23`, `AC63`, data loaders, resource managers, map models, and login initialization candidates. This makes it the main old-server reference for orientation.

## Why wlophoenix Is Auxiliary

The wlophoenix project should be used as a secondary reference for protocol skeleton and AC dispatch comparison. It must not override real target-client traces, and it must not be marked confirmed without trace evidence.

## Why Real Trace Is the Final Judge

Only sanitized packet traces from the actual target client can prove packet framing, ordering, state transitions, field meanings, and edge cases for this project. Reference implementations can suggest what to test, but they cannot confirm compatibility by themselves.

## Future Client Access

Future non-legacy clients should connect through a `ModernGateway` or another extension protocol. They should use Application commands/events rather than depending on the old AC/SubAC compatibility layer. This protects old-client compatibility while allowing cleaner modern clients.

## Headless Server Requirement

The server core must run without GUI because production and automated tests need deterministic headless operation. The GUI is a management console: it can embed a local host or later connect remotely, but it must not directly assemble packets, mutate databases, or operate sockets.
