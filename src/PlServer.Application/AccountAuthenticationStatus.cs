namespace PlServer.Application;

public enum AccountAuthenticationStatus
{
    AccountNotFound = 0,
    AccountDisabled = 1,
    InvalidCredentials = 2,
    CandidateAuthenticated = 3
}
