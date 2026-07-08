using System.Net;
using PlServer.Session;

namespace PlServer.Network;

public sealed class ClientConnectionContext
{
    public ClientConnectionContext(
        string connectionId,
        EndPoint? remoteEndPoint,
        DateTimeOffset connectedAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
        ConnectionId = connectionId;
        RemoteEndPoint = remoteEndPoint;
        ConnectedAtUtc = connectedAtUtc;
    }

    public string ConnectionId { get; }

    public EndPoint? RemoteEndPoint { get; }

    public DateTimeOffset ConnectedAtUtc { get; }

    public SessionState CurrentSessionState { get; set; } = SessionState.Connected;

    public string? AccountName { get; set; }

    public string? CharacterName { get; set; }
}
