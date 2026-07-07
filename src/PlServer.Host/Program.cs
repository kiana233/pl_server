using PlServer.Network;

Console.WriteLine("梦幻服务端 Host 骨架");

if (!args.Contains("--listen", StringComparer.OrdinalIgnoreCase))
{
    Console.WriteLine("默认不启动 TCP 监听。使用 --listen --host 127.0.0.1 --port 0 启动骨架监听。");
    return;
}

var host = ReadOption(args, "--host") ?? "127.0.0.1";
var portText = ReadOption(args, "--port") ?? "0";

if (!int.TryParse(portText, out var port))
{
    Console.WriteLine("端口参数无效。");
    return;
}

var options = new TcpServerOptions
{
    Host = host,
    Port = port,
    StartInConsole = true
};

await using var server = new TcpServerHost(options);
using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cts.Cancel();
};

await server.StartAsync(cts.Token);
Console.WriteLine($"TCP Host 骨架已监听: {server.BoundEndPoint}");
Console.WriteLine("按 Ctrl+C 停止。");

try
{
    await Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);
}
catch (OperationCanceledException)
{
}
finally
{
    await server.StopAsync();
}

static string? ReadOption(string[] args, string name)
{
    for (var index = 0; index < args.Length - 1; index++)
    {
        if (string.Equals(args[index], name, StringComparison.OrdinalIgnoreCase))
        {
            return args[index + 1];
        }
    }

    return null;
}
