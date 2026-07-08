using System.Net.Sockets;

namespace PlServer.Network;

public sealed class ClientConnection : IAsyncDisposable
{
    private readonly TcpClient _tcpClient;
    private readonly IConnectionRegistry _registry;
    private int _disconnected;

    public ClientConnection(
        TcpClient tcpClient,
        ClientConnectionContext context,
        IConnectionRegistry registry)
    {
        _tcpClient = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
        Context = context ?? throw new ArgumentNullException(nameof(context));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public ClientConnectionContext Context { get; }

    public bool IsConnected => _tcpClient.Connected && _disconnected == 0;

    public NetworkStream GetStream()
    {
        return _tcpClient.GetStream();
    }

    public ValueTask DisconnectAsync()
    {
        if (Interlocked.Exchange(ref _disconnected, 1) == 0)
        {
            try
            {
                _tcpClient.Close();
            }
            finally
            {
                _registry.Remove(Context.ConnectionId);
                _tcpClient.Dispose();
            }
        }

        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync().ConfigureAwait(false);
    }
}
