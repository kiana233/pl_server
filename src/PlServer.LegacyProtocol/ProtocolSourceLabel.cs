namespace PlServer.LegacyProtocol;

public enum ProtocolSourceLabel
{
    TraceClient,
    ReferenceMuayad,
    ReferenceWlophoenix,
    Assumption,
    Unknown
}

public static class ProtocolSourceLabelExtensions
{
    public static string ToContractString(this ProtocolSourceLabel sourceLabel)
    {
        return sourceLabel switch
        {
            ProtocolSourceLabel.TraceClient => "trace:client",
            ProtocolSourceLabel.ReferenceMuayad => "reference:muayad",
            ProtocolSourceLabel.ReferenceWlophoenix => "reference:wlophoenix",
            ProtocolSourceLabel.Assumption => "assumption",
            ProtocolSourceLabel.Unknown => "unknown",
            _ => "unknown"
        };
    }
}
