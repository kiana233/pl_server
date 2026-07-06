using System.Text;

namespace PlServer.Diagnostics;

public sealed class JsonLinesProtocolTraceSink : IProtocolTraceSink
{
    private readonly StreamWriter _writer;

    public JsonLinesProtocolTraceSink(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Trace file path is required.", nameof(filePath));
        }

        var directory = Path.GetDirectoryName(Path.GetFullPath(filePath));
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
        _writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    public void Write(ProtocolTraceEvent traceEvent)
    {
        _writer.WriteLine(ProtocolTraceFormatter.FormatJsonLine(traceEvent));
    }

    public void Flush()
    {
        _writer.Flush();
    }

    public void Dispose()
    {
        _writer.Dispose();
    }
}
