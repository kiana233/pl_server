using PlServer.LegacyProtocol;
using PlServer.Session;

namespace PlServer.Application;

public sealed class LoginResponseCandidatePlanner
{
    private readonly LoginResponseContractCandidateRegistry registry;

    public LoginResponseCandidatePlanner()
        : this(LoginResponseContractCandidateRegistry.CreateDefault())
    {
    }

    public LoginResponseCandidatePlanner(LoginResponseContractCandidateRegistry registry)
    {
        this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public LoginResponseCandidatePlanResult Plan(
        LoginRequestParseResult parserResult,
        AccountAuthenticationResult? authenticationResult,
        SessionState currentSessionState,
        ActionHandlerResult? handlerResult = null)
    {
        ArgumentNullException.ThrowIfNull(parserResult);

        if (parserResult.Status is LoginRequestParseStatus.OpaquePayload
            or LoginRequestParseStatus.UnknownLayout
            or LoginRequestParseStatus.InvalidPacket)
        {
            return LoginResponseCandidatePlanResult.CannotPlanUnknownRequestLayout(parserResult);
        }

        if (authenticationResult is null)
        {
            return LoginResponseCandidatePlanResult.CannotPlanWithoutAuthentication();
        }

        var kind = authenticationResult.Status == AccountAuthenticationStatus.CandidateAuthenticated
            ? LoginResponseKind.LoginSuccessCandidate
            : LoginResponseKind.LoginFailureCandidate;

        if (!registry.TryResolveByKind(kind, out var candidate) || candidate is null)
        {
            return LoginResponseCandidatePlanResult.CannotPlanWithoutAuthentication();
        }

        _ = currentSessionState;
        _ = handlerResult;

        return LoginResponseCandidatePlanResult.CandidateOnlyNoPacketGenerated(candidate);
    }
}
