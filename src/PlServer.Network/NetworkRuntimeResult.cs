namespace PlServer.Network;

public sealed record NetworkRuntimeResult(
    NetworkRuntimeStatus Status,
    string Message,
    Exception? Exception = null)
{
    public bool Succeeded => Exception is null
        && Status is not NetworkRuntimeStatus.Faulted;
}
