using PlServer.LegacyProtocol;

namespace PlServer.LegacyProtocol.Tests;

public sealed class LoginResponseContractCandidateRegistryTests
{
    [Fact]
    public void Registry_seeds_login_success_candidate()
    {
        var registry = LoginResponseContractCandidateRegistry.CreateDefault();

        var found = registry.TryResolveByKind(
            LoginResponseKind.LoginSuccessCandidate,
            out var candidate);

        Assert.True(found);
        Assert.NotNull(candidate);
        Assert.Equal("LoginSuccessCandidate", candidate.Name);
    }

    [Fact]
    public void Registry_seeds_login_failure_candidate()
    {
        var registry = LoginResponseContractCandidateRegistry.CreateDefault();

        var found = registry.TryResolveByKind(
            LoginResponseKind.LoginFailureCandidate,
            out var candidate);

        Assert.True(found);
        Assert.NotNull(candidate);
        Assert.Equal("LoginFailureCandidate", candidate.Name);
    }

    [Fact]
    public void Registry_seeds_character_list_follows_candidate()
    {
        var registry = LoginResponseContractCandidateRegistry.CreateDefault();

        var found = registry.TryResolveByKind(
            LoginResponseKind.CharacterListFollowsCandidate,
            out var candidate);

        Assert.True(found);
        Assert.NotNull(candidate);
        Assert.Equal("CharacterListFollowsCandidate", candidate.Name);
    }

    [Fact]
    public void All_seeded_candidates_are_server_to_client()
    {
        var registry = LoginResponseContractCandidateRegistry.CreateDefault();

        Assert.All(
            registry.GetAll(),
            candidate => Assert.Equal(LegacyPacketDirection.S2C, candidate.Direction));
    }

    [Fact]
    public void All_seeded_candidates_are_pending_or_unknown_not_confirmed()
    {
        var registry = LoginResponseContractCandidateRegistry.CreateDefault();

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
        var registry = new LoginResponseContractCandidateRegistry();
        var candidate = CreateCandidate("DuplicateCandidate");
        registry.Register(candidate);

        Assert.Throws<InvalidOperationException>(() => registry.Register(candidate));
    }

    [Fact]
    public void Unknown_candidate_lookup_does_not_throw()
    {
        var registry = LoginResponseContractCandidateRegistry.CreateDefault();

        var foundByName = registry.TryResolveByName("MissingCandidate", out var byName);
        var foundByKind = registry.TryResolveByKind(LoginResponseKind.Unknown, out var byKind);

        Assert.False(foundByName);
        Assert.Null(byName);
        Assert.False(foundByKind);
        Assert.Null(byKind);
    }

    [Fact]
    public void Field_descriptors_can_be_stored_without_bytes_generation()
    {
        var descriptor = new LoginResponseFieldDescriptor(
            "LayoutUnknownField",
            null,
            null,
            "bytes",
            "Candidate field only; no bytes generated.",
            ProtocolSourceLabel.Assumption,
            ProtocolEvidenceStatus.PendingTargetClientTrace);
        var candidate = new LoginResponseContractCandidate(
            "CandidateWithField",
            LoginResponseKind.LoginFailureCandidate,
            LegacyPacketDirection.S2C,
            ProtocolSourceLabel.Assumption,
            ProtocolEvidenceStatus.PendingTargetClientTrace,
            LoginResponseContractStatus.LayoutUnknown,
            fieldDescriptors: new[] { descriptor });
        var plan = new LoginResponseContractPlan(
            LoginResponseContractPlanStatus.CandidateOnlyNoPacketGenerated,
            candidate);

        Assert.Single(candidate.FieldDescriptors);
        Assert.False(plan.ShouldGeneratePacket);
        Assert.Empty(plan.GeneratedBytes);
        Assert.False(plan.IsConfirmed);
    }

    private static LoginResponseContractCandidate CreateCandidate(string name)
    {
        return new LoginResponseContractCandidate(
            name,
            LoginResponseKind.LoginFailureCandidate,
            LegacyPacketDirection.S2C,
            ProtocolSourceLabel.Assumption,
            ProtocolEvidenceStatus.PendingTargetClientTrace,
            LoginResponseContractStatus.LayoutUnknown);
    }
}
