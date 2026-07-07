namespace PlServer.Session;

public enum SessionTransitionErrorCode
{
    InvalidPacket,
    UnknownPacket,
    PacketRejected,
    MovementBeforeInMap
}
