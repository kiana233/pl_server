namespace PlServer.Session;

public sealed record SessionStateGuardResult(
    bool Allowed,
    string Reason,
    SessionState CurrentState,
    SessionPacketKind PacketKind)
{
    public bool Rejected => !Allowed;
}
