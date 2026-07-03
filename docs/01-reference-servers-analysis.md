# 01 Reference Servers Analysis

## Purpose

This document tracks what can be learned from reference servers without blindly copying code.

## Known References

- `wlophoenix/Wonderland-Private-Server`
- `muayad-mahmoud/Wonderland-Online-Private-Server`

## Initial Assessment

`wlophoenix/Wonderland-Private-Server` is currently the stronger structural and protocol reference. Useful areas include:

- packet framing
- AC0 handshake/version/function flags
- AC63 login and character selection
- AC06 movement
- core domain model boundaries

## Rules

- Do not copy copyrighted resources.
- Do not blindly copy reference code.
- Record whether each conclusion is a reference fact, a client-trace fact, or an assumption.
- The target client's sanitized traces are the final standard.

## Next Work

TASK-0002 should expand this document with source-labeled analysis.
