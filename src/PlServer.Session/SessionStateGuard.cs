namespace PlServer.Session;

public sealed class SessionStateGuard
{
    public SessionStateGuardResult CanAccept(SessionState state, SessionPacketKind kind)
    {
        if (kind == SessionPacketKind.InvalidPacket)
        {
            return Reject(state, kind, "Invalid packets are recorded and do not advance session state.");
        }

        if (kind == SessionPacketKind.Unknown)
        {
            return Allow(state, kind, "Unknown packets are recorded without blocking the session.");
        }

        if (kind == SessionPacketKind.DisconnectCandidate)
        {
            return Allow(state, kind, "Disconnect candidates are allowed from any state.");
        }

        if (kind == SessionPacketKind.MovementCandidate && state != SessionState.InMap)
        {
            return Reject(state, kind, "Movement candidates are only allowed after the session reaches InMap.");
        }

        return (state, kind) switch
        {
            (SessionState.Connected, SessionPacketKind.HandshakeCandidate) => Allow(state, kind, "Handshake can follow Connected."),
            (SessionState.HandshakeDone, SessionPacketKind.LoginRequestCandidate) => Allow(state, kind, "Login request can follow handshake."),
            (SessionState.LoginPending, SessionPacketKind.LoginAcceptedCandidate) => Allow(state, kind, "Login accepted can follow login pending."),
            (SessionState.LoginAccepted, SessionPacketKind.CharacterListCandidate) => Allow(state, kind, "Character list can follow login accepted."),
            (SessionState.CharacterListShown, SessionPacketKind.CharacterSelectCandidate) => Allow(state, kind, "Character select can follow character list."),
            (SessionState.CharacterSelected, SessionPacketKind.EnterMapCandidate) => Allow(state, kind, "Enter-map can follow character selection."),
            (SessionState.EnteringMap, SessionPacketKind.InMapReadyCandidate) => Allow(state, kind, "In-map ready can follow entering map."),
            (SessionState.InMap, SessionPacketKind.MovementCandidate) => Allow(state, kind, "Movement is allowed in map."),
            _ => Reject(state, kind, $"Packet kind {kind} is not valid for state {state}.")
        };
    }

    public SessionStateGuardResult Validate(SessionState state, SessionPacketClassification classification)
    {
        ArgumentNullException.ThrowIfNull(classification);
        return CanAccept(state, classification.Kind);
    }

    private static SessionStateGuardResult Allow(SessionState state, SessionPacketKind kind, string reason)
    {
        return new SessionStateGuardResult(true, reason, state, kind);
    }

    private static SessionStateGuardResult Reject(SessionState state, SessionPacketKind kind, string reason)
    {
        return new SessionStateGuardResult(false, reason, state, kind);
    }
}
