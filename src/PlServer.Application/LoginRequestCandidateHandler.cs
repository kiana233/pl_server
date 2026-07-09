using PlServer.LegacyProtocol;
using PlServer.Session;

namespace PlServer.Application;

public sealed class LoginRequestCandidateHandler : IActionHandler
{
    public string HandlerName => nameof(LoginRequestCandidateHandler);

    public bool CanHandle(LegacyProtocolContract contract)
    {
        ArgumentNullException.ThrowIfNull(contract);
        return contract.ProtocolName == "LoginRequestCandidate";
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
                "login request candidate-only handler invoked",
                "pending target-client trace",
                "payload kept opaque; no account authentication performed",
                "account repository not invoked because login payload fields are opaque",
                "no login response packet generated",
                "no character list generated",
                $"ac: {context.Ac}",
                $"subAc: {context.SubAc}",
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
