namespace PlServer.Protocol
{

    public sealed class PacketCodec
    {
        private readonly PacketCodecOptions _options;

        public PacketCodec()
            : this(PacketCodecOptions.Default)
        {
        }

        public PacketCodec(PacketCodecOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
            options.Validate();
            _options = options;
        }

        public PacketDecodeResult Decode(byte[]? rawBytes)
        {
            var errors = new List<PacketValidationError>();

            if (rawBytes is null || rawBytes.Length == 0)
            {
                errors.Add(new PacketValidationError(
                    PacketValidationErrorCode.NullOrEmptyInput,
                    "Input frame is null or empty."));

                return CreateDecodeResult(
                    rawBytes ?? Array.Empty<byte>(),
                    Array.Empty<byte>(),
                    null,
                    0,
                    Array.Empty<byte>(),
                    null,
                    null,
                    errors);
            }

            var rawCopy = rawBytes.ToArray();
            var decodedBytes = _options.XorEnabled
                ? XorCodec.Transform(rawCopy, _options.XorKey)
                : rawCopy.ToArray();

            if (decodedBytes.Length < _options.HeaderLength)
            {
                errors.Add(new PacketValidationError(
                    PacketValidationErrorCode.FrameTooShort,
                    $"Input frame is shorter than the configured header length {_options.HeaderLength}."));

                return CreateDecodeResult(rawCopy, decodedBytes, null, 0, Array.Empty<byte>(), null, null, errors);
            }

            var payloadLength = ReadPayloadLength(decodedBytes);
            var header = new PacketHeader(decodedBytes[0], decodedBytes[1], payloadLength);

            if (!header.Matches(_options))
            {
                errors.Add(new PacketValidationError(
                    PacketValidationErrorCode.InvalidHeader,
                    "Frame header does not match the configured header bytes."));
            }

            var availablePayloadLength = Math.Max(0, decodedBytes.Length - _options.PayloadOffset);
            var expectedFrameLength = _options.PayloadOffset + payloadLength;

            if (payloadLength > availablePayloadLength)
            {
                errors.Add(new PacketValidationError(
                    PacketValidationErrorCode.DeclaredLengthLargerThanAvailable,
                    "Declared payload length is larger than the available payload bytes."));
            }

            if (payloadLength == 0)
            {
                errors.Add(new PacketValidationError(
                    PacketValidationErrorCode.DeclaredLengthSmallerThanExpected,
                    "Declared payload length is smaller than the minimum payload required for AC."));
            }

            if (decodedBytes.Length != expectedFrameLength)
            {
                errors.Add(new PacketValidationError(
                    PacketValidationErrorCode.PayloadLengthMismatch,
                    "Frame byte count does not match the declared payload length."));
            }

            var payloadBytesToCopy = Math.Min(payloadLength, availablePayloadLength);
            var payload = payloadBytesToCopy > 0
                ? decodedBytes.AsSpan(_options.PayloadOffset, payloadBytesToCopy).ToArray()
                : Array.Empty<byte>();

            byte? ac = null;
            byte? subAc = null;

            if (payload.Length > _options.AcOffsetInPayload)
            {
                ac = payload[_options.AcOffsetInPayload];
            }
            else
            {
                errors.Add(new PacketValidationError(
                    PacketValidationErrorCode.MissingAc,
                    "Payload does not contain an AC byte."));
            }

            if (payload.Length > _options.SubAcOffsetInPayload)
            {
                subAc = payload[_options.SubAcOffsetInPayload];
            }

            return CreateDecodeResult(rawCopy, decodedBytes, header, payloadLength, payload, ac, subAc, errors);
        }

        public PacketEncodeResult Encode(byte ac, byte? subAc = null, ReadOnlySpan<byte> payloadBody = default)
        {
            var payloadLength = 1 + (subAc.HasValue ? 1 : 0) + payloadBody.Length;

            if (payloadLength > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(payloadBody), payloadBody.Length, "Payload is too large.");
            }

            var payload = new byte[payloadLength];
            payload[0] = ac;

            var bodyOffset = 1;
            if (subAc.HasValue)
            {
                payload[1] = subAc.Value;
                bodyOffset = 2;
            }

            payloadBody.CopyTo(payload.AsSpan(bodyOffset));

            var writer = new PacketWriter();
            writer.WriteByte(_options.HeaderByte0);
            writer.WriteByte(_options.HeaderByte1);
            writer.WriteUInt16LittleEndian((ushort)payload.Length);
            writer.WriteBytes(payload);

            var decodedBytes = writer.ToArray();
            var encodedBytes = _options.XorEnabled
                ? XorCodec.Transform(decodedBytes, _options.XorKey)
                : decodedBytes.ToArray();

            var header = new PacketHeader(_options.HeaderByte0, _options.HeaderByte1, (ushort)payload.Length);
            return new PacketEncodeResult(decodedBytes, encodedBytes, header, (ushort)payload.Length, payload, ac, subAc);
        }

        private ushort ReadPayloadLength(ReadOnlySpan<byte> decodedBytes)
        {
            return (ushort)(decodedBytes[_options.LengthOffset] | (decodedBytes[_options.LengthOffset + 1] << 8));
        }

        private static PacketDecodeResult CreateDecodeResult(
            byte[] rawBytes,
            byte[] decodedBytes,
            PacketHeader? header,
            ushort payloadLength,
            byte[] payload,
            byte? ac,
            byte? subAc,
            IReadOnlyList<PacketValidationError> errors)
        {
            return new PacketDecodeResult(rawBytes, decodedBytes, header, payloadLength, payload, ac, subAc, errors);
        }
    }

}
