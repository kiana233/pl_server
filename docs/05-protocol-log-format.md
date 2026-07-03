# 05 Protocol Log Format

Protocol logs must be safe to commit only after sanitization.

## Fields

```text
time
session_id
direction C2S/S2C
state_before
raw_hex
decoded_hex
ac
sub_ac
length
handler
state_after
result
```

## Example Shape

```text
2026-07-02 22:01:10.120
session=001
dir=C2S
state=Connected
raw=...
decoded=F4 44 03 00 00 ...
ac=0
sub=null
handler=AC0
next_state=HandshakeDone
result=ok
```

## Sanitization

Remove accounts, passwords, tokens, machine identifiers, private IPs where necessary, and any personally identifying values before committing traces.
