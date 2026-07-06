namespace PlServer.Replay;

public sealed class ReplayCase
{
    public ReplayCase(IReadOnlyList<ReplayStep> steps)
    {
        Steps = steps;
    }

    public IReadOnlyList<ReplayStep> Steps { get; }
}
