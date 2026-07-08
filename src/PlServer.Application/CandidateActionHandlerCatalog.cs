using PlServer.LegacyProtocol;

namespace PlServer.Application;

public static class CandidateActionHandlerCatalog
{
    public static ActionHandlerRegistry CreateDefaultRegistry()
    {
        var registry = new ActionHandlerRegistry();
        RegisterDefaultCandidates(registry);
        return registry;
    }

    public static void RegisterDefaultCandidates(ActionHandlerRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);

        registry.Register(new ActionHandlerDescriptor(
            new LegacyProtocolKey(0x00, null),
            nameof(HandshakeCandidateHandler),
            new HandshakeCandidateHandler()));

        registry.Register(new ActionHandlerDescriptor(
            new LegacyProtocolKey(0x63, 0x04),
            nameof(LoginRequestCandidateHandler),
            new LoginRequestCandidateHandler()));
    }
}
