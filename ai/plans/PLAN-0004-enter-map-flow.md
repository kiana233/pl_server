# PLAN-0004: Enter Map Flow

## Goal

Rebuild the minimal character-selection to map-entry path.

## Expected Order

1. AC63/2 character select.
2. Bind selected character to session.
3. Transition through `LoadingWorld`.
4. Send minimal player profile.
5. Send minimal map facts.
6. Send own character appearance.
7. Transition to `InMap`.

## Acceptance Direction

The client should not black-screen or disconnect after selecting a character, and server logs should show the session reaching `InMap`.
