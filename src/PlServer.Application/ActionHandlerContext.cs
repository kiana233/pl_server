using PlServer.LegacyProtocol;
using PlServer.Protocol;
using PlServer.Session;

namespace PlServer.Application;

public sealed class ActionHandlerContext
{
    public ActionHandlerContext(
        ActionRouteRequest request,
        LegacyProtocolContract contract,
        SessionPacketKind packetKind,
        SessionStateGuardResult sessionGuardResult)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
        Contract = contract ?? throw new ArgumentNullException(nameof(contract));
        PacketKind = packetKind;
        SessionGuardResult = sessionGuardResult ?? throw new ArgumentNullException(nameof(sessionGuardResult));
    }

    public ActionRouteRequest Request { get; }

    public LegacyProtocolContract Contract { get; }

    public SessionPacketKind PacketKind { get; }

    public SessionStateGuardResult SessionGuardResult { get; }

    public PacketDecodeResult PacketDecodeResult => Request.PacketDecodeResult;

    public byte? Ac => PacketDecodeResult.Ac;

    public byte? SubAc => PacketDecodeResult.SubAc;

    public int PayloadLength => PacketDecodeResult.PayloadLength;
}
