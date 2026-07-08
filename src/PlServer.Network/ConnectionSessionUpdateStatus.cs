namespace PlServer.Network;

public enum ConnectionSessionUpdateStatus
{
    Applied,
    NoChange,
    Rejected,
    InvalidPacket,
    UnknownPacket
}
