using PlServer.LegacyProtocol;
using PlServer.Session;

namespace PlServer.Application;

public sealed class MissingActionHandler : IActionHandler
{
    public string HandlerName => "MissingActionHandler";

    public bool CanHandle(LegacyProtocolContract contract)
    {
        ArgumentNullException.ThrowIfNull(contract);
        return false;
    }

    public ValueTask<ActionRouteResult> HandleAsync(
        ActionRouteRequest request,
        LegacyProtocolContract contract,
        SessionPacketKind packetKind,
        SessionStateGuardResult sessionGuardResult,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(contract);
        ArgumentNullException.ThrowIfNull(sessionGuardResult);

        cancellationToken.ThrowIfCancellationRequested();

        var result = new ActionRouteResult(
            false,
            ActionRouteStatus.MissingHandler,
            contract,
            HandlerName,
            packetKind,
            sessionGuardResult.Allowed,
            "No action handler is registered for this protocol contract.",
            request.PacketDecodeResult.ValidationErrors,
            contract.SourceLabelText,
            contract.EvidenceStatus,
            new[] { "missing handler skeleton only" });

        return ValueTask.FromResult(result);
    }
}
