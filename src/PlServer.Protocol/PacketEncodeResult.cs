namespace PlServer.Protocol
{
    public sealed class PacketEncodeResult
    {
        public PacketEncodeResult(
            byte[] decodedBytes,
            byte[] encodedBytes,
            PacketHeader header,
            ushort payloadLength,
            byte[] payload,
            byte ac,
            byte? subAc)
        {
            DecodedBytes = decodedBytes;
            EncodedBytes = encodedBytes;
            Header = header;
            PayloadLength = payloadLength;
            Payload = payload;
            Ac = ac;
            SubAc = subAc;
        }

        public byte[] DecodedBytes { get; }

        public byte[] EncodedBytes { get; }

        public PacketHeader Header { get; }

        public ushort PayloadLength { get; }

        public byte[] Payload { get; }

        public byte Ac { get; }

        public byte? SubAc { get; }
    }
}
