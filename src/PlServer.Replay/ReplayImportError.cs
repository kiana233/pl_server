namespace PlServer.Replay;

public sealed record ReplayImportError(
    int LineNumber,
    string Message);
