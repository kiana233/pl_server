using System.Collections.Concurrent;

namespace PlServer.Network;

public sealed class SendPipeline
{
    private readonly ConcurrentQueue<byte[]> _queuedFrames = new();

    public int Count => _queuedFrames.Count;

    public ValueTask<NetworkRuntimeResult> QueueAsync(
        ClientConnectionContext connection,
        byte[] bytes,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(bytes);
        cancellationToken.ThrowIfCancellationRequested();

        _queuedFrames.Enqueue(bytes.ToArray());

        return ValueTask.FromResult(new NetworkRuntimeResult(
            NetworkRuntimeStatus.SendQueued,
            "Outgoing bytes were queued by the skeleton send pipeline; no gameplay response was generated."));
    }

    public IReadOnlyList<byte[]> DrainQueuedFrames()
    {
        var frames = new List<byte[]>();
        while (_queuedFrames.TryDequeue(out var frame))
        {
            frames.Add(frame);
        }

        return frames;
    }
}
