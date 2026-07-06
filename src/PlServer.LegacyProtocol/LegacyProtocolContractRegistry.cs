namespace PlServer.LegacyProtocol;

public sealed class LegacyProtocolContractRegistry
{
    private readonly Dictionary<LegacyProtocolKey, LegacyProtocolContract> _contracts = new();

    public void Register(LegacyProtocolContract contract)
    {
        ArgumentNullException.ThrowIfNull(contract);

        if (_contracts.ContainsKey(contract.Key))
        {
            throw new InvalidOperationException($"Duplicate legacy protocol contract key: {contract.Key}.");
        }

        _contracts.Add(contract.Key, contract);
    }

    public bool TryResolve(
        byte ac,
        byte? subAc,
        LegacyPacketDirection direction,
        out LegacyProtocolContract contract)
    {
        var result = ResolveCore(ac, subAc, direction, includeUnknownFallback: false);
        contract = result.Contract;
        return result.Found;
    }

    public LegacyProtocolContractLookupResult ResolveOrUnknown(
        byte ac,
        byte? subAc,
        LegacyPacketDirection direction = LegacyPacketDirection.Any)
    {
        return ResolveCore(ac, subAc, direction, includeUnknownFallback: true);
    }

    public IReadOnlyList<LegacyProtocolContract> GetAll()
    {
        return _contracts.Values.ToArray();
    }

    private LegacyProtocolContractLookupResult ResolveCore(
        byte ac,
        byte? subAc,
        LegacyPacketDirection direction,
        bool includeUnknownFallback)
    {
        if (_contracts.TryGetValue(new LegacyProtocolKey(ac, subAc, direction), out var exactDirection))
        {
            return new LegacyProtocolContractLookupResult(true, exactDirection, LegacyProtocolContractMatchKind.Exact);
        }

        if (direction != LegacyPacketDirection.Any
            && _contracts.TryGetValue(new LegacyProtocolKey(ac, subAc, LegacyPacketDirection.Any), out var exactAny))
        {
            return new LegacyProtocolContractLookupResult(true, exactAny, LegacyProtocolContractMatchKind.Exact);
        }

        if (_contracts.TryGetValue(new LegacyProtocolKey(ac, null, direction), out var acOnlyDirection))
        {
            return new LegacyProtocolContractLookupResult(true, acOnlyDirection, LegacyProtocolContractMatchKind.AcOnly);
        }

        if (direction != LegacyPacketDirection.Any
            && _contracts.TryGetValue(new LegacyProtocolKey(ac, null, LegacyPacketDirection.Any), out var acOnlyAny))
        {
            return new LegacyProtocolContractLookupResult(true, acOnlyAny, LegacyProtocolContractMatchKind.AcOnly);
        }

        if (includeUnknownFallback && _contracts.TryGetValue(LegacyProtocolKey.Unknown, out var unknown))
        {
            return new LegacyProtocolContractLookupResult(false, unknown, LegacyProtocolContractMatchKind.UnknownFallback);
        }

        return new LegacyProtocolContractLookupResult(
            false,
            LegacyProtocolContractCatalog.UnknownFallbackContract,
            LegacyProtocolContractMatchKind.UnknownFallback);
    }
}
