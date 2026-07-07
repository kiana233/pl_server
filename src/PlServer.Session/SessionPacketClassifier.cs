using PlServer.Diagnostics;
using PlServer.Protocol;

namespace PlServer.Session;

public sealed class SessionPacketClassifier
{
    public SessionPacketClassification Classify(PacketDecodeResult? decodeResult)
    {
        if (decodeResult is null)
        {
            return Invalid(null, null, Array.Empty<PacketValidationError>(), "Decode result is null.");
        }

        if (!decodeResult.IsValid)
        {
            return Invalid(
                decodeResult.Ac,
                decodeResult.SubAc,
                decodeResult.ValidationErrors,
                "Decode result contains validation errors.");
        }

        return Classify(decodeResult.Ac, decodeResult.SubAc);
    }

    public SessionPacketClassification Classify(byte? ac, byte? subAc)
    {
        return ac switch
        {
            0x00 => Known(
                SessionPacketKind.HandshakeCandidate,
                ac,
                subAc,
                ProtocolTraceSourceLabel.ReferenceMuayad,
                "AC0 is a handshake candidate."),

            0x63 when subAc == 0x04 => Known(
                SessionPacketKind.LoginRequestCandidate,
                ac,
                subAc,
                ProtocolTraceSourceLabel.ReferenceMuayad,
                "AC63/SubAC4 is a login request candidate."),

            0x63 when subAc == 0x01 => Known(
                SessionPacketKind.CharacterListCandidate,
                ac,
                subAc,
                ProtocolTraceSourceLabel.ReferenceMuayad,
                "AC63/SubAC1 is treated as a character-list candidate until target-client traces disambiguate it."),

            0x63 when subAc == 0x02 => Known(
                SessionPacketKind.CharacterSelectCandidate,
                ac,
                subAc,
                ProtocolTraceSourceLabel.ReferenceMuayad,
                "AC63/SubAC2 is a character-select candidate."),

            0x12 => Known(
                SessionPacketKind.EnterMapCandidate,
                ac,
                subAc,
                ProtocolTraceSourceLabel.Assumption,
                "AC12 is an enter-map candidate assumption."),

            0x20 => Known(
                SessionPacketKind.EnterMapCandidate,
                ac,
                subAc,
                ProtocolTraceSourceLabel.Assumption,
                "AC20 is an enter-map candidate assumption."),

            0x06 when subAc == 0x01 => Known(
                SessionPacketKind.MovementCandidate,
                ac,
                subAc,
                ProtocolTraceSourceLabel.ReferenceMuayad,
                "AC06/SubAC1 is a movement candidate."),

            _ => Known(
                SessionPacketKind.Unknown,
                ac,
                subAc,
                ProtocolTraceSourceLabel.Assumption,
                "No session packet classification rule matched.")
        };
    }

    public static SessionPacketClassification Explicit(
        SessionPacketKind kind,
        string reason,
        byte? ac = null,
        byte? subAc = null)
    {
        return new SessionPacketClassification(
            kind,
            ac,
            subAc,
            ProtocolTraceSourceLabel.Assumption,
            ProtocolTraceStatus.PendingTargetClientTrace,
            reason);
    }

    private static SessionPacketClassification Known(
        SessionPacketKind kind,
        byte? ac,
        byte? subAc,
        ProtocolTraceSourceLabel sourceLabel,
        string reason)
    {
        return new SessionPacketClassification(
            kind,
            ac,
            subAc,
            sourceLabel,
            ProtocolTraceStatus.PendingTargetClientTrace,
            reason);
    }

    private static SessionPacketClassification Invalid(
        byte? ac,
        byte? subAc,
        IReadOnlyList<PacketValidationError> validationErrors,
        string reason)
    {
        return new SessionPacketClassification(
            SessionPacketKind.InvalidPacket,
            ac,
            subAc,
            ProtocolTraceSourceLabel.Assumption,
            ProtocolTraceStatus.PendingTargetClientTrace,
            reason,
            validationErrors);
    }
}
