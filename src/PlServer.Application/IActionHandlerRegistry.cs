using PlServer.LegacyProtocol;

namespace PlServer.Application;

public interface IActionHandlerRegistry
{
    void Register(ActionHandlerDescriptor descriptor);

    bool TryResolve(LegacyProtocolContract contract, out ActionHandlerDescriptor? descriptor);
}
