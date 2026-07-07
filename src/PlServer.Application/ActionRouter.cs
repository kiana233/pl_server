using PlServer.LegacyProtocol;
using PlServer.Session;

namespace PlServer.Application;

public sealed class ActionRouter
{
    private readonly LegacyProtocolContractRegistry _contractRegistry;
    private readonly IActionHandlerRegistry _handlerRegistry;
    private readonly SessionPacketClassifier _packetClassifier;
    private readonly SessionStateGuard _sessionStateGuard;

    public ActionRouter(
        LegacyProtocolContractRegistry contractRegistry,
        IActionHandlerRegistry handlerRegistry,
        SessionPacketClassifier? packetClassifier = null,
        SessionStateGuard? sessionStateGuard = null)
    {
        _contractRegistry = contractRegistry ?? throw new ArgumentNullException(nameof(contractRegistry));
        _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        _packetClassifier = packetClassifier ?? new SessionPacketClassifier();
        _sessionStateGuard = sessionStateGuard ?? new SessionStateGuard();
    }

    public async ValueTask<ActionRouteResult> RouteAsync(
        ActionRouteRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var classification = _packetClassifier.Classify(request.PacketDecodeResult);

        if (!request.PacketDecodeResult.IsValid)
        {
            return new ActionRouteResult(
                false,
                ActionRouteStatus.InvalidPacket,
                null,
                null,
                classification.Kind,
                false,
                classification.Reason,
                request.PacketDecodeResult.ValidationErrors,
                "assumption",
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                new[] { "invalid packet rejected before contract lookup" });
        }

        if (request.PacketDecodeResult.Ac is not { } ac)
        {
            return new ActionRouteResult(
                false,
                ActionRouteStatus.MissingContract,
                null,
                null,
                classification.Kind,
                false,
                "Decoded packet does not include an AC byte.",
                request.PacketDecodeResult.ValidationErrors,
                "unknown",
                ProtocolEvidenceStatus.Unknown,
                new[] { "contract lookup skipped because AC is missing" });
        }

        var contractLookup = _contractRegistry.ResolveOrUnknown(
            ac,
            request.PacketDecodeResult.SubAc,
            request.Direction);
        var contract = contractLookup.Contract;

        if (!contractLookup.Found)
        {
            return new ActionRouteResult(
                false,
                ActionRouteStatus.UnknownPacket,
                contract,
                null,
                classification.Kind,
                true,
                "No protocol contract matched the decoded AC/SubAC.",
                request.PacketDecodeResult.ValidationErrors,
                contract.SourceLabelText,
                contract.EvidenceStatus,
                contract.Notes);
        }

        var guardResult = _sessionStateGuard.Validate(request.SessionState, classification);
        if (!guardResult.Allowed)
        {
            return new ActionRouteResult(
                false,
                ActionRouteStatus.RejectedBySessionGuard,
                contract,
                null,
                classification.Kind,
                false,
                guardResult.Reason,
                request.PacketDecodeResult.ValidationErrors,
                contract.SourceLabelText,
                contract.EvidenceStatus,
                contract.Notes);
        }

        if (!_handlerRegistry.TryResolve(contract, out var descriptor) || descriptor is null)
        {
            return new ActionRouteResult(
                false,
                ActionRouteStatus.MissingHandler,
                contract,
                null,
                classification.Kind,
                true,
                "No action handler is registered for this protocol contract.",
                request.PacketDecodeResult.ValidationErrors,
                contract.SourceLabelText,
                contract.EvidenceStatus,
                contract.Notes);
        }

        if (!descriptor.Handler.CanHandle(contract))
        {
            return new ActionRouteResult(
                false,
                ActionRouteStatus.MissingHandler,
                contract,
                descriptor.HandlerName,
                classification.Kind,
                true,
                "Registered action handler cannot handle this contract.",
                request.PacketDecodeResult.ValidationErrors,
                contract.SourceLabelText,
                contract.EvidenceStatus,
                contract.Notes);
        }

        return await descriptor.Handler
            .HandleAsync(request, contract, classification.Kind, guardResult, cancellationToken)
            .ConfigureAwait(false);
    }
}
