# 03 Server Rebuild Roadmap

## Rebuild Order

1. Repository workflow
2. Reference server analysis
3. Protocol fact table
4. Packet codec
5. TCP frame splitter
6. XOR encode/decode
7. Packet logger
8. Replay test framework
9. Session state machine
10. AC0 handshake
11. AC63 login
12. Character list and selection
13. Minimal enter-map flow
14. AC06 movement
15. Inventory
16. Equipment
17. NPC
18. Warp
19. Battle

## Key Rule

Do not implement battle, quests, pets, or broader gameplay before protocol framing, replay tests, login, enter-map, and movement are stable.
