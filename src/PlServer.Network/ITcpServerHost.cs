using System.Net;

namespace PlServer.Network;

public interface ITcpServerHost
{
    bool IsRunning { get; }

    EndPoint? BoundEndPoint { get; }

    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
