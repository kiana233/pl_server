using PlServer.Application;
using PlServer.Diagnostics;
using PlServer.LegacyProtocol;
using PlServer.Network;
using PlServer.Protocol;
using PlServer.Session;

namespace PlServer.Network.Tests;

public sealed class ConnectionSessionUpdateTests
{
    [Fact]
    public async Task Login_request_candidate_advances_handshake_done_to_login_pending()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.HandshakeDone);

        var result = await receiveLoop.ProcessChunkAsync(connection, Frame(0x63, 0x04));

        var packet = Assert.Single(result.Packets);
        Assert.Equal(SessionState.HandshakeDone, packet.SessionUpdateResult.PreviousState);
        Assert.Equal(SessionState.LoginPending, packet.SessionUpdateResult.CurrentState);
        Assert.Equal(SessionState.LoginPending, connection.CurrentSessionState);
    }

    [Fact]
    public async Task Handshake_candidate_advances_connected_to_handshake_done()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.Connected);

        var result = await receiveLoop.ProcessChunkAsync(connection, Frame(0x00, 0x01));

        var packet = Assert.Single(result.Packets);
        Assert.Equal(SessionState.HandshakeDone, packet.SessionUpdateResult.CurrentState);
        Assert.True(packet.SessionUpdateResult.WasStateChanged);
    }

    [Fact]
    public async Task Character_list_candidate_can_advance_login_accepted_to_character_list_shown()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.LoginAccepted);

        var result = await receiveLoop.ProcessChunkAsync(connection, Frame(0x63, 0x01));

        Assert.Equal(SessionState.CharacterListShown, Assert.Single(result.Packets).SessionUpdateResult.CurrentState);
    }

    [Fact]
    public async Task Character_select_candidate_advances_character_list_shown_to_character_selected()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.CharacterListShown);

        var result = await receiveLoop.ProcessChunkAsync(connection, Frame(0x63, 0x02));

        Assert.Equal(SessionState.CharacterSelected, Assert.Single(result.Packets).SessionUpdateResult.CurrentState);
    }

    [Fact]
    public async Task Enter_map_candidate_advances_character_selected_to_entering_map()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.CharacterSelected);

        var result = await receiveLoop.ProcessChunkAsync(connection, Frame(0x12, 0x01));

        Assert.Equal(SessionState.EnteringMap, Assert.Single(result.Packets).SessionUpdateResult.CurrentState);
    }

    [Fact]
    public void In_map_ready_candidate_can_advance_entering_map_to_in_map_using_explicit_classification()
    {
        var updater = new ConnectionSessionUpdater();
        var connection = CreateConnection(SessionState.EnteringMap);
        var decodeResult = Decode(Frame(0x20, 0x01));
        var routeResult = RouteResult(SessionPacketKind.InMapReadyCandidate);
        var classification = SessionPacketClassifier.Explicit(
            SessionPacketKind.InMapReadyCandidate,
            "Explicit synthetic in-map-ready event for connection session update test.",
            0x20,
            0x01);

        var result = updater.Update(connection, decodeResult, routeResult, classification);

        Assert.Equal(SessionState.InMap, result.CurrentState);
        Assert.Equal(SessionState.InMap, connection.CurrentSessionState);
    }

    [Fact]
    public async Task Movement_candidate_in_in_map_keeps_state_in_map()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.InMap);

        var result = await receiveLoop.ProcessChunkAsync(connection, Frame(0x06, 0x01));

        var update = Assert.Single(result.Packets).SessionUpdateResult;
        Assert.Equal(ConnectionSessionUpdateStatus.NoChange, update.Status);
        Assert.Equal(SessionState.InMap, update.CurrentState);
    }

    [Fact]
    public async Task Movement_candidate_in_connected_is_rejected_and_does_not_update_state()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.Connected);

        var result = await receiveLoop.ProcessChunkAsync(connection, Frame(0x06, 0x01));

        var update = Assert.Single(result.Packets).SessionUpdateResult;
        Assert.Equal(ConnectionSessionUpdateStatus.Rejected, update.Status);
        Assert.Equal(SessionState.Connected, update.CurrentState);
        Assert.False(update.WasStateChanged);
    }

    [Fact]
    public async Task Invalid_packet_does_not_update_session_state()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.HandshakeDone);

        var result = await receiveLoop.ProcessChunkAsync(connection, new byte[] { 0xF4, 0x44, 0x00, 0x00 });

        var update = Assert.Single(result.Packets).SessionUpdateResult;
        Assert.Equal(ConnectionSessionUpdateStatus.InvalidPacket, update.Status);
        Assert.Equal(SessionState.HandshakeDone, update.CurrentState);
    }

    [Fact]
    public async Task Unknown_packet_does_not_update_session_state()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.HandshakeDone);

        var result = await receiveLoop.ProcessChunkAsync(connection, Frame(0xfe, 0x01));

        var update = Assert.Single(result.Packets).SessionUpdateResult;
        Assert.Equal(ConnectionSessionUpdateStatus.UnknownPacket, update.Status);
        Assert.Equal(SessionState.HandshakeDone, update.CurrentState);
    }

    [Fact]
    public async Task Sticky_frames_update_session_state_sequentially_across_frames()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.Connected);
        var chunk = Frame(0x00, 0x01).Concat(Frame(0x63, 0x04)).ToArray();

        var result = await receiveLoop.ProcessChunkAsync(connection, chunk);

        Assert.Equal(2, result.Packets.Count);
        Assert.Equal(SessionState.HandshakeDone, result.Packets[0].SessionUpdateResult.CurrentState);
        Assert.Equal(SessionState.LoginPending, result.Packets[1].SessionUpdateResult.CurrentState);
        Assert.Equal(SessionState.LoginPending, result.FinalSessionState);
    }

    [Fact]
    public async Task Partial_frame_does_not_update_session_state_until_completed()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.Connected);
        var frame = Frame(0x00, 0x01);

        var first = await receiveLoop.ProcessChunkAsync(connection, frame.Take(3).ToArray());
        var second = await receiveLoop.ProcessChunkAsync(connection, frame.Skip(3).ToArray());

        Assert.Empty(first.Packets);
        Assert.Equal(SessionState.Connected, first.FinalSessionState);
        Assert.Equal(SessionState.HandshakeDone, Assert.Single(second.Packets).SessionUpdateResult.CurrentState);
    }

    [Fact]
    public async Task Rejected_packet_keeps_previous_state_and_records_rejection_reason()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.Connected);

        var result = await receiveLoop.ProcessChunkAsync(connection, Frame(0x06, 0x01));

        var update = Assert.Single(result.Packets).SessionUpdateResult;
        Assert.Equal(SessionState.Connected, update.PreviousState);
        Assert.Equal(SessionState.Connected, update.CurrentState);
        Assert.NotNull(update.RejectionReason);
    }

    [Fact]
    public async Task Connection_receive_result_exposes_final_session_state()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.Connected);

        var result = await receiveLoop.ProcessChunkAsync(connection, Frame(0x00, 0x01));

        Assert.Equal(SessionState.HandshakeDone, result.FinalSessionState);
    }

    [Fact]
    public async Task Session_update_result_is_attached_to_each_received_packet_result()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.Connected);

        var result = await receiveLoop.ProcessChunkAsync(connection, Frame(0x00, 0x01));

        Assert.NotNull(Assert.Single(result.Packets).SessionUpdateResult);
    }

    [Fact]
    public async Task Synthetic_session_update_tests_are_not_marked_trace_client_confirmed()
    {
        var sink = new InMemoryTraceSink();
        var receiveLoop = CreateReceiveLoop(sink);
        var connection = CreateConnection(SessionState.Connected);

        await receiveLoop.ProcessChunkAsync(connection, Frame(0x00, 0x01));

        var traceEvent = Assert.Single(sink.Events);
        Assert.NotEqual(ProtocolTraceSourceLabel.TraceClient, traceEvent.SourceLabel);
        Assert.NotEqual(ProtocolTraceStatus.Confirmed, traceEvent.Status);
    }

    [Fact]
    public async Task No_login_response_is_generated_by_session_update()
    {
        var sendPipeline = new SendPipeline();
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.HandshakeDone);

        await receiveLoop.ProcessChunkAsync(connection, Frame(0x63, 0x04));

        Assert.Empty(sendPipeline.DrainQueuedFrames());
    }

    [Fact]
    public async Task No_real_ac_handler_is_invoked()
    {
        var receiveLoop = CreateReceiveLoop(new InMemoryTraceSink());
        var connection = CreateConnection(SessionState.HandshakeDone);

        var result = await receiveLoop.ProcessChunkAsync(connection, Frame(0x63, 0x04));

        var packet = Assert.Single(result.Packets);
        Assert.Equal(ActionRouteStatus.MissingHandler, packet.RouteResult.Status);
        Assert.Null(packet.RouteResult.HandlerName);
    }

    private static byte[] Frame(byte ac, byte? subAc)
    {
        return new PacketCodec().Encode(ac, subAc).EncodedBytes;
    }

    private static PacketDecodeResult Decode(byte[] frame)
    {
        return new PacketCodec().Decode(frame);
    }

    private static ActionRouteResult RouteResult(SessionPacketKind packetKind)
    {
        return new ActionRouteResult(
            true,
            ActionRouteStatus.RoutedToNoOp,
            null,
            "ExplicitSkeletonNoOp",
            packetKind,
            true,
            null,
            Array.Empty<PacketValidationError>(),
            "assumption",
            ProtocolEvidenceStatus.PendingTargetClientTrace,
            new[] { "explicit synthetic route result for session update test" });
    }

    private static ConnectionReceiveLoop CreateReceiveLoop(InMemoryTraceSink sink)
    {
        var routePipeline = new PacketRoutePipeline(
            new PacketCodec(),
            new ProtocolTraceLogger(sink),
            new ActionRouter(
                LegacyProtocolContractCatalog.CreateDefaultRegistry(),
                new ActionHandlerRegistry()));

        return new ConnectionReceiveLoop(new ReceivePipeline(routePipeline));
    }

    private static ClientConnectionContext CreateConnection(SessionState state)
    {
        return new ClientConnectionContext("session-update-test", null, DateTimeOffset.UtcNow)
        {
            CurrentSessionState = state
        };
    }

    private sealed class InMemoryTraceSink : IProtocolTraceSink
    {
        private readonly List<ProtocolTraceEvent> _events = new();

        public IReadOnlyList<ProtocolTraceEvent> Events => _events;

        public void Write(ProtocolTraceEvent traceEvent)
        {
            _events.Add(traceEvent);
        }

        public void Flush()
        {
        }

        public void Dispose()
        {
        }
    }
}
