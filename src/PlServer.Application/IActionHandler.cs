using PlServer.LegacyProtocol;
using PlServer.Session;

namespace PlServer.Application;

public interface IActionHandler
{
    string HandlerName { get; }

    bool CanHandle(LegacyProtocolContract contract);

    ValueTask<ActionRouteResult> HandleAsync(
        ActionRouteRequest request,
        LegacyProtocolContract contract,
        SessionPacketKind packetKind,
        SessionStateGuardResult sessionGuardResult,
        CancellationToken cancellationToken = default);
}
