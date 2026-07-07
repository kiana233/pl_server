using PlServer.LegacyProtocol;
using PlServer.Session;

namespace PlServer.Application;

public sealed class NoOpActionHandler : IActionHandler
{
    public NoOpActionHandler(string handlerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(handlerName);
        HandlerName = handlerName;
    }

    public string HandlerName { get; }

    public bool CanHandle(LegacyProtocolContract contract)
    {
        ArgumentNullException.ThrowIfNull(contract);
        return true;
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
            true,
            ActionRouteStatus.RoutedToNoOp,
            contract,
            HandlerName,
            packetKind,
            sessionGuardResult.Allowed,
            null,
            request.PacketDecodeResult.ValidationErrors,
            contract.SourceLabelText,
            contract.EvidenceStatus,
            new[] { "handler skeleton only", "no gameplay or login business executed" });

        return ValueTask.FromResult(result);
    }
}
