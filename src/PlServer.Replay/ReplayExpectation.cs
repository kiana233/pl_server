namespace PlServer.Replay;

public sealed record ReplayExpectation(
    byte? ExpectedAc,
    byte? ExpectedSubAc);
