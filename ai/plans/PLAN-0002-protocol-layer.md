# PLAN-0002: Protocol Layer

## Goal

Build the protocol layer before gameplay features.

## Target Components

- `PacketFrame`
- `PacketCodec`
- `PacketReader`
- `PacketWriter`
- `FrameSplitter`
- `XorCodec`
- `ProtocolConstants`
- `ProtocolTrace`

## Required Behavior

- Supports `F4 44` header.
- Validates length.
- Reads AC/SubAC.
- Handles TCP half packets and sticky packets.
- Can resynchronize after invalid headers.
- Supports raw and decoded logging.
- Keeps unknown protocol behavior recorded as assumptions.
