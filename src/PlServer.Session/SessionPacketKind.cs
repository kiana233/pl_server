namespace PlServer.Session;

public enum SessionPacketKind
{
    Unknown,
    HandshakeCandidate,
    LoginRequestCandidate,
    LoginAcceptedCandidate,
    CharacterListCandidate,
    CharacterSelectCandidate,
    EnterMapCandidate,
    InMapReadyCandidate,
    MovementCandidate,
    DisconnectCandidate,
    InvalidPacket
}
