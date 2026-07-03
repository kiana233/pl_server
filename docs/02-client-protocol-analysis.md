# 02 Client Protocol Analysis

## Current State

No sanitized target-client traces have been added yet.

## Initial Reference-Led Facts

- `F4 44` / `0x44F4` appears to be the packet header.
- The length field appears to describe payload length.
- Payload starts with AC and often SubAC.
- AC0 appears related to handshake/version/function flags.
- AC63 appears related to login, character list, and character selection.
- AC06 appears related to movement.

## Required Next Evidence

- Sanitized handshake trace.
- Sanitized login-failed trace.
- Sanitized login-success trace.
- Sanitized character-list trace.
- Sanitized character-select trace.
- Sanitized enter-map trace.
- Sanitized movement trace.
