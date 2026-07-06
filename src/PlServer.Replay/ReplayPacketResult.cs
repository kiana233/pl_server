using PlServer.Protocol;

namespace PlServer.Replay;

public sealed class ReplayPacketResult
{
    public int StepIndex { get; init; }

    public ReplayDirection Direction { get; init; }

    public bool IsValid { get; init; }

    public byte? Ac { get; init; }

    public byte? SubAc { get; init; }

    public byte? ExpectedAc { get; init; }

    public byte? ExpectedSubAc { get; init; }

    public bool MatchedExpectedAc { get; init; }

    public bool MatchedExpectedSubAc { get; init; }

    public IReadOnlyList<PacketValidationError> ValidationErrors { get; init; } = Array.Empty<PacketValidationError>();
}
