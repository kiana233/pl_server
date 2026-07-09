# Trace Sanitization Policy

Every trace committed to this repository must be sanitized before it enters Git.

## Required Redactions

- Delete account names or replace them with placeholders such as `account_001`.
- Delete character names or replace them with placeholders such as `character_001`.
- Delete passwords, tokens, cookies, session keys, and authentication secrets.
- Delete real IP addresses, MAC addresses, machine names, filesystem paths, device identifiers, and network identifiers.
- Delete chat content, private message content, personal notes, and any personally identifying data.
- Delete raw capture metadata that identifies the user, machine, network, or environment.

## Raw Hex And Decoded Hex Rules

If `rawHex` or `decodedHex` contains plaintext account names, passwords, tokens, chat content, or other sensitive data, use field-level redaction or do not submit the event.

Allowed placeholder examples:

- `account_001`
- `character_001`
- `[REDACTED_PASSWORD]`
- `[REDACTED_TOKEN]`
- `[REDACTED_IP]`

If it is unclear whether a value is sensitive, do not commit it.

## Commit Rules

- Every trace committed to the repository must be sanitized.
- Raw `.pcap`, `.pcapng`, `.har`, binary captures, databases, private traces, and unreviewed traces must remain outside Git.
- A sanitized trace must not use `trace:client` until a human reviewer confirms that sanitization is complete.
- A sanitized trace must not use `confirmed` until replay and evidence review are complete.

## Review Checklist

- No account password, token, cookie, or session key remains.
- No real IP address, MAC address, machine name, path, or device identifier remains.
- No chat, private message, or personal note remains.
- The trace is replayable or has a documented reason why replay is not possible.
- The report records who reviewed the trace and what protocol facts it supports.
