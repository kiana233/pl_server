namespace PlServer.Protocol
{
    public sealed record PacketHeader(
        byte Byte0,
        byte Byte1,
        ushort PayloadLength)
    {
        public bool Matches(PacketCodecOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
            return Byte0 == options.HeaderByte0 && Byte1 == options.HeaderByte1;
        }
    }
}
