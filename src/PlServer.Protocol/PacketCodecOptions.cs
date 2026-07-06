namespace PlServer.Protocol
{

    public sealed class PacketCodecOptions
    {
        public byte HeaderByte0 { get; init; } = 0xF4;

        public byte HeaderByte1 { get; init; } = 0x44;

        public int LengthOffset { get; init; } = 2;

        public int LengthSize { get; init; } = 2;

        public bool LengthIsLittleEndian { get; init; } = true;

        public int PayloadOffset { get; init; } = 4;

        public int AcOffsetInPayload { get; init; } = 0;

        public int SubAcOffsetInPayload { get; init; } = 1;

        public byte XorKey { get; init; } = 0xAD;

        public bool XorEnabled { get; init; }

        public XorScope XorScope { get; init; } = XorScope.WholeFrame;

        public int HeaderLength => PayloadOffset;

        public static PacketCodecOptions Default { get; } = new();

        public void Validate()
        {
            if (LengthSize != 2)
            {
                throw new ArgumentOutOfRangeException(nameof(LengthSize), LengthSize, "Only 2-byte length fields are supported.");
            }

            if (!LengthIsLittleEndian)
            {
                throw new NotSupportedException("Only little-endian length fields are supported.");
            }

            if (LengthOffset < 0
                || PayloadOffset < 0
                || AcOffsetInPayload < 0
                || SubAcOffsetInPayload < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(PacketCodecOptions), "Offsets cannot be negative.");
            }

            if (LengthOffset + LengthSize > PayloadOffset)
            {
                throw new InvalidOperationException("Length field must fit before the payload offset.");
            }

            if (XorScope != XorScope.WholeFrame)
            {
                throw new NotSupportedException("Only whole-frame XOR is supported.");
            }
        }
    }

}
