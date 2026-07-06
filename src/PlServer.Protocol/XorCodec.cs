namespace PlServer.Protocol
{

    public static class XorCodec
    {
        public static byte[] Transform(ReadOnlySpan<byte> bytes, byte key)
        {
            var transformed = bytes.ToArray();
            TransformInPlace(transformed, key);
            return transformed;
        }

        public static void TransformInPlace(Span<byte> bytes, byte key)
        {
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ key);
            }
        }
    }

}
