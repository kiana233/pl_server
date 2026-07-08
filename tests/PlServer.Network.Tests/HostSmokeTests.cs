using PlServer.Application;
using PlServer.Diagnostics;
using PlServer.Session;

namespace PlServer.Network.Tests;

public sealed class HostSmokeTests
{
    [Fact]
    public async Task Synthetic_client_can_connect_to_loopback_host_on_port_zero()
    {
        await using var fixture = new HostSmokeTestFixture();
        await fixture.StartAsync();

        await using var client = await fixture.ConnectClientAsync();

        Assert.True(fixture.Host.IsRunning);
        Assert.NotEqual(0, fixture.BoundEndPoint.Port);
        Assert.Equal(SessionState.Connected, (await fixture.WaitForConnectionAsync()).CurrentSessionState);
    }

    [Fact]
    public async Task Host_processes_ac0_handshake_candidate_and_reaches_handshake_done()
    {
        await using var fixture = new HostSmokeTestFixture();
        await fixture.StartAsync();
        await using var client = await fixture.ConnectClientAsync();

        await client.SendAsync(HostSmokeTestFixture.Frame(0x00, 0x01));

        Assert.True(await fixture.WaitForSessionStateAsync(SessionState.HandshakeDone));
        Assert.True(await fixture.WaitForTraceCountAsync(1));
        AssertStateChange(
            Assert.Single(fixture.TraceSink.Events).StateChange,
            "Connected",
            "HandshakeDone",
            "HandshakeCandidate",
            wasStateChanged: true);
    }

    [Fact]
    public async Task Host_processes_login_candidate_after_handshake_and_reaches_login_pending()
    {
        await using var fixture = new HostSmokeTestFixture();
        await fixture.StartAsync();
        await using var client = await fixture.ConnectClientAsync();

        await client.SendAsync(HostSmokeTestFixture.Frame(0x00, 0x01));
        Assert.True(await fixture.WaitForSessionStateAsync(SessionState.HandshakeDone));
        await client.SendAsync(HostSmokeTestFixture.Frame(0x63, 0x04));

        Assert.True(await fixture.WaitForSessionStateAsync(SessionState.LoginPending));
        Assert.True(await fixture.WaitForTraceCountAsync(2));
        AssertStateChange(
            fixture.TraceSink.Events[1].StateChange,
            "HandshakeDone",
            "LoginPending",
            "LoginRequestCandidate",
            wasStateChanged: true);
    }

    [Fact]
    public async Task Host_processes_sticky_frames_sequentially_from_one_tcp_write()
    {
        await using var fixture = new HostSmokeTestFixture();
        await fixture.StartAsync();
        await using var client = await fixture.ConnectClientAsync();
        var stickyFrames = HostSmokeTestFixture.Frame(0x00, 0x01)
            .Concat(HostSmokeTestFixture.Frame(0x63, 0x04))
            .ToArray();

        await client.SendAsync(stickyFrames);

        Assert.True(await fixture.WaitForSessionStateAsync(SessionState.LoginPending));
        Assert.True(await fixture.WaitForTraceCountAsync(2));
        Assert.Collection(
            fixture.TraceSink.Events.Take(2),
            first => Assert.Equal((byte?)0x00, first.Ac),
            second =>
            {
                Assert.Equal((byte?)0x63, second.Ac);
                Assert.Equal((byte?)0x04, second.SubAc);
            });
        AssertStateChange(
            fixture.TraceSink.Events[0].StateChange,
            "Connected",
            "HandshakeDone",
            "HandshakeCandidate",
            wasStateChanged: true);
        AssertStateChange(
            fixture.TraceSink.Events[1].StateChange,
            "HandshakeDone",
            "LoginPending",
            "LoginRequestCandidate",
            wasStateChanged: true);
    }

    [Fact]
    public async Task Host_waits_for_partial_frame_completion_before_updating_state()
    {
        await using var fixture = new HostSmokeTestFixture();
        await fixture.StartAsync();
        await using var client = await fixture.ConnectClientAsync();
        var frame = HostSmokeTestFixture.Frame(0x00, 0x01);

        await client.SendAsync(frame.Take(3).ToArray());
        await Task.Delay(100);
        Assert.Equal(SessionState.Connected, (await fixture.WaitForConnectionAsync()).CurrentSessionState);
        Assert.Empty(fixture.TraceSink.Events);

        await client.SendAsync(frame.Skip(3).ToArray());

        Assert.True(await fixture.WaitForSessionStateAsync(SessionState.HandshakeDone));
        Assert.True(await fixture.WaitForTraceCountAsync(1));
    }

    [Fact]
    public async Task Host_ignores_malformed_bytes_without_crashing_and_can_process_later_frame()
    {
        await using var fixture = new HostSmokeTestFixture();
        await fixture.StartAsync();
        await using var client = await fixture.ConnectClientAsync();

        await client.SendAsync(new byte[] { 0x01, 0x02, 0x03, 0x04 });
        await Task.Delay(100);
        Assert.True(fixture.Host.IsRunning);
        Assert.Equal(SessionState.Connected, (await fixture.WaitForConnectionAsync()).CurrentSessionState);

        await client.SendAsync(HostSmokeTestFixture.Frame(0x00, 0x01));

        Assert.True(await fixture.WaitForSessionStateAsync(SessionState.HandshakeDone));
    }

    [Fact]
    public async Task Host_rejects_movement_candidate_before_in_map_and_keeps_connected_state()
    {
        await using var fixture = new HostSmokeTestFixture();
        await fixture.StartAsync();
        await using var client = await fixture.ConnectClientAsync();

        await client.SendAsync(HostSmokeTestFixture.Frame(0x06, 0x01));
        Assert.True(await fixture.WaitForTraceCountAsync(1));
        await Task.Delay(100);

        Assert.Equal(SessionState.Connected, (await fixture.WaitForConnectionAsync()).CurrentSessionState);
        var stateChange = Assert.Single(fixture.TraceSink.Events).StateChange;
        AssertStateChange(
            stateChange,
            "Connected",
            "Connected",
            "MovementCandidate",
            wasStateChanged: false);
        Assert.NotNull(stateChange!.RejectionReason);
        Assert.NotEmpty(stateChange.Errors);
    }

    [Fact]
    public async Task Host_invalid_packet_trace_preserves_validation_errors_and_no_false_state_change()
    {
        await using var fixture = new HostSmokeTestFixture();
        await fixture.StartAsync();
        await using var client = await fixture.ConnectClientAsync();

        await client.SendAsync(new byte[] { 0xF4, 0x44, 0x00, 0x00 });

        Assert.True(await fixture.WaitForTraceCountAsync(1));
        var traceEvent = Assert.Single(fixture.TraceSink.Events);
        Assert.NotEmpty(traceEvent.ValidationErrors);
        AssertStateChange(
            traceEvent.StateChange,
            "Connected",
            "Connected",
            "InvalidPacket",
            wasStateChanged: false);
    }

    [Fact]
    public async Task Host_writes_trace_event_for_synthetic_packet_without_trace_client_confirmation()
    {
        await using var fixture = new HostSmokeTestFixture();
        await fixture.StartAsync();
        await using var client = await fixture.ConnectClientAsync();

        await client.SendAsync(HostSmokeTestFixture.Frame(0x63, 0x04));

        Assert.True(await fixture.WaitForTraceCountAsync(1));
        var traceEvent = Assert.Single(fixture.TraceSink.Events);
        Assert.Equal((byte?)0x63, traceEvent.Ac);
        Assert.Equal((byte?)0x04, traceEvent.SubAc);
        Assert.NotEqual(ProtocolTraceSourceLabel.TraceClient, traceEvent.SourceLabel);
        Assert.NotEqual(ProtocolTraceStatus.Confirmed, traceEvent.Status);
        Assert.NotNull(traceEvent.StateChange);
    }

    [Fact]
    public async Task Host_does_not_send_login_response_automatically()
    {
        await using var fixture = new HostSmokeTestFixture();
        await fixture.StartAsync();
        await using var client = await fixture.ConnectClientAsync();

        await client.SendChunksAsync(
            HostSmokeTestFixture.Frame(0x00, 0x01),
            HostSmokeTestFixture.Frame(0x63, 0x04));
        Assert.True(await fixture.WaitForSessionStateAsync(SessionState.LoginPending));
        await Task.Delay(100);

        Assert.Empty(await client.ReadAvailableAsync());
        Assert.Equal(0, client.AvailableBytes);
    }

    [Fact]
    public async Task Host_uses_empty_action_handler_registry_so_no_real_ac_handler_is_invoked()
    {
        await using var fixture = new HostSmokeTestFixture();
        await fixture.StartAsync();
        await using var client = await fixture.ConnectClientAsync();

        await client.SendChunksAsync(
            HostSmokeTestFixture.Frame(0x00, 0x01),
            HostSmokeTestFixture.Frame(0x63, 0x04));

        Assert.True(await fixture.WaitForSessionStateAsync(SessionState.LoginPending));
        Assert.True(fixture.HandlerResolveCount >= 2);
        Assert.Equal(0, fixture.HandlerRegisterCount);
    }

    [Fact]
    public async Task Stop_async_shuts_down_host_and_active_synthetic_client_cleanly()
    {
        await using var fixture = new HostSmokeTestFixture();
        await fixture.StartAsync();
        await using var client = await fixture.ConnectClientAsync();

        await fixture.Host.StopAsync();
        await client.DisconnectAsync();

        Assert.False(fixture.Host.IsRunning);
        Assert.Empty(fixture.ConnectionRegistry.GetAll());
    }

    [Fact]
    public async Task Host_smoke_tests_use_in_memory_trace_sink_without_temp_trace_files()
    {
        await using var fixture = new HostSmokeTestFixture();
        await fixture.StartAsync();
        await using var client = await fixture.ConnectClientAsync();

        await client.SendAsync(HostSmokeTestFixture.Frame(0x00, 0x01));

        Assert.True(await fixture.WaitForTraceCountAsync(1));
        Assert.IsType<HostSmokeTestFixture.InMemoryProtocolTraceSink>(fixture.TraceSink);
    }

    private static void AssertStateChange(
        ProtocolTraceStateChange? stateChange,
        string previousState,
        string currentState,
        string packetKind,
        bool wasStateChanged)
    {
        Assert.NotNull(stateChange);
        Assert.Equal(previousState, stateChange.PreviousState);
        Assert.Equal(currentState, stateChange.CurrentState);
        Assert.Equal(packetKind, stateChange.PacketKind);
        Assert.Equal(wasStateChanged, stateChange.WasStateChanged);
    }
}
