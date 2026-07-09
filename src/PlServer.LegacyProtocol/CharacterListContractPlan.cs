namespace PlServer.LegacyProtocol;

public sealed class CharacterListContractPlan
{
    public CharacterListContractPlan(
        CharacterListContractPlanStatus status,
        CharacterListContractCandidate? candidate,
        IEnumerable<string>? notes = null)
    {
        Status = status;
        Candidate = candidate;
        Notes = notes?.Where(note => !string.IsNullOrWhiteSpace(note)).ToArray() ?? Array.Empty<string>();
    }

    public CharacterListContractPlanStatus Status { get; }

    public CharacterListContractCandidate? Candidate { get; }

    public bool ShouldGeneratePacket => false;

    public byte[] GeneratedBytes => Array.Empty<byte>();

    public bool IsConfirmed => false;

    public IReadOnlyList<string> Notes { get; }
}
