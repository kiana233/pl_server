using PlServer.LegacyProtocol;
using PlServer.Protocol;
using PlServer.Session;

namespace PlServer.Application.Tests;

public sealed class LoginRequestCandidateParserTests
{
    [Fact]
    public void Opaque_parser_returns_opaque_payload()
    {
        var parser = new OpaqueLoginRequestCandidateParser();

        var result = parser.Parse(CreateContext(ValidPacket(0x63, 0x04, payloadLength: 7)));

        Assert.Equal(LoginRequestParseStatus.OpaquePayload, result.Status);
        Assert.Equal(ProtocolEvidenceStatus.PendingTargetClientTrace, result.EvidenceStatus);
        Assert.False(result.IsConfirmed);
    }

    [Fact]
    public void Opaque_parser_records_payload_length_and_ac_subac_context()
    {
        var parser = new OpaqueLoginRequestCandidateParser();

        var result = parser.Parse(CreateContext(ValidPacket(0x63, 0x04, payloadLength: 9)));

        Assert.Equal(9, result.PayloadLength);
        Assert.Contains(result.Notes, note => note.Contains("payload length: 9", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.Notes, note => note.Contains("ac: 63", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.Notes, note => note.Contains("subAc: 04", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Opaque_parser_does_not_expose_password_token_cookie_or_session_key_fields()
    {
        var parser = new OpaqueLoginRequestCandidateParser();

        var result = parser.Parse(CreateContext(ValidPacket(0x63, 0x04, payloadLength: 11)));
        var fieldNames = result.Fields.Select(field => field.FieldName).ToArray();

        Assert.DoesNotContain(fieldNames, name => name.Contains("password", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(fieldNames, name => name.Contains("token", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(fieldNames, name => name.Contains("cookie", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(fieldNames, name => name.Contains("session key", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(fieldNames, name => name.Contains("sessionkey", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Secret_candidate_field_value_is_redacted()
    {
        var value = RedactedFieldValue.FromSensitivity(LoginRequestFieldSensitivity.SecretCandidate, "ignored");

        Assert.True(value.IsRedacted);
        Assert.Equal("[redacted]", value.DisplayValue);
    }

    [Fact]
    public void Unknown_sensitive_field_value_is_redacted()
    {
        var value = RedactedFieldValue.FromSensitivity(LoginRequestFieldSensitivity.UnknownSensitive, "ignored");

        Assert.True(value.IsRedacted);
        Assert.Equal("[redacted]", value.DisplayValue);
    }

    [Fact]
    public async Task Login_request_candidate_handler_invokes_parser_and_records_parser_status()
    {
        var parser = new TrackingParser();
        var handler = new LoginRequestCandidateHandler(new LoginRequestCandidateParserRegistry(new[] { parser }));
        var context = CreateContext(ValidPacket(0x63, 0x04, payloadLength: 5));

        var result = await handler.HandleAsync(
            context.Request,
            context.Contract,
            context.PacketKind,
            context.SessionGuardResult);

        Assert.Equal(1, parser.ParseCount);
        Assert.Contains(result.HandlerNotes, note => note.Contains("parser status: OpaquePayload", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.HandlerNotes, note => note.Contains("parser status from tracking parser", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Handler_does_not_invoke_account_service_when_parser_is_opaque()
    {
        var handler = new LoginRequestCandidateHandler();
        var context = CreateContext(ValidPacket(0x63, 0x04, payloadLength: 5));

        var result = await handler.HandleAsync(
            context.Request,
            context.Contract,
            context.PacketKind,
            context.SessionGuardResult);

        Assert.Contains(
            result.HandlerNotes,
            note => note.Contains("account repository not invoked because login field layout is unknown", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(result.HandlerNotes, note => note.Contains("CandidateAuthenticated", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Handler_does_not_generate_login_or_character_list_response()
    {
        var handler = new LoginRequestCandidateHandler();
        var context = CreateContext(ValidPacket(0x63, 0x04, payloadLength: 5));

        var result = await handler.HandleAsync(
            context.Request,
            context.Contract,
            context.PacketKind,
            context.SessionGuardResult);

        Assert.False(result.HandlerResult?.HasResponsePackets);
        Assert.Empty(result.HandlerResult?.ResponsePackets ?? Array.Empty<byte[]>());
        Assert.Contains(result.HandlerNotes, note => note.Contains("no login response packet generated", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.HandlerNotes, note => note.Contains("no character list generated", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Parser_result_remains_pending_or_unknown_and_not_confirmed()
    {
        var parser = new OpaqueLoginRequestCandidateParser();

        var result = parser.Parse(CreateContext(ValidPacket(0x63, 0x04, payloadLength: 5)));

        Assert.Contains(result.EvidenceStatus, new[]
        {
            ProtocolEvidenceStatus.PendingTargetClientTrace,
            ProtocolEvidenceStatus.Unknown
        });
        Assert.False(result.IsConfirmed);
        Assert.NotEqual(ProtocolEvidenceStatus.Confirmed, result.EvidenceStatus);
    }

    [Fact]
    public void Reference_or_synthetic_packet_cannot_promote_parser_to_confirmed()
    {
        var parser = new OpaqueLoginRequestCandidateParser();

        var result = parser.Parse(CreateContext(ValidPacket(0x63, 0x04, payloadLength: 5)));

        Assert.Equal("reference:muayad", result.SourceLabel);
        Assert.False(result.IsConfirmed);
        Assert.NotEqual(ProtocolEvidenceStatus.Confirmed, result.EvidenceStatus);
    }

    [Fact]
    public void Invalid_packet_returns_invalid_packet_without_throwing()
    {
        var parser = new OpaqueLoginRequestCandidateParser();

        var result = parser.Parse(CreateContext(InvalidPacket()));

        Assert.Equal(LoginRequestParseStatus.InvalidPacket, result.Status);
        Assert.NotEmpty(result.ValidationErrors);
        Assert.False(result.IsConfirmed);
    }

    private static ActionHandlerContext CreateContext(PacketDecodeResult packetDecodeResult)
    {
        var contract = LegacyProtocolContractCatalog.CreateDefaultRegistry()
            .ResolveOrUnknown(0x63, 0x04, LegacyPacketDirection.C2S)
            .Contract;
        var request = new ActionRouteRequest(
            "parser-test-connection",
            SessionState.HandshakeDone,
            packetDecodeResult,
            LegacyPacketDirection.C2S,
            null,
            null);
        var guardResult = new SessionStateGuard().CanAccept(
            SessionState.HandshakeDone,
            SessionPacketKind.LoginRequestCandidate);

        return new ActionHandlerContext(
            request,
            contract,
            SessionPacketKind.LoginRequestCandidate,
            guardResult);
    }

    private static PacketDecodeResult ValidPacket(byte ac, byte? subAc, ushort payloadLength)
    {
        return new PacketDecodeResult(
            new[] { ac },
            new[] { ac },
            null,
            payloadLength,
            new byte[payloadLength],
            ac,
            subAc,
            Array.Empty<PacketValidationError>());
    }

    private static PacketDecodeResult InvalidPacket()
    {
        return new PacketDecodeResult(
            Array.Empty<byte>(),
            Array.Empty<byte>(),
            null,
            0,
            Array.Empty<byte>(),
            null,
            null,
            new[]
            {
                new PacketValidationError(
                    PacketValidationErrorCode.InvalidHeader,
                    "Synthetic invalid packet for login request parser test.")
            });
    }

    private sealed class TrackingParser : ILoginRequestCandidateParser
    {
        public string ParserName => nameof(TrackingParser);

        public int ParseCount { get; private set; }

        public bool CanParse(ActionHandlerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return true;
        }

        public LoginRequestParseResult Parse(ActionHandlerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            ParseCount++;

            var result = new OpaqueLoginRequestCandidateParser().Parse(context);
            return LoginRequestParseResult.OpaquePayload(
                context,
                result.Fields,
                result.Notes.Concat(new[] { "parser status from tracking parser" }).ToArray());
        }
    }
}
