namespace PlServer.Network;

public sealed class ConnectionRegistry : IConnectionRegistry
{
    private readonly object _gate = new();
    private readonly Dictionary<string, ClientConnectionContext> _connections = new();

    public void Register(ClientConnectionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        lock (_gate)
        {
            if (_connections.ContainsKey(context.ConnectionId))
            {
                throw new InvalidOperationException($"Duplicate connection id: {context.ConnectionId}.");
            }

            _connections.Add(context.ConnectionId, context);
        }
    }

    public bool Remove(string connectionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);

        lock (_gate)
        {
            return _connections.Remove(connectionId);
        }
    }

    public ClientConnectionContext? Get(string connectionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);

        lock (_gate)
        {
            return _connections.TryGetValue(connectionId, out var context)
                ? context
                : null;
        }
    }

    public IReadOnlyList<ClientConnectionContext> GetAll()
    {
        lock (_gate)
        {
            return _connections.Values.ToArray();
        }
    }
}
