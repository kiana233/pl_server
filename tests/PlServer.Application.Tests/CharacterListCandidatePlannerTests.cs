using System.Reflection;
using PlServer.Core;
using PlServer.LegacyProtocol;
using PlServer.Protocol;
using PlServer.Session;

namespace PlServer.Application.Tests;

public sealed class CharacterListCandidatePlannerTests
{
    [Fact]
    public void Planner_cannot_plan_without_login_response_plan()
    {
        var planner = new CharacterListCandidatePlanner();

        var result = planner.Plan(
            loginResponsePlan: null,
            authenticationResult: null,
            SessionState.LoginPending,
            characterRepositoryAvailable: true);

        Assert.Equal(CharacterListCandidatePlanStatus.CannotPlanWithoutConfirmedLoginResponse, result.Status);
        Assert.False(result.ShouldGeneratePacket);
        Assert.Empty(result.GeneratedBytes);
    }

    [Fact]
    public void Planner_cannot_plan_without_confirmed_login_response()
    {
        var planner = new CharacterListCandidatePlanner();

        var result = planner.Plan(
            CreateLoginResponseCandidatePlan(),
            AccountAuthenticationResult.CandidateAuthenticated(CreateAccount()),
            SessionState.LoginPending,
            characterRepositoryAvailable: true);

        Assert.Equal(CharacterListCandidatePlanStatus.CannotPlanWithoutConfirmedLoginResponse, result.Status);
        Assert.False(result.ShouldGeneratePacket);
        Assert.Empty(result.GeneratedBytes);
    }

    [Fact]
    public void Planner_records_character_repository_not_implemented()
    {
        var planner = new CharacterListCandidatePlanner();

        var result = planner.Plan(
            CreateLoginResponseCandidatePlan(),
            AccountAuthenticationResult.CandidateAuthenticated(CreateAccount()),
            SessionState.LoginPending);

        Assert.Equal(CharacterListCandidatePlanStatus.CharacterRepositoryNotImplemented, result.Status);
        Assert.False(result.ShouldGeneratePacket);
        Assert.Empty(result.GeneratedBytes);
        Assert.Contains(result.Notes, note => note.Contains("character repository is not implemented", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Planner_does_not_hold_packet_writer_dependency()
    {
        var fieldTypes = typeof(CharacterListCandidatePlanner)
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Select(field => field.FieldType.Name)
            .ToArray();

        Assert.DoesNotContain("PacketWriter", fieldTypes);
    }

    [Fact]
    public void Candidate_authenticated_does_not_mean_character_list_ready()
    {
        var planner = new CharacterListCandidatePlanner();

        var result = planner.Plan(
            CreateLoginResponseCandidatePlan(),
            AccountAuthenticationResult.CandidateAuthenticated(CreateAccount()),
            SessionState.LoginPending);

        Assert.Equal(CharacterListCandidatePlanStatus.CharacterRepositoryNotImplemented, result.Status);
        Assert.False(result.ShouldGeneratePacket);
        Assert.Empty(result.GeneratedBytes);
        Assert.Null(result.Candidate);
    }

    [Fact]
    public void Plan_result_remains_pending_target_client_trace_and_not_confirmed()
    {
        var planner = new CharacterListCandidatePlanner();

        var result = planner.Plan(
            CreateLoginResponseCandidatePlan(),
            AccountAuthenticationResult.CandidateAuthenticated(CreateAccount()),
            SessionState.LoginPending);

        Assert.Equal(ProtocolEvidenceStatus.PendingTargetClientTrace, result.EvidenceStatus);
        Assert.False(result.IsConfirmed);
        Assert.NotEqual(ProtocolEvidenceStatus.Confirmed, result.EvidenceStatus);
    }

    [Fact]
    public void Plan_result_has_no_sensitive_or_character_identity_fields()
    {
        var propertyNames = typeof(CharacterListCandidatePlanResult)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        Assert.DoesNotContain(propertyNames, name => name.Contains("password", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("token", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("cookie", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("sessionkey", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("charactername", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("accountname", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Login_request_candidate_handler_records_character_list_not_generated()
    {
        var handler = new LoginRequestCandidateHandler();
        var context = CreateContext(ValidPacket(0x63, 0x04, payloadLength: 5));

        var result = await handler.HandleAsync(
            context.Request,
            context.Contract,
            context.PacketKind,
            context.SessionGuardResult);

        Assert.False(result.HandlerResult?.HasResponsePackets);
        Assert.Contains(result.HandlerNotes, note => note.Contains("character list planning status", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.HandlerNotes, note => note.Contains("character list response not generated", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.HandlerNotes, note => note.Contains("no character selection response generated", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.HandlerNotes, note => note.Contains("no enter-map response generated", StringComparison.OrdinalIgnoreCase));
    }

    private static LoginResponseCandidatePlanResult CreateLoginResponseCandidatePlan()
    {
        var registry = LoginResponseContractCandidateRegistry.CreateDefault();

        Assert.True(registry.TryResolveByKind(
            LoginResponseKind.LoginSuccessCandidate,
            out var candidate));
        Assert.NotNull(candidate);

        return LoginResponseCandidatePlanResult.CandidateOnlyNoPacketGenerated(candidate);
    }

    private static ActionHandlerContext CreateContext(PacketDecodeResult packetDecodeResult)
    {
        var contract = LegacyProtocolContractCatalog.CreateDefaultRegistry()
            .ResolveOrUnknown(0x63, 0x04, LegacyPacketDirection.C2S)
            .Contract;
        var request = new ActionRouteRequest(
            "character-list-planner-test-connection",
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
