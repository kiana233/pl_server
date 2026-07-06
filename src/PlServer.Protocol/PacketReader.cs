namespace PlServer.Protocol
{
    public sealed class PacketReader
    {
        private readonly ReadOnlyMemory<byte> _bytes;
        private int _position;

        public PacketReader(ReadOnlyMemory<byte> bytes)
        {
            _bytes = bytes;
        }

        public int Remaining => _bytes.Length - _position;

        public byte ReadByte()
        {
            EnsureAvailable(1);
            return _bytes.Span[_position++];
        }

        public ushort ReadUInt16LittleEndian()
        {
            EnsureAvailable(2);
            var span = _bytes.Span;
            var value = (ushort)(span[_position] | (span[_position + 1] << 8));
            _position += 2;
            return value;
        }

        public uint ReadUInt32LittleEndian()
        {
            EnsureAvailable(4);
            var span = _bytes.Span;
            var value =
                (uint)span[_position]
                | ((uint)span[_position + 1] << 8)
                | ((uint)span[_position + 2] << 16)
                | ((uint)span[_position + 3] << 24);
            _position += 4;
            return value;
        }

        public byte[] ReadBytes(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "Count cannot be negative.");
            }

            EnsureAvailable(count);
            var bytes = _bytes.Slice(_position, count).ToArray();
            _position += count;
            return bytes;
        }

        private void EnsureAvailable(int count)
        {
            if (Remaining < count)
            {
                throw new InvalidOperationException("Not enough bytes remain in the packet reader.");
            }
        }
    }
}
