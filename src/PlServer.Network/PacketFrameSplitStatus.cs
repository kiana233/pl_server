namespace PlServer.Network;

public enum PacketFrameSplitStatus
{
    NoFrame,
    FramesAvailable,
    Errors,
    FramesAndErrors
}
