namespace PlServer.Application;

public enum LoginRequestFieldSensitivity
{
    Public = 0,
    AccountIdentifierCandidate = 1,
    SecretCandidate = 2,
    TokenCandidate = 3,
    UnknownSensitive = 4,
    Redacted = 5
}
