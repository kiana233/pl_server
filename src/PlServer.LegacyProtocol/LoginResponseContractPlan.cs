namespace PlServer.LegacyProtocol;

public sealed class LoginResponseContractPlan
{
    public LoginResponseContractPlan(
        LoginResponseContractPlanStatus status,
        LoginResponseContractCandidate? candidate,
        IEnumerable<string>? notes = null)
    {
        Status = status;
        Candidate = candidate;
        Notes = notes?.Where(note => !string.IsNullOrWhiteSpace(note)).ToArray() ?? Array.Empty<string>();
    }

    public LoginResponseContractPlanStatus Status { get; }

    public LoginResponseContractCandidate? Candidate { get; }

    public bool ShouldGeneratePacket => false;

    public byte[] GeneratedBytes => Array.Empty<byte>();

    public bool IsConfirmed => false;

    public IReadOnlyList<string> Notes { get; }
}
