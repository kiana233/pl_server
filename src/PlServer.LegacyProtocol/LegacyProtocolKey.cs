namespace PlServer.LegacyProtocol;

public sealed record LegacyProtocolKey(
    byte? Ac,
    byte? SubAc,
    LegacyPacketDirection Direction = LegacyPacketDirection.Any)
{
    public static LegacyProtocolKey Unknown { get; } = new(null, null, LegacyPacketDirection.Any);

    public bool IsUnknownFallback => Ac is null;
}
