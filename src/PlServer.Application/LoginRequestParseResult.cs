using PlServer.LegacyProtocol;
using PlServer.Protocol;

namespace PlServer.Application;

public sealed class LoginRequestParseResult
{
    private LoginRequestParseResult(
        LoginRequestParseStatus status,
        string sourceLabel,
        ProtocolEvidenceStatus evidenceStatus,
        int payloadLength,
        IReadOnlyList<LoginRequestField>? fields = null,
        IReadOnlyList<string>? notes = null,
        IReadOnlyList<PacketValidationError>? validationErrors = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceLabel);

        Status = status;
        SourceLabel = sourceLabel;
        EvidenceStatus = evidenceStatus;
        PayloadLength = payloadLength;
        Fields = fields ?? Array.Empty<LoginRequestField>();
        Notes = notes ?? Array.Empty<string>();
        ValidationErrors = validationErrors ?? Array.Empty<PacketValidationError>();
    }

    public LoginRequestParseStatus Status { get; }

    public string SourceLabel { get; }

    public ProtocolEvidenceStatus EvidenceStatus { get; }

    public int PayloadLength { get; }

    public IReadOnlyList<LoginRequestField> Fields { get; }

    public IReadOnlyList<string> Notes { get; }

    public IReadOnlyList<PacketValidationError> ValidationErrors { get; }

    public bool IsConfirmed => false;

    public static LoginRequestParseResult InvalidPacket(
        ActionHandlerContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return new LoginRequestParseResult(
            LoginRequestParseStatus.InvalidPacket,
            "unknown",
            ProtocolEvidenceStatus.Unknown,
            context.PayloadLength,
            validationErrors: context.Request.PacketDecodeResult.ValidationErrors,
            notes: new[]
            {
                "login request parser status: InvalidPacket",
                "invalid packet; parser did not inspect payload fields",
                "no password/token emitted"
            });
    }

    public static LoginRequestParseResult OpaquePayload(
        ActionHandlerContext context,
        IReadOnlyList<LoginRequestField> fields,
        IReadOnlyList<string> notes)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(fields);
        ArgumentNullException.ThrowIfNull(notes);

        return new LoginRequestParseResult(
            LoginRequestParseStatus.OpaquePayload,
            context.Contract.SourceLabelText,
            context.Contract.EvidenceStatus,
            context.PayloadLength,
            fields,
            notes,
            context.Request.PacketDecodeResult.ValidationErrors);
    }

    public static LoginRequestParseResult ParsedCandidate(
        ActionHandlerContext context,
        IReadOnlyList<LoginRequestField> fields,
        IReadOnlyList<string> notes)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(fields);
        ArgumentNullException.ThrowIfNull(notes);

        return new LoginRequestParseResult(
            LoginRequestParseStatus.ParsedCandidate,
            context.Contract.SourceLabelText,
            context.Contract.EvidenceStatus,
            context.PayloadLength,
            fields,
            notes,
            context.Request.PacketDecodeResult.ValidationErrors);
    }
}
