namespace PlServer.Session;

public sealed class SessionStateMachine
{
    private readonly SessionStateGuard _guard;

    public SessionStateMachine()
        : this(new SessionStateGuard())
    {
    }

    public SessionStateMachine(SessionStateGuard guard)
    {
        _guard = guard;
        CurrentState = SessionState.Connected;
    }

    public SessionState CurrentState { get; private set; }

    public SessionTransitionResult Apply(SessionPacketClassification classification)
    {
        ArgumentNullException.ThrowIfNull(classification);

        var previousState = CurrentState;
        var guardResult = _guard.Validate(previousState, classification);
        var errors = new List<SessionTransitionError>();

        if (classification.Kind == SessionPacketKind.InvalidPacket)
        {
            errors.Add(new SessionTransitionError(
                SessionTransitionErrorCode.InvalidPacket,
                "Invalid packet was recorded and session state did not change."));

            return BuildResult(previousState, previousState, classification, false, errors);
        }

        if (classification.Kind == SessionPacketKind.Unknown)
        {
            errors.Add(new SessionTransitionError(
                SessionTransitionErrorCode.UnknownPacket,
                "Unknown packet was recorded and session state did not change."));

            return BuildResult(previousState, previousState, classification, true, errors);
        }

        if (!guardResult.Allowed)
        {
            errors.Add(new SessionTransitionError(
                classification.Kind == SessionPacketKind.MovementCandidate
                    ? SessionTransitionErrorCode.MovementBeforeInMap
                    : SessionTransitionErrorCode.PacketRejected,
                guardResult.Reason));

            return BuildResult(previousState, previousState, classification, false, errors);
        }

        var nextState = GetNextState(previousState, classification.Kind);
        CurrentState = nextState;

        return BuildResult(previousState, nextState, classification, true, errors);
    }

    public void Reset()
    {
        CurrentState = SessionState.Connected;
    }

    private static SessionState GetNextState(SessionState state, SessionPacketKind kind)
    {
        if (kind == SessionPacketKind.DisconnectCandidate)
        {
            return SessionState.Disconnected;
        }

        return (state, kind) switch
        {
            (SessionState.Connected, SessionPacketKind.HandshakeCandidate) => SessionState.HandshakeDone,
            (SessionState.HandshakeDone, SessionPacketKind.LoginRequestCandidate) => SessionState.LoginPending,
            (SessionState.LoginPending, SessionPacketKind.LoginAcceptedCandidate) => SessionState.LoginAccepted,
            (SessionState.LoginAccepted, SessionPacketKind.CharacterListCandidate) => SessionState.CharacterListShown,
            (SessionState.CharacterListShown, SessionPacketKind.CharacterSelectCandidate) => SessionState.CharacterSelected,
            (SessionState.CharacterSelected, SessionPacketKind.EnterMapCandidate) => SessionState.EnteringMap,
            (SessionState.EnteringMap, SessionPacketKind.InMapReadyCandidate) => SessionState.InMap,
            (SessionState.InMap, SessionPacketKind.MovementCandidate) => SessionState.InMap,
            _ => state
        };
    }

    private static SessionTransitionResult BuildResult(
        SessionState previousState,
        SessionState currentState,
        SessionPacketClassification classification,
        bool allowed,
        IReadOnlyList<SessionTransitionError> errors)
    {
        return new SessionTransitionResult(previousState, currentState, classification, allowed, errors);
    }
}
