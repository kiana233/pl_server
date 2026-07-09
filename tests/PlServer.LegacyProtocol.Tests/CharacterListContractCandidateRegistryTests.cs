using PlServer.LegacyProtocol;

namespace PlServer.LegacyProtocol.Tests;

public sealed class CharacterListContractCandidateRegistryTests
{
    [Fact]
    public void Registry_seeds_character_list_follows_login_candidate()
    {
        var registry = CharacterListContractCandidateRegistry.CreateDefault();

        var found = registry.TryResolveByKind(
            CharacterListResponseKind.CharacterListFollowsLoginCandidate,
            out var candidate);

        Assert.True(found);
        Assert.NotNull(candidate);
        Assert.Equal("CharacterListFollowsLoginCandidate", candidate.Name);
    }

    [Fact]
    public void Registry_seeds_empty_character_list_candidate()
    {
        var registry = CharacterListContractCandidateRegistry.CreateDefault();

        var found = registry.TryResolveByKind(
            CharacterListResponseKind.EmptyCharacterListCandidate,
            out var candidate);

        Assert.True(found);
        Assert.NotNull(candidate);
        Assert.Equal("EmptyCharacterListCandidate", candidate.Name);
    }

    [Fact]
    public void Registry_seeds_character_list_unavailable_candidate()
    {
        var registry = CharacterListContractCandidateRegistry.CreateDefault();

        var found = registry.TryResolveByKind(
            CharacterListResponseKind.CharacterListUnavailableCandidate,
            out var candidate);

        Assert.True(found);
        Assert.NotNull(candidate);
        Assert.Equal("CharacterListUnavailableCandidate", candidate.Name);
    }

    [Fact]
    public void All_seeded_candidates_are_server_to_client()
    {
        var registry = CharacterListContractCandidateRegistry.CreateDefault();

        Assert.All(
            registry.GetAll(),
            candidate => Assert.Equal(LegacyPacketDirection.S2C, candidate.Direction));
    }

    [Fact]
    public void All_seeded_candidates_are_pending_or_unknown_not_confirmed()
    {
        var registry = CharacterListContractCandidateRegistry.CreateDefault();

        Assert.All(registry.GetAll(), candidate =>
        {
            Assert.Contains(candidate.EvidenceStatus, new[]
            {
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                ProtocolEvidenceStatus.Unknown
            });
            Assert.False(candidate.IsConfirmed);
            Assert.NotEqual(ProtocolEvidenceStatus.Confirmed, candidate.EvidenceStatus);
            Assert.NotEqual(ProtocolSourceLabel.TraceClient, candidate.SourceLabel);
        });
    }

    [Fact]
    public void Duplicate_candidate_name_is_rejected()
    {
        var registry = new CharacterListContractCandidateRegistry();
        var candidate = CreateCandidate("DuplicateCandidate");
        registry.Register(candidate);

        Assert.Throws<InvalidOperationException>(() => registry.Register(candidate));
    }

    [Fact]
    public void Unknown_candidate_lookup_does_not_throw()
    {
        var registry = CharacterListContractCandidateRegistry.CreateDefault();

        var foundByName = registry.TryResolveByName("MissingCandidate", out var byName);
        var foundByKind = registry.TryResolveByKind(CharacterListResponseKind.Unknown, out var byKind);

        Assert.False(foundByName);
        Assert.Null(byName);
        Assert.False(foundByKind);
        Assert.Null(byKind);
    }

    [Fact]
    public void Field_descriptors_can_be_stored_without_bytes_generation()
    {
        var descriptor = new CharacterListFieldDescriptor(
            "LayoutUnknownField",
            null,
            null,
            "bytes",
            "Candidate field only; no bytes generated.",
            ProtocolSourceLabel.Assumption,
            ProtocolEvidenceStatus.PendingTargetClientTrace);
        var candidate = new CharacterListContractCandidate(
            "CandidateWithField",
            CharacterListResponseKind.EmptyCharacterListCandidate,
            LegacyPacketDirection.S2C,
            ProtocolSourceLabel.Assumption,
            ProtocolEvidenceStatus.PendingTargetClientTrace,
            CharacterListContractStatus.LayoutUnknown,
            fieldDescriptors: new[] { descriptor });
        var plan = new CharacterListContractPlan(
            CharacterListContractPlanStatus.CandidateOnlyNoPacketGenerated,
            candidate);

        Assert.Single(candidate.FieldDescriptors);
        Assert.False(plan.ShouldGeneratePacket);
        Assert.Empty(plan.GeneratedBytes);
        Assert.False(plan.IsConfirmed);
    }

    [Fact]
    public void Seeded_candidate_notes_explain_layout_unknown_or_sanitized_trace()
    {
        var registry = CharacterListContractCandidateRegistry.CreateDefault();

        Assert.All(registry.GetAll(), candidate =>
        {
            var notes = string.Join(" ", candidate.Notes);

            Assert.Contains("layout unknown", notes, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("sanitized target-client trace", notes, StringComparison.OrdinalIgnoreCase);
        });
    }

    private static CharacterListContractCandidate CreateCandidate(string name)
    {
        return new CharacterListContractCandidate(
            name,
            CharacterListResponseKind.EmptyCharacterListCandidate,
            LegacyPacketDirection.S2C,
            ProtocolSourceLabel.Assumption,
            ProtocolEvidenceStatus.PendingTargetClientTrace,
            CharacterListContractStatus.LayoutUnknown);
    }
}
