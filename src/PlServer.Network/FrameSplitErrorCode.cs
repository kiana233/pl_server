namespace PlServer.Network;

public enum FrameSplitErrorCode
{
    LeadingNoiseDiscarded,
    InvalidDeclaredLength,
    FrameTooLarge,
    MalformedBytesDiscarded
}
