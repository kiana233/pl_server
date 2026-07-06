namespace PlServer.Diagnostics;

public sealed class PacketBehaviorRegistry
{
    private readonly Dictionary<(byte Ac, byte? SubAc), PacketBehaviorDescriptor> _entries = new()
    {
        [(0x00, null)] = new PacketBehaviorDescriptor(
            "HandshakeCandidate",
            "握手候选包",
            ProtocolTraceSourceLabel.ReferenceMuayad,
            ProtocolTraceStatus.PendingTargetClientTrace),

        [(0x63, 0x04)] = new PacketBehaviorDescriptor(
            "LoginRequestCandidate",
            "玩家登录请求候选包",
            ProtocolTraceSourceLabel.ReferenceMuayad,
            ProtocolTraceStatus.PendingTargetClientTrace),

        [(0x63, 0x02)] = new PacketBehaviorDescriptor(
            "CharacterSelectCandidate",
            "玩家选角请求候选包",
            ProtocolTraceSourceLabel.ReferenceMuayad,
            ProtocolTraceStatus.PendingTargetClientTrace),

        [(0x06, 0x01)] = new PacketBehaviorDescriptor(
            "MovementCandidate",
            "玩家移动请求候选包",
            ProtocolTraceSourceLabel.ReferenceMuayad,
            ProtocolTraceStatus.PendingTargetClientTrace)
    };

    public PacketBehaviorDescriptor Resolve(byte? ac, byte? subAc)
    {
        if (!ac.HasValue)
        {
            return Unknown;
        }

        if (_entries.TryGetValue((ac.Value, subAc), out var exactMatch))
        {
            return exactMatch;
        }

        if (ac.Value == 0x00 && _entries.TryGetValue((0x00, null), out var handshakeCandidate))
        {
            return handshakeCandidate;
        }

        return Unknown;
    }

    public IReadOnlyCollection<PacketBehaviorDescriptor> Entries => _entries.Values;

    private static PacketBehaviorDescriptor Unknown { get; } = new(
        "UnknownPacket",
        "未知协议包",
        ProtocolTraceSourceLabel.Unknown,
        ProtocolTraceStatus.Unknown);
}
