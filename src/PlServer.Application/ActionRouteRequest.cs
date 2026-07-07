using PlServer.LegacyProtocol;
using PlServer.Protocol;
using PlServer.Session;

namespace PlServer.Application;

public sealed record ActionRouteRequest(
    string ConnectionId,
    SessionState SessionState,
    PacketDecodeResult PacketDecodeResult,
    LegacyPacketDirection Direction,
    string? AccountName = null,
    string? CharacterName = null);
