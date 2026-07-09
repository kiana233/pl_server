namespace PlServer.LegacyProtocol;

public enum CharacterListContractPlanStatus
{
    Unknown = 0,
    CannotPlanWithoutConfirmedLoginResponse = 1,
    CannotPlanWithoutCharacterRepository = 2,
    CharacterRepositoryNotImplemented = 3,
    CandidateOnlyNoPacketGenerated = 4,
    Rejected = 5
}
