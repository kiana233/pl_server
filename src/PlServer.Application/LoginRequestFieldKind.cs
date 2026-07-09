namespace PlServer.Application;

public enum LoginRequestFieldKind
{
    Unknown = 0,
    Ac = 1,
    SubAc = 2,
    PayloadLength = 3,
    AccountIdentifierCandidate = 4,
    SecretCandidate = 5,
    TokenCandidate = 6,
    OpaquePayload = 7
}
