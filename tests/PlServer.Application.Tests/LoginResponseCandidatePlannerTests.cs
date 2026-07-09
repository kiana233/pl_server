using System.Reflection;
using PlServer.Core;
using PlServer.LegacyProtocol;
using PlServer.Protocol;
using PlServer.Session;

namespace PlServer.Application.Tests;

public sealed class LoginResponseCandidatePlannerTests
{
    [Fact]
    public void Planner_cannot_plan_when_request_layout_is_unknown()
    {
        var planner = new LoginResponseCandidatePlanner();

        var result = planner.Plan(
            CreateOpaqueParseResult(),
            authenticationResult: null,
            SessionState.LoginPending);

        Assert.Equal(LoginResponseCandidatePlanStatus.CannotPlanUnknownRequestLayout, result.Status);
        Assert.False(result.ShouldGeneratePacket);
        Assert.Empty(result.GeneratedBytes);
    }

    [Fact]
    public void Planner_cannot_plan_without_authentication_result()
    {
        var planner = new LoginResponseCandidatePlanner();

        var result = planner.Plan(
            CreateParsedCandidateResult(),
            authenticationResult: null,
            SessionState.LoginPending);

        Assert.Equal(LoginResponseCandidatePlanStatus.CannotPlanWithoutAuthentication, result.Status);
        Assert.False(result.ShouldGeneratePacket);
    }

    [Fact]
    public void Planner_returns_no_generated_bytes_for_candidate_authenticated()
    {
        var planner = new LoginResponseCandidatePlanner();

        var result = planner.Plan(
            CreateParsedCandidateResult(),
            AccountAuthenticationResult.CandidateAuthenticated(CreateAccount()),
            SessionState.LoginPending);

        Assert.Equal(LoginResponseCandidatePlanStatus.CandidateOnlyNoPacketGenerated, result.Status);
        Assert.Equal(LoginResponseKind.LoginSuccessCandidate, result.Candidate?.Kind);
        Assert.False(result.ShouldGeneratePacket);
        Assert.Empty(result.GeneratedBytes);
    }

    [Fact]
    public void Planner_returns_no_generated_bytes_for_invalid_credentials()
    {
        var planner = new LoginResponseCandidatePlanner();
        var account = CreateAccount();

        var result = planner.Plan(
            CreateParsedCandidateResult(),
            AccountAuthenticationResult.InvalidCredentials(account),
            SessionState.LoginPending);

        Assert.Equal(LoginResponseKind.LoginFailureCandidate, result.Candidate?.Kind);
        Assert.False(result.ShouldGeneratePacket);
        Assert.Empty(result.GeneratedBytes);
    }

    [Fact]
    public void Planner_does_not_hold_packet_writer_dependency()
    {
        var fieldTypes = typeof(LoginResponseCandidatePlanner)
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Select(field => field.FieldType.Name)
            .ToArray();

        Assert.DoesNotContain("PacketWriter", fieldTypes);
    }

    [Fact]
    public async Task Login_request_candidate_handler_records_response_planning_status()
    {
        var handler = new LoginRequestCandidateHandler();
        var context = CreateContext(ValidPacket(0x63, 0x04, payloadLength: 5));

        var result = await handler.HandleAsync(
            context.Request,
            context.Contract,
            context.PacketKind,
            context.SessionGuardResult);

        Assert.Contains(result.HandlerNotes, note => note.Contains("response planning status", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.HandlerNotes, note => note.Contains("CannotPlanUnknownRequestLayout", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.HandlerNotes, note => note.Contains("no response generated", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Handler_does_not_generate_login_success_or_failure_response()
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
        Assert.DoesNotContain(result.HandlerNotes, note => note.Contains("login success response generated", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(result.HandlerNotes, note => note.Contains("login failure response generated", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Handler_does_not_generate_character_list_response()
    {
        var handler = new LoginRequestCandidateHandler();
        var context = CreateContext(ValidPacket(0x63, 0x04, payloadLength: 5));

        var result = await handler.HandleAsync(
            context.Request,
            context.Contract,
            context.PacketKind,
            context.SessionGuardResult);

        Assert.False(result.HandlerResult?.HasResponsePackets);
        Assert.Contains(result.HandlerNotes, note => note.Contains("no character list generated", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Candidate_authenticated_does_not_mean_client_login_success()
    {
        var result = AccountAuthenticationResult.CandidateAuthenticated(CreateAccount());

        Assert.Equal(AccountAuthenticationStatus.CandidateAuthenticated, result.Status);
        Assert.False(result.IsProtocolLoginSuccess);
        Assert.False(result.HasResponsePackets);
    }

    [Fact]
    public void Response_candidate_remains_pending_target_client_trace_and_not_confirmed()
    {
        var planner = new LoginResponseCandidatePlanner();

        var result = planner.Plan(
            CreateParsedCandidateResult(),
            AccountAuthenticationResult.CandidateAuthenticated(CreateAccount()),
            SessionState.LoginPending);

        Assert.Equal(ProtocolEvidenceStatus.PendingTargetClientTrace, result.EvidenceStatus);
        Assert.False(result.IsConfirmed);
        Assert.NotEqual(ProtocolEvidenceStatus.Confirmed, result.EvidenceStatus);
    }

    [Fact]
    public void Response_planning_result_has_no_password_or_token_fields()
    {
        var propertyNames = typeof(LoginResponseCandidatePlanResult)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        Assert.DoesNotContain(propertyNames, name => name.Contains("password", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("token", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("cookie", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("sessionkey", StringComparison.OrdinalIgnoreCase));
    }

    private static LoginRequestParseResult CreateOpaqueParseResult()
    {
        return new OpaqueLoginRequestCandidateParser().Parse(
            CreateContext(ValidPacket(0x63, 0x04, payloadLength: 5)));
    }

    private static LoginRequestParseResult CreateParsedCandidateResult()
    {
        var context = CreateContext(ValidPacket(0x63, 0x04, payloadLength: 5));

        return LoginRequestParseResult.ParsedCandidate(
            context,
            Array.Empty<LoginRequestField>(),
            new[] { "parsed candidate for planner test only" });
    }

    private static ActionHandlerContext CreateContext(PacketDecodeResult packetDecodeResult)
    {
        var contract = LegacyProtocolContractCatalog.CreateDefaultRegistry()
            .ResolveOrUnknown(0x63, 0x04, LegacyPacketDirection.C2S)
            .Contract;
        var request = new ActionRouteRequest(
            "response-planner-test-connection",
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

    private static AccountRecord CreateAccount()
    {
        return new AccountRecord(
            new AccountId("candidate-account"),
            new AccountName("candidate-account"),
            AccountStatus.Active,
            DateTimeOffset.UnixEpoch);
    }
}
