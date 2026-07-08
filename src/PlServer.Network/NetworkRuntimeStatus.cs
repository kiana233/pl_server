namespace PlServer.Network;

public enum NetworkRuntimeStatus
{
    Started,
    Stopped,
    PacketRouted,
    InvalidPacket,
    UnknownPacket,
    RejectedBySessionGuard,
    SendQueued,
    Cancelled,
    Faulted
}
