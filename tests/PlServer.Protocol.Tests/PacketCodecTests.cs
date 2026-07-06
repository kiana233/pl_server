using PlServer.Protocol;

namespace PlServer.Protocol.Tests
{
    public sealed class PacketCodecTests
    {
        [Fact]
        public void Encode_writes_f4_44_header()
        {
            var result = new PacketCodec().Encode(0x63, 0x01);

            Assert.Equal(0xF4, result.EncodedBytes[0]);
            Assert.Equal(0x44, result.EncodedBytes[1]);
        }

        [Fact]
        public void Encode_writes_little_endian_payload_length()
        {
            var result = new PacketCodec().Encode(0x63, 0x01, new byte[] { 0xAA, 0xBB, 0xCC });

            Assert.Equal(0x05, result.EncodedBytes[2]);
            Assert.Equal(0x00, result.EncodedBytes[3]);
        }

        [Fact]
        public void Decode_reads_ac_and_subac()
        {
            var frame = new byte[] { 0xF4, 0x44, 0x03, 0x00, 0x63, 0x01, 0xAA };

            var result = new PacketCodec().Decode(frame);

            Assert.True(result.IsValid);
            Assert.True(result.Ac.HasValue);
            Assert.True(result.SubAc.HasValue);
            Assert.Equal((byte)0x63, result.Ac.Value);
            Assert.Equal((byte)0x01, result.SubAc.Value);
        }

        [Fact]
        public void Decode_allows_missing_subac_when_payload_length_is_one()
        {
            var frame = new byte[] { 0xF4, 0x44, 0x01, 0x00, 0x06 };

            var result = new PacketCodec().Decode(frame);

            Assert.True(result.IsValid);
            Assert.True(result.Ac.HasValue);
            Assert.Equal((byte)0x06, result.Ac.Value);
            Assert.Null(result.SubAc);
        }

        [Fact]
        public void Decode_rejects_invalid_header()
        {
            var frame = new byte[] { 0x00, 0x44, 0x01, 0x00, 0x06 };

            var result = new PacketCodec().Decode(frame);

            Assert.False(result.IsValid);
            Assert.Contains(result.ValidationErrors, e => e.Code == PacketValidationErrorCode.InvalidHeader);
        }

        [Fact]
        public void Decode_rejects_frame_shorter_than_header_length()
        {
            var frame = new byte[] { 0xF4, 0x44, 0x01 };

            var result = new PacketCodec().Decode(frame);

            Assert.False(result.IsValid);
            Assert.Contains(result.ValidationErrors, e => e.Code == PacketValidationErrorCode.FrameTooShort);
        }

        [Fact]
        public void Decode_reports_declared_length_larger_than_available_bytes()
        {
            var frame = new byte[] { 0xF4, 0x44, 0x04, 0x00, 0x63, 0x01 };

            var result = new PacketCodec().Decode(frame);

            Assert.False(result.IsValid);
            Assert.Contains(result.ValidationErrors, e => e.Code == PacketValidationErrorCode.DeclaredLengthLargerThanAvailable);
            Assert.Contains(result.ValidationErrors, e => e.Code == PacketValidationErrorCode.PayloadLengthMismatch);
        }

        [Fact]
        public void Xor_encode_decode_roundtrip_succeeds()
        {
            var codec = new PacketCodec(new PacketCodecOptions { XorEnabled = true });

            var encoded = codec.Encode(0x63, 0x01, new byte[] { 0xAA, 0xBB });
            var decoded = codec.Decode(encoded.EncodedBytes);

            Assert.True(decoded.IsValid);
            Assert.NotEqual(encoded.DecodedBytes, encoded.EncodedBytes);
            Assert.Equal(encoded.DecodedBytes, decoded.DecodedBytes);
            Assert.True(decoded.Ac.HasValue);
            Assert.True(decoded.SubAc.HasValue);
            Assert.Equal((byte)0x63, decoded.Ac.Value);
            Assert.Equal((byte)0x01, decoded.SubAc.Value);
        }

        [Fact]
        public void PacketReader_reads_little_endian_ushort_and_uint()
        {
            var reader = new PacketReader(new byte[] { 0x34, 0x12, 0x78, 0x56, 0x34, 0x12 });

            Assert.Equal(0x1234, reader.ReadUInt16LittleEndian());
            Assert.Equal(0x12345678u, reader.ReadUInt32LittleEndian());
            Assert.Equal(0, reader.Remaining);
        }

        [Fact]
        public void PacketWriter_writes_little_endian_ushort_and_uint()
        {
            var writer = new PacketWriter();

            writer.WriteUInt16LittleEndian(0x1234);
            writer.WriteUInt32LittleEndian(0x12345678);

            Assert.Equal(new byte[] { 0x34, 0x12, 0x78, 0x56, 0x34, 0x12 }, writer.ToArray());
        }

        [Fact]
        public void Codec_options_can_customize_header()
        {
            var codec = new PacketCodec(new PacketCodecOptions
            {
                HeaderByte0 = 0xAA,
                HeaderByte1 = 0xBB
            });

            var encoded = codec.Encode(0x01);
            var decoded = codec.Decode(encoded.EncodedBytes);

            Assert.Equal(0xAA, encoded.EncodedBytes[0]);
            Assert.Equal(0xBB, encoded.EncodedBytes[1]);
            Assert.True(decoded.IsValid);
        }

        [Fact]
        public void Encoded_length_excludes_header_and_length_prefix()
        {
            var result = new PacketCodec().Encode(0x20, 0x01, new byte[] { 0x10, 0x11 });

            Assert.Equal(4, result.PayloadLength);
            Assert.Equal(result.EncodedBytes.Length - 4, result.PayloadLength);
        }

        [Fact]
        public void Decode_does_not_throw_on_malformed_frames()
        {
            var codec = new PacketCodec();
            var malformedFrames = new byte[][]
            {
                Array.Empty<byte>(),
                new byte[] { 0xF4 },
                new byte[] { 0x00, 0x00, 0xFF, 0xFF },
                new byte[] { 0xF4, 0x44, 0x02, 0x00, 0x63 }
            };

            foreach (var malformedFrame in malformedFrames)
            {
                var exception = Record.Exception(() => codec.Decode(malformedFrame));

                Assert.Null(exception);
            }
        }

        [Fact]
        public void Empty_payload_reports_missing_ac()
        {
            var frame = new byte[] { 0xF4, 0x44, 0x00, 0x00 };

            var result = new PacketCodec().Decode(frame);

            Assert.False(result.IsValid);
            Assert.Contains(result.ValidationErrors, e => e.Code == PacketValidationErrorCode.MissingAc);
            Assert.Contains(result.ValidationErrors, e => e.Code == PacketValidationErrorCode.DeclaredLengthSmallerThanExpected);
        }
    }
}
