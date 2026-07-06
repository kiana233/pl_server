namespace PlServer.Session;

public sealed record SessionTransitionError(
    SessionTransitionErrorCode Code,
    string Message);
