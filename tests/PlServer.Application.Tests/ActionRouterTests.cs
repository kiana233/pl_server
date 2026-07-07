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
    public async Task Missing_handler_returns_missing_handler()
    {
        var router = CreateRouter();
        var request = CreateRequest(ValidPacket(0x63, 0x04), SessionState.HandshakeDone);

        var result = await router.RouteAsync(request);

        Assert.False(result.IsRouted);
        Assert.Equal(ActionRouteStatus.MissingHandler, result.Status);
        Assert.Null(result.HandlerName);
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
}
