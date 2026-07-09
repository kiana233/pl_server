using PlServer.LegacyProtocol;

namespace PlServer.Application;

public sealed class OpaqueLoginRequestCandidateParser : ILoginRequestCandidateParser
{
    public string ParserName => nameof(OpaqueLoginRequestCandidateParser);

    public bool CanParse(ActionHandlerContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Contract.ProtocolName == "LoginRequestCandidate";
    }

    public LoginRequestParseResult Parse(ActionHandlerContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.Request.PacketDecodeResult.IsValid)
        {
            return LoginRequestParseResult.InvalidPacket(context);
        }

        var fields = new[]
        {
            new LoginRequestField(
                "ac",
                LoginRequestFieldKind.Ac,
                offset: null,
                length: 1,
                LoginRequestFieldSensitivity.Public,
                RedactedFieldValue.Public(context.Ac?.ToString("X2") ?? "unknown"),
                context.Contract.SourceLabelText,
                context.Contract.EvidenceStatus,
                new[] { "AC is decoded by PacketCodec, not login field parsing." }),
            new LoginRequestField(
                "subAc",
                LoginRequestFieldKind.SubAc,
                offset: null,
                length: context.SubAc.HasValue ? 1 : null,
                LoginRequestFieldSensitivity.Public,
                RedactedFieldValue.Public(context.SubAc?.ToString("X2") ?? "none"),
                context.Contract.SourceLabelText,
                context.Contract.EvidenceStatus,
                new[] { "SubAC is decoded by PacketCodec, not login field parsing." }),
            new LoginRequestField(
                "payload",
                LoginRequestFieldKind.OpaquePayload,
                offset: 0,
                length: context.PayloadLength,
                LoginRequestFieldSensitivity.UnknownSensitive,
                RedactedFieldValue.Redacted,
                "unknown",
                ProtocolEvidenceStatus.Unknown,
                new[]
                {
                    "field layout unknown",
                    "payload bytes intentionally not emitted"
                })
        };

        var notes = new[]
        {
            $"login request parser: {ParserName}",
            $"login request parser status: {LoginRequestParseStatus.OpaquePayload}",
            $"payload length: {context.PayloadLength}",
            $"ac: {context.Ac?.ToString("X2") ?? "unknown"}",
            $"subAc: {context.SubAc?.ToString("X2") ?? "none"}",
            "field layout unknown",
            "pending target-client trace",
            "no password/token emitted",
            "no account identifier emitted",
            "parser result is not confirmed"
        };

        return LoginRequestParseResult.OpaquePayload(context, fields, notes);
    }
}
