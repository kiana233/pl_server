# 07 Legal And Resource Boundaries

## Do Not Commit

- complete game client binaries
- copyrighted client resources with unclear rights
- full `Data`, `Map`, `Item`, `Npc`, `Skill`, or equivalent binary resources
- private server commercialization content
- accounts, passwords, tokens, or private packet data
- raw packet captures containing personal data

## Allowed

- sanitized protocol logs
- self-written protocol analysis
- server code written for this project
- mock test data
- minimized sample frames
- resource hash, size, and version records

## Resource Fact Examples

Allowed:

```text
Item.dat size = xxxx
Item.dat sha256 = xxxx
client version = xxxx
```

Not allowed:

```text
complete Item.dat
complete client exe
complete map resources
```
