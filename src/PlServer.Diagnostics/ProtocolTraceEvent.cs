using PlServer.Protocol;

namespace PlServer.Diagnostics;

public sealed class ProtocolTraceEvent
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    public ProtocolTraceDirection Direction { get; init; } = ProtocolTraceDirection.Internal;

    public string ConnectionId { get; init; } = string.Empty;

    public string? AccountName { get; init; }

    public string? CharacterName { get; init; }

    public string? SessionState { get; init; }

    public string RawHex { get; init; } = string.Empty;

    public string DecodedHex { get; init; } = string.Empty;

    public PacketHeader? Header { get; init; }

    public ushort? PayloadLength { get; init; }

    public byte? Ac { get; init; }

    public byte? SubAc { get; init; }

    public string? ProtocolName { get; init; }

    public string? Behavior { get; init; }

    public string? Handler { get; init; }

    public string? HandlerStatus { get; init; }

    public IReadOnlyList<string> HandlerNotes { get; init; } = Array.Empty<string>();

    public string? RouteStatus { get; init; }

    public string Result { get; init; } = string.Empty;

    public ProtocolTraceSourceLabel SourceLabel { get; init; } = ProtocolTraceSourceLabel.Unknown;

    public ProtocolTraceStatus Status { get; init; } = ProtocolTraceStatus.Unknown;

    public IReadOnlyList<PacketValidationError> ValidationErrors { get; init; } = Array.Empty<PacketValidationError>();

    public IReadOnlyList<ProtocolTraceResourceCheck> ResourceChecks { get; init; } = Array.Empty<ProtocolTraceResourceCheck>();

    public ProtocolTraceStateChange? StateChange { get; init; }

    public ProtocolTraceEvent WithStateChange(
        ProtocolTraceStateChange stateChange,
        string? sessionState = null)
    {
        ArgumentNullException.ThrowIfNull(stateChange);

        return new ProtocolTraceEvent
        {
            Timestamp = Timestamp,
            Direction = Direction,
            ConnectionId = ConnectionId,
            AccountName = AccountName,
            CharacterName = CharacterName,
            SessionState = sessionState ?? SessionState,
            RawHex = RawHex,
            DecodedHex = DecodedHex,
            Header = Header,
            PayloadLength = PayloadLength,
            Ac = Ac,
            SubAc = SubAc,
            ProtocolName = ProtocolName,
            Behavior = Behavior,
            Handler = Handler,
            HandlerStatus = HandlerStatus,
            HandlerNotes = HandlerNotes,
            RouteStatus = RouteStatus,
            Result = Result,
            SourceLabel = SourceLabel,
            Status = Status,
            ValidationErrors = ValidationErrors,
            ResourceChecks = ResourceChecks,
            StateChange = stateChange
        };
    }
}
