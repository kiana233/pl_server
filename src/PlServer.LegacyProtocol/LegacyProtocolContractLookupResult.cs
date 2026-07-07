namespace PlServer.LegacyProtocol;

public sealed record LegacyProtocolContractLookupResult(
    bool Found,
    LegacyProtocolContract Contract,
    LegacyProtocolContractMatchKind MatchKind);
