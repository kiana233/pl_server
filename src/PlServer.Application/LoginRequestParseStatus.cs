namespace PlServer.Application;

public enum LoginRequestParseStatus
{
    UnknownLayout = 0,
    OpaquePayload = 1,
    ParsedCandidate = 2,
    Rejected = 3,
    InvalidPacket = 4
}
