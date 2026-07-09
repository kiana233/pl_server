using PlServer.LegacyProtocol;
using PlServer.Session;

namespace PlServer.Application;

public sealed class LoginRequestCandidateHandler : IActionHandler
{
    private readonly LoginRequestCandidateParserRegistry parserRegistry;

    public LoginRequestCandidateHandler()
        : this(LoginRequestCandidateParserRegistry.CreateDefault())
    {
    }

    public LoginRequestCandidateHandler(LoginRequestCandidateParserRegistry parserRegistry)
    {
        this.parserRegistry = parserRegistry ?? throw new ArgumentNullException(nameof(parserRegistry));
    }

    public string HandlerName => nameof(LoginRequestCandidateHandler);

    public bool CanHandle(LegacyProtocolContract contract)
    {
        ArgumentNullException.ThrowIfNull(contract);
        return contract.ProtocolName == "LoginRequestCandidate";
    }

    public ValueTask<ActionRouteResult> HandleAsync(
        ActionRouteRequest request,
        LegacyProtocolContract contract,
        SessionPacketKind packetKind,
        SessionStateGuardResult sessionGuardResult,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(contract);
        ArgumentNullException.ThrowIfNull(sessionGuardResult);

        cancellationToken.ThrowIfCancellationRequested();

        var context = new ActionHandlerContext(request, contract, packetKind, sessionGuardResult);
        var parseResult = parserRegistry.Parse(context);
        var notes = new List<string>
        {
            "login request candidate-only handler invoked",
            "pending target-client trace",
            "payload kept opaque; no account authentication performed",
            "account repository not invoked because login field layout is unknown",
            "no login response packet generated",
            "no character list generated",
            $"parser status: {parseResult.Status}",
            $"parser source: {parseResult.SourceLabel}",
            $"parser evidence status: {parseResult.EvidenceStatus}",
            $"parser is confirmed: {parseResult.IsConfirmed}",
            $"parser payload length: {parseResult.PayloadLength}"
        };
        notes.AddRange(parseResult.Notes);

        var handlerResult = new ActionHandlerResult(
            HandlerName,
            ActionHandlerStatus.CandidateHandled,
            notes);

        return ValueTask.FromResult(ActionRouteResult.FromHandler(
            contract,
            packetKind,
            sessionGuardResult,
            request.PacketDecodeResult.ValidationErrors,
            handlerResult));
    }
}
