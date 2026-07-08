using PlServer.Application;
using PlServer.Diagnostics;
using PlServer.Protocol;

namespace PlServer.Network;

public sealed record ReceivedPacketContext(
    ClientConnectionContext Connection,
    byte[] RawBytes,
    PacketDecodeResult DecodeResult,
    ActionRouteResult RouteResult,
    ProtocolTraceEvent TraceEvent);
