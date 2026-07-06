namespace PlServer.Replay;

public sealed class ReplayImportResult
{
    public ReplayImportResult(
        ReplayCase replayCase,
        IReadOnlyList<ReplayImportError> errors)
    {
        ReplayCase = replayCase;
        Errors = errors;
    }

    public ReplayCase ReplayCase { get; }

    public IReadOnlyList<ReplayImportError> Errors { get; }

    public bool HasErrors => Errors.Count > 0;
}
