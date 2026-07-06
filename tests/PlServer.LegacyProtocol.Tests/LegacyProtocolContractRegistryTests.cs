using PlServer.LegacyProtocol;

namespace PlServer.LegacyProtocol.Tests;

public sealed class LegacyProtocolContractRegistryTests
{
    [Fact]
    public void Registry_resolves_exact_ac63_subac4_as_login_request_candidate()
    {
        var registry = LegacyProtocolContractCatalog.CreateDefaultRegistry();

        var result = registry.ResolveOrUnknown(0x63, 0x04, LegacyPacketDirection.C2S);

        Assert.True(result.Found);
        Assert.Equal(LegacyProtocolContractMatchKind.Exact, result.MatchKind);
        Assert.Equal("LoginRequestCandidate", result.Contract.ProtocolName);
    }

    [Fact]
    public void Registry_resolves_exact_ac63_subac2_as_character_select_candidate()
    {
        var registry = LegacyProtocolContractCatalog.CreateDefaultRegistry();

        var result = registry.ResolveOrUnknown(0x63, 0x02, LegacyPacketDirection.C2S);

        Assert.True(result.Found);
        Assert.Equal("CharacterSelectCandidate", result.Contract.ProtocolName);
    }

    [Fact]
    public void Registry_resolves_ac0_with_any_subac_as_handshake_candidate()
    {
        var registry = LegacyProtocolContractCatalog.CreateDefaultRegistry();

        var result = registry.ResolveOrUnknown(0x00, 0x7f, LegacyPacketDirection.C2S);

        Assert.True(result.Found);
        Assert.Equal(LegacyProtocolContractMatchKind.AcOnly, result.MatchKind);
        Assert.Equal("HandshakeCandidate", result.Contract.ProtocolName);
    }

    [Fact]
    public void Registry_resolves_ac12_with_any_subac_as_enter_map_candidate()
    {
        var registry = LegacyProtocolContractCatalog.CreateDefaultRegistry();

        var result = registry.ResolveOrUnknown(0x12, 0x33, LegacyPacketDirection.S2C);

        Assert.True(result.Found);
        Assert.Equal(LegacyProtocolContractMatchKind.AcOnly, result.MatchKind);
        Assert.Equal("EnterMapCandidate", result.Contract.ProtocolName);
    }

    [Fact]
    public void Exact_ac_subac_match_wins_over_ac_only_match()
    {
        var registry = new LegacyProtocolContractRegistry();
        registry.Register(new LegacyProtocolContract(
            new LegacyProtocolKey(0x63, null),
            "AcOnlyCandidate",
            "AC-only 候选包",
            null,
            ProtocolSourceLabel.Assumption,
            ProtocolEvidenceStatus.PendingTargetClientTrace));
        registry.Register(new LegacyProtocolContract(
            new LegacyProtocolKey(0x63, 0x04),
            "ExactCandidate",
            "精确候选包",
            null,
            ProtocolSourceLabel.ReferenceMuayad,
            ProtocolEvidenceStatus.PendingTargetClientTrace));

        var result = registry.ResolveOrUnknown(0x63, 0x04, LegacyPacketDirection.C2S);

        Assert.Equal(LegacyProtocolContractMatchKind.Exact, result.MatchKind);
        Assert.Equal("ExactCandidate", result.Contract.ProtocolName);
    }

    [Fact]
    public void Unknown_ac_resolves_to_unknown_without_throwing()
    {
        var registry = LegacyProtocolContractCatalog.CreateDefaultRegistry();

        var result = registry.ResolveOrUnknown(0xfe, 0x01, LegacyPacketDirection.C2S);

        Assert.False(result.Found);
        Assert.Equal(LegacyProtocolContractMatchKind.UnknownFallback, result.MatchKind);
        Assert.Equal("Unknown", result.Contract.ProtocolName);
    }

    [Fact]
    public void Duplicate_exact_key_registration_is_rejected()
    {
        var registry = new LegacyProtocolContractRegistry();
        var contract = new LegacyProtocolContract(
            new LegacyProtocolKey(0x63, 0x04),
            "LoginRequestCandidate",
            "玩家登录请求候选包",
            null,
            ProtocolSourceLabel.ReferenceMuayad,
            ProtocolEvidenceStatus.PendingTargetClientTrace);
        registry.Register(contract);

        Assert.Throws<InvalidOperationException>(() => registry.Register(contract));
    }

    [Fact]
    public void All_reference_muayad_contracts_are_pending_target_client_trace_not_confirmed()
    {
        var registry = LegacyProtocolContractCatalog.CreateDefaultRegistry();

        var referenceContracts = registry.GetAll()
            .Where(contract => contract.SourceLabel == ProtocolSourceLabel.ReferenceMuayad)
            .ToArray();

        Assert.NotEmpty(referenceContracts);
        Assert.All(referenceContracts, contract =>
        {
            Assert.Equal(ProtocolEvidenceStatus.PendingTargetClientTrace, contract.EvidenceStatus);
            Assert.NotEqual(ProtocolEvidenceStatus.Confirmed, contract.EvidenceStatus);
        });
    }

    [Fact]
    public void Contract_contains_chinese_behavior_text()
    {
        var registry = LegacyProtocolContractCatalog.CreateDefaultRegistry();

        var result = registry.ResolveOrUnknown(0x63, 0x04, LegacyPacketDirection.C2S);

        Assert.Contains("登录", result.Contract.ChineseBehavior);
    }

    [Fact]
    public void Contract_exposes_source_label_string_as_reference_muayad()
    {
        var registry = LegacyProtocolContractCatalog.CreateDefaultRegistry();

        var result = registry.ResolveOrUnknown(0x63, 0x04, LegacyPacketDirection.C2S);

        Assert.Equal("reference:muayad", result.Contract.SourceLabelText);
    }

    [Fact]
    public void Field_descriptors_can_be_stored_and_retrieved()
    {
        var descriptor = new LegacyProtocolFieldDescriptor(
            "AccountNameCandidate",
            0,
            null,
            "string",
            "Candidate account name field.",
            ProtocolSourceLabel.Assumption,
            ProtocolEvidenceStatus.PendingTargetClientTrace);
        var contract = new LegacyProtocolContract(
            new LegacyProtocolKey(0x63, 0x05),
            "CustomCandidate",
            "自定义候选包",
            null,
            ProtocolSourceLabel.Assumption,
            ProtocolEvidenceStatus.PendingTargetClientTrace,
            fieldDescriptors: new[] { descriptor });
        var registry = new LegacyProtocolContractRegistry();
        registry.Register(contract);

        var result = registry.ResolveOrUnknown(0x63, 0x05, LegacyPacketDirection.C2S);

        Assert.Single(result.Contract.FieldDescriptors);
        Assert.Equal("AccountNameCandidate", result.Contract.FieldDescriptors[0].FieldName);
        Assert.Equal("assumption", result.Contract.FieldDescriptors[0].SourceLabelText);
    }

    [Fact]
    public void Allowed_directions_can_include_c2s_and_s2c()
    {
        var registry = LegacyProtocolContractCatalog.CreateDefaultRegistry();

        var result = registry.ResolveOrUnknown(0x00, 0x01, LegacyPacketDirection.C2S);

        Assert.Contains(LegacyPacketDirection.C2S, result.Contract.AllowedDirections);
        Assert.Contains(LegacyPacketDirection.S2C, result.Contract.AllowedDirections);
    }

    [Fact]
    public void Required_session_state_can_be_attached_without_running_business_logic()
    {
        var registry = LegacyProtocolContractCatalog.CreateDefaultRegistry();

        var result = registry.ResolveOrUnknown(0x06, 0x01, LegacyPacketDirection.C2S);

        Assert.Equal(LegacyProtocolSessionRequirement.InMap, result.Contract.RequiredSessionState);
        Assert.Equal(LegacyProtocolSessionRequirement.InMap, result.Contract.ResultSessionState);
    }

    [Fact]
    public void Registry_get_all_returns_seeded_catalog_entries()
    {
        var registry = LegacyProtocolContractCatalog.CreateDefaultRegistry();

        var contracts = registry.GetAll();

        Assert.True(contracts.Count >= 8);
        Assert.Contains(contracts, contract => contract.ProtocolName == "HandshakeCandidate");
        Assert.Contains(contracts, contract => contract.ProtocolName == "Unknown");
    }

    [Fact]
    public void No_contract_creates_or_invokes_ac_handler()
    {
        var registry = LegacyProtocolContractCatalog.CreateDefaultRegistry();

        Assert.All(registry.GetAll(), contract =>
        {
            Assert.DoesNotContain("Handler", contract.ProtocolName, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(contract.Notes, note => note.Contains("implemented handler", StringComparison.OrdinalIgnoreCase));
        });
    }

    [Fact]
    public void No_contract_marks_synthetic_or_reference_behavior_as_trace_client_confirmed()
    {
        var registry = LegacyProtocolContractCatalog.CreateDefaultRegistry();

        Assert.All(registry.GetAll(), contract =>
        {
            if (contract.SourceLabel is ProtocolSourceLabel.ReferenceMuayad or ProtocolSourceLabel.ReferenceWlophoenix)
            {
                Assert.NotEqual(ProtocolEvidenceStatus.Confirmed, contract.EvidenceStatus);
                Assert.NotEqual(ProtocolSourceLabel.TraceClient, contract.SourceLabel);
            }
        });
    }
}
