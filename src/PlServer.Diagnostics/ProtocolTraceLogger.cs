using PlServer.Protocol;

namespace PlServer.Diagnostics;

public sealed class ProtocolTraceLogger
{
    private readonly IProtocolTraceSink _sink;
    private readonly PacketBehaviorRegistry _behaviorRegistry;

    public ProtocolTraceLogger(IProtocolTraceSink sink, PacketBehaviorRegistry? behaviorRegistry = null)
    {
        _sink = sink ?? throw new ArgumentNullException(nameof(sink));
        _behaviorRegistry = behaviorRegistry ?? new PacketBehaviorRegistry();
    }

    public void Log(ProtocolTraceEvent traceEvent)
    {
        _sink.Write(traceEvent);
    }

    public ProtocolTraceEvent CreatePacketDecodeEvent(
        ProtocolTraceDirection direction,
        string connectionId,
        PacketDecodeResult decodeResult,
        string? accountName = null,
        string? characterName = null,
        string? sessionState = null,
        string result = "decoded",
        string? routeStatus = null,
        string? handler = null,
        string? handlerStatus = null,
        IReadOnlyList<string>? handlerNotes = null,
        ProtocolTraceResourceCheck[]? resourceChecks = null,
        ProtocolTraceStateChange? stateChange = null)
    {
        ArgumentNullException.ThrowIfNull(decodeResult);

        var behavior = _behaviorRegistry.Resolve(decodeResult.Ac, decodeResult.SubAc);
        return new ProtocolTraceEvent
        {
            Direction = direction,
            ConnectionId = connectionId,
            AccountName = accountName,
            CharacterName = characterName,
            SessionState = sessionState,
            RawHex = ProtocolTraceFormatter.FormatHex(decodeResult.RawBytes),
            DecodedHex = ProtocolTraceFormatter.FormatHex(decodeResult.DecodedBytes),
            Header = decodeResult.Header,
            PayloadLength = decodeResult.PayloadLength,
            Ac = decodeResult.Ac,
            SubAc = decodeResult.SubAc,
            ProtocolName = behavior.ProtocolName,
            Behavior = behavior.Behavior,
            Handler = handler ?? behavior.Handler,
            HandlerStatus = handlerStatus,
            HandlerNotes = handlerNotes ?? Array.Empty<string>(),
            RouteStatus = routeStatus,
            Result = decodeResult.IsValid ? result : "invalid",
            SourceLabel = behavior.SourceLabel,
            Status = decodeResult.IsValid ? behavior.Status : ProtocolTraceStatus.Invalid,
            ValidationErrors = decodeResult.ValidationErrors,
            ResourceChecks = resourceChecks ?? Array.Empty<ProtocolTraceResourceCheck>(),
            StateChange = stateChange
        };
    }

    public void LogPacketDecodeResult(
        ProtocolTraceDirection direction,
        string connectionId,
        PacketDecodeResult decodeResult,
        string? accountName = null,
        string? characterName = null,
        string? sessionState = null,
        string result = "decoded",
        string? routeStatus = null,
        string? handler = null,
        string? handlerStatus = null,
        IReadOnlyList<string>? handlerNotes = null,
        ProtocolTraceResourceCheck[]? resourceChecks = null,
        ProtocolTraceStateChange? stateChange = null)
    {
        var traceEvent = CreatePacketDecodeEvent(
            direction,
            connectionId,
            decodeResult,
            accountName,
            characterName,
            sessionState,
            result,
            routeStatus,
            handler,
            handlerStatus,
            handlerNotes,
            resourceChecks,
            stateChange);

        Log(traceEvent);
    }

    public void Flush()
    {
        _sink.Flush();
    }
}
