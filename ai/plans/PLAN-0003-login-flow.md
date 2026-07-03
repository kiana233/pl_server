# PLAN-0003: Login Flow

## Goal

Rebuild login only after packet framing, trace logging, and replay direction exist.

## Expected Order

1. AC0 handshake skeleton.
2. Account model.
3. AC63/4 parser.
4. Login result responses.
5. Character-slot payload.
6. Login replay test.

## Acceptance Direction

The client can enter the character-selection screen, and both empty and occupied slots display correctly.
