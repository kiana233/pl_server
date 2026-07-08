using PlServer.Application;
using PlServer.Diagnostics;
using PlServer.LegacyProtocol;
using PlServer.Network;
using PlServer.Protocol;
using PlServer.Session;

namespace PlServer.Network.Tests;

public sealed class PacketFrameReadBufferTests
{
    [Fact]
    public void Splitter_returns_no_frame_for_incomplete_header()
    {
        var buffer = new PacketFrameReadBuffer();

        buffer.Append(new byte[] { 0xF4 });
        var result = buffer.TryReadFrames();

        Assert.Empty(result.Frames);
        Assert.Equal(PacketFrameSplitStatus.NoFrame, result.Status);
        Assert.Equal(1, result.RemainingBufferLength);
    }

    [Fact]
    public void Splitter_returns_no_frame_for_partial_frame()
    {
        var frame = Frame(0x63, 0x04);
        var buffer = new PacketFrameReadBuffer();

        buffer.Append(frame.Take(frame.Length - 1).ToArray());
        var result = buffer.TryReadFrames();

        Assert.Empty(result.Frames);
        Assert.Equal(frame.Length - 1, result.RemainingBufferLength);
    }

    [Fact]
    public void Splitter_preserves_partial_frame_until_remaining_bytes_arrive()
    {
        var frame = Frame(0x63, 0x04);
        var buffer = new PacketFrameReadBuffer();

        buffer.Append(frame.Take(3).ToArray());
        Assert.Empty(buffer.TryReadFrames().Frames);
        buffer.Append(frame.Skip(3).ToArray());
        var result = buffer.TryReadFrames();

        var receivedFrame = Assert.Single(result.Frames);
        Assert.Equal(frame, receivedFrame.FrameBytes);
        Assert.Equal(0, result.RemainingBufferLength);
    }

    [Fact]
    public void Splitter_extracts_one_complete_frame()
    {
        var frame = Frame(0x00, 0x01);
        var buffer = new PacketFrameReadBuffer();

        buffer.Append(frame);
        var result = buffer.TryReadFrames();

        Assert.Single(result.Frames);
        Assert.Equal(PacketFrameSplitStatus.FramesAvailable, result.Status);
    }

    [Fact]
    public void Splitter_extracts_two_sticky_frames_from_one_chunk()
    {
        var first = Frame(0x00, 0x01);
        var second = Frame(0x63, 0x04);
        var buffer = new PacketFrameReadBuffer();

        buffer.Append(first.Concat(second).ToArray());
        var result = buffer.TryReadFrames();

        Assert.Equal(2, result.Frames.Count);
        Assert.Equal(first, result.Frames[0].FrameBytes);
        Assert.Equal(second, result.Frames[1].FrameBytes);
    }

    [Fact]
    public void Splitter_handles_frame_split_across_multiple_chunks()
    {
        var first = Frame(0x00, 0x01);
        var second = Frame(0x63, 0x04);
        var combined = first.Concat(second).ToArray();
        var buffer = new PacketFrameReadBuffer();

        buffer.Append(combined.Take(first.Length + 2).ToArray());
        var firstResult = buffer.TryReadFrames();
        buffer.Append(combined.Skip(first.Length + 2).ToArray());
        var secondResult = buffer.TryReadFrames();

        Assert.Single(firstResult.Frames);
        Assert.Single(secondResult.Frames);
        Assert.Equal(second, secondResult.Frames[0].FrameBytes);
    }

    [Fact]
    public void Splitter_resyncs_after_leading_noise_before_header()
    {
        var frame = Frame(0x63, 0x04);
        var buffer = new PacketFrameReadBuffer();

        buffer.Append(new byte[] { 0x01, 0x02, 0x03 }.Concat(frame).ToArray());
        var result = buffer.TryReadFrames();

        Assert.Single(result.Frames);
        Assert.Contains(result.Errors, error => error.Code == FrameSplitErrorCode.LeadingNoiseDiscarded);
    }

    [Fact]
    public void Splitter_reports_error_for_invalid_declared_length()
    {
        var buffer = new PacketFrameReadBuffer();

        buffer.Append(new byte[] { 0xF4, 0x44, 0x00, 0x00 });
        var result = buffer.TryReadFrames();

        Assert.Single(result.Frames);
        Assert.Contains(result.Errors, error => error.Code == FrameSplitErrorCode.InvalidDeclaredLength);
    }

    [Fact]
    public void Splitter_respects_max_frame_size()
    {
        var splitter = new PacketFrameSplitter(new PacketFrameSplitterOptions
        {
            MaxFrameSize = 5
        });
        var buffer = new PacketFrameReadBuffer(splitter);

        buffer.Append(Frame(0x63, 0x04));
        var result = buffer.TryReadFrames();

        Assert.Empty(result.Frames);
        Assert.Contains(result.Errors, error => error.Code == FrameSplitErrorCode.FrameTooLarge);
    }

    [Fact]
    public void Splitter_does_not_throw_on_malformed_bytes()
    {
        var buffer = new PacketFrameReadBuffer();

        var exception = Record.Exception(() =>
        {
            buffer.Append(new byte[] { 0x01, 0x02, 0x03, 0x04 });
            buffer.TryReadFrames();
        });

        Assert.Null(exception);
    }

    [Fact]
    public async Task Receive_loop_routes_two_sticky_frames_separately()
    {
        var sink = new InMemoryTraceSink();
        var receiveLoop = CreateReceiveLoop(sink);
        var context = CreateConnection(SessionState.HandshakeDone);
        var chunk = Frame(0x63, 0x04).Concat(Frame(0x63, 0x04)).ToArray();

        var result = await receiveLoop.ProcessChunkAsync(context, chunk);

        Assert.Equal(2, result.Packets.Count);
        Assert.Equal(2, sink.Events.Count);
        Assert.All(result.Packets, packet => Assert.Equal("LoginRequestCandidate", packet.RouteResult.ProtocolName));
    }

    [Fact]
    public async Task Receive_loop_preserves_packet_codec_validation_errors()
    {
        var sink = new InMemoryTraceSink();
        var receiveLoop = CreateReceiveLoop(sink);
        var context = CreateConnection(SessionState.Connected);

        var result = await receiveLoop.ProcessChunkAsync(context, new byte[] { 0xF4, 0x44, 0x00, 0x00 });

        var packet = Assert.Single(result.Packets);
        Assert.NotEmpty(packet.DecodeResult.ValidationErrors);
        Assert.Equal(packet.DecodeResult.ValidationErrors, packet.RouteResult.ValidationErrors);
    }

    [Fact]
    public async Task Receive_loop_routes_ac63_subac4_to_action_router_skeleton()
    {
        var sink = new InMemoryTraceSink();
        var receiveLoop = CreateReceiveLoop(sink);
        var context = CreateConnection(SessionState.HandshakeDone);

        var result = await receiveLoop.ProcessChunkAsync(context, Frame(0x63, 0x04));

        var packet = Assert.Single(result.Packets);
        Assert.Equal("LoginRequestCandidate", packet.RouteResult.ProtocolName);
        Assert.Equal(ActionRouteStatus.MissingHandler, packet.RouteResult.Status);
    }

    [Fact]
    public async Task Receive_loop_rejects_movement_candidate_in_connected_state_through_session_guard()
    {
        var sink = new InMemoryTraceSink();
        var receiveLoop = CreateReceiveLoop(sink);
        var context = CreateConnection(SessionState.Connected);

        var result = await receiveLoop.ProcessChunkAsync(context, Frame(0x06, 0x01));

        var packet = Assert.Single(result.Packets);
        Assert.Equal(ActionRouteStatus.RejectedBySessionGuard, packet.RouteResult.Status);
    }

    [Fact]
    public async Task Stop_async_cancels_receive_loop_cleanly()
    {
        await using var host = new TcpServerHost(new TcpServerOptions { Port = 0 });

        await host.StartAsync();
        await host.StopAsync();

        Assert.False(host.IsRunning);
    }

    [Fact]
    public async Task Synthetic_stream_traffic_is_not_marked_trace_client_confirmed()
    {
        var sink = new InMemoryTraceSink();
        var receiveLoop = CreateReceiveLoop(sink);
        var context = CreateConnection(SessionState.HandshakeDone);

        await receiveLoop.ProcessChunkAsync(context, Frame(0x63, 0x04));

        var traceEvent = Assert.Single(sink.Events);
        Assert.NotEqual(ProtocolTraceSourceLabel.TraceClient, traceEvent.SourceLabel);
        Assert.NotEqual(ProtocolTraceStatus.Confirmed, traceEvent.Status);
    }

    [Fact]
    public async Task Send_pipeline_does_not_generate_login_response_automatically()
    {
        var pipeline = new SendPipeline();
        var context = CreateConnection(SessionState.HandshakeDone);

        var result = await pipeline.QueueAsync(context, Frame(0x63, 0x04));
        var frames = pipeline.DrainQueuedFrames();

        Assert.Equal(NetworkRuntimeStatus.SendQueued, result.Status);
        Assert.Single(frames);
        Assert.Contains("no gameplay response", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static byte[] Frame(byte ac, byte? subAc)
    {
        return new PacketCodec().Encode(ac, subAc).EncodedBytes;
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
        return new ClientConnectionContext("stream-test-connection", null, DateTimeOffset.UtcNow)
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
