namespace PlServer.Application;

public enum LoginResponseCandidatePlanStatus
{
    Unknown = 0,
    CannotPlanUnknownRequestLayout = 1,
    CannotPlanWithoutAuthentication = 2,
    CandidateOnlyNoPacketGenerated = 3,
    Rejected = 4
}
