namespace PlServer.Session;

public sealed record SessionTransitionResult(
    SessionState PreviousState,
    SessionState CurrentState,
    SessionPacketClassification Classification,
    bool Allowed,
    IReadOnlyList<SessionTransitionError> Errors)
{
    public bool StateChanged => PreviousState != CurrentState;
}
