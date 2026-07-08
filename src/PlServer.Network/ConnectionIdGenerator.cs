namespace PlServer.Network;

public sealed class ConnectionIdGenerator
{
    private long _nextId;

    public string NextId()
    {
        var value = Interlocked.Increment(ref _nextId);
        return $"connection-{value:D8}";
    }
}
