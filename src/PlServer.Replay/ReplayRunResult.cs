namespace PlServer.Replay;

public sealed class ReplayRunResult
{
    public ReplayRunResult(
        IReadOnlyList<ReplayPacketResult> results,
        IReadOnlyList<ReplayImportError> importErrors)
    {
        Results = results;
        ImportErrors = importErrors;
    }

    public int TotalSteps => Results.Count;

    public int PassedSteps => Results.Count(result => result.IsValid
        && result.MatchedExpectedAc
        && result.MatchedExpectedSubAc);

    public int FailedSteps => TotalSteps - PassedSteps + ImportErrors.Count;

    public IReadOnlyList<ReplayPacketResult> Results { get; }

    public IReadOnlyList<ReplayImportError> ImportErrors { get; }
}
