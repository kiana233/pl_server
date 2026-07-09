namespace PlServer.LegacyProtocol;

public enum LoginResponseContractPlanStatus
{
    Unknown = 0,
    CannotPlanUnknownRequestLayout = 1,
    CannotPlanWithoutAuthentication = 2,
    CandidateOnlyNoPacketGenerated = 3,
    Rejected = 4
}
