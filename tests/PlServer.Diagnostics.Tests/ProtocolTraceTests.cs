using System.Text.Json;
using PlServer.Diagnostics;
using PlServer.Protocol;

namespace PlServer.Diagnostics.Tests;

public sealed class ProtocolTraceTests
{
    [Fact]
    public void Hex_formatter_outputs_uppercase_spaced_hex()
    {
        var result = ProtocolTraceFormatter.FormatHex(new byte[] { 0xF4, 0x44, 0x03, 0x00, 0x63, 0x01, 0xAA });

        Assert.Equal("F4 44 03 00 63 01 AA", result);
    }

    [Fact]
    public void Hex_formatter_handles_empty_input()
    {
        Assert.Equal(string.Empty, ProtocolTraceFormatter.FormatHex(Array.Empty<byte>()));
        Assert.Equal(string.Empty, ProtocolTraceFormatter.FormatHex(null));
    }

    [Fact]
    public void ProtocolTraceEvent_json_is_one_line()
    {
        var json = ProtocolTraceFormatter.FormatJsonLine(CreateTraceEvent());

        Assert.DoesNotContain("\r", json);
        Assert.DoesNotContain("\n", json);
    }

    [Fact]
    public void ProtocolTraceStateChange_can_represent_previous_and_current_state()
    {
        var stateChange = CreateStateChange();

        Assert.Equal("Connected", stateChange.PreviousState);
        Assert.Equal("HandshakeDone", stateChange.CurrentState);
        Assert.Equal("HandshakeCandidate", stateChange.PacketKind);
        Assert.True(stateChange.WasStateChanged);
    }

    [Fact]
    public void Json_contains_direction_ac_subac_rawHex_and_decodedHex()
    {
        using var document = JsonDocument.Parse(ProtocolTraceFormatter.FormatJsonLine(CreateTraceEvent()));
        var root = document.RootElement;

        Assert.Equal("C2S", root.GetProperty("direction").GetString());
        Assert.Equal(0x63, root.GetProperty("ac").GetByte());
        Assert.Equal(0x04, root.GetProperty("subAc").GetByte());
        Assert.Equal("F4 44 02 00 63 04", root.GetProperty("rawHex").GetString());
        Assert.Equal("F4 44 02 00 63 04", root.GetProperty("decodedHex").GetString());
    }

    [Fact]
    public void Json_contains_state_change_previous_current_and_wasStateChanged()
    {
        using var document = JsonDocument.Parse(ProtocolTraceFormatter.FormatJsonLine(CreateTraceEvent(CreateStateChange())));
        var stateChange = document.RootElement.GetProperty("stateChange");

        Assert.Equal("Connected", stateChange.GetProperty("previousState").GetString());
        Assert.Equal("HandshakeDone", stateChange.GetProperty("currentState").GetString());
        Assert.Equal("HandshakeCandidate", stateChange.GetProperty("packetKind").GetString());
        Assert.True(stateChange.GetProperty("wasStateChanged").GetBoolean());
    }

    [Fact]
    public void Json_contains_rejectionReason_when_state_change_is_rejected()
    {
        var stateChange = new ProtocolTraceStateChange(
            "Connected",
            "Connected",
            "MovementCandidate",
            false,
            "Movement candidates are only allowed after the session reaches InMap.",
            new[]
            {
                new ProtocolTraceStateChangeError(
                    "MovementBeforeInMap",
                    "Movement candidates are only allowed after the session reaches InMap.")
            },
            new[] { "candidate session update only" });

        using var document = JsonDocument.Parse(ProtocolTraceFormatter.FormatJsonLine(CreateTraceEvent(stateChange)));
        var stateChangeJson = document.RootElement.GetProperty("stateChange");

        Assert.Equal(
            "Movement candidates are only allowed after the session reaches InMap.",
            stateChangeJson.GetProperty("rejectionReason").GetString());
        Assert.Single(stateChangeJson.GetProperty("errors").EnumerateArray());
    }

    [Fact]
    public void SourceLabel_serializes_as_reference_muayad()
    {
        using var document = JsonDocument.Parse(ProtocolTraceFormatter.FormatJsonLine(CreateTraceEvent()));

        Assert.Equal("reference:muayad", document.RootElement.GetProperty("sourceLabel").GetString());
    }

    [Fact]
    public void Status_serializes_as_pending_target_client_trace()
    {
        using var document = JsonDocument.Parse(ProtocolTraceFormatter.FormatJsonLine(CreateTraceEvent()));

        Assert.Equal("pending-target-client-trace", document.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public void Json_for_synthetic_or_reference_event_is_not_trace_client_or_confirmed()
    {
        using var document = JsonDocument.Parse(ProtocolTraceFormatter.FormatJsonLine(CreateTraceEvent(CreateStateChange())));
        var root = document.RootElement;

        Assert.NotEqual("trace:client", root.GetProperty("sourceLabel").GetString());
        Assert.NotEqual("confirmed", root.GetProperty("status").GetString());
    }

    [Fact]
    public void JsonLines_sink_creates_directory_and_writes_one_line_per_event()
    {
        var filePath = CreateTempTracePath();

        using (var sink = new JsonLinesProtocolTraceSink(filePath))
        {
            sink.Write(CreateTraceEvent());
            sink.Flush();
        }

        Assert.True(File.Exists(filePath));
        Assert.Single(File.ReadAllLines(filePath));
    }

    [Fact]
    public void JsonLines_sink_appends_multiple_events()
    {
        var filePath = CreateTempTracePath();

        using (var sink = new JsonLinesProtocolTraceSink(filePath))
        {
            sink.Write(CreateTraceEvent());
            sink.Flush();
        }

        using (var sink = new JsonLinesProtocolTraceSink(filePath))
        {
            sink.Write(CreateTraceEvent());
            sink.Flush();
        }

        Assert.Equal(2, File.ReadAllLines(filePath).Length);
    }

    [Fact]
    public void Logger_preserves_PacketCodec_validation_errors()
    {
        var decodeResult = new PacketCodec().Decode(new byte[] { 0xF4, 0x44, 0x04, 0x00, 0x63, 0x04 });
        var sink = new CapturingProtocolTraceSink();
        var logger = new ProtocolTraceLogger(sink);

        logger.LogPacketDecodeResult(ProtocolTraceDirection.C2S, "conn-1", decodeResult);

        var traceEvent = Assert.Single(sink.Events);
        Assert.Contains(
            traceEvent.ValidationErrors,
            error => error.Code == PacketValidationErrorCode.DeclaredLengthLargerThanAvailable);
        Assert.Contains(
            traceEvent.ValidationErrors,
            error => error.Code == PacketValidationErrorCode.PayloadLengthMismatch);
    }

    [Fact]
    public void Behavior_registry_resolves_AC63_4_to_login_request_candidate()
    {
        var descriptor = new PacketBehaviorRegistry().Resolve(0x63, 0x04);

        Assert.Equal("LoginRequestCandidate", descriptor.ProtocolName);
        Assert.Equal(ProtocolTraceSourceLabel.ReferenceMuayad, descriptor.SourceLabel);
        Assert.Equal(ProtocolTraceStatus.PendingTargetClientTrace, descriptor.Status);
    }

    [Fact]
    public void Behavior_registry_resolves_AC06_1_to_movement_candidate()
    {
        var descriptor = new PacketBehaviorRegistry().Resolve(0x06, 0x01);

        Assert.Equal("MovementCandidate", descriptor.ProtocolName);
        Assert.Equal(ProtocolTraceSourceLabel.ReferenceMuayad, descriptor.SourceLabel);
        Assert.Equal(ProtocolTraceStatus.PendingTargetClientTrace, descriptor.Status);
    }

    [Fact]
    public void Unknown_AC_resolves_to_unknown_behavior_without_throwing()
    {
        var exception = Record.Exception(() => new PacketBehaviorRegistry().Resolve(0xFF, 0xFF));
        var descriptor = new PacketBehaviorRegistry().Resolve(0xFF, 0xFF);

        Assert.Null(exception);
        Assert.Equal("UnknownPacket", descriptor.ProtocolName);
        Assert.Equal(ProtocolTraceSourceLabel.Unknown, descriptor.SourceLabel);
    }

    [Fact]
    public void ProtocolTraceEvent_has_no_sensitive_password_field()
    {
        var propertyNames = typeof(ProtocolTraceEvent)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        Assert.DoesNotContain(propertyNames, name => name.Contains("password", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Enriched_json_has_no_sensitive_password_field()
    {
        var json = ProtocolTraceFormatter.FormatJsonLine(CreateTraceEvent(CreateStateChange()));

        Assert.DoesNotContain("password", json, StringComparison.OrdinalIgnoreCase);
    }

    private static ProtocolTraceEvent CreateTraceEvent(ProtocolTraceStateChange? stateChange = null)
    {
        return new ProtocolTraceEvent
        {
            Timestamp = new DateTimeOffset(2026, 7, 6, 0, 0, 0, TimeSpan.Zero),
            Direction = ProtocolTraceDirection.C2S,
            ConnectionId = "conn-1",
            RawHex = "F4 44 02 00 63 04",
            DecodedHex = "F4 44 02 00 63 04",
            Header = new PacketHeader(0xF4, 0x44, 2),
            PayloadLength = 2,
            Ac = 0x63,
            SubAc = 0x04,
            ProtocolName = "LoginRequestCandidate",
            Behavior = "玩家登录请求候选包",
            RouteStatus = "MissingHandler",
            Result = "decoded",
            SourceLabel = ProtocolTraceSourceLabel.ReferenceMuayad,
            Status = ProtocolTraceStatus.PendingTargetClientTrace,
            StateChange = stateChange
        };
    }

    private static ProtocolTraceStateChange CreateStateChange()
    {
        return new ProtocolTraceStateChange(
            "Connected",
            "HandshakeDone",
            "HandshakeCandidate",
            true,
            null,
            Array.Empty<ProtocolTraceStateChangeError>(),
            new[] { "candidate session update only" });
    }

    private static string CreateTempTracePath()
    {
        return Path.Combine(
            Path.GetTempPath(),
            "pl-server-tests",
            Guid.NewGuid().ToString("N"),
            "protocol-trace.jsonl");
    }

    private sealed class CapturingProtocolTraceSink : IProtocolTraceSink
    {
        public List<ProtocolTraceEvent> Events { get; } = new();

        public void Write(ProtocolTraceEvent traceEvent)
        {
            Events.Add(traceEvent);
        }

        public void Flush()
        {
        }

        public void Dispose()
        {
        }
    }
}
