# Target Client Trace Requirements

Date: 2026-07-09

## Purpose

This file lists the target-client traces needed to continue protocol work safely. Current synthetic tests and reference-server observations are useful, but they do not confirm target-client behavior.

## Current Trace Status

- No real target-client trace is committed to this repository.
- Synthetic host smoke tests are not target-client trace.
- Reference behavior remains `pending-target-client-trace`.
- Any future target-client trace must be sanitized before it can enter Git.

## P0 Trace Needs

- First C2S packet after client connection.
- S2C handshake response, if one exists.
- C2S packet after clicking the login button.
- Login failure and login success candidate traffic, only if safely sanitized.
- C2S/S2C traffic around character-list display.
- C2S packet after selecting a character.
- S2C packets before and after entering map.
- First C2S movement packet inside the map.

## P1 Trace Needs

- Disconnect and reconnect.
- Map transition.
- NPC conversation open.
- Backpack open.
- Item move.
- Battle entry.

## Minimum Event Fields

- `timestamp`
- `direction`
- `connectionId`
- `sessionState`
- `rawHex` or `decodedHex`
- `ac`
- `subAc`
- `protocolName`
- `behavior`
- `sourceLabel`
- `evidenceStatus`
- `validationErrors`
- `stateChange`
- `notes`

## Safety Requirements

- Do not include plaintext account names, passwords, tokens, cookies, or session keys.
- Do not include real IP addresses, MAC addresses, machine names, filesystem paths, or device identifiers.
- Do not include chat text, private messages, personal notes, or other personal data.
- Do not commit raw `.pcap`, `.pcapng`, `.har`, database, or binary capture files.
- Do not mark anything `trace:client confirmed` without sanitization, replay, and human review.

## Suggested Next Use

Use these requirements to design TASK-0024 or later trace capture review work before implementing real login response or gameplay behavior.
