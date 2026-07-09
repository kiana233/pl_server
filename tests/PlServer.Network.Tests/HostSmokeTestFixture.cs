using System.Net;
using PlServer.Application;
using PlServer.Diagnostics;
using PlServer.LegacyProtocol;
using PlServer.Network;
using PlServer.Protocol;
using PlServer.Session;

namespace PlServer.Network.Tests;

internal sealed class HostSmokeTestFixture : IAsyncDisposable
{
    private readonly RecordingActionHandlerRegistry _handlerRegistry;

    public HostSmokeTestFixture()
    {
        TraceSink = new InMemoryProtocolTraceSink();
        ConnectionRegistry = new ConnectionRegistry();
        _handlerRegistry = new RecordingActionHandlerRegistry();
        RegisterCandidateHandlers(_handlerRegistry);
        Host = new TcpServerHost(
            new TcpServerOptions { Port = 0 },
            ConnectionRegistry,
            receivePipeline: CreateReceivePipeline(TraceSink, _handlerRegistry));
    }

    public InMemoryProtocolTraceSink TraceSink { get; }

    public ConnectionRegistry ConnectionRegistry { get; }

    public TcpServerHost Host { get; }

    public int HandlerResolveCount => _handlerRegistry.ResolveCount;

    public int HandlerRegisterCount => _handlerRegistry.RegisterCount;

    public IReadOnlyList<string> ResolvedHandlerNames => _handlerRegistry.ResolvedHandlerNames;

    public IPEndPoint BoundEndPoint => Assert.IsType<IPEndPoint>(Host.BoundEndPoint);

    public static byte[] Frame(byte ac, byte? subAc)
    {
        return new PacketCodec().Encode(ac, subAc).EncodedBytes;
    }

    public async Task StartAsync()
    {
        await Host.StartAsync().ConfigureAwait(false);
    }

    public async Task<SyntheticTcpClient> ConnectClientAsync()
    {
        var endpoint = BoundEndPoint;
        var client = new SyntheticTcpClient();
        await client.ConnectAsync(endpoint.Address.ToString(), endpoint.Port).ConfigureAwait(false);
        Assert.True(await WaitUntilAsync(() => ConnectionRegistry.GetAll().Count == 1).ConfigureAwait(false));
        return client;
    }

    public async Task<ClientConnectionContext> WaitForConnectionAsync()
    {
        ClientConnectionContext? connection = null;
        var found = await WaitUntilAsync(() =>
        {
            connection = ConnectionRegistry.GetAll().SingleOrDefault();
            return connection is not null;
        }).ConfigureAwait(false);

        Assert.True(found);
        return connection!;
    }

    public async Task<bool> WaitForSessionStateAsync(SessionState expectedState)
    {
        return await WaitUntilAsync(() =>
        {
            var connection = ConnectionRegistry.GetAll().SingleOrDefault();
            return connection?.CurrentSessionState == expectedState;
        }).ConfigureAwait(false);
    }

    public async Task<bool> WaitForTraceCountAsync(int expectedCount)
    {
        return await WaitUntilAsync(() => TraceSink.Events.Count >= expectedCount).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await Host.DisposeAsync().ConfigureAwait(false);
        TraceSink.Dispose();
    }

    private static ReceivePipeline CreateReceivePipeline(
        IProtocolTraceSink sink,
        IActionHandlerRegistry handlerRegistry)
    {
        var routePipeline = new PacketRoutePipeline(
            new PacketCodec(),
            new ProtocolTraceLogger(sink),
            new ActionRouter(
                LegacyProtocolContractCatalog.CreateDefaultRegistry(),
                handlerRegistry));

        return new ReceivePipeline(routePipeline);
    }

    private static void RegisterCandidateHandlers(IActionHandlerRegistry registry)
    {
        registry.Register(new ActionHandlerDescriptor(
            new LegacyProtocolKey(0x00, null),
            nameof(HandshakeCandidateHandler),
            new HandshakeCandidateHandler()));

        registry.Register(new ActionHandlerDescriptor(
            new LegacyProtocolKey(0x63, 0x04),
            nameof(LoginRequestCandidateHandler),
            new LoginRequestCandidateHandler()));
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

            await Task.Delay(25).ConfigureAwait(false);
        }

        return predicate();
    }

    internal sealed class InMemoryProtocolTraceSink : IProtocolTraceSink
    {
        private readonly object _gate = new();
        private readonly List<ProtocolTraceEvent> _events = new();

        public IReadOnlyList<ProtocolTraceEvent> Events
        {
            get
            {
                lock (_gate)
                {
                    return _events.ToArray();
                }
            }
        }

        public void Write(ProtocolTraceEvent traceEvent)
        {
            ArgumentNullException.ThrowIfNull(traceEvent);

            lock (_gate)
            {
                _events.Add(traceEvent);
            }
        }

        public void Flush()
        {
        }

        public void Dispose()
        {
        }
    }

    private sealed class RecordingActionHandlerRegistry : IActionHandlerRegistry
    {
        private readonly Dictionary<LegacyProtocolKey, ActionHandlerDescriptor> _handlers = new();
        private readonly List<string> _resolvedHandlerNames = new();

        public int ResolveCount { get; private set; }

        public int RegisterCount { get; private set; }

        public IReadOnlyList<string> ResolvedHandlerNames => _resolvedHandlerNames.ToArray();

        public void Register(ActionHandlerDescriptor descriptor)
        {
            ArgumentNullException.ThrowIfNull(descriptor);
            RegisterCount++;
            _handlers.Add(descriptor.ContractKey, descriptor);
        }

        public bool TryResolve(LegacyProtocolContract contract, out ActionHandlerDescriptor? descriptor)
        {
            ArgumentNullException.ThrowIfNull(contract);
            ResolveCount++;
            var found = _handlers.TryGetValue(contract.Key, out descriptor);
            if (found && descriptor is not null)
            {
                _resolvedHandlerNames.Add(descriptor.HandlerName);
            }

            return found;
        }
    }
}
