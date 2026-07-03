# Reference Server Analysis

This file summarizes reference-server conclusions without copying implementation code.

## Known References

- `wlophoenix/Wonderland-Private-Server`
- `muayad-mahmoud/Wonderland-Online-Private-Server`

## Initial Direction

`wlophoenix/Wonderland-Private-Server` is currently treated as the more useful structural and protocol reference, especially for packet framing, AC0, AC63, AC06, and core domain model boundaries.

The project must not blindly copy either repository. Reference code may guide analysis, but the final behavior must be verified against the real target client through sanitized traces.

## Initial Reference Facts

- Packet framing: `F4 44` / `0x44F4`, length, payload, AC/SubAC.
- AC0: connection initialization, version response, function flags.
- AC63: login, character slots, character selection.
- AC06: movement direction and coordinate update.
- Core model areas: Player, Character, Inventory, Map, Battle.

## Risk

Reference servers may be incomplete, outdated, incompatible with the target client, or incorrect for edge cases.
