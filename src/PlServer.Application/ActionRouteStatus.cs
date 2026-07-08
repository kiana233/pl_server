namespace PlServer.Application;

public enum ActionRouteStatus
{
    CandidateHandled,
    RoutedToNoOp,
    MissingContract,
    MissingHandler,
    RejectedBySessionGuard,
    InvalidPacket,
    UnknownPacket,
    NotImplemented
}
