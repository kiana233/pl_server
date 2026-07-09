# Target Client Trace Capture Guide

This guide defines the safe process for collecting target-client protocol observations for this repository.

## Purpose

Target-client traces are needed to move protocol facts from `reference:muayad`, `reference:wlophoenix`, or `assumption` to reviewed target-client evidence. This guide does not implement a capture tool and does not authorize committing raw captures.

## Pre-Capture Checklist

- Confirm the capture is for this repository's protocol research only.
- Use a controlled test account that contains no personal data.
- Use a controlled test server endpoint.
- Close unrelated applications before capturing traffic.
- Prepare a local private workspace outside the repository for raw captures.
- Confirm `.pcap`, `.pcapng`, `.har`, raw traces, private traces, databases, passwords, tokens, real IP addresses, and device identifiers will not be committed.
- Confirm the target client files under `D:\Wonderland\client` remain read-only.

## Sensitive Data That Must Not Be Captured Or Committed

- Account names unless replaced with placeholders such as `account_001`.
- Passwords, tokens, cookies, session keys, or authentication secrets.
- Real IP addresses, MAC addresses, machine names, local paths, device identifiers, or network identifiers.
- Chat text, private messages, personal notes, or other personal data.
- Raw `.pcap`, `.pcapng`, `.har`, database, binary, or private packet dump files.

## Recommended Capture Scenarios

- Start the client.
- Connect to the server.
- Capture AC0 handshake candidate traffic.
- Click login and capture AC63/SubAC4 login request candidate traffic.
- Observe character-list candidate traffic.
- Select a character and capture selection candidate traffic.
- Observe enter-map candidate traffic.
- Send the first in-map movement candidate.
- Disconnect.

## Minimum Information Per Scenario

Each sanitized event should record only the minimum protocol fields needed for replay and review:

- `timestamp`
- `direction` as `C2S` or `S2C`
- `rawHex` or `decodedHex`
- `ac`
- `subAc`
- `sessionState`
- `protocolName` or behavior candidate
- `sourceLabel`
- `evidenceStatus`
- `notes`

## Explicit Non-Requirements

- Do not record plaintext account passwords.
- Do not record real IP addresses.
- Do not commit raw packet captures.
- Do not modify the client.
- Do not mark any trace as `trace:client confirmed` until sanitization, replay, and human review are complete.
