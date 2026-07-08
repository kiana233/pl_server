using System.Net.Sockets;

namespace PlServer.Network.Tests;

internal sealed class SyntheticTcpClient : IAsyncDisposable
{
    private readonly TcpClient _client = new();

    public int AvailableBytes => _client.Available;

    public async Task ConnectAsync(string host, int port, CancellationToken cancellationToken = default)
    {
        await _client.ConnectAsync(host, port, cancellationToken).ConfigureAwait(false);
    }

    public async Task SendAsync(byte[] bytes, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        var stream = _client.GetStream();
        await stream.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
        await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task SendChunksAsync(params byte[][] chunks)
    {
        ArgumentNullException.ThrowIfNull(chunks);

        foreach (var chunk in chunks)
        {
            await SendAsync(chunk).ConfigureAwait(false);
        }
    }

    public async Task<byte[]> ReadAvailableAsync(CancellationToken cancellationToken = default)
    {
        var available = _client.Available;
        if (available == 0)
        {
            return Array.Empty<byte>();
        }

        var buffer = new byte[available];
        var bytesRead = await _client
            .GetStream()
            .ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)
            .ConfigureAwait(false);

        return bytesRead == buffer.Length
            ? buffer
            : buffer[..bytesRead];
    }

    public Task DisconnectAsync()
    {
        _client.Close();
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync().ConfigureAwait(false);
        _client.Dispose();
    }
}
