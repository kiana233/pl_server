namespace PlServer.Network;

public sealed class TcpServerOptions
{
    public string Host { get; init; } = "127.0.0.1";

    public int Port { get; init; }

    public int Backlog { get; init; } = 100;

    public int ReceiveBufferSize { get; init; } = 8192;

    public int MaxFrameSize { get; init; } = 64 * 1024;

    public bool EnableXor { get; init; }

    public string? TraceOutputPath { get; init; }

    public bool StartInConsole { get; init; }
}
