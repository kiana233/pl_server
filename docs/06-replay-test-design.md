# 06 Replay Test Design

## Purpose

Replay tests prevent symptom-based fixes from breaking earlier flows.

## Target Replay Cases

- `Replay-0001`: handshake
- `Replay-0002`: login failed
- `Replay-0003`: login success
- `Replay-0004`: character list
- `Replay-0005`: character select
- `Replay-0006`: enter map
- `Replay-0007`: movement
- `Replay-0008`: disconnect and reconnect

## Rules

- Use sanitized traces only.
- Prefer minimal frames over full private captures.
- Each replay case must state expected session transitions.
- Each replay case must avoid real credentials and private user data.
