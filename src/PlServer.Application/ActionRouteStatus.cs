namespace PlServer.Application;

public enum ActionRouteStatus
{
    RoutedToNoOp,
    MissingContract,
    MissingHandler,
    RejectedBySessionGuard,
    InvalidPacket,
    UnknownPacket,
    NotImplemented
}
