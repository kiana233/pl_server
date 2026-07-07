using PlServer.LegacyProtocol;

namespace PlServer.Application;

public sealed class ActionHandlerDescriptor
{
    public ActionHandlerDescriptor(
        LegacyProtocolKey contractKey,
        string handlerName,
        IActionHandler handler)
    {
        ContractKey = contractKey ?? throw new ArgumentNullException(nameof(contractKey));
        ArgumentException.ThrowIfNullOrWhiteSpace(handlerName);
        HandlerName = handlerName;
        Handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public LegacyProtocolKey ContractKey { get; }

    public string HandlerName { get; }

    public IActionHandler Handler { get; }
}
