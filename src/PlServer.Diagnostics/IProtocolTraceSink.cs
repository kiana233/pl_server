namespace PlServer.Diagnostics;

public interface IProtocolTraceSink : IDisposable
{
    void Write(ProtocolTraceEvent traceEvent);

    void Flush();
}
