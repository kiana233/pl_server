using PlServer.Application;
using PlServer.Protocol;
using PlServer.Session;

namespace PlServer.Network;

public sealed class ConnectionSessionUpdater
{
    private readonly SessionPacketClassifier _classifier;
    private readonly SessionStateGuard _guard;
    private readonly ConnectionSessionUpdatePolicy _policy;

    public ConnectionSessionUpdater(
        SessionPacketClassifier? classifier = null,
        SessionStateGuard? guard = null,
        ConnectionSessionUpdatePolicy? policy = null)
    {
        _classifier = classifier ?? new SessionPacketClassifier();
        _guard = guard ?? new SessionStateGuard();
        _policy = policy ?? new ConnectionSessionUpdatePolicy();
    }

    public ConnectionSessionUpdateResult Update(
        ClientConnectionContext connection,
        PacketDecodeResult decodeResult,
        ActionRouteResult routeResult,
        SessionPacketClassification? classification = null)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(decodeResult);
        ArgumentNullException.ThrowIfNull(routeResult);

        var effectiveClassification = classification ?? _classifier.Classify(decodeResult);
        var previousState = connection.CurrentSessionState;
        var stateMachine = new SessionStateMachine(previousState, _guard);
        var transition = stateMachine.Apply(effectiveClassification);

        var currentState = previousState;
        var notes = new List<string>
        {
            "candidate session update only",
            "synthetic packet classification is not target-client trace confirmation"
        };

        if (transition.Allowed
            && _policy.ApplyAllowedCandidateTransitions
            && effectiveClassification.Kind is not SessionPacketKind.Unknown
            && effectiveClassification.Kind is not SessionPacketKind.InvalidPacket)
        {
            currentState = transition.CurrentState;
            connection.CurrentSessionState = currentState;
        }

        if (effectiveClassification.Kind == SessionPacketKind.Unknown && _policy.KeepUnknownPacketsConnected)
        {
            notes.Add("unknown packet did not update connection session state");
        }

        if (effectiveClassification.Kind == SessionPacketKind.InvalidPacket && _policy.KeepInvalidPacketsConnected)
        {
            notes.Add("invalid packet did not update connection session state");
        }

        var rejectionReason = transition.Allowed
            ? null
            : transition.Errors.FirstOrDefault()?.Message ?? routeResult.RejectionReason;
        var status = GetStatus(effectiveClassification.Kind, transition.Allowed, previousState != currentState);

        return new ConnectionSessionUpdateResult(
            previousState,
            currentState,
            effectiveClassification.Kind,
            status,
            previousState != currentState,
            rejectionReason,
            transition.Errors,
            notes);
    }

    private static ConnectionSessionUpdateStatus GetStatus(
        SessionPacketKind packetKind,
        bool allowed,
        bool wasStateChanged)
    {
        if (packetKind == SessionPacketKind.InvalidPacket)
        {
            return ConnectionSessionUpdateStatus.InvalidPacket;
        }

        if (packetKind == SessionPacketKind.Unknown)
        {
            return ConnectionSessionUpdateStatus.UnknownPacket;
        }

        if (!allowed)
        {
            return ConnectionSessionUpdateStatus.Rejected;
        }

        return wasStateChanged
            ? ConnectionSessionUpdateStatus.Applied
            : ConnectionSessionUpdateStatus.NoChange;
    }
}
