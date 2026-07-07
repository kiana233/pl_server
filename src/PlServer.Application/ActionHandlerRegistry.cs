using PlServer.LegacyProtocol;

namespace PlServer.Application;

public sealed class ActionHandlerRegistry : IActionHandlerRegistry
{
    private readonly Dictionary<LegacyProtocolKey, ActionHandlerDescriptor> _handlers = new();

    public void Register(ActionHandlerDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        if (_handlers.ContainsKey(descriptor.ContractKey))
        {
            throw new InvalidOperationException($"Duplicate action handler registration: {descriptor.ContractKey}.");
        }

        _handlers.Add(descriptor.ContractKey, descriptor);
    }

    public bool TryResolve(LegacyProtocolContract contract, out ActionHandlerDescriptor? descriptor)
    {
        ArgumentNullException.ThrowIfNull(contract);
        return _handlers.TryGetValue(contract.Key, out descriptor);
    }
}
