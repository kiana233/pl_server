using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using PlServer.Application;
using PlServer.Diagnostics;
using PlServer.LegacyProtocol;
using PlServer.Protocol;

namespace PlServer.Network;

public sealed class TcpServerHost : ITcpServerHost, IAsyncDisposable
{
    private readonly TcpServerOptions _options;
    private readonly IConnectionRegistry _connectionRegistry;
    private readonly ConnectionIdGenerator _connectionIdGenerator;
    private readonly ReceivePipeline _receivePipeline;
    private readonly PacketFrameSplitterOptions _splitterOptions;
    private readonly ConcurrentDictionary<string, ClientConnection> _connections = new();
    private readonly object _gate = new();

    private TcpListener? _listener;
    private CancellationTokenSource? _lifetimeCts;
    private Task? _acceptLoopTask;

    public TcpServerHost(
        TcpServerOptions? options = null,
        IConnectionRegistry? connectionRegistry = null,
        ConnectionIdGenerator? connectionIdGenerator = null,
        ReceivePipeline? receivePipeline = null)
    {
        _options = options ?? new TcpServerOptions();
        _connectionRegistry = connectionRegistry ?? new ConnectionRegistry();
        _connectionIdGenerator = connectionIdGenerator ?? new ConnectionIdGenerator();
        _receivePipeline = receivePipeline ?? CreateDefaultReceivePipeline(_options);
        _splitterOptions = new PacketFrameSplitterOptions
        {
            CodecOptions = new PacketCodecOptions
            {
                XorEnabled = _options.EnableXor
            },
            MaxFrameSize = _options.MaxFrameSize
        };
    }

    public bool IsRunning { get; private set; }

    public EndPoint? BoundEndPoint { get; private set; }

    public IConnectionRegistry ConnectionRegistry => _connectionRegistry;

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            if (IsRunning)
            {
                return Task.CompletedTask;
            }

            var address = IPAddress.Parse(_options.Host);
            _listener = new TcpListener(address, _options.Port);
            _listener.Start(_options.Backlog);
            BoundEndPoint = _listener.LocalEndpoint;
            _lifetimeCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            IsRunning = true;
            _acceptLoopTask = Task.Run(() => AcceptLoopAsync(_lifetimeCts.Token), CancellationToken.None);
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        Task? acceptLoopTask;
        IReadOnlyList<ClientConnection> connections;

        lock (_gate)
        {
            if (!IsRunning && _listener is null)
            {
                return;
            }

            IsRunning = false;
            _lifetimeCts?.Cancel();
            _listener?.Stop();
            _listener = null;
            acceptLoopTask = _acceptLoopTask;
            connections = _connections.Values.ToArray();
        }

        foreach (var connection in connections)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await connection.DisconnectAsync().ConfigureAwait(false);
            _connections.TryRemove(connection.Context.ConnectionId, out _);
        }

        if (acceptLoopTask is not null)
        {
            try
            {
                await acceptLoopTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException) when (!IsRunning)
            {
            }
        }

        _lifetimeCts?.Dispose();
        _lifetimeCts = null;
        _acceptLoopTask = null;
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync().ConfigureAwait(false);
    }

    private async Task AcceptLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            TcpClient tcpClient;

            try
            {
                tcpClient = await _listener!.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (SocketException) when (cancellationToken.IsCancellationRequested || !IsRunning)
            {
                break;
            }

            tcpClient.ReceiveBufferSize = _options.ReceiveBufferSize;

            var context = new ClientConnectionContext(
                _connectionIdGenerator.NextId(),
                tcpClient.Client.RemoteEndPoint,
                DateTimeOffset.UtcNow);
            _connectionRegistry.Register(context);

            var connection = new ClientConnection(tcpClient, context, _connectionRegistry);
            _connections.TryAdd(context.ConnectionId, connection);
            _ = Task.Run(() => ProcessConnectionAsync(connection, cancellationToken), CancellationToken.None);
        }
    }

    private async Task ProcessConnectionAsync(ClientConnection connection, CancellationToken cancellationToken)
    {
        var buffer = new byte[_options.ReceiveBufferSize];
        var receiveLoop = new ConnectionReceiveLoop(
            _receivePipeline,
            new PacketFrameReadBuffer(new PacketFrameSplitter(_splitterOptions)));

        try
        {
            await using (connection.ConfigureAwait(false))
            {
                var stream = connection.GetStream();

                while (!cancellationToken.IsCancellationRequested)
                {
                    var bytesRead = await stream
                        .ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)
                        .ConfigureAwait(false);

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    var chunk = buffer.AsSpan(0, bytesRead).ToArray();
                    await receiveLoop
                        .ProcessChunkAsync(connection.Context, chunk, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (IOException) when (cancellationToken.IsCancellationRequested || !IsRunning)
        {
        }
        catch (SocketException) when (cancellationToken.IsCancellationRequested || !IsRunning)
        {
        }
        finally
        {
            await connection.DisconnectAsync().ConfigureAwait(false);
            _connections.TryRemove(connection.Context.ConnectionId, out _);
        }
    }

    private static ReceivePipeline CreateDefaultReceivePipeline(TcpServerOptions options)
    {
        var packetCodec = new PacketCodec(new PacketCodecOptions
        {
            XorEnabled = options.EnableXor
        });

        IProtocolTraceSink sink = string.IsNullOrWhiteSpace(options.TraceOutputPath)
            ? new NullProtocolTraceSink()
            : new JsonLinesProtocolTraceSink(options.TraceOutputPath);
        var traceLogger = new ProtocolTraceLogger(sink);
        var actionRouter = new ActionRouter(
            LegacyProtocolContractCatalog.CreateDefaultRegistry(),
            CandidateActionHandlerCatalog.CreateDefaultRegistry());

        return new ReceivePipeline(new PacketRoutePipeline(packetCodec, traceLogger, actionRouter));
    }
}
