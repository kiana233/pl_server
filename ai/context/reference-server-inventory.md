# Reference Server Inventory

Task ID: TASK-0009-local-intake-and-architecture-v2

Scope: read-only metadata scan of `D:\pl\server`. No files or source snippets were copied.

## Summary

The reference server has a structure consistent with the muayad-style server layout: `NetWork`, `NetWork\ACS`, `DataLoaders`, `NetWork\DataExt`, and `pServer\data` are present. It should be used as `reference:muayad` style evidence only, never as confirmed target-client behavior.

## Key Paths

| Target | Exists | Path | Role summary | Suspected muayad-style structure |
| --- | --- | --- | --- | --- |
| `NetWork/Packet.cs` | yes | `D:\pl\server\NetWork\Packet.cs` | Packet container/framing reference candidate. | yes |
| `NetWork/Server.cs` | yes | `D:\pl\server\NetWork\Server.cs` | TCP server/session send-receive reference candidate. | yes |
| `NetWork/ACS` | yes | `D:\pl\server\NetWork\ACS` | AC dispatch handler directory. | yes |
| `AC0` | yes | `D:\pl\server\NetWork\ACS\AC0.cs` | Handshake-related AC reference candidate. | yes |
| `AC06` | yes | `D:\pl\server\NetWork\ACS\AC06.cs` | Movement-related AC reference candidate. | yes |
| `AC12` | yes | `D:\pl\server\NetWork\ACS\AC12.cs` | AC handler reference candidate. | yes |
| `AC20` | yes | `D:\pl\server\NetWork\ACS\AC20.cs` | AC handler reference candidate. | yes |
| `AC22` | yes | `D:\pl\server\NetWork\ACS\AC22.cs` | AC handler reference candidate. | yes |
| `AC23` | yes | `D:\pl\server\NetWork\ACS\AC23.cs` | AC handler reference candidate. | yes |
| `AC63` | yes | `D:\pl\server\NetWork\ACS\AC63.cs` | Login-related AC reference candidate. | yes |
| `DataLoaders` | yes | `D:\pl\server\DataLoaders` | Resource loader directory. | yes |
| `ItemLoader` | yes | `D:\pl\server\DataLoaders\ItemLoader.cs` | Item data loader reference candidate. | yes |
| `Skill Manager` | yes | `D:\pl\server\DataLoaders\Skill Manager.cs` | Skill data manager reference candidate. | yes |
| `NpcManager` | yes | `D:\pl\server\DataLoaders\NpcManager.cs` | NPC data manager reference candidate. | yes |
| `SceneLoader` | yes | `D:\pl\server\DataLoaders\SceneLoader.cs` | Scene data loader reference candidate. | yes |
| `EveLoader` | yes | `D:\pl\server\DataLoaders\EveLoader.cs` | Event data loader reference candidate. | yes |
| `GroundMMg` | yes | `D:\pl\server\DataLoaders\GroundMMg.cs` | Ground/map data loader reference candidate. | yes |
| `Map.cs` | yes | `D:\pl\server\NetWork\DataExt\Map.cs` | Map data/model reference candidate. | yes |
| `pServer/data` | yes | `D:\pl\server\pServer\data` | Local data directory containing resource and database files. | yes |
| SQLite DB files | yes | `D:\pl\server\pServer\data\Pserver.db`, `D:\pl\server\pServer\data\WonderlandPServer.s3db`, backups | Reference database files only; must not be copied or committed. | yes |

## Observed Metadata

| Path | Size bytes | Last modified |
| --- | ---: | --- |
| `D:\pl\server\NetWork\Packet.cs` | 6921 | 2026-06-30 15:11:32 +08:00 |
| `D:\pl\server\NetWork\Server.cs` | 10071 | 2026-06-30 15:11:32 +08:00 |
| `D:\pl\server\NetWork\ACS\AC0.cs` | 1041 | 2023-06-15 16:25:40 +08:00 |
| `D:\pl\server\NetWork\ACS\AC06.cs` | 2170 | 2026-06-30 09:07:56 +08:00 |
| `D:\pl\server\NetWork\ACS\AC12.cs` | 1464 | 2023-06-15 16:25:40 +08:00 |
| `D:\pl\server\NetWork\ACS\AC20.cs` | 6179 | 2026-06-30 09:07:56 +08:00 |
| `D:\pl\server\NetWork\ACS\AC22.cs` | 1203 | 2023-06-15 16:25:40 +08:00 |
| `D:\pl\server\NetWork\ACS\AC23.cs` | 11425 | 2026-07-01 08:14:10 +08:00 |
| `D:\pl\server\NetWork\ACS\AC63.cs` | 26655 | 2026-07-01 08:14:40 +08:00 |
| `D:\pl\server\DataLoaders\ItemLoader.cs` | 8373 | 2026-06-30 15:10:22 +08:00 |
| `D:\pl\server\DataLoaders\Skill Manager.cs` | 7505 | 2026-06-30 16:13:11 +08:00 |
| `D:\pl\server\DataLoaders\NpcManager.cs` | 11836 | 2026-06-30 16:09:23 +08:00 |
| `D:\pl\server\DataLoaders\SceneLoader.cs` | 13034 | 2026-06-25 19:38:33 +08:00 |
| `D:\pl\server\DataLoaders\EveLoader.cs` | 30023 | 2023-06-15 16:25:40 +08:00 |
| `D:\pl\server\DataLoaders\GroundMMg.cs` | 6795 | 2026-06-26 23:04:32 +08:00 |
| `D:\pl\server\NetWork\DataExt\Map.cs` | 43413 | 2026-07-01 08:13:55 +08:00 |

## Boundary

- This inventory is not a protocol fact table.
- Any behavior inferred later from these files must be labeled `reference:muayad` or another accurate source label.
- Real target-client traces remain the highest-trust source.
