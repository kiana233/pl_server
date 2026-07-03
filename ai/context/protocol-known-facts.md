# Protocol Known Facts

This file records protocol facts for the rebuild. Every entry must keep its source label.

## Source Labels

- `reference:wlophoenix`
- `reference:muayad`
- `trace:client`
- `assumption`

## Initial Known Facts

| Area | Fact | Source | Status |
| --- | --- | --- | --- |
| Packet header | Packet frames appear to use `F4 44` / `0x44F4`. | reference:wlophoenix | Needs client-trace confirmation |
| Length field | The length field is understood as payload length after the first 4 bytes. | reference:wlophoenix | Needs client-trace confirmation |
| Payload | Payload begins with AC and often SubAC. | reference:wlophoenix | Needs client-trace confirmation |
| AC0 | AC0 is associated with connection initialization, version, and function flags. | reference:wlophoenix | Needs client-trace confirmation |
| AC63/4 | AC63 SubAC 4 is associated with login credentials/version/login-code parsing. | reference:wlophoenix | Needs client-trace confirmation |
| AC63/2 | AC63 SubAC 2 is associated with character-slot selection. | reference:wlophoenix | Needs client-trace confirmation |
| AC06/1 | AC06 SubAC 1 is associated with movement direction and coordinates. | reference:wlophoenix | Needs client-trace confirmation |

## Unknowns Requiring Client Traces

- Exact XOR encode/decode behavior for the target client.
- Exact login request field ordering and string encoding.
- Exact character-slot payload shape for empty and occupied slots.
- Exact AC0 response sequence required by the target client.
- Exact enter-map packet sequence.
- Version/resource compatibility facts for the target client.
