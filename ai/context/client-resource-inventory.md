# Client Resource Inventory

Task ID: TASK-0009-local-intake-and-architecture-v2

Scope: read-only metadata scan of `D:\Wonderland\client`. No files were copied.

## Summary

All requested target client resource names were found under `D:\Wonderland\client\data`. SHA-256 hashes were calculated successfully for the listed files.

## Files

| Expected resource | Found | Path | Size bytes | Last modified | SHA-256 |
| --- | --- | --- | ---: | --- | --- |
| `Item.dat` / `Item.Dat` | yes | `D:\Wonderland\client\data\Item.Dat` | 2169761 | 2014-03-13 17:35:14 +08:00 | `29CD4B7238934DED9BE3CCC455680C5AAAA37523A81E38901A6AC3D8B15323D9` |
| `Skill.dat` / `Skill.Dat` | yes | `D:\Wonderland\client\data\Skill.dat` | 123432 | 2014-03-13 17:35:14 +08:00 | `8C36FF35A307B63AF571A16FDA0D91945BAA3B4AB37FC556372DAF1D3922A817` |
| `Npc.dat` / `Npc.Dat` | yes | `D:\Wonderland\client\data\Npc.Dat` | 613962 | 2014-03-13 17:35:14 +08:00 | `4429A7362FEA1354EAD996535E037F4DAB61DA85A046720EAFFDEDFFC244F8B4` |
| `SceneData.Dat` | yes | `D:\Wonderland\client\data\SceneData.Dat` | 142004 | 2014-03-13 17:35:14 +08:00 | `6AC9AE871FC025565E563BC815C931EB739FCCF679E0409EA73C46DE6C80B880` |
| `eve.EMG` / `eve.Emg` | yes | `D:\Wonderland\client\data\eve.Emg` | 4926564 | 2014-03-13 17:35:14 +08:00 | `DC65F8FF1C5F8C2A98351468547FF99E91E2C892CAD10C958E604066F33AE5D9` |
| `Ground.MMG` | yes | `D:\Wonderland\client\data\Ground.MMG` | 20882690 | 2014-03-13 17:35:14 +08:00 | `DFCB32C66DAC29FDA54CC676581E72647AB192CED237EFE51C988E30F919AC70` |
| `Talk.dat` | yes | `D:\Wonderland\client\data\Talk.dat` | 4735656 | 2014-03-13 17:35:14 +08:00 | `CE26EB0FF8CA5192345387C7CB9F6477FC74B6274C1F768D313232B76EC80F1A` |
| `Compound.dat` | yes | `D:\Wonderland\client\data\Compound.dat` | 10920 | 2014-03-13 17:35:12 +08:00 | `56241D01DD1ED65473B78FEB55AF31960F759B93A64764878C493EB64A797675` |
| `Formula.dat` | yes | `D:\Wonderland\client\data\Formula.dat` | 407 | 2014-03-13 17:35:14 +08:00 | `F42940878D193E390525D759CE0586469DA6DE2EA9E95E71228DB60BF099797D` |

## Notes

- This inventory records existence and metadata only.
- These resources are compatibility targets, not source material to commit into `pl_server`.
- Protocol facts derived from these files still require source labels and should not be treated as packet-behavior confirmation.
