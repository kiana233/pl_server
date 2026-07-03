# Protocol Source Policy

## Source Trust Order

1. `trace:client` is the highest-trust source.
2. `reference:muayad` is the primary reference server source.
3. `reference:wlophoenix` is an auxiliary reference source.
4. `assumption` is a pending hypothesis only.
5. `unknown` is not allowed to enter business implementation.

## Required Labels

Every AC/SubAC fact must have:

- source
- status
- evidence location
- risk or verification note

Allowed source values:

- `trace:client`
- `reference:muayad`
- `reference:wlophoenix`
- `assumption`
- `unknown`

Allowed status values should distinguish at least:

- `confirmed`
- `pending-target-client-trace`
- `reference-only`
- `assumption`
- `unknown`

## Confirmation Rules

Reference behavior cannot be marked `confirmed` by itself. A behavior without real target-client trace evidence must stay `pending-target-client-trace` or `reference-only`.

`unknown` facts must not drive business implementation. They may appear in reports, open questions, and test plans only.

## Implementation Gate

Before implementing a protocol handler, the task must identify the expected source label and status. If no trace exists, the implementation must explicitly preserve uncertainty and avoid pretending compatibility is proven.
