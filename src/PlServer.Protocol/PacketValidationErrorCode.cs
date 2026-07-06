namespace PlServer.Protocol
{
    public enum PacketValidationErrorCode
    {
        NullOrEmptyInput,
        FrameTooShort,
        InvalidHeader,
        DeclaredLengthLargerThanAvailable,
        DeclaredLengthSmallerThanExpected,
        PayloadLengthMismatch,
        MissingAc
    }
}
