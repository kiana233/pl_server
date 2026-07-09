namespace PlServer.Application;

public enum CharacterListCandidatePlanStatus
{
    Unknown = 0,
    CannotPlanWithoutConfirmedLoginResponse = 1,
    CannotPlanWithoutCharacterRepository = 2,
    CharacterRepositoryNotImplemented = 3,
    CandidateOnlyNoPacketGenerated = 4,
    Rejected = 5
}
