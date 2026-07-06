namespace PlServer.Protocol;

public sealed record PacketFrame(
    byte[] RawBytes,
    byte[] DecodedBytes,
    PacketHeader? Header,
    ushort PayloadLength,
    byte[] Payload,
    byte? Ac,
    byte? SubAc);
