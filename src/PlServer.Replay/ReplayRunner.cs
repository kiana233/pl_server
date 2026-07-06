using PlServer.Protocol;

namespace PlServer.Replay;

public sealed class ReplayRunner
{
    private readonly PacketCodec _packetCodec;

    public ReplayRunner(PacketCodec? packetCodec = null)
    {
        _packetCodec = packetCodec ?? new PacketCodec();
    }

    public ReplayRunResult Run(ReplayImportResult importResult)
    {
        ArgumentNullException.ThrowIfNull(importResult);
        return Run(importResult.ReplayCase, importResult.Errors);
    }

    public ReplayRunResult Run(
        ReplayCase replayCase,
        IReadOnlyList<ReplayImportError>? importErrors = null)
    {
        ArgumentNullException.ThrowIfNull(replayCase);

        var results = replayCase.Steps.Select(RunStep).ToArray();
        return new ReplayRunResult(results, importErrors ?? Array.Empty<ReplayImportError>());
    }

    private ReplayPacketResult RunStep(ReplayStep step)
    {
        var bytes = step.DecodedBytes.Length > 0
            ? step.DecodedBytes
            : step.RawBytes;

        var decodeResult = _packetCodec.Decode(bytes);
        return new ReplayPacketResult
        {
            StepIndex = step.StepIndex,
            Direction = step.Direction,
            IsValid = decodeResult.IsValid,
            Ac = decodeResult.Ac,
            SubAc = decodeResult.SubAc,
            ExpectedAc = step.ExpectedAc,
            ExpectedSubAc = step.ExpectedSubAc,
            MatchedExpectedAc = !step.ExpectedAc.HasValue || decodeResult.Ac == step.ExpectedAc,
            MatchedExpectedSubAc = !step.ExpectedSubAc.HasValue || decodeResult.SubAc == step.ExpectedSubAc,
            ValidationErrors = decodeResult.ValidationErrors
        };
    }
}
