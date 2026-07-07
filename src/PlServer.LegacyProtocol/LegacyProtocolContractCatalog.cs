namespace PlServer.LegacyProtocol;

public static class LegacyProtocolContractCatalog
{
    public static LegacyProtocolContractRegistry CreateDefaultRegistry()
    {
        var registry = new LegacyProtocolContractRegistry();

        foreach (var contract in CreateDefaultContracts())
        {
            registry.Register(contract);
        }

        return registry;
    }

    public static IReadOnlyList<LegacyProtocolContract> CreateDefaultContracts()
    {
        return new[]
        {
            new LegacyProtocolContract(
                new LegacyProtocolKey(0x00, null),
                "HandshakeCandidate",
                "握手候选包",
                "Handshake candidate packet.",
                ProtocolSourceLabel.ReferenceMuayad,
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                resultSessionState: LegacyProtocolSessionRequirement.HandshakeDone,
                allowedDirections: new[] { LegacyPacketDirection.C2S, LegacyPacketDirection.S2C },
                notes: new[] { "Candidate metadata only; no handshake handler is implemented." }),

            new LegacyProtocolContract(
                new LegacyProtocolKey(0x63, 0x04),
                "LoginRequestCandidate",
                "玩家登录请求候选包",
                "Player login request candidate packet.",
                ProtocolSourceLabel.ReferenceMuayad,
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                requiredSessionState: LegacyProtocolSessionRequirement.HandshakeDone,
                resultSessionState: LegacyProtocolSessionRequirement.LoginPending,
                allowedDirections: new[] { LegacyPacketDirection.C2S },
                fieldDescriptors: new[]
                {
                    new LegacyProtocolFieldDescriptor(
                        "LoginPayloadCandidate",
                        null,
                        null,
                        "bytes",
                        "Opaque login payload candidate; offsets are pending target-client trace.",
                        ProtocolSourceLabel.ReferenceMuayad,
                        ProtocolEvidenceStatus.PendingTargetClientTrace)
                },
                notes: new[] { "Candidate metadata only; no login handler is implemented." }),

            new LegacyProtocolContract(
                new LegacyProtocolKey(0x63, 0x01),
                "CharacterListCandidate",
                "角色列表候选包",
                "Character list candidate packet.",
                ProtocolSourceLabel.ReferenceMuayad,
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                requiredSessionState: LegacyProtocolSessionRequirement.LoginAccepted,
                resultSessionState: LegacyProtocolSessionRequirement.CharacterListShown,
                allowedDirections: new[] { LegacyPacketDirection.S2C }),

            new LegacyProtocolContract(
                new LegacyProtocolKey(0x63, 0x02),
                "CharacterSelectCandidate",
                "角色选择候选包",
                "Character select candidate packet.",
                ProtocolSourceLabel.ReferenceMuayad,
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                requiredSessionState: LegacyProtocolSessionRequirement.CharacterListShown,
                resultSessionState: LegacyProtocolSessionRequirement.CharacterSelected,
                allowedDirections: new[] { LegacyPacketDirection.C2S }),

            new LegacyProtocolContract(
                new LegacyProtocolKey(0x06, 0x01),
                "MovementCandidate",
                "地图移动候选包",
                "Map movement candidate packet.",
                ProtocolSourceLabel.ReferenceMuayad,
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                requiredSessionState: LegacyProtocolSessionRequirement.InMap,
                resultSessionState: LegacyProtocolSessionRequirement.InMap,
                allowedDirections: new[] { LegacyPacketDirection.C2S }),

            new LegacyProtocolContract(
                new LegacyProtocolKey(0x12, null),
                "EnterMapCandidate",
                "进入地图候选包",
                "Enter-map candidate packet.",
                ProtocolSourceLabel.Assumption,
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                requiredSessionState: LegacyProtocolSessionRequirement.CharacterSelected,
                resultSessionState: LegacyProtocolSessionRequirement.EnteringMap,
                allowedDirections: new[] { LegacyPacketDirection.S2C },
                notes: new[] { "AC12 contract is an assumption until target-client trace confirms it." }),

            new LegacyProtocolContract(
                new LegacyProtocolKey(0x20, null),
                "EnterMapCandidate",
                "进入地图或地图状态候选包",
                "Enter-map or map-state candidate packet.",
                ProtocolSourceLabel.Assumption,
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                requiredSessionState: LegacyProtocolSessionRequirement.CharacterSelected,
                resultSessionState: LegacyProtocolSessionRequirement.EnteringMap,
                allowedDirections: new[] { LegacyPacketDirection.S2C },
                notes: new[] { "AC20 contract is an assumption until target-client trace confirms it." }),

            UnknownFallbackContract
        };
    }

    public static LegacyProtocolContract UnknownFallbackContract { get; } = new(
        LegacyProtocolKey.Unknown,
        "Unknown",
        "未识别协议包",
        "Unknown legacy protocol packet.",
        ProtocolSourceLabel.Unknown,
        ProtocolEvidenceStatus.Unknown,
        allowedDirections: new[] { LegacyPacketDirection.Any },
        notes: new[] { "Fallback metadata only; no handler is implemented." });
}
