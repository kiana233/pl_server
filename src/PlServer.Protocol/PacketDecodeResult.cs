namespace PlServer.Protocol
{

    public sealed class PacketDecodeResult
    {
        public PacketDecodeResult(
            byte[] rawBytes,
            byte[] decodedBytes,
            PacketHeader? header,
            ushort payloadLength,
            byte[] payload,
            byte? ac,
            byte? subAc,
            IReadOnlyList<PacketValidationError> validationErrors)
        {
            RawBytes = rawBytes;
            DecodedBytes = decodedBytes;
            Header = header;
            PayloadLength = payloadLength;
            Payload = payload;
            Ac = ac;
            SubAc = subAc;
            ValidationErrors = validationErrors;
        }

        public byte[] RawBytes { get; }

        public byte[] DecodedBytes { get; }

        public PacketHeader? Header { get; }

        public ushort PayloadLength { get; }

        public byte[] Payload { get; }

        public byte? Ac { get; }

        public byte? SubAc { get; }

        public bool IsValid => ValidationErrors.Count == 0;

        public IReadOnlyList<PacketValidationError> ValidationErrors { get; }

        public PacketFrame ToFrame()
        {
            return new PacketFrame(RawBytes, DecodedBytes, Header, PayloadLength, Payload, Ac, SubAc);
        }
    }

}
