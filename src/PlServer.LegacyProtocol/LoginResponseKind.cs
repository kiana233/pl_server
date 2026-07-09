namespace PlServer.LegacyProtocol;

public enum LoginResponseKind
{
    Unknown = 0,
    LoginSuccessCandidate = 1,
    LoginFailureCandidate = 2,
    AccountBlockedCandidate = 3,
    ServerMaintenanceCandidate = 4,
    CharacterListFollowsCandidate = 5
}
