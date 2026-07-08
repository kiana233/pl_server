using System.Net;
using System.Net.Sockets;
using PlServer.Application;
using PlServer.Diagnostics;
using PlServer.LegacyProtocol;
using PlServer.Network;
using PlServer.Protocol;
using PlServer.Session;

namespace PlServer.Network.Tests;

public sealed class TcpHostSkeletonTests
{
    [Fact]
    public async Task Tcp_server_host_can_start_and_stop_on_loopback_with_port_zero()
    {
        await using var host = new TcpServerHost(new TcpServerOptions { Port = 0 });

        await host.StartAsync();

        Assert.True(host.IsRunning);
        Assert.NotNull(host.BoundEndPoint);

        await host.StopAsync();

        Assert.False(host.IsRunning);
    }

    [Fact]
    public async Task Tcp_server_host_exposes_bound_endpoint_after_start()
    {
        await using var host = new TcpServerHost(new TcpServerOptions { Port = 0 });

        await host.StartAsync();

        var endpoint = Assert.IsType<IPEndPoint>(host.BoundEndPoint);
        Assert.Equal(IPAddress.Loopback, endpoint.Address);
        Assert.NotEqual(0, endpoint.Port);
    }

    [Fact]
    public async Task Tcp_server_host_accepts_at_least_one_local_tcp_client()
    {
        await using var host = new TcpServerHost(new TcpServerOptions { Port = 0 });
        await host.StartAsync();
        var endpoint = Assert.IsType<IPEndPoint>(host.BoundEndPoint);

        using var client = new TcpClient();
        await client.ConnectAsync(endpoint.Address, endpoint.Port);

        Assert.True(await WaitUntilAsync(() => host.ConnectionRegistry.GetAll().Count == 1));
    }

    [Fact]
    public async Task Stop_closes_active_connections_and_clears_registry()
    {
        await using var host = new TcpServerHost(new TcpServerOptions { Port = 0 });
        await host.StartAsync();
        var endpoint = Assert.IsType<IPEndPoint>(host.BoundEndPoint);
        using var client = new TcpClient();
        await client.ConnectAsync(endpoint.Address, endpoint.Port);
        Assert.True(await WaitUntilAsync(() => host.ConnectionRegistry.GetAll().Count == 1));

        await host.StopAsync();

        Assert.Empty(host.ConnectionRegistry.GetAll());
    }

    [Fact]
    public void Connection_registry_registers_and_removes_connections()
    {
        var registry = new ConnectionRegistry();
        var context = new ClientConnectionContext("connection-test", null, DateTimeOffset.UtcNow);

        registry.Register(context);
        var removed = registry.Remove(context.ConnectionId);

        Assert.True(removed);
        Assert.Empty(registry.GetAll());
    }

    [Fact]
    public void Connection_id_generator_returns_unique_ids()
    {
        var generator = new ConnectionIdGenerator();

        var first = generator.NextId();
        var second = generator.NextId();

        Assert.NotEqual(first, second);
    }

    [Fact]
    public async Task Receive_pipeline_decodes_valid_packet_through_packet_codec()
    {
        var sink = new InMemoryTraceSink();
        var pipeline = CreateReceivePipeline(sink);
        var context = CreateConnection(SessionState.HandshakeDone);
        var frame = new PacketCodec().Encode(0x63, 0x04).EncodedBytes;

        var received = await pipeline.ReceiveAsync(context, frame);

        Assert.True(received.DecodeResult.IsValid);
        Assert.Equal((byte?)0x63, received.DecodeResult.Ac);
        Assert.Equal((byte?)0x04, received.DecodeResult.SubAc);
    }

    [Fact]
    public async Task Receive_pipeline_returns_invalid_packet_for_malformed_frame_without_throwing()
    {
        var sink = new InMemoryTraceSink();
        var pipeline = CreateReceivePipeline(sink);
        var context = CreateConnection(SessionState.Connected);

        var received = await pipeline.ReceiveAsync(context, new byte[] { 0x01 });

        Assert.False(received.DecodeResult.IsValid);
        Assert.Equal(ActionRouteStatus.InvalidPacket, received.RouteResult.Status);
    }

    [Fact]
    public async Task Receive_pipeline_routes_ac63_subac4_to_action_router_skeleton()
    {
        var sink = new InMemoryTraceSink();
        var pipeline = CreateReceivePipeline(sink);
        var context = CreateConnection(SessionState.HandshakeDone);
        var frame = new PacketCodec().Encode(0x63, 0x04).EncodedBytes;

        var received = await pipeline.ReceiveAsync(context, frame);

        Assert.Equal("LoginRequestCandidate", received.RouteResult.ProtocolName);
        Assert.Equal(ActionRouteStatus.MissingHandler, received.RouteResult.Status);
    }

    [Fact]
    public async Task Receive_pipeline_rejects_movement_candidate_in_connected_state_through_session_guard()
    {
        var sink = new InMemoryTraceSink();
        var pipeline = CreateReceivePipeline(sink);
        var context = CreateConnection(SessionState.Connected);
        var frame = new PacketCodec().Encode(0x06, 0x01).EncodedBytes;

        var received = await pipeline.ReceiveAsync(context, frame);

        Assert.Equal(ActionRouteStatus.RejectedBySessionGuard, received.RouteResult.Status);
        Assert.False(received.RouteResult.SessionGuardAllowed);
    }

    [Fact]
    public async Task Receive_pipeline_preserves_packet_codec_validation_errors()
    {
        var sink = new InMemoryTraceSink();
        var pipeline = CreateReceivePipeline(sink);
        var context = CreateConnection(SessionState.Connected);

        var received = await pipeline.ReceiveAsync(context, Array.Empty<byte>());

        Assert.NotEmpty(received.DecodeResult.ValidationErrors);
        Assert.Equal(received.DecodeResult.ValidationErrors, received.RouteResult.ValidationErrors);
    }

    [Fact]
    public async Task Packet_route_pipeline_writes_protocol_trace_event()
    {
        var sink = new InMemoryTraceSink();
        var pipeline = CreateReceivePipeline(sink);
        var context = CreateConnection(SessionState.HandshakeDone);
        var frame = new PacketCodec().Encode(0x63, 0x04).EncodedBytes;

        await pipeline.ReceiveAsync(context, frame);

        var traceEvent = Assert.Single(sink.Events);
        Assert.Equal(ProtocolTraceDirection.C2S, traceEvent.Direction);
        Assert.Equal(context.ConnectionId, traceEvent.ConnectionId);
    }

    [Fact]
    public async Task Send_pipeline_can_queue_outgoing_bytes_without_generating_gameplay_response()
    {
        var pipeline = new SendPipeline();
        var context = CreateConnection(SessionState.Connected);

        var result = await pipeline.QueueAsync(context, new byte[] { 0x01, 0x02 });
        var frames = pipeline.DrainQueuedFrames();

        Assert.Equal(NetworkRuntimeStatus.SendQueued, result.Status);
        Assert.Single(frames);
        Assert.Contains("no gameplay response", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Tcp_host_does_not_implement_login_business_logic()
    {
        var sink = new InMemoryTraceSink();
        var pipeline = CreateReceivePipeline(sink);
        var context = CreateConnection(SessionState.HandshakeDone);
        var frame = new PacketCodec().Encode(0x63, 0x04).EncodedBytes;

        var received = await pipeline.ReceiveAsync(context, frame);

        Assert.Equal(ActionRouteStatus.MissingHandler, received.RouteResult.Status);
        Assert.Null(context.AccountName);
    }

    [Fact]
    public async Task Tcp_host_does_not_require_gui()
    {
        await using var host = new TcpServerHost(new TcpServerOptions { Port = 0 });

        await host.StartAsync();

        Assert.True(host.IsRunning);
        Assert.DoesNotContain("Gui", typeof(TcpServerHost).Assembly.GetName().Name!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Synthetic_network_test_traffic_is_not_marked_trace_client_confirmed()
    {
        var sink = new InMemoryTraceSink();
        var pipeline = CreateReceivePipeline(sink);
        var context = CreateConnection(SessionState.HandshakeDone);
        var frame = new PacketCodec().Encode(0x63, 0x04).EncodedBytes;

        await pipeline.ReceiveAsync(context, frame);

        var traceEvent = Assert.Single(sink.Events);
        Assert.NotEqual(ProtocolTraceSourceLabel.TraceClient, traceEvent.SourceLabel);
        Assert.NotEqual(ProtocolTraceStatus.Confirmed, traceEvent.Status);
    }

    private static ReceivePipeline CreateReceivePipeline(InMemoryTraceSink sink)
    {
        var routePipeline = new PacketRoutePipeline(
            new PacketCodec(),
            new ProtocolTraceLogger(sink),
            new ActionRouter(
                LegacyProtocolContractCatalog.CreateDefaultRegistry(),
                new ActionHandlerRegistry()));

        return new ReceivePipeline(routePipeline);
    }

    private static ClientConnectionContext CreateConnection(SessionState state)
    {
        return new ClientConnectionContext("test-connection", null, DateTimeOffset.UtcNow)
        {
            CurrentSessionState = state
        };
    }

    private static async Task<bool> WaitUntilAsync(Func<bool> predicate)
    {
        var deadline = DateTimeOffset.UtcNow.AddSeconds(5);
        while (DateTimeOffset.UtcNow < deadline)
        {
            if (predicate())
            {
                return true;
            }

            await Task.Delay(25);
        }

        return predicate();
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
