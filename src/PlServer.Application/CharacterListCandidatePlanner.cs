using PlServer.LegacyProtocol;
using PlServer.Session;

namespace PlServer.Application;

public sealed class CharacterListCandidatePlanner
{
    private readonly CharacterListContractCandidateRegistry registry;

    public CharacterListCandidatePlanner()
        : this(CharacterListContractCandidateRegistry.CreateDefault())
    {
    }

    public CharacterListCandidatePlanner(CharacterListContractCandidateRegistry registry)
    {
        this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public CharacterListCandidatePlanResult Plan(
        LoginResponseCandidatePlanResult? loginResponsePlan,
        AccountAuthenticationResult? authenticationResult,
        SessionState currentSessionState,
        bool characterRepositoryAvailable = false)
    {
        _ = authenticationResult;
        _ = currentSessionState;

        if (loginResponsePlan is null)
        {
            return CharacterListCandidatePlanResult.CannotPlanWithoutConfirmedLoginResponse(loginResponsePlan);
        }

        if (!characterRepositoryAvailable)
        {
            return CharacterListCandidatePlanResult.CharacterRepositoryNotImplemented(loginResponsePlan);
        }

        if (!loginResponsePlan.IsConfirmed || !loginResponsePlan.ShouldGeneratePacket)
        {
            return CharacterListCandidatePlanResult.CannotPlanWithoutConfirmedLoginResponse(loginResponsePlan);
        }

        if (!registry.TryResolveByKind(
                CharacterListResponseKind.CharacterListFollowsLoginCandidate,
                out var candidate)
            || candidate is null)
        {
            return CharacterListCandidatePlanResult.CannotPlanWithoutConfirmedLoginResponse(loginResponsePlan);
        }

        return CharacterListCandidatePlanResult.CandidateOnlyNoPacketGenerated(candidate);
    }
}
