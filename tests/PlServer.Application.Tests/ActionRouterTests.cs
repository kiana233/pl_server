using PlServer.Application;
using PlServer.LegacyProtocol;
using PlServer.Protocol;
using PlServer.Session;

namespace PlServer.Application.Tests;

public sealed class ActionRouterTests
{
    [Fact]
    public async Task Invalid_packet_decode_result_returns_invalid_packet()
    {
        var router = CreateRouter();
        var request = CreateRequest(InvalidPacket(), SessionState.Connected);

        var result = await router.RouteAsync(request);

        Assert.False(result.IsRouted);
        Assert.Equal(ActionRouteStatus.InvalidPacket, result.Status);
        Assert.Equal(SessionPacketKind.InvalidPacket, result.PacketKind);
        Assert.Single(result.ValidationErrors);
    }

    [Fact]
    public async Task Unknown_ac_returns_unknown_packet_without_throwing()
    {
        var router = CreateRouter();
        var request = CreateRequest(ValidPacket(0xfe, 0x01), SessionState.Connected);

        var result = await router.RouteAsync(request);

        Assert.False(result.IsRouted);
        Assert.Equal(ActionRouteStatus.UnknownPacket, result.Status);
        Assert.Equal("Unknown", result.Contract?.ProtocolName);
    }

    [Fact]
    public async Task Ac63_subac4_resolves_login_request_candidate_contract()
    {
        var router = CreateRouter();
        var request = CreateRequest(ValidPacket(0x63, 0x04), SessionState.HandshakeDone);

        var result = await router.RouteAsync(request);

        Assert.Equal(ActionRouteStatus.MissingHandler, result.Status);
        Assert.Equal("LoginRequestCandidate", result.Contract?.ProtocolName);
        Assert.Contains("登录", result.ChineseBehavior);
    }

    [Fact]
    public async Task Ac63_subac4_can_route_to_noop_handler()
    {
        var registry = new ActionHandlerRegistry();
        registry.Register(new ActionHandlerDescriptor(
            new LegacyProtocolKey(0x63, 0x04),
            "LoginRequestNoOp",
            new NoOpActionHandler("LoginRequestNoOp")));
        var router = CreateRouter(registry);
        var request = CreateRequest(ValidPacket(0x63, 0x04), SessionState.HandshakeDone);

        var result = await router.RouteAsync(request);

        Assert.True(result.IsRouted);
        Assert.Equal(ActionRouteStatus.RoutedToNoOp, result.Status);
        Assert.Equal("LoginRequestNoOp", result.HandlerName);
    }

    [Fact]
    public async Task ActionRouter_invokes_handshake_candidate_handler_for_ac0()
    {
        var router = CreateCandidateRouter();
        var request = CreateRequest(ValidPacket(0x00, 0x01), SessionState.Connected);

        var result = await router.RouteAsync(request);

        Assert.True(result.IsRouted);
        Assert.Equal(ActionRouteStatus.CandidateHandled, result.Status);
        Assert.Equal(nameof(HandshakeCandidateHandler), result.HandlerName);
        Assert.Equal(ActionHandlerStatus.CandidateHandled, result.HandlerStatus);
        Assert.NotNull(result.HandlerResult);
    }

    [Fact]
    public async Task ActionRouter_invokes_login_request_candidate_handler_for_ac63_subac4()
    {
        var router = CreateCandidateRouter();
        var request = CreateRequest(ValidPacket(0x63, 0x04), SessionState.HandshakeDone);

        var result = await router.RouteAsync(request);

        Assert.True(result.IsRouted);
        Assert.Equal(ActionRouteStatus.CandidateHandled, result.Status);
        Assert.Equal(nameof(LoginRequestCandidateHandler), result.HandlerName);
        Assert.Equal(ActionHandlerStatus.CandidateHandled, result.HandlerStatus);
        Assert.Contains(result.HandlerNotes, note => note.Contains("pending target-client trace", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Router_does_not_resolve_or_invoke_handler_when_session_guard_rejects_packet()
    {
        var registry = new TrackingActionHandlerRegistry();
        var router = new ActionRouter(LegacyProtocolContractCatalog.CreateDefaultRegistry(), registry);
        var request = CreateRequest(ValidPacket(0x06, 0x01), SessionState.Connected);

        var result = await router.RouteAsync(request);

        Assert.Equal(ActionRouteStatus.RejectedBySessionGuard, result.Status);
        Assert.Equal(0, registry.ResolveCount);
        Assert.Null(result.HandlerName);
        Assert.Null(result.HandlerResult);
    }

    [Fact]
    public async Task Missing_handler_returns_missing_handler()
    {
        var router = CreateRouter();
        var request = CreateRequest(ValidPacket(0x63, 0x04), SessionState.HandshakeDone);

        var result = await router.RouteAsync(request);

        Assert.False(result.IsRouted);
        Assert.Equal(ActionRouteStatus.MissingHandler, result.Status);
        Assert.Null(result.HandlerName);
        Assert.Null(result.HandlerResult);
    }

    [Fact]
    public async Task Handler_result_is_attached_to_action_route_result()
    {
        var router = CreateCandidateRouter();
        var request = CreateRequest(ValidPacket(0x63, 0x04), SessionState.HandshakeDone);

        var result = await router.RouteAsync(request);

        Assert.NotNull(result.HandlerResult);
        Assert.Equal(nameof(LoginRequestCandidateHandler), result.HandlerResult.HandlerName);
        Assert.Equal(ActionHandlerStatus.CandidateHandled, result.HandlerResult.Status);
        Assert.Contains("candidate-only", string.Join(" ", result.Notes), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Candidate_handlers_do_not_generate_response_packets()
    {
        var router = CreateCandidateRouter();
        var handshake = await router.RouteAsync(CreateRequest(ValidPacket(0x00, 0x01), SessionState.Connected));
        var login = await router.RouteAsync(CreateRequest(ValidPacket(0x63, 0x04), SessionState.HandshakeDone));

        Assert.False(handshake.HandlerResult?.HasResponsePackets);
        Assert.False(login.HandlerResult?.HasResponsePackets);
        Assert.Empty(handshake.HandlerResult?.ResponsePackets ?? Array.Empty<byte[]>());
        Assert.Empty(login.HandlerResult?.ResponsePackets ?? Array.Empty<byte[]>());
    }

    [Fact]
    public void Login_request_candidate_handler_does_not_expose_password_field()
    {
        var propertyNames = typeof(LoginRequestCandidateHandler)
            .GetProperties()
            .Select(property => property.Name)
            .Concat(typeof(ActionHandlerResult).GetProperties().Select(property => property.Name))
            .Concat(typeof(ActionHandlerContext).GetProperties().Select(property => property.Name))
            .ToArray();

        Assert.DoesNotContain(propertyNames, name => name.Contains("password", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Candidate_handlers_preserve_source_label_and_pending_evidence_status()
    {
        var router = CreateCandidateRouter();
        var result = await router.RouteAsync(CreateRequest(ValidPacket(0x63, 0x04), SessionState.HandshakeDone));

        Assert.Equal("reference:muayad", result.SourceLabel);
        Assert.Equal(ProtocolEvidenceStatus.PendingTargetClientTrace, result.EvidenceStatus);
        Assert.NotEqual(ProtocolEvidenceStatus.Confirmed, result.EvidenceStatus);
    }

    [Fact]
    public async Task Movement_candidate_in_connected_state_is_rejected_by_session_guard()
    {
        var router = CreateRouter();
        var request = CreateRequest(ValidPacket(0x06, 0x01), SessionState.Connected);

        var result = await router.RouteAsync(request);

        Assert.False(result.IsRouted);
        Assert.Equal(ActionRouteStatus.RejectedBySessionGuard, result.Status);
        Assert.False(result.SessionGuardAllowed);
        Assert.Equal(SessionPacketKind.MovementCandidate, result.PacketKind);
    }

    [Fact]
    public async Task Movement_candidate_in_in_map_state_is_allowed_by_session_guard()
    {
        var router = CreateRouter();
        var request = CreateRequest(ValidPacket(0x06, 0x01), SessionState.InMap);

        var result = await router.RouteAsync(request);

        Assert.Equal(ActionRouteStatus.MissingHandler, result.Status);
        Assert.True(result.SessionGuardAllowed);
        Assert.Equal(SessionPacketKind.MovementCandidate, result.PacketKind);
    }

    [Fact]
    public void Duplicate_handler_registration_is_rejected()
    {
        var registry = new ActionHandlerRegistry();
        var descriptor = new ActionHandlerDescriptor(
            new LegacyProtocolKey(0x63, 0x04),
            "LoginRequestNoOp",
            new NoOpActionHandler("LoginRequestNoOp"));
        registry.Register(descriptor);

        Assert.Throws<InvalidOperationException>(() => registry.Register(descriptor));
    }

    [Fact]
    public async Task Router_preserves_source_label_and_evidence_status()
    {
        var router = CreateRouter();
        var request = CreateRequest(ValidPacket(0x63, 0x04), SessionState.HandshakeDone);

        var result = await router.RouteAsync(request);

        Assert.Equal("reference:muayad", result.SourceLabel);
        Assert.Equal(ProtocolEvidenceStatus.PendingTargetClientTrace, result.EvidenceStatus);
    }

    [Fact]
    public async Task Router_does_not_mark_reference_behavior_as_confirmed()
    {
        var router = CreateRouter();
        var request = CreateRequest(ValidPacket(0x63, 0x04), SessionState.HandshakeDone);

        var result = await router.RouteAsync(request);

        Assert.NotEqual(ProtocolEvidenceStatus.Confirmed, result.EvidenceStatus);
        Assert.Equal(ProtocolSourceLabel.ReferenceMuayad, result.Contract?.SourceLabel);
    }

    [Fact]
    public async Task Router_does_not_execute_login_business_logic()
    {
        var registry = new ActionHandlerRegistry();
        registry.Register(new ActionHandlerDescriptor(
            new LegacyProtocolKey(0x63, 0x04),
            "LoginRequestNoOp",
            new NoOpActionHandler("LoginRequestNoOp")));
        var router = CreateRouter(registry);
        var request = CreateRequest(ValidPacket(0x63, 0x04), SessionState.HandshakeDone, accountName: "demo-account");

        var result = await router.RouteAsync(request);

        Assert.Equal(ActionRouteStatus.RoutedToNoOp, result.Status);
        Assert.Contains(result.Notes, note => note.Contains("no gameplay or login business", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Router_does_not_require_tcp_host()
    {
        var router = CreateRouter();
        var request = CreateRequest(ValidPacket(0x00, 0x00), SessionState.Connected);

        var result = await router.RouteAsync(request);

        Assert.Equal(ActionRouteStatus.MissingHandler, result.Status);
        Assert.DoesNotContain(result.Notes, note => note.Contains("TCP Host", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Router_does_not_require_gui()
    {
        var router = CreateRouter();
        var request = CreateRequest(ValidPacket(0x00, 0x00), SessionState.Connected);

        var result = await router.RouteAsync(request);

        Assert.Equal(ActionRouteStatus.MissingHandler, result.Status);
        Assert.DoesNotContain(result.Notes, note => note.Contains("GUI", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Noop_action_handler_returns_routed_to_noop()
    {
        var handler = new NoOpActionHandler("SkeletonNoOp");
        var contract = LegacyProtocolContractCatalog.CreateDefaultRegistry()
            .ResolveOrUnknown(0x63, 0x04, LegacyPacketDirection.C2S)
            .Contract;
        var request = CreateRequest(ValidPacket(0x63, 0x04), SessionState.HandshakeDone);
        var guardResult = new SessionStateGuard().CanAccept(SessionState.HandshakeDone, SessionPacketKind.LoginRequestCandidate);

        var result = await handler.HandleAsync(request, contract, SessionPacketKind.LoginRequestCandidate, guardResult);

        Assert.True(result.IsRouted);
        Assert.Equal(ActionRouteStatus.RoutedToNoOp, result.Status);
        Assert.Contains("handler skeleton only", result.Notes);
    }

    [Fact]
    public async Task Route_result_contains_contract_protocol_name_and_chinese_behavior()
    {
        var router = CreateRouter();
        var request = CreateRequest(ValidPacket(0x06, 0x01), SessionState.InMap);

        var result = await router.RouteAsync(request);

        Assert.Equal("MovementCandidate", result.ProtocolName);
        Assert.Contains("移动", result.ChineseBehavior);
    }

    private static ActionRouter CreateRouter(ActionHandlerRegistry? handlerRegistry = null)
    {
        return new ActionRouter(
            LegacyProtocolContractCatalog.CreateDefaultRegistry(),
            handlerRegistry ?? new ActionHandlerRegistry());
    }

    private static ActionRouter CreateCandidateRouter()
    {
        return new ActionRouter(
            LegacyProtocolContractCatalog.CreateDefaultRegistry(),
            CandidateActionHandlerCatalog.CreateDefaultRegistry());
    }

    private static ActionRouteRequest CreateRequest(
        PacketDecodeResult packet,
        SessionState sessionState,
        string? accountName = null)
    {
        return new ActionRouteRequest(
            "test-connection",
            sessionState,
            packet,
            LegacyPacketDirection.C2S,
            accountName,
            null);
    }

    private static PacketDecodeResult ValidPacket(byte ac, byte? subAc)
    {
        return new PacketDecodeResult(
            new[] { ac },
            new[] { ac },
            null,
            0,
            Array.Empty<byte>(),
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
                    "Synthetic invalid packet for ActionRouter test.")
            });
    }

    private sealed class TrackingActionHandlerRegistry : IActionHandlerRegistry
    {
        public int ResolveCount { get; private set; }

        public void Register(ActionHandlerDescriptor descriptor)
        {
            ArgumentNullException.ThrowIfNull(descriptor);
        }

        public bool TryResolve(LegacyProtocolContract contract, out ActionHandlerDescriptor? descriptor)
        {
            ArgumentNullException.ThrowIfNull(contract);
            ResolveCount++;
            descriptor = new ActionHandlerDescriptor(
                contract.Key,
                "UnexpectedNoOp",
                new NoOpActionHandler("UnexpectedNoOp"));
            return true;
        }
    }
}
