namespace PlServer.Network;

public sealed class ConnectionSessionUpdatePolicy
{
    public bool ApplyAllowedCandidateTransitions { get; init; } = true;

    public bool KeepUnknownPacketsConnected { get; init; } = true;

    public bool KeepInvalidPacketsConnected { get; init; } = true;
}
