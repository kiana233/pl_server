namespace PlServer.Protocol
{

    public sealed record PacketValidationError(
        PacketValidationErrorCode Code,
        string Message);

}
