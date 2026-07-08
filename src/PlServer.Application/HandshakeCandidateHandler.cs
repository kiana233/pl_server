using PlServer.LegacyProtocol;
using PlServer.Session;

namespace PlServer.Application;

public sealed class HandshakeCandidateHandler : IActionHandler
{
    public string HandlerName => nameof(HandshakeCandidateHandler);

    public bool CanHandle(LegacyProtocolContract contract)
    {
        ArgumentNullException.ThrowIfNull(contract);
        return contract.ProtocolName == "HandshakeCandidate";
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

        var context = new ActionHandlerContext(request, contract, packetKind, sessionGuardResult);
        var handlerResult = new ActionHandlerResult(
            HandlerName,
            ActionHandlerStatus.CandidateHandled,
            new[]
            {
                "handshake candidate-only handler invoked",
                "pending target-client trace",
                "no response packet generated",
                $"payload length: {context.PayloadLength}"
            });

        return ValueTask.FromResult(ActionRouteResult.FromHandler(
            contract,
            packetKind,
            sessionGuardResult,
            request.PacketDecodeResult.ValidationErrors,
            handlerResult));
    }
}
