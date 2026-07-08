using PlServer.Application;
using PlServer.Protocol;

namespace PlServer.Network;

public sealed record ReceivedPacketResult(
    ClientConnectionContext Connection,
    byte[] RawBytes,
    PacketDecodeResult DecodeResult,
    ActionRouteResult RouteResult,
    ConnectionSessionUpdateResult SessionUpdateResult);
