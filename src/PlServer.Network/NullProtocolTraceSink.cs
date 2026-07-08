using PlServer.Diagnostics;

namespace PlServer.Network;

public sealed class NullProtocolTraceSink : IProtocolTraceSink
{
    public void Write(ProtocolTraceEvent traceEvent)
    {
        ArgumentNullException.ThrowIfNull(traceEvent);
    }

    public void Flush()
    {
    }

    public void Dispose()
    {
    }
}
