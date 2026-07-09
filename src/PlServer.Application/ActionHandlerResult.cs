namespace PlServer.Application;

public sealed class ActionHandlerResult
{
    public ActionHandlerResult(
        string handlerName,
        ActionHandlerStatus status,
        IReadOnlyList<string>? notes = null,
        IReadOnlyList<byte[]>? responsePackets = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(handlerName);
        HandlerName = handlerName;
        Status = status;
        Notes = notes ?? Array.Empty<string>();
        ResponsePackets = responsePackets ?? Array.Empty<byte[]>();
    }

    public string HandlerName { get; }

    public ActionHandlerStatus Status { get; }

    public IReadOnlyList<string> Notes { get; }

    public IReadOnlyList<byte[]> ResponsePackets { get; }

    public bool HasResponsePackets => ResponsePackets.Count > 0;
}
