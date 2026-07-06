using System.Text.Json;
using PlServer.Protocol;

namespace PlServer.Diagnostics;

public static class ProtocolTraceFormatter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    public static string FormatHex(byte[]? bytes)
    {
        if (bytes is null || bytes.Length == 0)
        {
            return string.Empty;
        }

        return string.Join(" ", bytes.Select(value => value.ToString("X2")));
    }

    public static string FormatJsonLine(ProtocolTraceEvent traceEvent)
    {
        ArgumentNullException.ThrowIfNull(traceEvent);

        var payload = new Dictionary<string, object?>
        {
            ["timestamp"] = traceEvent.Timestamp,
            ["direction"] = traceEvent.Direction.ToString(),
            ["connectionId"] = traceEvent.ConnectionId,
            ["accountName"] = traceEvent.AccountName,
            ["characterName"] = traceEvent.CharacterName,
            ["sessionState"] = traceEvent.SessionState,
            ["rawHex"] = traceEvent.RawHex,
            ["decodedHex"] = traceEvent.DecodedHex,
            ["header"] = FormatHeader(traceEvent.Header),
            ["payloadLength"] = traceEvent.PayloadLength,
            ["ac"] = traceEvent.Ac,
            ["subAc"] = traceEvent.SubAc,
            ["protocolName"] = traceEvent.ProtocolName,
            ["behavior"] = traceEvent.Behavior,
            ["handler"] = traceEvent.Handler,
            ["result"] = traceEvent.Result,
            ["sourceLabel"] = FormatSourceLabel(traceEvent.SourceLabel),
            ["status"] = FormatStatus(traceEvent.Status),
            ["validationErrors"] = traceEvent.ValidationErrors.Select(FormatValidationError).ToArray(),
            ["resourceChecks"] = traceEvent.ResourceChecks.Select(FormatResourceCheck).ToArray(),
            ["stateChange"] = FormatStateChange(traceEvent.StateChange)
        };

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    public static string FormatSourceLabel(ProtocolTraceSourceLabel sourceLabel)
    {
        return sourceLabel switch
        {
            ProtocolTraceSourceLabel.TraceClient => "trace:client",
            ProtocolTraceSourceLabel.ReferenceMuayad => "reference:muayad",
            ProtocolTraceSourceLabel.ReferenceWlophoenix => "reference:wlophoenix",
            ProtocolTraceSourceLabel.Assumption => "assumption",
            _ => "unknown"
        };
    }

    public static string FormatStatus(ProtocolTraceStatus status)
    {
        return status switch
        {
            ProtocolTraceStatus.Confirmed => "confirmed",
            ProtocolTraceStatus.PendingTargetClientTrace => "pending-target-client-trace",
            ProtocolTraceStatus.Assumption => "assumption",
            ProtocolTraceStatus.Invalid => "invalid",
            ProtocolTraceStatus.Rejected => "rejected",
            _ => "unknown"
        };
    }

    private static object? FormatHeader(PacketHeader? header)
    {
        if (header is null)
        {
            return null;
        }

        return new Dictionary<string, object?>
        {
            ["byte0"] = header.Byte0,
            ["byte1"] = header.Byte1,
            ["payloadLength"] = header.PayloadLength
        };
    }

    private static object FormatValidationError(PacketValidationError error)
    {
        return new Dictionary<string, object?>
        {
            ["code"] = error.Code.ToString(),
            ["message"] = error.Message
        };
    }

    private static object FormatResourceCheck(ProtocolTraceResourceCheck resourceCheck)
    {
        return new Dictionary<string, object?>
        {
            ["name"] = resourceCheck.Name,
            ["result"] = resourceCheck.Result,
            ["sourceLabel"] = FormatSourceLabel(resourceCheck.SourceLabel),
            ["status"] = FormatStatus(resourceCheck.Status)
        };
    }

    private static object? FormatStateChange(ProtocolTraceStateChange? stateChange)
    {
        if (stateChange is null)
        {
            return null;
        }

        return new Dictionary<string, object?>
        {
            ["fromState"] = stateChange.FromState,
            ["toState"] = stateChange.ToState,
            ["reason"] = stateChange.Reason
        };
    }
}
