namespace PlServer.LegacyProtocol;

public enum CharacterListResponseKind
{
    Unknown = 0,
    CharacterListFollowsLoginCandidate = 1,
    EmptyCharacterListCandidate = 2,
    CharacterListUnavailableCandidate = 3,
    CharacterListLayoutUnknownCandidate = 4
}
